namespace LevelUp.API.DTOs.Modules;
public record AssignableModuleResponse(
    Guid Id,
    string Title,
    bool IsAlreadyCompleted,
    bool IsCurrentlyEnrolled
);
