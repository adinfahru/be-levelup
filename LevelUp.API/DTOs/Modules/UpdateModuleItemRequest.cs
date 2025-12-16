using System.ComponentModel.DataAnnotations;

namespace LevelUp.API.DTOs.Modules;

public record UpdateModuleItemRequest(
    [Required]
    [StringLength(200, MinimumLength = 3)]
    string Title,

    [StringLength(1000)]
    string? Descriptions,

    [StringLength(500)]
    string? Url,

    bool IsFinalSubmission
);
