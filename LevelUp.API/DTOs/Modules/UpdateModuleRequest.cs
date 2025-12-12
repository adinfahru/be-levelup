namespace LevelUp.API.DTOs.Modules;

public record UpdateModuleRequest(
    string? Title,
    string? Description,
    int? EstimatedDays,
    bool? IsActive
);
