using Learning.Application.Learners;
using Learning.Domain.Enums;
using Learning.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learning.Infrastructure.Learners;

public sealed class EfLearnerProfileService : ILearnerProfileService
{
    private readonly LearningDbContext _dbContext;

    public EfLearnerProfileService(LearningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<LearnerProfileSummaryDto>> GetLearnersAsync(CancellationToken cancellationToken)
    {
        var learners = await _dbContext.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.FullName)
            .Select(employee => new
            {
                employee.Id,
                employee.EmployeeNumber,
                employee.FullName,
                employee.Department,
                employee.JobRole
            })
            .ToArrayAsync(cancellationToken);

        var learnerIds = learners.Select(learner => learner.Id).ToArray();
        var latestRisks = await _dbContext.RiskAssessments
            .AsNoTracking()
            .Where(risk => learnerIds.Contains(risk.EmployeeId))
            .GroupBy(risk => risk.EmployeeId)
            .Select(group => new
            {
                EmployeeId = group.Key,
                RiskLevel = group.OrderByDescending(risk => risk.AssessedAt).First().RiskLevel
            })
            .ToDictionaryAsync(risk => risk.EmployeeId, risk => risk.RiskLevel, cancellationToken);
        var openInterventions = await _dbContext.Interventions
            .AsNoTracking()
            .Where(intervention => learnerIds.Contains(intervention.EmployeeId) && intervention.Status != InterventionStatus.Completed)
            .GroupBy(intervention => intervention.EmployeeId)
            .ToDictionaryAsync(group => group.Key, group => group.Count(), cancellationToken);
        var milestoneCounts = await _dbContext.CompetencyMilestones
            .AsNoTracking()
            .Where(milestone => learnerIds.Contains(milestone.EmployeeId))
            .GroupBy(milestone => milestone.EmployeeId)
            .Select(group => new
            {
                EmployeeId = group.Key,
                Completed = group.Count(milestone => milestone.Status == "Completed"),
                Overdue = group.Count(milestone => milestone.Status == "Overdue")
            })
            .ToDictionaryAsync(milestone => milestone.EmployeeId, cancellationToken);

        return learners
            .Select(learner =>
            {
                milestoneCounts.TryGetValue(learner.Id, out var milestones);
                return new LearnerProfileSummaryDto(
                    learner.Id,
                    learner.EmployeeNumber,
                    learner.FullName,
                    learner.Department,
                    learner.JobRole,
                    latestRisks.GetValueOrDefault(learner.Id, RiskLevel.None),
                    openInterventions.GetValueOrDefault(learner.Id),
                    milestones?.Completed ?? 0,
                    milestones?.Overdue ?? 0);
            })
            .ToArray();
    }

