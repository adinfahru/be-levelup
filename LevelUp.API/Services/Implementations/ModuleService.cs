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
  private readonly IUnitOfWork _unitOfWork;

  public ModuleService(
      IModuleRepository moduleRepository,
      IModuleItemRepository moduleItemRepository,
      IUnitOfWork unitOfWork)
  {
    _moduleRepository = moduleRepository;
    _moduleItemRepository = moduleItemRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<(List<ModuleResponse> items, int total)> GetAllAsync(
      int page,
      int limit,
      bool? isActive,
      Guid? createdBy)
  {
    var query = (await _moduleRepository.GetAllAsync(CancellationToken.None)).AsQueryable();

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
            m.CreatedAt,
            m.Items.Count
        ))
        .ToList();

    return (items, total);
  }

  public async Task<ModuleDetailResponse?> GetByIdAsync(Guid id)
  {
    var module = await _moduleRepository.FirstOrDefaultAsync(m => m.Id == id);
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

    return new ModuleDetailResponse(
        module.Id,
        module.Title ?? string.Empty,
        module.Description,
        module.EstimatedDays,
        module.IsActive,
        module.CreatedBy,
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
          Descriptions = itemRequest.Description,
          Url = itemRequest.Url,
          IsFinalSubmission = itemRequest.IsFinalSubmission
        };
        await _moduleItemRepository.CreateAsync(item, CancellationToken.None);
      }
    }

    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);

    return new ModuleResponse(
        module.Id,
        module.Title,
        module.Description,
        module.EstimatedDays,
        module.IsActive,
        module.CreatedBy,
        module.CreatedAt,
        request.Items?.Count ?? 0
    );
  }

  public async Task<ModuleResponse?> UpdateAsync(Guid id, UpdateModuleRequest request, Guid managerId)
  {
    var module = await _moduleRepository.FirstOrDefaultAsync(m => m.Id == id);
    if (module == null) return null;

    // Authorization: Only creator can update
    if (module.CreatedBy != managerId)
      throw new UnauthorizedAccessException("You can only update modules you created");

    if (request.Title != null) module.Title = request.Title;
    if (request.Description != null) module.Description = request.Description;
    if (request.EstimatedDays.HasValue) module.EstimatedDays = request.EstimatedDays.Value;
    if (request.IsActive.HasValue) module.IsActive = request.IsActive.Value;
    module.UpdatedAt = DateTime.UtcNow;

    await _moduleRepository.UpdateAsync(module);
    await _unitOfWork.CommitTransactionAsync(() => Task.CompletedTask, CancellationToken.None);

    var itemCount = (await _moduleItemRepository.GetAllAsync(CancellationToken.None)).Count(i => i.ModuleId == id);

    return new ModuleResponse(
        module.Id,
        module.Title ?? string.Empty,
        module.Description,
        module.EstimatedDays,
        module.IsActive,
        module.CreatedBy,
        module.CreatedAt,
        itemCount
    );
  }
}
