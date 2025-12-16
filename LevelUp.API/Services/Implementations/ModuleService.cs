using LevelUp.API.DTOs.ModuleItems;
using LevelUp.API.DTOs.Modules;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Services.Implementations;

public class ModuleService : IModuleService
{
  private readonly IModuleRepository _moduleRepository;
  private readonly IModuleItemRepository _moduleItemRepository;
  private readonly IEnrollmentRepository _enrollmentRepository;
  private readonly IUnitOfWork _unitOfWork;

  public ModuleService(
      IModuleRepository moduleRepository,
      IModuleItemRepository moduleItemRepository,
      IEnrollmentRepository enrollmentRepository,
      IUnitOfWork unitOfWork)
  {
    _moduleRepository = moduleRepository;
    _moduleItemRepository = moduleItemRepository;
    _enrollmentRepository = enrollmentRepository;
    _unitOfWork = unitOfWork;
  }

  public Task<(List<ModuleResponse> items, int total)> GetAllAsync(
      int page,
      int limit,
      bool? isActive,
      Guid? createdBy)
  {
    IQueryable<Module> query = _moduleRepository.GetQueryable()
        .Include(m => m.Items)
        .Include(m => m.Enrollments)
        .Include(m => m.Creator)
            .ThenInclude(c => c!.Employee);

    if (isActive.HasValue)
      query = query.Where(m => m.IsActive == isActive.Value);

    if (createdBy.HasValue)
      query = query.Where(m => m.CreatedBy == createdBy.Value);

    var total = query.Count();
    var items = query
        .OrderByDescending(m => m.CreatedAt)
        .Skip((page - 1) * limit)
        .Take(limit)
        .Select(m => new ModuleResponse(
            m.Id,
            m.Title ?? string.Empty,
            m.Description,
            m.EstimatedDays,
            m.IsActive,
            m.CreatedBy,
            m.Creator!.Employee!.FirstName + " " + m.Creator.Employee.LastName,
            m.CreatedAt,
            m.Items.Count,
            m.Enrollments.Count,
            m.Enrollments.Count(e => e.Status == EnrollmentStatus.OnGoing)
        ))
        .ToList();

    return Task.FromResult((items, total));
  }

  public async Task<ModuleDetailResponse?> GetByIdAsync(Guid id)
  {
    var module = await _moduleRepository.GetQueryable()
        .Include(m => m.Creator)
            .ThenInclude(c => c!.Employee)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (module == null) return null;

    var items = (await _moduleItemRepository.GetAllAsync(CancellationToken.None))
        .Where(i => i.ModuleId == id)
        .OrderBy(i => i.OrderIndex)
        .Select(i => new ModuleItemResponse(
            i.Id,
            i.ModuleId,
            i.Title ?? string.Empty,
            i.OrderIndex,
            i.Descriptions,
            i.Url,
            i.IsFinalSubmission
        ))
        .ToList();

    var createdByName = module.Creator?.Employee != null
        ? $"{module.Creator.Employee.FirstName} {module.Creator.Employee.LastName}"
        : "Unknown";

    return new ModuleDetailResponse(
        module.Id,
        module.Title ?? string.Empty,
        module.Description,
        module.EstimatedDays,
        module.IsActive,
        module.CreatedBy,
        createdByName,
        module.CreatedAt,
        items
    );
  }

