# Detailed Functional Requirements by Module and Feature

## 1. Learner Profile Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Learner Identity | DFR-001 | The system shall display learner identity including employee number, full name, department, job role, and manager. | Must | Employee Learner, Trainer, Manager, L&D Administrator | Employee record, manager assignment | Learner identity header | Profile header shows selected learner identity. | Implemented |
| Profile Switching | DFR-002 | The system shall allow authorized users to switch learner context from a header dropdown. | Must | Trainer, Manager, L&D Administrator | Learner list | Selected learner profile | Selecting a learner reloads all profile-dependent views. | Implemented |
| Program Summary | DFR-003 | The system shall show assigned or detected learning programs for the learner. | Must | Trainer, Manager, L&D Administrator | Attendance, assessment, competency records | Program tags/list | Programs are derived from learning records and shown on profile. | Implemented |
| Attendance Summary | DFR-004 | The system shall calculate total sessions, attended sessions, missed sessions, average attendance, and last attendance date. | Must | Trainer, Manager, L&D Administrator | Attendance records | Attendance metric card | Attendance metric updates after ingestion. | Implemented |
| Assessment Summary | DFR-005 | The system shall calculate total assessments, average score, passed count, failed count, and latest assessment date. | Must | Trainer, Manager, L&D Administrator | Assessment records | Assessment metric card | Assessment metric updates after ingestion. | Implemented |
| Profile Drilldowns | DFR-006 | The system shall provide drilldown tables for attendance and assessment records. | Must | Trainer, Manager, L&D Administrator | Learning records | Detail tables | Detail rows show program, session/competency, status, score/attendance, and dates. | Implemented |

## 2. Data Ingestion Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Data Source Management | DFR-007 | The system shall manage data sources with name, type, owner, endpoint, and active status. | Must | System Administrator | Data source details | Data source record | Existing source can be updated without creating duplicate. | Implemented |
| API Ingestion | DFR-008 | The system shall ingest employee, attendance, assessment, and competency milestone records through REST APIs. | Must | System Administrator | JSON payload | Import batch, persisted record | Valid records are stored and linked to learner profile. | Implemented |
| UI Form Ingestion | DFR-009 | The system shall provide UI forms for employee, attendance, assessment, and competency milestone ingestion. | Must | System Administrator, L&D Administrator | Form fields | API submission result | User can submit records without using external tooling. | Implemented |
| Excel Ingestion | DFR-010 | The system shall accept Excel file uploads for supported ingestion types. | Must | System Administrator | Excel workbook, import type, data source | Upload result summary | Workbook rows are parsed and processed with success/failure counts. | Partially Implemented |
| Import Batch Tracking | DFR-011 | The system shall track each import batch with source, type, file name, status, total, success, and failed counts. | Must | System Administrator, L&D Administrator | Ingestion request | Import batch list | Import dashboard shows recent batch status. | Implemented |
| Validation Errors | DFR-012 | The system shall create import errors for missing fields, invalid references, invalid ranges, and duplicates. | Must | System Administrator, L&D Administrator | Invalid record | Import error row | Error includes field, message, raw value, and resolution state. | Implemented |
| Reconciliation Issues | DFR-013 | The system shall create reconciliation issues when records reference missing employees, programs, sessions, competencies, or managers. | Must | System Administrator, L&D Administrator | Invalid reference | Reconciliation issue | Issue appears in validation and reconciliation module. | Implemented |

## 3. Competency Tracking Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Milestone Tracking | DFR-014 | The system shall track competency milestone status, due date, completion date, and competency code. | Must | Trainer, L&D Administrator | Competency milestone records | Competency progress table | Status and due date are visible per learner. | Implemented |
| Milestone Updates | DFR-015 | The system shall update existing milestone status when new milestone ingestion arrives for the same learner, program, and competency. | Must | System Administrator | Milestone ingestion payload | Updated milestone | Existing milestone is updated instead of duplicated. | Implemented |
| Overdue Visibility | DFR-016 | The system shall identify incomplete or overdue competencies for compliance readiness. | Must | Trainer, Manager, L&D Administrator | Milestone due dates and statuses | Compliance readiness summary | Overdue or incomplete milestones affect readiness. | Implemented |

## 4. Risk Rules and Risk Assessment Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Risk Rule Configuration | DFR-017 | The system shall allow authorized users to create risk rules with name, condition expression, severity, version, and active status. | Must | L&D Administrator | Rule details | Risk rule | Rule is saved and can be activated/deactivated. | Partially Implemented |
| Risk Assessment | DFR-018 | The system shall classify learners into None, Low, Medium, High, or Critical risk levels. | Must | Trainer, Manager, L&D Administrator | Attendance, assessment, milestone, active rules | Risk assessment | Highest applicable severity determines visible risk level. | Partially Implemented |
| Explainability | DFR-019 | The system shall show triggered rule names and reasons for each learner risk result. | Must | Trainer, Manager, L&D Administrator | Risk triggers | Trigger list | Risk status includes explainable trigger reasons. | Partially Implemented |
| Risk History | DFR-020 | The system shall preserve historical risk assessments for audit and trend analysis. | Should | L&D Administrator, Compliance Officer | Risk recalculation events | Risk history table | User can view prior risk assessments. | Implemented |

