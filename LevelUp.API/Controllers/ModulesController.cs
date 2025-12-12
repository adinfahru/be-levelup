using LevelUp.API.DTOs.Modules;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var managerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var module = await _moduleService.CreateAsync(request, managerId);

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
}
