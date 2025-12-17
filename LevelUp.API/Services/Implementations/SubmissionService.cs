using System.Security.Claims;
using LevelUp.API.Data;
using LevelUp.API.DTOs.Submissions;
using LevelUp.API.Entity;
using LevelUp.API.Services.Interfaces;
using MentorHub.API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.API.Services.Implementations;

public class SubmissionService : ISubmissionService
{
        private readonly LevelUpDbContext _context;
    private readonly IEmailHandler _email;

    public SubmissionService(LevelUpDbContext context, IEmailHandler email)
    {
        _context = context;
        _email = email;
    }

    // ===============================
    // GET LIST SUBMISSIONS (MANAGER)
    // ===============================
    public async Task<IEnumerable<SubmissionListResponse>> GetSubmissionsAsync(Guid managerId)
    {
        var submissions = await _context.Submissions
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Items)
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Module!)
                    .ThenInclude(m => m.Items)
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Account!)
                    .ThenInclude(a => a.Employee)
            .Where(s =>
                // 🔒 FILTER MANAGER (INI KUNCI UTAMA)
                s.Enrollment!.Module!.CreatedBy == managerId

                // ✅ Enrollment selesai
                && s.Enrollment.Items.All(ei => ei.IsCompleted)

                // ✅ Module punya final submission
                && s.Enrollment.Module.Items.Any(mi => mi.IsFinalSubmission)
            )
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return submissions.Select(s =>
        {
            var completedCount = s.Enrollment!.Items.Count(ei => ei.IsCompleted);
            var totalCount = s.Enrollment.Module!.Items.Count;

            return new SubmissionListResponse(
                SubmissionId: s.Id,
                EnrollmentId: s.EnrollmentId,

                EmployeeId: s.Enrollment.Account!.Employee!.Id,
                EmployeeName:
                    $"{s.Enrollment.Account.Employee.FirstName} {s.Enrollment.Account.Employee.LastName}",
                Email: s.Enrollment.Account.Email!,

                ModuleId: s.Enrollment.Module!.Id,
                ModuleTitle: s.Enrollment.Module.Title!,

                CompletedCount: completedCount,
                TotalCount: totalCount,

                Status: s.Status.ToString(),
                SubmittedAt: s.CreatedAt
            );
        });
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
            submission.Status.ToString(),

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
 // APPROVE / REJECT SUBMISSION
    public async Task<SubmissionReviewResponse> ReviewSubmissionAsync(
        Guid submissionId,
        Guid managerId,
        SubmissionReviewRequest request
    )
    {
        var submission = await _context.Submissions
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Module!)
            .Include(s => s.Enrollment!)
                .ThenInclude(e => e.Account!)
            .FirstOrDefaultAsync(s => s.Id == submissionId);

        if (submission == null)
            throw new Exception("Submission not found");

        if (submission.Enrollment!.Module!.CreatedBy != managerId)
            throw new UnauthorizedAccessException("Not your submission");

        if (string.IsNullOrWhiteSpace(request.Status))
            throw new Exception("Status is required");

        if (!Enum.TryParse(request.Status, true, out SubmissionStatus parsedStatus))
            throw new Exception("Invalid submission status");

        if (parsedStatus == SubmissionStatus.Rejected)
        {
            if (string.IsNullOrWhiteSpace(request.ManagerFeedback))
                throw new Exception("Manager feedback is required");

            if (!request.EstimatedDays.HasValue || request.EstimatedDays <= 0)
                throw new Exception("Estimated days must be greater than 0");

            submission.Enrollment.TargetDate = AddWorkingDays(
                submission.Enrollment.TargetDate,
                request.EstimatedDays.Value
            );

            submission.ManagerFeedback = request.ManagerFeedback;
            submission.EstimatedDays = request.EstimatedDays;
        }
        else
        {
            submission.ManagerFeedback = null;
            submission.EstimatedDays = null;
        }

        submission.Status = parsedStatus;
        submission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        try
        {
            await _email.EmailAsync(new EmailDto(
                submission.Enrollment.Account!.Email!,
                parsedStatus == SubmissionStatus.Approved
                    ? "Submission Approved"
                    : "Submission Rejected",
                parsedStatus == SubmissionStatus.Approved
                    ? $"<p>Your submission for <b>{submission.Enrollment.Module!.Title}</b> is approved.</p>"
                    : $"<p>Your submission for <b>{submission.Enrollment.Module!.Title}</b> is <b>Rejected</b>.</p><br/><p>Feedback: {submission.ManagerFeedback}<br/>Estimated Days: {submission.EstimatedDays}</p>"
            ));
        }
        catch (Exception ex)
        {
            Console.WriteLine("EMAIL FAILED:");
            Console.WriteLine(ex.Message);
        }

        return new SubmissionReviewResponse(
            submission.Id,
            submission.Status.ToString(),
            submission.ManagerFeedback,
            submission.EstimatedDays
        );
    }

    // ===============================
    // HELPER: TAMBAH HARI KERJA
    // ===============================
    private static DateTime AddWorkingDays(
        DateTime startDate,
        int workingDays
    )
    {
        var date = startDate;
        var addedDays = 0;

        while (addedDays < workingDays)
        {
            date = date.AddDays(1);

            // Skip weekend
            if (date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            addedDays++;
        }

        return date;
    }
}
