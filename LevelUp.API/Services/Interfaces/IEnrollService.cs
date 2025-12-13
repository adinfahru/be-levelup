using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IEnrollService
{
    Task<EnrollmentResponse> EnrollAsync(string email, Guid moduleId, CancellationToken cancellationToken);
    Task<EnrollmentResponse> ResumeEnrollmentAsync(Guid enrollmentId, string email, CancellationToken cancellationToken);
    Task<EnrollmentResponse?> GetCurrentEnrollmentAsync(string email, CancellationToken cancellationToken);
    Task<List<EnrollmentResponse>> GetEnrollmentHistoryAsync(string email, CancellationToken cancellationToken);
    Task<EnrollmentResponse> SubmitEnrollmentItemAsync(Guid enrollmentId, string email, SubmitChecklistRequest request, CancellationToken cancellationToken);
    Task<EnrollmentResponse> GetEnrollmentProgressAsync(Guid enrollmentId, string email, CancellationToken cancellationToken);

}