  public async Task<ModuleResponse> CreateAsync(CreateModuleRequest request, Guid managerId)
  {
    // Validation: Sequential orderIndex
    if (request.Items != null && request.Items.Any())
    {
      var orderIndexes = request.Items.Select(i => i.OrderIndex).OrderBy(o => o).ToList();
      for (int i = 0; i < orderIndexes.Count; i++)
      {
        if (orderIndexes[i] != i + 1)
          throw new InvalidOperationException("OrderIndex must be sequential starting from 1");
      }

      // Validation: Only 1 final submission
      var finalSubmissionCount = request.Items.Count(i => i.IsFinalSubmission);
      if (finalSubmissionCount > 1)
        throw new InvalidOperationException("Only one item can be marked as final submission");
    }

    var module = new Module
    {
      Id = Guid.NewGuid(),
      Title = request.Title,
      Description = request.Description,
      EstimatedDays = request.EstimatedDays,
      IsActive = true,
      CreatedBy = managerId,
      CreatedAt = DateTime.UtcNow
    };

    await _unitOfWork.CommitTransactionAsync(async () =>
    {
      await _moduleRepository.CreateAsync(module, CancellationToken.None);

      if (request.Items != null)
      {
        foreach (var itemRequest in request.Items)
        {
          var item = new ModuleItem
          {
            Id = Guid.NewGuid(),
            ModuleId = module.Id,
            Title = itemRequest.Title,
            OrderIndex = itemRequest.OrderIndex,
            Descriptions = itemRequest.Descriptions,
            Url = itemRequest.Url,
            IsFinalSubmission = itemRequest.IsFinalSubmission
          };
          await _moduleItemRepository.CreateAsync(item, CancellationToken.None);
        }
      }
    }, CancellationToken.None);

    // Get creator name
    var creator = await _moduleRepository.GetQueryable()
        .Where(m => m.Id == module.Id)
        .Include(m => m.Creator)
            .ThenInclude(c => c!.Employee)
        .Select(m => m.Creator)
        .FirstOrDefaultAsync();

    var createdByName = creator?.Employee != null
        ? $"{creator.Employee.FirstName} {creator.Employee.LastName}"
        : "Unknown";

    return new ModuleResponse(
        module.Id,
        module.Title,
        module.Description,
        module.EstimatedDays,
        module.IsActive,
        module.CreatedBy,
        createdByName,
        module.CreatedAt,
        request.Items?.Count ?? 0,
        0, // enrolledCount - new module has no enrollments
        0  // activeCount - new module has no active enrollments
    );
  }

  public async Task<ModuleResponse?> UpdateAsync(Guid id, UpdateModuleRequest request, Guid managerId)
  {
    var module = await _moduleRepository.FirstOrDefaultAsync(m => m.Id == id);
    if (module == null) return null;

    // Authorization: Only creator can update
    if (module.CreatedBy != managerId)
      throw new UnauthorizedAccessException("You can only update modules you created");

    // Check if there are ongoing enrollments for this module
    var hasOngoingEnrollments = (await _enrollmentRepository.GetAllAsync(CancellationToken.None))
        .Any(e => e.ModuleId == id && e.Status == EnrollmentStatus.OnGoing);

    // Simple rule: If there are ongoing enrollments, block ALL updates
    if (hasOngoingEnrollments)
    {
      throw new InvalidOperationException(
          "Cannot update module with ongoing enrollments. " +
          "Wait until all enrollments are completed or paused before making any changes.");
    }

    // No ongoing enrollments - allow updating all fields
    if (request.Title != null) module.Title = request.Title;
    if (request.Description != null) module.Description = request.Description;
    if (request.EstimatedDays.HasValue) module.EstimatedDays = request.EstimatedDays.Value;
    if (request.IsActive.HasValue) module.IsActive = request.IsActive.Value;
    module.UpdatedAt = DateTime.UtcNow;

    await _moduleRepository.UpdateAsync(module);
    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);

    var itemCount = (await _moduleItemRepository.GetAllAsync(CancellationToken.None)).Count(i => i.ModuleId == id);
    var enrollments = (await _enrollmentRepository.GetAllAsync(CancellationToken.None)).Where(e => e.ModuleId == id).ToList();

    // Get creator name
    var creator = await _moduleRepository.GetQueryable()
        .Where(m => m.Id == id)
        .Include(m => m.Creator)
            .ThenInclude(c => c!.Employee)
        .Select(m => m.Creator)
        .FirstOrDefaultAsync();

    var createdByName = creator?.Employee != null
        ? $"{creator.Employee.FirstName} {creator.Employee.LastName}"
        : "Unknown";

