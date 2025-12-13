namespace LevelUp.API.DTOs.Modules;

public record CreateModuleRequest(
    string Title,
    string? Description,
    int EstimatedDays,
    List<CreateModuleItemRequest>? Items
);

public record CreateModuleItemRequest(
    string Title,
    int OrderIndex,
    string? Description,
    string? Url,
    bool IsFinalSubmission
);
