# Acceptance Criteria and Requirement Traceability Matrix

## Acceptance Criteria

## AC-01 Learner Profile Aggregation

- Given valid attendance, assessment, and competency data exists for an employee, when a user opens the learner profile, then the system shows consolidated learning progress.
- Given a learner has multiple programs, when the user filters by program, then only records for that program are shown.
- Given a learner has risk history, when the user views the profile, then current and historical risk status are visible to authorized users.

## AC-02 Data Ingestion

- Given valid source records are submitted, when ingestion runs, then records are stored and linked to the correct employee.
- Given records are missing required fields, when ingestion runs, then those records are rejected or flagged with clear error reasons.
- Given duplicate records are submitted, when ingestion runs, then the system prevents duplicate business records or marks them for review.

## AC-03 Risk Rules

- Given an authorized user creates a valid risk rule, when the rule is activated, then it is applied during risk assessment.
- Given a rule condition is invalid, when the user tries to save the rule, then the system shows a validation error.
- Given a rule triggers for a learner, when the risk result is displayed, then the system shows the rule name and reason.

## AC-04 At-Risk Learner Classification

- Given learner data violates a configured rule, when risk assessment runs, then the learner is classified with the correct risk level.
- Given multiple rules trigger, when the risk level is calculated, then the highest applicable severity or configured aggregation logic is applied.
- Given learner data improves, when risk assessment runs again, then risk status is updated while preserving history.

## AC-05 Intervention Tracking

- Given a learner is at risk, when a trainer creates an intervention, then the system stores owner, due date, type, action, and status.
- Given an intervention is overdue, when dashboards are refreshed, then it appears in overdue intervention counts.
- Given an intervention is completed, when the outcome is recorded, then the system stores outcome and completion date.

## AC-06 Dashboards and Reports

- Given authorized users open dashboards, when filters are applied, then metrics update based on selected scope.
- Given a compliance officer generates a report, when export is selected, then a downloadable report is produced with timestamp and filters.
- Given a user lacks permission, when attempting to view sensitive report data, then the system denies or masks access.

## AC-07 Audit and Compliance

- Given a user changes a risk rule, when the change is saved, then the system records who changed it, what changed, and when.
- Given a report is generated, when audit history is reviewed, then report generation metadata is available.
- Given data is corrected manually, when audit history is reviewed, then original and corrected values are traceable where policy allows.

## Requirement Traceability Matrix

| Requirement ID | Requirement Summary | Use Case | Acceptance Criteria | Primary Role |
|---|---|---|---|---|
| FR-01 | Learner profile aggregation | UC-01 | AC-01 | Trainer, L&D Administrator, Manager |
| FR-02 | Attendance data ingestion | UC-02 | AC-02 | System Administrator |
| FR-03 | Assessment score ingestion | UC-02 | AC-02 | System Administrator |
| FR-04 | Competency milestone tracking | UC-01 | AC-01 | Trainer, L&D Administrator |
| FR-05 | Configurable risk rules | UC-03 | AC-03 | L&D Administrator |
| FR-06 | At-risk learner classification | UC-04 | AC-04 | Trainer, L&D Administrator |
| FR-07 | Intervention management | UC-05 | AC-05 | Trainer, Manager, L&D Administrator |
| FR-08 | Notifications and escalations | UC-05 | AC-05 | Trainer, Manager |
| FR-09 | Dashboards | UC-04, UC-05 | AC-06 | Trainer, Manager, L&D Administrator |
| FR-10 | Compliance reporting | UC-06 | AC-06, AC-07 | Compliance Officer |
| FR-11 | Administration | UC-03, UC-07 | AC-02, AC-03 | System Administrator, L&D Administrator |
| FR-12 | Audit and history | UC-06, UC-07 | AC-07 | Compliance Officer, System Administrator |

## Definition of Done for Functional Scope

- Functional requirements are reviewed by product owner and stakeholders.
- User roles and permissions are approved.
- Risk rule examples are accepted by L&D stakeholders.
- Must-have use cases have acceptance criteria.
- Dashboards and reports are mapped to user decisions.
- Compliance report fields are reviewed by compliance stakeholders.
- Requirements are traceable to use cases and acceptance criteria.
