using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Entity;

namespace LevelUp.API.Services.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(Guid accountId, Guid moduleId, CancellationToken cancellationToken);

    Task<EnrollmentResponse> ResumeEnrollmentAsync(Guid enrollmentId, Guid accountId,CancellationToken cancellationToken);

    Task<EnrollmentResponse?> GetCurrentEnrollmentAsync(Guid accountId,CancellationToken cancellationToken);

    Task<List<EnrollmentResponse>> GetEnrollmentHistoryAsync(Guid accountId,CancellationToken cancellationToken);

    Task<EnrollmentResponse> SubmitEnrollmentItemAsync(Guid enrollmentId,Guid accountId,SubmitChecklistRequest request,CancellationToken cancellationToken);

    Task<EnrollmentResponse> GetEnrollmentProgressAsync(Guid enrollmentId,Guid accountId,CancellationToken cancellationToken);

    Task<EnrollmentResponse> AssignEnrollmentAsync(Guid managerAccountId, AssignEnrollmentRequest request, CancellationToken cancellationToken);
}

