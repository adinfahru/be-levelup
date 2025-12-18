using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;

namespace LevelUp.API.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IModuleRepository _moduleRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEnrollmentItemRepository _enrollmentItemRepository;
    private readonly IEmailHandler _emailHandler;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork,
        IModuleRepository moduleRepository,
        IModuleItemRepository moduleItemRepository,
        ISubmissionRepository submissionRepository,
        IAccountRepository accountRepository,
        IEnrollmentItemRepository enrollmentItemRepository,
        IEmployeeRepository employeeRepository,
        IEmailHandler emailHandler)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _moduleRepository = moduleRepository;
        _enrollmentItemRepository = enrollmentItemRepository;
        _employeeRepository = employeeRepository;
        _submissionRepository = submissionRepository;
        _emailHandler = emailHandler;
    }

    public async Task<EnrollmentResponse> GetEnrollmentProgressAsync(
    Guid enrollmentId,
    Guid accountId,
    CancellationToken cancellationToken)
    {
        // 1. Ambil enrollment (HARUS milik account)
        var enrollment = await _enrollmentRepository
            .GetByIdAndAccountIdAsync(enrollmentId, accountId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found");

        // 2. Ambil module
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        // 3. Ambil checklist items (WAJIB include ModuleItem)
        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        if (!enrollmentItems.Any())
            throw new InvalidOperationException("Enrollment has no checklist items");

        // 4. Urutkan checklist
        var orderedItems = enrollmentItems
            .OrderBy(ei => ei.ModuleItem!.OrderIndex)
            .ToList();

        // 5. Mapping checklist
        var sections = orderedItems.Select(ei => new EnrollmentItemDto(
            EnrollmentItemId: ei.Id,
            ModuleItemId: ei.ModuleItemId,
            OrderIndex: ei.ModuleItem!.OrderIndex,
            ModuleItemTitle: ei.ModuleItem.Title!,
            ModuleItemDescription: ei.ModuleItem.Descriptions!,
            ModuleItemUrl: ei.ModuleItem.Url!,
            IsFinalSubmission: ei.ModuleItem.IsFinalSubmission,
            IsCompleted: ei.IsCompleted,
            EvidenceUrl: ei.EvidenceUrl,
            CompletedAt: ei.CompletedAt
        )).ToList();

        // 6. Response
        return new EnrollmentResponse(
            EnrollmentId: enrollment.Id,
            ModuleId: module.Id,
            ModuleTitle: module.Title!,
            ModuleDescription: module.Description!,
            StartDate: enrollment.StartDate,
            TargetDate: enrollment.TargetDate,
            CompletedDate: enrollment.CompletedDate,
            Status: enrollment.Status,
            CurrentProgress: enrollment.CurrentProgress,
            Sections: sections
        );
    }


    public async Task<EnrollmentResponse> SubmitEnrollmentItemAsync(
    Guid enrollmentId,
    Guid accountId,
    SubmitChecklistRequest request,
    CancellationToken cancellationToken)
    {
        // 1. Ambil employee
        var employee = await _employeeRepository
            .GetByAccountIdAsync(accountId, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        if (!employee.IsIdle)
            throw new InvalidOperationException(
                "Checklist cannot be submitted while employee is not idle");

        // 2. Ambil enrollment (HARUS milik employee)
        var enrollment = await _enrollmentRepository
            .GetByIdAndAccountIdAsync(enrollmentId, accountId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found");

        if (enrollment.Status != EnrollmentStatus.OnGoing)
            throw new InvalidOperationException("Enrollment is not ongoing");

        // 3. Validasi hari kerja (Senin–Jumat)
        var today = DateTime.UtcNow.DayOfWeek;
        if (today is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException(
                "Checklist can only be submitted on working days (Monday–Friday)");

        // 4. Ambil enrollment items + ModuleItem
        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        if (!enrollmentItems.Any())
            throw new InvalidOperationException("Enrollment has no checklist items");

        var orderedItems = enrollmentItems
            .OrderBy(ei => ei.ModuleItem!.OrderIndex)
            .ToList();

        // 5. RULE: 1 checklist per hari (kecuali final submission)
        var todayDate = DateTime.UtcNow.Date;

        var alreadySubmittedToday = orderedItems.Any(i =>
            i.IsCompleted &&
            i.CompletedAt.HasValue &&
            i.CompletedAt.Value.Date == todayDate);

        var nextItem = orderedItems.FirstOrDefault(i => !i.IsCompleted)
            ?? throw new InvalidOperationException("All checklist items already completed");

        if (alreadySubmittedToday && !nextItem.ModuleItem!.IsFinalSubmission)
            throw new InvalidOperationException(
                "Only one checklist can be completed per working day");

        // 6. Anti skip (harus urut)
        if (nextItem.ModuleItemId != request.ModuleItemId)
            throw new InvalidOperationException(
                "Checklist must be completed in order");

        // 7. Final submission wajib evidence
        if (nextItem.ModuleItem != null &&
            nextItem.ModuleItem.IsFinalSubmission &&
            string.IsNullOrWhiteSpace(request.EvidenceUrl))
        {
            throw new InvalidOperationException(
                "Final assignment requires evidence URL");
        }

        // 8. Submit checklist
        nextItem.IsCompleted = true;
        nextItem.EvidenceUrl = request.EvidenceUrl;
        nextItem.Feedback = request.Feedback;
        nextItem.CompletedAt = DateTime.UtcNow;

        // 8.5 HANDLE FINAL SUBMISSION (CREATE / RESUBMIT)
        // 8.5 HANDLE FINAL SUBMISSION (CREATE / RESUBMIT)
        if (nextItem.ModuleItem!.IsFinalSubmission)
        {
            var submission = await _submissionRepository
                .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

            var isResubmission = false;

            if (submission == null)
            {
                // 🆕 first time final submission
                submission = new Submission
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = enrollment.Id,
                    Status = SubmissionStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _submissionRepository.CreateAsync(submission, cancellationToken);
            }
            else if (submission.Status == SubmissionStatus.Rejected)
            {
                // 🔁 resubmission after rejected
                submission.Status = SubmissionStatus.Pending;
                submission.ManagerFeedback = null;
                submission.EstimatedDays = null;
                submission.UpdatedAt = DateTime.UtcNow;

                isResubmission = true;

                await _submissionRepository.UpdateAsync(submission);
            }

            try
            {
                var enrollmodule = await _moduleRepository.GetByIdWithCreatorAsync(enrollment.ModuleId, cancellationToken);

                await _emailHandler.EmailAsync(new EmailDto(
                    enrollmodule.Creator.Email!,
                    isResubmission
                        ? "Submission Resubmitted"
                        : "New Final Submission",
                    isResubmission
                        ? $"<p>The final submission for <b>{enrollmodule.Title}</b> has been resubmitted and is ready for review.</p>"
                        : $"<p>A new final submission for <b>{enrollmodule.Title}</b> has been submitted and requires your review.</p>"
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMAIL FAILED:");
                Console.WriteLine(ex.Message);
            }
        }


        // 9. Update progress
        var completedCount = orderedItems.Count(i => i.IsCompleted);
        var totalCount = orderedItems.Count;

        enrollment.CurrentProgress =
            (int)Math.Round((double)completedCount / totalCount * 100);

        // 10. Penentuan status enrollment
        if (completedCount == totalCount)
        {
            if (nextItem.ModuleItem != null &&
                !nextItem.ModuleItem.IsFinalSubmission)
            {
                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.CompletedDate = DateTime.UtcNow;
            }

            // enrollment.Status = EnrollmentStatus.Completed; ini buat pengecekan tanpa review manager
            // enrollment.CompletedDate = DateTime.UtcNow;
            // kalau final submission → tetap OnGoing (menunggu review)
        }

        enrollment.UpdatedAt = DateTime.UtcNow;

        // 11. Transaction
        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _enrollmentItemRepository.UpdateAsync(nextItem);
            await _enrollmentRepository.UpdateAsync(enrollment);
        }, cancellationToken);

        // 12. Mapping response
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        var sections = orderedItems.Select(ei => new EnrollmentItemDto(
            EnrollmentItemId: ei.Id,
            ModuleItemId: ei.ModuleItemId,
            OrderIndex: ei.ModuleItem!.OrderIndex,
            ModuleItemTitle: ei.ModuleItem.Title!,
            ModuleItemDescription: ei.ModuleItem.Descriptions!,
            ModuleItemUrl: ei.ModuleItem.Url!,
            IsFinalSubmission: ei.ModuleItem.IsFinalSubmission,
            IsCompleted: ei.IsCompleted,
            EvidenceUrl: ei.EvidenceUrl,
            CompletedAt: ei.CompletedAt
        )).ToList();

        return new EnrollmentResponse(
            EnrollmentId: enrollment.Id,
            ModuleId: module.Id,
            ModuleTitle: module.Title!,
            ModuleDescription: module.Description!,
            StartDate: enrollment.StartDate,
            TargetDate: enrollment.TargetDate,
            CompletedDate: enrollment.CompletedDate,
            Status: enrollment.Status,
            CurrentProgress: enrollment.CurrentProgress,
            Sections: sections
        );
    }

    public async Task<EnrollmentResponse> EnrollAsync(
    Guid accountId,
    Guid moduleId,
    CancellationToken cancellationToken)
    {
        // 1. Ambil employee (berdasarkan accountId dari JWT)
        var employee = await _employeeRepository
            .GetByAccountIdAsync(accountId, cancellationToken);

        if (employee is null)
            throw new InvalidOperationException("Employee not found");

        // 2. Cek idle
        if (!employee.IsIdle)
            throw new InvalidOperationException("Employee is not idle");

        // 3. Cek enrollment aktif
        var activeEnrollment =
            await _enrollmentRepository
                .GetActiveByUserIdAsync(accountId, cancellationToken);

        if (activeEnrollment is not null)
            throw new InvalidOperationException("User already has active enrollment");

        // 3.1 Cek apakah module PERNAH diselesaikan
        var hasCompletedModule =
            await _enrollmentRepository
                .HasCompletedModuleAsync(accountId, moduleId, cancellationToken);

        if (hasCompletedModule)
            throw new InvalidOperationException(
                "You have already completed this module");


        // 4. Ambil module + items
        var module = await _moduleRepository
            .GetByIdWithItemsAsync(moduleId, cancellationToken);

        if (module is null)
            throw new InvalidOperationException("Module not found");

        if (!module.Items.Any())
            throw new InvalidOperationException("Module has no sections");

        // 5. Create enrollment
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            AccountId = accountId, // ✅ source of truth
            ModuleId = moduleId,
            StartDate = DateTime.UtcNow,
            TargetDate = DateTime.UtcNow.AddDays(module.EstimatedDays),
            Status = EnrollmentStatus.OnGoing,
            CreatedAt = DateTime.UtcNow,
            CurrentProgress = 0
        };

        // 6. Generate enrollment items
        var enrollmentItems = module.Items
            .OrderBy(i => i.OrderIndex)
            .Select(i => new EnrollmentItem
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollment.Id,
                ModuleItemId = i.Id,
                IsCompleted = false
            })
            .ToList();

        // 7. Transaction
        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);
            await _enrollmentItemRepository.CreateManyAsync(enrollmentItems, cancellationToken);
        }, cancellationToken);

        // 8. Mapping response (UI-ready)
        var sections = enrollmentItems.Select(ei =>
        {
            var item = module.Items.First(i => i.Id == ei.ModuleItemId);

            return new EnrollmentItemDto(
                EnrollmentItemId: ei.Id,
                ModuleItemId: item.Id,
                OrderIndex: item.OrderIndex,
                ModuleItemTitle: item.Title!,
                ModuleItemDescription: item.Descriptions!,
                ModuleItemUrl: item.Url!,
                IsFinalSubmission: item.IsFinalSubmission,
                IsCompleted: false,
                EvidenceUrl: null,
                CompletedAt: null
            );
        }).ToList();

        return new EnrollmentResponse(
            EnrollmentId: enrollment.Id,
            ModuleId: module.Id,
            ModuleTitle: module.Title!,
            ModuleDescription: module.Description!,
            StartDate: enrollment.StartDate,
            TargetDate: enrollment.TargetDate,
            CompletedDate: null,
            Status: enrollment.Status,
            CurrentProgress: 0,
            Sections: sections
        );
    }


    public async Task<EnrollmentResponse?> GetCurrentEnrollmentAsync(
    Guid accountId,
    CancellationToken cancellationToken)
    {
        // 1. Ambil employee
        var employee = await _employeeRepository
            .GetByAccountIdAsync(accountId, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        // 2. Ambil enrollment current (OnGoing / Paused / WaitingReview)
        var enrollment = await _enrollmentRepository
            .GetActiveByUserIdAsync(accountId, cancellationToken);

        if (enrollment is null)
            return null;

        // 3. Auto-pause kalau employee tidak idle
        if (!employee.IsIdle && enrollment.Status == EnrollmentStatus.OnGoing)
        {
            enrollment.Status = EnrollmentStatus.Paused;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        // 4. Ambil module
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        // 5. Ambil enrollment items
        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        var sections = enrollmentItems
            .OrderBy(ei => ei.ModuleItem!.OrderIndex)
            .Select(ei => new EnrollmentItemDto(
                EnrollmentItemId: ei.Id,
                ModuleItemId: ei.ModuleItemId,
                OrderIndex: ei.ModuleItem!.OrderIndex,
                ModuleItemTitle: ei.ModuleItem.Title!,
                ModuleItemDescription: ei.ModuleItem.Descriptions!,
                ModuleItemUrl: ei.ModuleItem.Url!,
                IsFinalSubmission: ei.ModuleItem.IsFinalSubmission,
                IsCompleted: ei.IsCompleted,
                EvidenceUrl: ei.EvidenceUrl,
                CompletedAt: ei.CompletedAt
            ))
            .ToList();

        // 6. Return ke FE (PAUSED TETAP DIKIRIM)
        return new EnrollmentResponse(
            EnrollmentId: enrollment.Id,
            ModuleId: module.Id,
            ModuleTitle: module.Title!,
            ModuleDescription: module.Description!,
            StartDate: enrollment.StartDate,
            TargetDate: enrollment.TargetDate,
            CompletedDate: enrollment.CompletedDate,
            Status: enrollment.Status,
            CurrentProgress: enrollment.CurrentProgress,
            Sections: sections
        );
    }


    public async Task<List<EnrollmentResponse>> GetEnrollmentHistoryAsync(
    Guid accountId,
    CancellationToken cancellationToken)
    {
        // 1. Validasi employee (business rule)
        _ = await _employeeRepository
            .GetByAccountIdAsync(accountId, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        // 2. Ambil enrollment COMPLETED
        var completedEnrollments =
            await _enrollmentRepository
                .GetCompletedByUserIdAsync(accountId, cancellationToken);

        if (!completedEnrollments.Any())
            return new List<EnrollmentResponse>();

        var responses = new List<EnrollmentResponse>();

        foreach (var enrollment in completedEnrollments)
        {
            // 3. Ambil module
            var module = await _moduleRepository
                .GetByIdAsync(enrollment.ModuleId, cancellationToken)
                ?? throw new InvalidOperationException("Module not found");

            // 4. Ambil enrollment items + ModuleItem
            var enrollmentItems =
                await _enrollmentItemRepository
                    .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

            // 5. Mapping sections
            var sections = enrollmentItems
                .OrderBy(ei => ei.ModuleItem!.OrderIndex)
                .Select(ei => new EnrollmentItemDto(
                    EnrollmentItemId: ei.Id,
                    ModuleItemId: ei.ModuleItemId,
                    OrderIndex: ei.ModuleItem!.OrderIndex,
                    ModuleItemTitle: ei.ModuleItem.Title!,
                    ModuleItemDescription: ei.ModuleItem.Descriptions!,
                    ModuleItemUrl: ei.ModuleItem.Url!,
                    IsFinalSubmission: ei.ModuleItem.IsFinalSubmission,
                    IsCompleted: ei.IsCompleted,
                    EvidenceUrl: ei.EvidenceUrl,
                    CompletedAt: ei.CompletedAt
                ))
                .ToList();

            responses.Add(new EnrollmentResponse(
                EnrollmentId: enrollment.Id,
                ModuleId: module.Id,
                ModuleTitle: module.Title!,
                ModuleDescription: module.Description!,
                StartDate: enrollment.StartDate,
                TargetDate: enrollment.TargetDate,
                CompletedDate: enrollment.CompletedDate,
                Status: enrollment.Status,
                CurrentProgress: enrollment.CurrentProgress,
                Sections: sections
            ));
        }

        return responses;
    }


    public async Task<EnrollmentResponse> ResumeEnrollmentAsync(
    Guid enrollmentId,
    Guid accountId,
    CancellationToken cancellationToken)
    {
        // 1. Ambil employee berdasarkan accountId (JWT)
        var employee = await _employeeRepository
            .GetByAccountIdAsync(accountId, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        // 2. Employee harus idle
        if (!employee.IsIdle)
            throw new InvalidOperationException(
                "Enrollment cannot be resumed while employee is not idle");

        // 3. Ambil enrollment MILIK account (ANTI-HACK)
        var enrollment = await _enrollmentRepository
            .GetByIdAndAccountIdAsync(enrollmentId, accountId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found");

        // 4. Enrollment harus PAUSED
        if (enrollment.Status != EnrollmentStatus.Paused)
            throw new InvalidOperationException(
                "Only paused enrollment can be resumed");

        // 5. Resume enrollment
        enrollment.Status = EnrollmentStatus.OnGoing;
        enrollment.UpdatedAt = DateTime.UtcNow;

        await _enrollmentRepository.UpdateAsync(enrollment);

        // 6. Ambil module + enrollment items
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        // 7. Mapping sections
        var sections = enrollmentItems
            .OrderBy(ei => ei.ModuleItem!.OrderIndex)
            .Select(ei => new EnrollmentItemDto(
                EnrollmentItemId: ei.Id,
                ModuleItemId: ei.ModuleItemId,
                OrderIndex: ei.ModuleItem!.OrderIndex,
                ModuleItemTitle: ei.ModuleItem.Title!,
                ModuleItemDescription: ei.ModuleItem.Descriptions!,
                ModuleItemUrl: ei.ModuleItem.Url!,
                IsFinalSubmission: ei.ModuleItem.IsFinalSubmission,
                IsCompleted: ei.IsCompleted,
                EvidenceUrl: ei.EvidenceUrl,
                CompletedAt: ei.CompletedAt
            ))
            .ToList();

        // 8. Response
        return new EnrollmentResponse(
            EnrollmentId: enrollment.Id,
            ModuleId: module.Id,
            ModuleTitle: module.Title!,
            ModuleDescription: module.Description!,
            StartDate: enrollment.StartDate,
            TargetDate: enrollment.TargetDate,
            CompletedDate: enrollment.CompletedDate,
            Status: enrollment.Status,
            CurrentProgress: enrollment.CurrentProgress,
            Sections: sections
        );
    }


}