    return new ModuleResponse(
        module.Id,
        module.Title ?? string.Empty,
        module.Description,
        module.EstimatedDays,
        module.IsActive,
        module.CreatedBy,
        createdByName,
        module.CreatedAt,
        itemCount,
        enrollments.Count,
        enrollments.Count(e => e.Status == EnrollmentStatus.OnGoing)
    );
  }

  // ============= MODULE ITEMS MANAGEMENT =============

  public async Task<ModuleItemResponse> AddItemAsync(Guid moduleId, DTOs.Modules.CreateModuleItemRequest request, Guid creatorId)
  {
    // Verify module exists and user is the creator
    var module = await _moduleRepository.GetByIdAsync(moduleId, CancellationToken.None);
    if (module == null)
      throw new KeyNotFoundException($"Module with ID {moduleId} not found");

    if (module.CreatedBy != creatorId)
      throw new UnauthorizedAccessException("Only the module creator can add items");

    // Check for ongoing enrollments (strict block)
    var hasOngoingEnrollments = (await _enrollmentRepository.GetAllAsync(CancellationToken.None))
        .Any(e => e.ModuleId == moduleId && e.Status == EnrollmentStatus.OnGoing);

    if (hasOngoingEnrollments)
      throw new InvalidOperationException("Cannot add items to a module with ongoing enrollments");

    // Get all existing items for this module
    var existingItems = (await _moduleItemRepository.GetAllAsync(CancellationToken.None))
        .Where(i => i.ModuleId == moduleId)
        .OrderBy(i => i.OrderIndex)
        .ToList();

    // If setting as final submission, unset others
    if (request.IsFinalSubmission)
    {
      foreach (var item in existingItems.Where(i => i.IsFinalSubmission))
      {
        item.IsFinalSubmission = false;
        await _moduleItemRepository.UpdateAsync(item);
      }
    }

    // Auto-adjust orderIndex: insert at requested position and shift others
    int finalOrderIndex = request.OrderIndex;
    if (request.OrderIndex <= existingItems.Count)
    {
      // Shift items at or after the requested position
      foreach (var item in existingItems.Where(i => i.OrderIndex >= request.OrderIndex))
      {
        item.OrderIndex++;
        await _moduleItemRepository.UpdateAsync(item);
      }
    }
    else
    {
      // If orderIndex > count, append at the end
      finalOrderIndex = existingItems.Count + 1;
    }

    var newItem = new ModuleItem
    {
      Id = Guid.NewGuid(),
      ModuleId = moduleId,
      Title = request.Title,
      OrderIndex = finalOrderIndex,
      Descriptions = request.Descriptions,
      Url = request.Url,
      IsFinalSubmission = request.IsFinalSubmission
    };

    await _moduleItemRepository.CreateAsync(newItem, CancellationToken.None);
    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);

    return new ModuleItemResponse(
      newItem.Id,
      newItem.ModuleId,
      newItem.Title,
      newItem.OrderIndex,
      newItem.Descriptions,
      newItem.Url,
      newItem.IsFinalSubmission
    );
  }

  public async Task<ModuleItemResponse?> UpdateItemAsync(Guid moduleId, Guid itemId, UpdateModuleItemRequest request, Guid creatorId)
  {
    // Verify module exists and user is the creator
    var module = await _moduleRepository.GetByIdAsync(moduleId, CancellationToken.None);
    if (module == null)
      throw new KeyNotFoundException($"Module with ID {moduleId} not found");

    if (module.CreatedBy != creatorId)
      throw new UnauthorizedAccessException("Only the module creator can update items");

    // Check for ongoing enrollments (strict block)
    var hasOngoingEnrollments = (await _enrollmentRepository.GetAllAsync(CancellationToken.None))
        .Any(e => e.ModuleId == moduleId && e.Status == EnrollmentStatus.OnGoing);

    if (hasOngoingEnrollments)
      throw new InvalidOperationException("Cannot update items when there are ongoing enrollments");

    // Get the item
    var item = await _moduleItemRepository.GetByIdAsync(itemId, CancellationToken.None);
    if (item == null || item.ModuleId != moduleId)
      return null;

    // If setting as final submission, unset others
    if (request.IsFinalSubmission && !item.IsFinalSubmission)
    {
      var otherItems = (await _moduleItemRepository.GetAllAsync(CancellationToken.None))
          .Where(i => i.ModuleId == moduleId && i.Id != itemId && i.IsFinalSubmission)
          .ToList();

      foreach (var otherItem in otherItems)
      {
        otherItem.IsFinalSubmission = false;
        await _moduleItemRepository.UpdateAsync(otherItem);
      }
    }

    // Update item
    item.Title = request.Title;
    item.Descriptions = request.Descriptions;
    item.Url = request.Url;
    item.IsFinalSubmission = request.IsFinalSubmission;

    await _moduleItemRepository.UpdateAsync(item);
    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);

    return new ModuleItemResponse(
      item.Id,
      item.ModuleId,
      item.Title,
      item.OrderIndex,
      item.Descriptions,
      item.Url,
      item.IsFinalSubmission
    );
  }

  public async Task<bool> DeleteItemAsync(Guid moduleId, Guid itemId, Guid creatorId)
  {
    // Verify module exists and user is the creator
    var module = await _moduleRepository.GetByIdAsync(moduleId, CancellationToken.None);
    if (module == null)
      throw new KeyNotFoundException($"Module with ID {moduleId} not found");

    if (module.CreatedBy != creatorId)
      throw new UnauthorizedAccessException("Only the module creator can delete items");

    // Check for ongoing enrollments (strict block)
    var hasOngoingEnrollments = (await _enrollmentRepository.GetAllAsync(CancellationToken.None))
        .Any(e => e.ModuleId == moduleId && e.Status == EnrollmentStatus.OnGoing);

    if (hasOngoingEnrollments)
      throw new InvalidOperationException("Cannot delete items when there are ongoing enrollments");

    // Get the item
    var item = await _moduleItemRepository.GetByIdAsync(itemId, CancellationToken.None);
    if (item == null || item.ModuleId != moduleId)
      return false;

    var allItems = (await _moduleItemRepository.GetAllAsync(CancellationToken.None))
        .Where(i => i.ModuleId == moduleId)
        .OrderBy(i => i.OrderIndex)
        .ToList();

    // If deleting the final submission item, auto-assign to the last remaining item
    bool wasFinalSubmission = item.IsFinalSubmission;

    // Delete the item
    await _moduleItemRepository.DeleteAsync(item);

    // Reindex remaining items (remove gaps)
    var remainingItems = allItems.Where(i => i.Id != itemId).OrderBy(i => i.OrderIndex).ToList();
    for (int i = 0; i < remainingItems.Count; i++)
    {
      remainingItems[i].OrderIndex = i + 1;
      await _moduleItemRepository.UpdateAsync(remainingItems[i]);
    }

    // If we deleted the final submission, assign to the last item
    if (wasFinalSubmission && remainingItems.Count > 0)
    {
      var lastItem = remainingItems.Last();
      lastItem.IsFinalSubmission = true;
      await _moduleItemRepository.UpdateAsync(lastItem);
    }

    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);
    return true;
  }

  public async Task<List<ModuleItemResponse>> ReorderItemsAsync(Guid moduleId, ReorderModuleItemsRequest request, Guid creatorId)
  {
    // Verify module exists and user is the creator
    var module = await _moduleRepository.GetByIdAsync(moduleId, CancellationToken.None);
    if (module == null)
      throw new KeyNotFoundException($"Module with ID {moduleId} not found");

    if (module.CreatedBy != creatorId)
      throw new UnauthorizedAccessException("Only the module creator can reorder items");

    // Check for ongoing enrollments (strict block - reordering changes structure)
    var hasOngoingEnrollments = (await _enrollmentRepository.GetAllAsync(CancellationToken.None))
        .Any(e => e.ModuleId == moduleId && e.Status == EnrollmentStatus.OnGoing);

    if (hasOngoingEnrollments)
      throw new InvalidOperationException("Cannot reorder items when there are ongoing enrollments");

    // Get all items for this module
    var allItems = (await _moduleItemRepository.GetAllAsync(CancellationToken.None))
        .Where(i => i.ModuleId == moduleId)
        .ToList();

    // Validate all itemIds exist
    var requestedIds = request.ItemOrders.Select(o => o.ItemId).ToHashSet();
    if (requestedIds.Count != allItems.Count || !requestedIds.SetEquals(allItems.Select(i => i.Id)))
      throw new InvalidOperationException("Item order list must include all module items exactly once");

    // Validate orderIndex is sequential starting from 1
    var orderedIndexes = request.ItemOrders.Select(o => o.NewOrderIndex).OrderBy(x => x).ToList();
    var expectedIndexes = Enumerable.Range(1, orderedIndexes.Count).ToList();
    if (!orderedIndexes.SequenceEqual(expectedIndexes))
      throw new InvalidOperationException("OrderIndex must be sequential starting from 1 with no gaps");

    // Apply new order
    foreach (var orderDto in request.ItemOrders)
    {
      var item = allItems.First(i => i.Id == orderDto.ItemId);
      item.OrderIndex = orderDto.NewOrderIndex;
      await _moduleItemRepository.UpdateAsync(item);
    }

    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);

    // Return updated items in order
    return allItems
        .OrderBy(i => i.OrderIndex)
        .Select(i => new ModuleItemResponse(
          i.Id,
          i.ModuleId,
          i.Title ?? string.Empty,
          i.OrderIndex,
          i.Descriptions,
          i.Url,
          i.IsFinalSubmission
        ))
        .ToList();
  }
}