    public async Task<LearnerProfileDto?> GetProfileAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == employeeId, cancellationToken);

        if (employee is null)
        {
            return null;
        }

        var managerName = await _dbContext.SystemUsers
            .AsNoTracking()
            .Where(user => user.Id == employee.ManagerId)
            .Select(user => user.DisplayName)
            .FirstOrDefaultAsync(cancellationToken) ?? "Unassigned";

        var programIds = await _dbContext.AttendanceRecords
            .AsNoTracking()
            .Where(record => record.EmployeeId == employeeId)
            .Select(record => record.ProgramId)
            .Union(_dbContext.AssessmentResults
                .AsNoTracking()
                .Where(result => result.EmployeeId == employeeId)
                .Select(result => result.ProgramId))
            .Union(_dbContext.CompetencyMilestones
                .AsNoTracking()
                .Where(milestone => milestone.EmployeeId == employeeId)
                .Select(milestone => milestone.ProgramId))
            .Distinct()
            .ToArrayAsync(cancellationToken);

        var assignedPrograms = await _dbContext.LearningPrograms
            .AsNoTracking()
            .Where(program => programIds.Contains(program.Id))
            .OrderBy(program => program.Name)
            .Select(program => program.Name)
            .ToArrayAsync(cancellationToken);

        var attendanceSummary = await GetAttendanceSummaryAsync(employeeId, cancellationToken)
            ?? EmptyAttendanceSummary();
        var assessmentSummary = await GetAssessmentSummaryAsync(employeeId, cancellationToken)
            ?? EmptyAssessmentSummary();
        var competencyProgress = await GetCompetencyProgressAsync(employeeId, cancellationToken)
            ?? Array.Empty<CompetencyProgressDto>();
        var riskStatus = await GetRiskStatusAsync(employeeId, cancellationToken)
            ?? new RiskStatusDto(RiskLevel.None, "No risk assessment found.", Array.Empty<string>(), DateTimeOffset.MinValue);
        var compliance = await GetComplianceReadinessAsync(employeeId, cancellationToken)
            ?? new ComplianceReadinessDto(true, 0, 0, 0, "No required competencies found.");
        var interventions = await GetInterventionHistoryAsync(employeeId, cancellationToken)
            ?? Array.Empty<InterventionHistoryItemDto>();
        var riskHistory = await GetRiskHistoryAsync(employeeId, cancellationToken)
            ?? Array.Empty<RiskHistoryItemDto>();
        var attendanceRecords = await GetAttendanceRecordsAsync(employeeId, cancellationToken)
            ?? Array.Empty<AttendanceRecordDto>();
        var assessmentRecords = await GetAssessmentRecordsAsync(employeeId, cancellationToken)
            ?? Array.Empty<AssessmentRecordDto>();

        return new LearnerProfileDto(
            employee.Id,
            employee.EmployeeNumber,
            employee.FullName,
            employee.Department,
            employee.JobRole,
            managerName,
            assignedPrograms,
            attendanceSummary,
            assessmentSummary,
            competencyProgress,
            riskStatus,
            compliance,
            interventions,
            riskHistory,
            attendanceRecords,
            assessmentRecords);
    }

    public async Task<AttendanceSummaryDto?> GetAttendanceSummaryAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.AttendanceRecords
            .AsNoTracking()
            .Where(record => record.EmployeeId == employeeId)
            .ToArrayAsync(cancellationToken);

        if (records.Length == 0)
        {
            return null;
        }

        var attendedSessions = records.Count(record => record.Status is AttendanceStatus.Present or AttendanceStatus.PartiallyAttended);
        var missedSessions = records.Count(record => record.Status is AttendanceStatus.Absent);
        var averageAttendance = decimal.Round(records.Average(record => record.AttendancePercentage), 2);
        var lastAttendanceDate = records
            .OrderByDescending(record => record.AttendanceDate)
            .Select(record => record.AttendanceDate)
            .FirstOrDefault();

        return new AttendanceSummaryDto(
            records.Length,
            attendedSessions,
            averageAttendance,
            missedSessions,
            lastAttendanceDate);
    }

    public async Task<AssessmentSummaryDto?> GetAssessmentSummaryAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.AssessmentResults
            .AsNoTracking()
            .Where(record => record.EmployeeId == employeeId)
            .ToArrayAsync(cancellationToken);

        if (records.Length == 0)
        {
            return null;
        }

        var averageScore = decimal.Round(records.Average(record => record.Score), 2);
        var lastAssessmentDate = records
            .OrderByDescending(record => record.AssessmentDate)
            .Select(record => record.AssessmentDate)
            .FirstOrDefault();

        return new AssessmentSummaryDto(
            records.Length,
            averageScore,
            records.Count(record => record.Status == AssessmentStatus.Passed),
            records.Count(record => record.Status == AssessmentStatus.Failed),
            lastAssessmentDate);
    }

    public async Task<IReadOnlyCollection<CompetencyProgressDto>?> GetCompetencyProgressAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var milestones = await _dbContext.CompetencyMilestones
            .AsNoTracking()
            .Where(milestone => milestone.EmployeeId == employeeId)
            .OrderBy(milestone => milestone.DueDate)
            .ToArrayAsync(cancellationToken);

        if (milestones.Length == 0)
        {
            return null;
        }

        var competencyCodes = milestones.Select(milestone => milestone.CompetencyCode).Distinct().ToArray();
        var competencyNames = await _dbContext.Competencies
            .AsNoTracking()
            .Where(competency => competencyCodes.Contains(competency.Code))
            .ToDictionaryAsync(competency => competency.Code, competency => competency.Name, cancellationToken);

        return milestones
            .Select(milestone => new CompetencyProgressDto(
                milestone.CompetencyCode,
                competencyNames.GetValueOrDefault(milestone.CompetencyCode, milestone.CompetencyCode),
                milestone.Status,
                milestone.DueDate,
                milestone.CompletedOn))
            .ToArray();
    }

    public async Task<RiskStatusDto?> GetRiskStatusAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var latestRisk = await _dbContext.RiskAssessments
            .AsNoTracking()
            .Where(assessment => assessment.EmployeeId == employeeId)
            .OrderByDescending(assessment => assessment.AssessedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestRisk is null)
        {
            return null;
        }

        var triggeredRules = await _dbContext.RiskAssessmentTriggers
            .AsNoTracking()
            .Where(trigger => trigger.RiskAssessmentId == latestRisk.Id)
            .OrderBy(trigger => trigger.RuleName)
            .Select(trigger => trigger.RuleName)
            .ToArrayAsync(cancellationToken);

        return new RiskStatusDto(latestRisk.RiskLevel, latestRisk.Reason, triggeredRules, latestRisk.AssessedAt);
    }

    public async Task<IReadOnlyCollection<InterventionHistoryItemDto>?> GetInterventionHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var interventions = await _dbContext.Interventions
            .AsNoTracking()
            .Where(intervention => intervention.EmployeeId == employeeId)
            .OrderByDescending(intervention => intervention.DueDate)
            .ToArrayAsync(cancellationToken);

        if (interventions.Length == 0)
        {
            return null;
        }

        var ownerIds = interventions.Select(intervention => intervention.OwnerId).Distinct().ToArray();
        var owners = await _dbContext.SystemUsers
            .AsNoTracking()
            .Where(user => ownerIds.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, user => user.DisplayName, cancellationToken);

        var interventionIds = interventions.Select(intervention => intervention.Id).ToArray();
        var outcomes = await _dbContext.InterventionOutcomes
            .AsNoTracking()
            .Where(outcome => interventionIds.Contains(outcome.InterventionId))
            .GroupBy(outcome => outcome.InterventionId)
            .ToDictionaryAsync(
                group => group.Key,
                group => group.OrderByDescending(outcome => outcome.CreatedAt).First().Outcome,
                cancellationToken);

        return interventions
            .Select(intervention => new InterventionHistoryItemDto(
                intervention.Id,
                intervention.InterventionType,
                owners.GetValueOrDefault(intervention.OwnerId, "Unassigned"),
                intervention.Status.ToString(),
                intervention.DueDate,
                outcomes.TryGetValue(intervention.Id, out var outcome) ? outcome : intervention.Outcome))
            .ToArray();
    }

    public async Task<ComplianceReadinessDto?> GetComplianceReadinessAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var milestones = await _dbContext.CompetencyMilestones
            .AsNoTracking()
            .Where(milestone => milestone.EmployeeId == employeeId)
            .ToArrayAsync(cancellationToken);

        if (milestones.Length == 0)
        {
            return null;
        }

        var completed = milestones.Count(milestone => milestone.Status == "Completed");
        var overdue = milestones.Count(milestone => milestone.Status == "Overdue");
        var isCompliant = overdue == 0 && completed == milestones.Length;
        var summary = isCompliant
            ? "All required competencies are complete."
            : $"{milestones.Length - completed} required competencies are not complete.";

        return new ComplianceReadinessDto(isCompliant, milestones.Length, completed, overdue, summary);
    }

    public async Task<IReadOnlyCollection<RiskHistoryItemDto>?> GetRiskHistoryAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var assessments = await _dbContext.RiskAssessments
            .AsNoTracking()
            .Where(assessment => assessment.EmployeeId == employeeId)
            .OrderByDescending(assessment => assessment.AssessedAt)
            .ToArrayAsync(cancellationToken);

        if (assessments.Length == 0)
        {
            return null;
        }

        var assessmentIds = assessments.Select(assessment => assessment.Id).ToArray();
        var triggers = await _dbContext.RiskAssessmentTriggers
            .AsNoTracking()
            .Where(trigger => assessmentIds.Contains(trigger.RiskAssessmentId))
            .GroupBy(trigger => trigger.RiskAssessmentId)
            .ToDictionaryAsync(
                group => group.Key,
                group => group.Select(trigger => trigger.RuleName).ToArray(),
                cancellationToken);

        return assessments
            .Select(assessment => new RiskHistoryItemDto(
                assessment.Id,
                assessment.RiskLevel,
                assessment.Reason,
                assessment.AssessedAt,
                triggers.GetValueOrDefault(assessment.Id, Array.Empty<string>())))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<AttendanceRecordDto>?> GetAttendanceRecordsAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var records = await (
            from attendance in _dbContext.AttendanceRecords.AsNoTracking()
            join program in _dbContext.LearningPrograms.AsNoTracking() on attendance.ProgramId equals program.Id
            join session in _dbContext.TrainingSessions.AsNoTracking() on attendance.SessionId equals session.Id
            where attendance.EmployeeId == employeeId
            orderby attendance.AttendanceDate descending
            select new AttendanceRecordDto(
                attendance.Id,
                program.Name,
                session.Title,
                attendance.Status.ToString(),
                attendance.AttendancePercentage,
                attendance.AttendanceDate))
            .ToArrayAsync(cancellationToken);

        return records.Length == 0 ? null : records;
    }

    public async Task<IReadOnlyCollection<AssessmentRecordDto>?> GetAssessmentRecordsAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var records = await (
            from assessment in _dbContext.AssessmentResults.AsNoTracking()
            join program in _dbContext.LearningPrograms.AsNoTracking() on assessment.ProgramId equals program.Id
            join competency in _dbContext.Competencies.AsNoTracking() on assessment.CompetencyId equals competency.Id
            where assessment.EmployeeId == employeeId
            orderby assessment.AssessmentDate descending
            select new AssessmentRecordDto(
                assessment.Id,
                program.Name,
                competency.Name,
                assessment.AssessmentType,
                assessment.Score,
                assessment.Status.ToString(),
                assessment.AttemptNumber,
                assessment.AssessmentDate))
            .ToArrayAsync(cancellationToken);

        return records.Length == 0 ? null : records;
    }

    private static AttendanceSummaryDto EmptyAttendanceSummary()
    {
        return new AttendanceSummaryDto(0, 0, 0m, 0, DateOnly.MinValue);
    }

    private static AssessmentSummaryDto EmptyAssessmentSummary()
    {
        return new AssessmentSummaryDto(0, 0m, 0, 0, DateOnly.MinValue);
    }
}
