using LevelUp.API.DTOs.Positions;
using LevelUpAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LevelUp.API.Utilities;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/positions")]
[Authorize(Roles = "Admin")]
public class PositionController : ControllerBase
{
    private readonly IPositionService _positionService;

    public PositionController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        var (positions, total) = await _positionService.GetAllPositionsAsync(isActive, cancellationToken);
        return Ok(new ApiResponse<IEnumerable<PositionResponse>>(200, "Success", positions, total));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var position = await _positionService.GetPositionByIdAsync(id, cancellationToken);

        if (position is null)
            return NotFound($"Position with ID {id} not found");

        return Ok(new ApiResponse<PositionResponse?>(position));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PositionRequest request, CancellationToken cancellationToken)
    {
        await _positionService.CreatePositionAsync(request, cancellationToken);
        return Ok(new ApiResponse<object>("Position created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PositionRequest request, CancellationToken cancellationToken)
    {
        await _positionService.UpdatePositionAsync(id, request, cancellationToken);
        return Ok(new ApiResponse<object>("Position update successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _positionService.DeletePositionAsync(id, cancellationToken);
        return Ok(new ApiResponse<object>("Position deleted successfully"));
    }
}
