using LevelUp.API.DTOs.Enrolls;
using LevelUp.API.Entity;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;

namespace LevelUp.API.Services.Implementations;

public class EnrollService : IEnrollService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IModuleRepository _moduleRepository;
    private readonly IModuleItemRepository _moduleItemRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEnrollmentItemRepository _enrollmentItemRepository;

    public EnrollService(
        IEnrollmentRepository enrollmentRepository,
        IUnitOfWork unitOfWork,
        IModuleRepository moduleRepository,
        IModuleItemRepository moduleItemRepository,
        IAccountRepository accountRepository,
        IEnrollmentItemRepository enrollmentItemRepository,
        IEmployeeRepository employeeRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _unitOfWork = unitOfWork;
        _moduleRepository = moduleRepository;
        _moduleItemRepository = moduleItemRepository;
        _accountRepository = accountRepository;
        _enrollmentItemRepository = enrollmentItemRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<EnrollmentResponse> GetEnrollmentProgressAsync(Guid enrollmentId, string email, CancellationToken cancellationToken)
    {
        // 1. Resolve account dari email (JWT source of truth)
        var account = await _accountRepository
            .GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Account not found");

        // 2. Ambil enrollment MILIK user
        var enrollment = await _enrollmentRepository
            .GetByIdAndAccountIdAsync(enrollmentId, account.Id, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found");

        // 3. Ambil module
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        // 4. Ambil checklist items (WAJIB INCLUDE ModuleItem)
        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        if (!enrollmentItems.Any())
            throw new InvalidOperationException("Enrollment has no checklist items");

        // 5. Urutkan checklist
        var orderedItems = enrollmentItems
            .OrderBy(ei => ei.ModuleItem!.OrderIndex)
            .ToList();

        // 6. Mapping checklist
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

        // 7. Response
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
    string email,
    SubmitChecklistRequest request,
    CancellationToken cancellationToken)
    {
        // 1. Resolve ACCOUNT dari EMAIL (JWT = source of truth)
        var account = await _accountRepository
            .FirstOrDefaultAsync(a => a.Email == email);

        if (account is null)
            throw new InvalidOperationException("Account not found");

        // 2. Ambil EMPLOYEE
        var employee = await _employeeRepository
            .GetByAccountIdAsync(account.Id, cancellationToken);

        if (employee is null)
            throw new InvalidOperationException("Employee not found");

        if (!employee.IsIdle)
            throw new InvalidOperationException(
                "Checklist cannot be submitted while employee is not idle");

        // 3. Ambil ENROLLMENT (harus milik user)
        var enrollment = await _enrollmentRepository
            .GetByIdAndAccountIdAsync(enrollmentId, account.Id, cancellationToken);

        if (enrollment is null)
            throw new InvalidOperationException("Enrollment not found");

        if (enrollment.Status != EnrollmentStatus.OnGoing)
            throw new InvalidOperationException("Enrollment is not ongoing");

        // 4. Validasi hari kerja (Senin–Jumat)
        var today = DateTime.UtcNow.DayOfWeek;
        if (today is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new InvalidOperationException(
                "Checklist can only be submitted on working days (Monday–Friday)");

        // 5. Ambil enrollment items + ModuleItem
        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        if (!enrollmentItems.Any())
            throw new InvalidOperationException("Enrollment has no checklist items");

        var orderedItems = enrollmentItems
            .OrderBy(ei => ei.ModuleItem!.OrderIndex)
            .ToList();

        // 6. RULE: 1 CHECKLIST PER HARI (KECUALI FINAL SUBMISSION)
        var todayDate = DateTime.UtcNow.Date;

        var alreadySubmittedToday = orderedItems.Any(i =>
            i.IsCompleted &&
            i.CompletedAt.HasValue &&
            i.CompletedAt.Value.Date == todayDate);

        // ambil next checklist lebih awal
        var nextItem = orderedItems.FirstOrDefault(i => !i.IsCompleted);

        if (nextItem is null)
            throw new InvalidOperationException("All checklist items already completed");

        if (alreadySubmittedToday && !nextItem.ModuleItem!.IsFinalSubmission)
            throw new InvalidOperationException(
                "Only one checklist can be completed per working day");

        // 7. Anti skip (harus urut)
        if (nextItem.ModuleItemId != request.ModuleItemId)
            throw new InvalidOperationException(
                "Checklist must be completed in order");

        // 8. Final submission wajib evidence
        if (nextItem.ModuleItem!.IsFinalSubmission &&
            string.IsNullOrWhiteSpace(request.EvidenceUrl))
        {
            throw new InvalidOperationException(
                "Final assignment requires evidence URL");
        }

        // 9. Submit checklist
        nextItem.IsCompleted = true;
        nextItem.EvidenceUrl = request.EvidenceUrl;
        nextItem.Feedback = request.Feedback;
        nextItem.CompletedAt = DateTime.UtcNow;

        // 10. Update progress
        var completedCount = orderedItems.Count(i => i.IsCompleted);
        var totalCount = orderedItems.Count;

        enrollment.CurrentProgress =
            (int)Math.Round((double)completedCount / totalCount * 100);

        // 11. PENENTUAN STATUS ENROLLMENT
        if (completedCount == totalCount)
        {
            if (nextItem.ModuleItem.IsFinalSubmission)
            {
                // FINAL PROJECT MASUK TAHAP REVIEW
                enrollment.Status = EnrollmentStatus.OnGoing;
            }
            else
            {
                enrollment.Status = EnrollmentStatus.Completed;
                enrollment.CompletedDate = DateTime.UtcNow;
            }
        }

        enrollment.UpdatedAt = DateTime.UtcNow;

        // 12. TRANSACTION
        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _enrollmentItemRepository.UpdateAsync(nextItem);
            await _enrollmentRepository.UpdateAsync(enrollment);
        }, cancellationToken);

        // 13. Mapping response
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken);

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
            ModuleId: module!.Id,
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

    public async Task<EnrollmentResponse> EnrollAsync(string email, Guid moduleId, CancellationToken cancellationToken)
    {
        // 1. Ambil account dari email
        var account = await _accountRepository
            .FirstOrDefaultAsync(a => a.Email == email);

        if (account is null)
            throw new InvalidOperationException("Account not found");

        // 2. Ambil employee (opsional tapi relevan)
        var employee = await _employeeRepository
            .GetByAccountIdAsync(account.Id, cancellationToken);

        if (employee is null)
            throw new InvalidOperationException("Employee not found");

        // 3. Cek idle
        if (!employee.IsIdle)
            throw new InvalidOperationException("Employee is not idle");

        // 4. Cek enrollment aktif
        var activeEnrollment =
            await _enrollmentRepository
                .GetActiveByUserIdAsync(account.Id, cancellationToken);

        if (activeEnrollment is not null)
            throw new InvalidOperationException("User already has active enrollment");

        // 5. Ambil module
        var module = await _moduleRepository.GetByIdWithItemsAsync(moduleId, cancellationToken);

        if (module is null)
            throw new InvalidOperationException("Module not found");

        if (!module.Items.Any())
            throw new InvalidOperationException("Module has no sections");

        // 6. Create enrollment
        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id, // ✅ AKHIRNYA
            ModuleId = moduleId,
            StartDate = DateTime.UtcNow,
            TargetDate = DateTime.UtcNow.AddDays(module.EstimatedDays),
            Status = EnrollmentStatus.OnGoing,
            CreatedAt = DateTime.UtcNow,
            CurrentProgress = 0
        };

        // 7. Enrollment items
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

        await _unitOfWork.CommitTransactionAsync(async () =>
        {
            await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);
            await _enrollmentItemRepository.CreateManyAsync(enrollmentItems, cancellationToken);
        }, cancellationToken);

        // 7. Mapping ke response (UI-ready)
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

    public async Task<EnrollmentResponse?> GetCurrentEnrollmentAsync(string email, CancellationToken cancellationToken)
    {
        // 1. Ambil account dari email (JWT source of truth)
        var account =
            await _accountRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Account not found");

        // 2. Ambil employee
        var employee =
            await _employeeRepository.GetByAccountIdAsync(account.Id, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        // 3. Ambil enrollment aktif
        var activeEnrollment =
            await _enrollmentRepository.GetActiveByUserIdAsync(account.Id, cancellationToken);

        // 4. Jika employee tidak idle → auto pause
        if (!employee.IsIdle)
        {
            if (activeEnrollment is not null &&
                activeEnrollment.Status == EnrollmentStatus.OnGoing)
            {
                activeEnrollment.Status = EnrollmentStatus.Paused;
                activeEnrollment.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CommitTransactionAsync(async () =>
                {
                    await _enrollmentRepository.UpdateAsync(activeEnrollment);
                }, cancellationToken);
            }

            return null; // learning disembunyikan
        }

        // 5. Idle tapi tidak ada enrollment
        if (activeEnrollment is null)
            return null;

        // 6. Ambil module
        var module =
            await _moduleRepository.GetByIdAsync(activeEnrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        // 7. Ambil enrollment items
        var enrollmentItems =
            await _enrollmentItemRepository.GetByEnrollmentIdAsync(
                activeEnrollment.Id,
                cancellationToken);

        // 8. Mapping sections
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

        // 9. Return response
        return new EnrollmentResponse(
            EnrollmentId: activeEnrollment.Id,
            ModuleId: module.Id,
            ModuleTitle: module.Title!,
            ModuleDescription: module.Description!,
            StartDate: activeEnrollment.StartDate,
            TargetDate: activeEnrollment.TargetDate,
            CompletedDate: activeEnrollment.CompletedDate,
            Status: activeEnrollment.Status,
            CurrentProgress: activeEnrollment.CurrentProgress,
            Sections: sections
        );
    }

    public async Task<List<EnrollmentResponse>> GetEnrollmentHistoryAsync(string email, CancellationToken cancellationToken)
    {
        // 1. Ambil account dari email (JWT source of truth)
        var account = await _accountRepository
            .GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Account not found");

        // 2. Validasi employee (business rule)
        _ = await _employeeRepository
            .GetByAccountIdAsync(account.Id, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        // 3. Ambil enrollment COMPLETED
        var completedEnrollments =
            await _enrollmentRepository
                .GetCompletedByUserIdAsync(account.Id, cancellationToken);

        if (!completedEnrollments.Any())
            return new List<EnrollmentResponse>();

        var responses = new List<EnrollmentResponse>();

        foreach (var enrollment in completedEnrollments)
        {
            // 4. Ambil module
            var module = await _moduleRepository
                .GetByIdAsync(enrollment.ModuleId, cancellationToken)
                ?? throw new InvalidOperationException("Module not found");

            // 5. Ambil enrollment items + ModuleItem
            var enrollmentItems =
                await _enrollmentItemRepository
                    .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

            // 6. Mapping sections
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

    public async Task<EnrollmentResponse> ResumeEnrollmentAsync(Guid enrollmentId, string email, CancellationToken cancellationToken)
    {
        // 1. Ambil account dari email (JWT source of truth)
        var account = await _accountRepository
            .GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Account not found");

        // 2. Ambil employee
        var employee = await _employeeRepository
            .GetByAccountIdAsync(account.Id, cancellationToken)
            ?? throw new InvalidOperationException("Employee not found");

        // 3. Employee harus idle
        if (!employee.IsIdle)
            throw new InvalidOperationException(
                "Enrollment cannot be resumed while employee is not idle");

        // 4. Ambil enrollment
        var enrollment = await _enrollmentRepository
            .GetByIdAsync(enrollmentId, cancellationToken)
            ?? throw new InvalidOperationException("Enrollment not found");

        // 5. Enrollment harus MILIK employee (ANTI-HACK)
        if (enrollment.AccountId != account.Id)
            throw new InvalidOperationException("Access denied");

        // 6. Enrollment harus PAUSED
        if (enrollment.Status != EnrollmentStatus.Paused)
            throw new InvalidOperationException(
                "Only paused enrollment can be resumed");

        // 7. Resume enrollment
        enrollment.Status = EnrollmentStatus.OnGoing;
        enrollment.UpdatedAt = DateTime.UtcNow;

        await _enrollmentRepository.UpdateAsync(enrollment);

        // 8. Ambil module + enrollment items
        var module = await _moduleRepository
            .GetByIdAsync(enrollment.ModuleId, cancellationToken)
            ?? throw new InvalidOperationException("Module not found");

        var enrollmentItems = await _enrollmentItemRepository
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);

        // 9. Mapping sections
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

        // 10. Response
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
