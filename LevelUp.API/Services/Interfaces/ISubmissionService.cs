using LevelUp.API.DTOs.Submissions;

namespace LevelUp.API.Services.Interfaces;

public interface ISubmissionService
{
    Task<IEnumerable<SubmissionListResponse>> GetSubmissionsAsync(Guid managerId);
    Task<SubmissionDetailResponse?> GetSubmissionDetailAsync(Guid submissionId);
    Task<SubmissionReviewResponse> ReviewSubmissionAsync(
        Guid submissionId,
        Guid managerId,
        SubmissionReviewRequest request
    );
}