## 5. Intervention Management Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Intervention Creation | DFR-021 | The system shall allow interventions to be created for at-risk learners. | Must | Trainer, Manager, L&D Administrator | Learner, program, type, owner, due date | Intervention record | Intervention appears in learner profile. | Partially Implemented |
| Intervention Notes | DFR-022 | The system shall allow owners to add notes to interventions. | Must | Trainer, Manager, L&D Administrator | Note text, author | Intervention note | Notes are stored with author and timestamp. | Partially Implemented |
| Outcome Recording | DFR-023 | The system shall allow users to record intervention outcome and measurement. | Must | Trainer, L&D Administrator | Outcome, measurement | Intervention outcome | Outcome is visible in intervention history. | Partially Implemented |
| Status Tracking | DFR-024 | The system shall track intervention status from planned through outcome recorded or closed. | Must | Trainer, Manager, L&D Administrator | Status updates | Status history/current status | Overdue or open interventions remain visible. | Partially Implemented |

## 6. Dashboard Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Metric Snapshot | DFR-025 | The system shall store dashboard metric snapshots by metric name, scope, value, and capture date. | Should | L&D Administrator | Aggregated learning data | Metric snapshots | Recent ingestion creates dashboard signal. | Implemented |
| Risk Dashboard | DFR-026 | The system shall show learner counts grouped by risk level. | Must | Trainer, Manager, L&D Administrator | Risk assessments | Risk dashboard cards | Counts reflect latest risk records. | Planned |
| Compliance Dashboard | DFR-027 | The system shall show compliance readiness, open interventions, attendance gaps, and assessment performance. | Must | Manager, L&D Administrator | Learning records | Dashboard cards and tables | Metrics support drilldown by learner or group. | Planned |

## 7. Compliance Reporting Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Report Templates | DFR-028 | The system shall maintain report templates with code, name, type, definition, and active status. | Must | Compliance Officer, L&D Administrator | Report template setup | Template list | Active templates are available for report requests. | Implemented |
| Report Requests | DFR-029 | The system shall create report requests with requester, template, filters, status, completion date, and output URI. | Must | Compliance Officer | Report filters | Report request | Request is tracked for audit and follow-up. | Implemented |
| Compliance Evidence | DFR-030 | The system shall include completion, attendance, assessment, competency, exception, and intervention evidence in reports. | Must | Compliance Officer | Learning records and filters | Compliance report content | Report supports compliance review. | Planned |

## 8. Notifications and Escalations Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Notification Queue | DFR-031 | The system shall create notification messages for high-risk learners and overdue interventions. | Should | Trainer, Manager, L&D Administrator | Risk/intervention events | Notification message | Notification includes subject, body, channel, and status. | Seeded |
| Escalation Rules | DFR-032 | The system shall define escalation rules with condition and target role. | Should | L&D Administrator | Escalation condition | Escalation rule | Rule can identify manager/admin escalation target. | Seeded |

## 9. Administration and Access Control Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| User Management | DFR-033 | The system shall maintain system users with external subject ID, display name, email, and active status. | Must | System Administrator | User identity | System user | Users are available for ownership and role mapping. | Implemented |
| Role Management | DFR-034 | The system shall maintain roles and user-role assignments with scope type and scope value. | Must | System Administrator | Role assignment | Access mapping | Roles support learner, trainer, manager, admin, and compliance access. | Implemented |
| System Configuration | DFR-035 | The system shall maintain system configuration values by key, value, and category. | Should | System Administrator | Configuration values | Config list | Configuration is queryable for operational settings. | Implemented |
| Data Source Administration | DFR-036 | The system shall allow administrators to configure and review data sources. | Must | System Administrator | Data source metadata | Data source table | Data source list is visible in ingestion module. | Implemented |

## 10. Audit and Operations Module

| Feature | Requirement ID | Requirement | Priority | Actors | Inputs | Outputs | Acceptance Notes | Status |
|---|---|---|---|---|---|---|---|---|
| Audit Logging | DFR-037 | The system shall record auditable actions with actor, action, entity type, entity ID, and timestamp. | Must | Compliance Officer, System Administrator | Business action | Audit log entry | Audit entries support compliance evidence. | Partially Implemented |
| Data Correction Traceability | DFR-038 | The system shall preserve before/after data where policy requires traceability. | Should | Compliance Officer | Data correction event | Audit before/after payload | Correction evidence can be reviewed. | Planned |
| Health Checks | DFR-039 | The system shall expose health endpoints for runtime verification. | Should | System Administrator | Health request | Health status | Health endpoint confirms app readiness. | Implemented |
| Deployment Verification | DFR-040 | The system shall provide repeatable setup, seed, build, and run steps for demo or deployment validation. | Should | System Administrator | Setup commands | Running app | Backend and frontend can be started from documented steps. | Implemented |
