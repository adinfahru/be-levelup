namespace LevelUp.API.DTOs.Modules;

public record ModuleResponse(
    Guid Id,
    string Title,
    string? Description,
    int EstimatedDays,
    bool IsActive,
    Guid CreatedBy,
    DateTime CreatedAt,
    int ItemCount
);

public record ModuleDetailResponse(
    Guid Id,
    string Title,
    string? Description,
    int EstimatedDays,
    bool IsActive,
    Guid CreatedBy,
    DateTime CreatedAt,
    List<ModuleItemResponse> Items
);

public record ModuleItemResponse(
    Guid Id,
    Guid ModuleId,
    string Title,
    int OrderIndex,
    string? Description,
    string? Url,
    bool IsFinalSubmission
);
