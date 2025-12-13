using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IEnrollService
{
    Task<EnrollmentResponse> EnrollAsync(string email, Guid moduleId, CancellationToken cancellationToken);
    Task<EnrollmentResponse> ResumeEnrollmentAsync(Guid enrollmentId, string email, CancellationToken cancellationToken);
    Task<EnrollmentResponse?> GetCurrentEnrollmentAsync(string email, CancellationToken cancellationToken);
    Task<List<EnrollmentResponse>> GetEnrollmentHistoryAsync(string email, CancellationToken cancellationToken);
    Task<EnrollmentResponse> CompleteSectionAsync(Guid enrollmentId, Guid moduleItemId, string email, string? evidenceUrl, CancellationToken cancellationToken);

}
