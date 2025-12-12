namespace LevelUp.API.DTOs.Positions;

public record PositionResponse
(
    Guid Id,
    string Title,
    bool IsActive
);
