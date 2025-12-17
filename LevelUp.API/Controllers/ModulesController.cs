using LevelUp.API.DTOs.ModuleItems;
using LevelUp.API.DTOs.Modules;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/modules")]
[Authorize(Roles = "Manager,Employee")]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModulesController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] bool? isActive = null,
        [FromQuery] Guid? createdBy = null)
    {
        var (items, total) = await _moduleService.GetAllAsync(page, limit, isActive, createdBy);
        var totalPages = (int)Math.Ceiling((double)total / limit);

        var response = new ApiResponse<object>(new
        {
            items,
            pagination = new
            {
                page,
                limit,
                total,
                totalPages
            }
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var module = await _moduleService.GetByIdAsync(id);
        if (module == null)
            throw new NullReferenceException("Module not found");

        return Ok(new ApiResponse<ModuleDetailResponse>(module));
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create([FromBody] CreateModuleRequest request)
    {
        var accountId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var module = await _moduleService.CreateAsync(request, accountId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = module.Id },
            new ApiResponse<ModuleResponse>(StatusCodes.Status201Created, "Module created successfully", module));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModuleRequest request)
    {
        var managerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var module = await _moduleService.UpdateAsync(id, request, managerId);

        if (module == null)
            throw new NullReferenceException("Module not found");

        return Ok(new ApiResponse<ModuleResponse>(module));
    }

    // ============= MODULE ITEMS ENDPOINTS =============

    [HttpPost("{moduleId}/items")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AddItem(Guid moduleId, [FromBody] CreateModuleItemRequest request)
    {
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var item = await _moduleService.AddItemAsync(moduleId, request, creatorId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = moduleId },
            new ApiResponse<ModuleItemResponse>(StatusCodes.Status201Created, "Module item added successfully", item));
    }

    [HttpPut("{moduleId}/items/{itemId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateItem(Guid moduleId, Guid itemId, [FromBody] UpdateModuleItemRequest request)
    {
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var item = await _moduleService.UpdateItemAsync(moduleId, itemId, request, creatorId);

        if (item == null)
            throw new NullReferenceException("Module item not found");

        return Ok(new ApiResponse<ModuleItemResponse>(StatusCodes.Status200OK, "Module item updated successfully", item));
    }

    [HttpDelete("{moduleId}/items/{itemId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteItem(Guid moduleId, Guid itemId)
    {
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var result = await _moduleService.DeleteItemAsync(moduleId, itemId, creatorId);

        if (!result)
            throw new NullReferenceException("Module item not found");

        return Ok(new ApiResponse<object>(StatusCodes.Status200OK, "Module item deleted successfully", null));
    }

    [HttpPut("{moduleId}/items/reorder")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ReorderItems(Guid moduleId, [FromBody] ReorderModuleItemsRequest request)
    {
        var creatorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var items = await _moduleService.ReorderItemsAsync(moduleId, request, creatorId);

        return Ok(new ApiResponse<List<ModuleItemResponse>>(StatusCodes.Status200OK, "Module items reordered successfully", items));
    }
}
