# Functional Requirements

## FR-01 Learner Profile Aggregation

- The system shall create a consolidated learning profile for each employee.
- The profile shall include employee identity, department, role, training programs, attendance, assessment scores, competency milestones, risk status, and intervention history.
- The system shall support viewing learning progress by employee, course/program, competency, department, manager, and compliance group.
- The system shall show the latest known status and historical trend where available.

## FR-02 Attendance Data Ingestion

- The system shall ingest employee training attendance records through APIs or file-based imports where APIs are unavailable.
- The system shall support attendance status values such as present, absent, partially attended, excused, and pending.
- The system shall validate required fields before accepting records.
- The system shall flag duplicate, incomplete, or inconsistent attendance records.

## FR-03 Assessment Score Ingestion

- The system shall ingest periodic assessment scores from external systems.
- The system shall support multiple score formats, such as percentage, pass/fail, grade bands, and competency rating levels.
- The system shall store assessment date, score, attempt number, assessment type, and source system.
- The system shall support score recalculation or correction with audit history.

## FR-04 Competency Milestone Tracking

- The system shall track competency-level milestones for each employee.
- The system shall support milestone statuses such as not started, in progress, completed, overdue, failed, waived, and exempted.
- The system shall track due dates, completion dates, evidence, and source system references.
- The system shall show competency progression over time.

## FR-05 Configurable Risk Rules

- The system shall allow authorized users to configure risk rules.
- Risk rules shall support attendance thresholds, score thresholds, overdue milestones, repeated failed attempts, missed sessions, and composite conditions.
- Each rule shall include name, description, condition, severity, target population, active dates, owner, and version.
- The system shall support rule testing before activation.

## FR-06 At-Risk Learner Classification

- The system shall classify learners into risk levels such as low, medium, high, and critical.
- Risk classification shall be explainable by showing which rules triggered the status.
- The system shall allow filtering and sorting learners by risk level, department, manager, program, competency, and due date.
- The system shall preserve risk history for audit and trend analysis.

## FR-07 Intervention Management

- The system shall allow trainers or L&D administrators to create interventions for at-risk learners.
- Intervention types shall include remedial training, coaching, mentoring, manager follow-up, reassessment, and learning plan adjustment.
- Each intervention shall include owner, due date, status, notes, planned action, actual action, and outcome.
- The system shall track whether the learner improved after intervention.

## FR-08 Notifications and Escalations

- The system shall notify relevant users when a learner becomes high risk or critical risk.
- The system shall notify intervention owners when actions are overdue.
- The system shall support escalation to managers or administrators based on configurable rules.
- Notifications shall include enough context to act without exposing unnecessary sensitive data.

## FR-09 Dashboards

- The system shall provide dashboards for learner risk, competency progress, attendance gaps, assessment performance, intervention status, and compliance readiness.
- Dashboards shall support filtering by department, program, competency, manager, location, role, risk level, and date range.
- Dashboards shall show both summary metrics and drill-down details.

## FR-10 Compliance Reporting

- The system shall generate compliance-ready reports showing training completion, attendance, assessment status, competency status, exceptions, and intervention evidence.
- Reports shall support export to CSV, Excel, or PDF.
- Reports shall include generated date, filters used, data source references, and report owner.
- The system shall maintain audit history for report generation.

## FR-11 Administration

- The system shall allow administrators to manage user roles, data source configuration, risk rule configuration, lookup values, and report settings.
- The system shall provide import status monitoring and error review.
- The system shall support configuration changes with audit history.

## FR-12 Audit and History

- The system shall record key user actions such as rule changes, intervention updates, report generation, and data corrections.
- The system shall preserve historical learning records and risk status changes.
- The system shall support audit review by authorized users.
