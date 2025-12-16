namespace LevelUp.API.DTOs.Modules;

public record ModuleResponse(
    Guid Id,
    string Title,
    string? Description,
    int EstimatedDays,
    bool IsActive,
    Guid CreatedBy,
    string CreatedByName,
    DateTime CreatedAt,
    int ItemCount,
    int EnrolledCount,
    int ActiveCount
);

public record ModuleDetailResponse(
    Guid Id,
    string Title,
    string? Description,
    int EstimatedDays,
    bool IsActive,
    Guid CreatedBy,
    string CreatedByName,
    DateTime CreatedAt,
    List<ModuleItemResponse> Items
);

public record ModuleItemResponse(
    Guid Id,
    Guid ModuleId,
    string Title,
    int OrderIndex,
    string? Descriptions,
    string? Url,
    bool IsFinalSubmission
);
