using LevelUp.API.Data;
using LevelUp.API.DTOs.Submissions;
using LevelUp.API.Entity;
using LevelUp.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Services.Implementations;

public class SubmissionService : ISubmissionService
{
    private readonly LevelUpDbContext _context;

    public SubmissionService(LevelUpDbContext context)
    {
        _context = context;
    }

    // ===============================
    // GET LIST SUBMISSIONS (MANAGER)
    // ===============================
    public async Task<IEnumerable<SubmissionListResponse>> GetSubmissionsAsync()
    {
        var submissions = await _context.Submissions
            .Include(s => s.Enrollment)
                .ThenInclude(e => e.Account)
                    .ThenInclude(a => a.Employee)
            .Include(s => s.Enrollment)
                .ThenInclude(e => e.Module)
                    .ThenInclude(m => m.Items)
            .Include(s => s.Enrollment)
                .ThenInclude(e => e.Items)
            .OrderByDescending(s => s.CreatedAt) // 🔥 ORDER DI ENTITY
            .ToListAsync();

        return submissions.Select(s => new SubmissionListResponse(
            s.Id,
            s.EnrollmentId,

            s.Enrollment!.Account!.Employee!.Id,
            s.Enrollment.Account.Employee.FirstName + " " +
            s.Enrollment.Account.Employee.LastName,
            s.Enrollment.Account.Email!,

            s.Enrollment.Module!.Id,
            s.Enrollment.Module.Title!,

            s.Enrollment.Items.Count(ei => ei.IsCompleted),
            s.Enrollment.Module.Items.Count,

            s.Status,
            s.CreatedAt
        ));
    }

    // ===============================
    // GET SUBMISSION DETAIL
    // ===============================
    public async Task<SubmissionDetailResponse?> GetSubmissionDetailAsync(Guid submissionId)
    {
        var submission = await _context.Submissions
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Account!)
                    .ThenInclude(a => a.Employee)
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Module!)
                    .ThenInclude(m => m.Items)
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Items)
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (submission == null)
            return null;

        var moduleItems = submission.Enrollment!.Module!.Items
            .OrderBy(mi => mi.OrderIndex)
            .ToList();

        var enrollmentItems = submission.Enrollment.Items;

        var completedCount = enrollmentItems.Count(ei => ei.IsCompleted);
        var totalCount = moduleItems.Count;

        var sections = moduleItems.Select(item =>
        {
            var enrollmentItem = enrollmentItems
                .FirstOrDefault(ei => ei.ModuleItemId == item.Id);

            var status =
                enrollmentItem?.IsCompleted == true
                    ? "Completed"
                    : enrollmentItem != null
                        ? "OnProgress"
                        : "Locked";

            return new SubmissionSectionResponse(
                item.OrderIndex,
                item.Title!,
                enrollmentItem?.Feedback,
                enrollmentItem?.EvidenceUrl,
                status
            );
        }).ToList();

        return new SubmissionDetailResponse(
            submission.Id,
            submission.Status,

            submission.Enrollment.Account!.Employee!.FirstName + " " +
            submission.Enrollment.Account.Employee.LastName,
            submission.Enrollment.Account.Email!,

            submission.Enrollment.Module!.Title!,

            submission.Notes,
            submission.ManagerFeedback,

            completedCount,
            totalCount,

            sections
        );
    }

    // ===============================
    // APPROVE / REJECT SUBMISSION
    // ===============================
    public async Task<SubmissionReviewResponse> ReviewSubmissionAsync(
        Guid submissionId,
        SubmissionReviewRequest request
    )
    {
        var submission = await _context.Submissions
            .Include(s => s.Enrollment)
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (submission == null)
            throw new Exception("Submission not found");

        submission.Status = request.Status;

        if (request.Status == SubmissionStatus.Rejected)
        {
            submission.ManagerFeedback = request.ManagerFeedback;
        }
        else
        {
            submission.ManagerFeedback = null;
        }

        submission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new SubmissionReviewResponse(
            submission.Id,
            submission.Status,
            submission.ManagerFeedback,
            submission.UpdatedAt!.Value
        );
    }
}
