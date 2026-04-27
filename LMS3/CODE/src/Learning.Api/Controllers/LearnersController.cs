using Learning.Application.Learners;
using Microsoft.AspNetCore.Mvc;

namespace Learning.Api.Controllers;

[ApiController]
[Route("api/learners")]
public sealed class LearnersController : ControllerBase
{
    private readonly ILearnerProfileService _learnerProfileService;

    public LearnersController(ILearnerProfileService learnerProfileService)
    {
        _learnerProfileService = learnerProfileService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<LearnerProfileSummaryDto>>> GetLearners(
        CancellationToken cancellationToken)
    {
        var learners = await _learnerProfileService.GetLearnersAsync(cancellationToken);

        return Ok(learners);
    }

    [HttpGet("{employeeId:guid}/profile")]
    public async Task<ActionResult<LearnerProfileDto>> GetProfile(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var profile = await _learnerProfileService.GetProfileAsync(employeeId, cancellationToken);

        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpGet("{employeeId:guid}/attendance-summary")]
    public async Task<ActionResult<AttendanceSummaryDto>> GetAttendanceSummary(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var summary = await _learnerProfileService.GetAttendanceSummaryAsync(employeeId, cancellationToken);

        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpGet("{employeeId:guid}/assessment-summary")]
    public async Task<ActionResult<AssessmentSummaryDto>> GetAssessmentSummary(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var summary = await _learnerProfileService.GetAssessmentSummaryAsync(employeeId, cancellationToken);

        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpGet("{employeeId:guid}/competency-progress")]
    public async Task<ActionResult<IReadOnlyCollection<CompetencyProgressDto>>> GetCompetencyProgress(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var progress = await _learnerProfileService.GetCompetencyProgressAsync(employeeId, cancellationToken);

        return progress is null ? NotFound() : Ok(progress);
    }

    [HttpGet("{employeeId:guid}/risk-status")]
    public async Task<ActionResult<RiskStatusDto>> GetRiskStatus(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var riskStatus = await _learnerProfileService.GetRiskStatusAsync(employeeId, cancellationToken);

        return riskStatus is null ? NotFound() : Ok(riskStatus);
    }

    [HttpGet("{employeeId:guid}/interventions")]
    public async Task<ActionResult<IReadOnlyCollection<InterventionHistoryItemDto>>> GetInterventionHistory(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var interventions = await _learnerProfileService.GetInterventionHistoryAsync(employeeId, cancellationToken);

        return interventions is null ? NotFound() : Ok(interventions);
    }

    [HttpGet("{employeeId:guid}/compliance-readiness")]
    public async Task<ActionResult<ComplianceReadinessDto>> GetComplianceReadiness(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var readiness = await _learnerProfileService.GetComplianceReadinessAsync(employeeId, cancellationToken);

        return readiness is null ? NotFound() : Ok(readiness);
    }

    [HttpGet("{employeeId:guid}/risk-history")]
    public async Task<ActionResult<IReadOnlyCollection<RiskHistoryItemDto>>> GetRiskHistory(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var riskHistory = await _learnerProfileService.GetRiskHistoryAsync(employeeId, cancellationToken);

        return riskHistory is null ? NotFound() : Ok(riskHistory);
    }

    [HttpGet("{employeeId:guid}/attendance-records")]
    public async Task<ActionResult<IReadOnlyCollection<AttendanceRecordDto>>> GetAttendanceRecords(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var records = await _learnerProfileService.GetAttendanceRecordsAsync(employeeId, cancellationToken);

        return records is null ? NotFound() : Ok(records);
    }

    [HttpGet("{employeeId:guid}/assessment-records")]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentRecordDto>>> GetAssessmentRecords(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        var records = await _learnerProfileService.GetAssessmentRecordsAsync(employeeId, cancellationToken);

        return records is null ? NotFound() : Ok(records);
    }
}
