# Use Cases and User Flows

## Actors

- Employee Learner: views own progress and assigned interventions.
- Trainer: reviews learner progress, identifies gaps, and records interventions.
- L&D Administrator: configures programs, rules, reports, and monitors overall learning compliance.
- Manager: reviews team risk and follows up with employees.
- Compliance Officer: reviews compliance reports and evidence.
- System Administrator: manages users, roles, integrations, and system configuration.

## UC-01 View Consolidated Learner Profile

Primary actor: Trainer, L&D Administrator, Manager, Employee Learner

Goal: View complete learning progress for an employee.

Main flow:

1. User searches for or opens an employee profile.
2. System displays employee identity and learning assignments.
3. System displays attendance, assessment scores, competency milestones, risk level, and intervention history.
4. User filters or drills into a specific program or competency.

Success outcome: User understands the learner's current learning status and gaps.

## UC-02 Ingest Learning Data

Primary actor: System Administrator

Goal: Bring external attendance, assessment, and competency milestone data into the system.

Main flow:

1. External source sends data through API or import.
2. System validates required fields and format.
3. System stores valid records.
4. System flags invalid or duplicate records for review.
5. System updates learner profiles and risk status.

Success outcome: Learning records are available for risk analysis and reporting.

## UC-03 Configure Risk Rule

Primary actor: L&D Administrator

Goal: Define a rule that identifies at-risk learners.

Main flow:

1. User creates a new risk rule.
2. User selects target population, condition, threshold, severity, and active period.
3. User tests the rule against sample or historical data.
4. System displays expected matches.
5. User activates the rule.

Success outcome: Rule is active and participates in risk classification.

## UC-04 Identify At-Risk Learners

Primary actor: Trainer, L&D Administrator

Goal: Review learners who require attention.

Main flow:

1. User opens the risk dashboard.
2. System displays learners grouped by risk level.
3. User filters by program, competency, department, manager, or due date.
4. User opens a learner to review triggered rules and learning gaps.
5. User creates or updates an intervention.

Success outcome: At-risk learners are identified with clear reasons and next actions.

## UC-05 Track Intervention

Primary actor: Trainer, Manager, L&D Administrator

Goal: Record support actions and outcomes.

Main flow:

1. User creates an intervention for a learner.
2. User selects intervention type, owner, due date, and expected outcome.
3. User records progress notes and completion status.
4. User records outcome after reassessment or follow-up.
5. System updates intervention history and learner status.

Success outcome: Intervention is traceable and outcome is measurable.

## UC-06 Generate Compliance Report

Primary actor: Compliance Officer, L&D Administrator

Goal: Produce compliance-ready evidence.

Main flow:

1. User selects report type, population, program, competency, and date range.
2. System generates report using current and historical learning data.
3. User reviews summary, exceptions, and detailed evidence.
4. User exports report.
5. System records report generation in audit history.

Success outcome: Compliance report is generated with traceable data.

## UC-07 Review Import Errors

Primary actor: System Administrator, L&D Administrator

Goal: Correct data quality issues.

Main flow:

1. User opens import monitoring page.
2. System displays failed, duplicate, or incomplete records.
3. User reviews error details.
4. User corrects the record or sends it back to the source system owner.
5. System reprocesses corrected records.

Success outcome: Data quality issues are resolved or tracked.
