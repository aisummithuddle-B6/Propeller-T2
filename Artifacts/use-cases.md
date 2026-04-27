# Detailed Use Cases by Module and Feature

## 1. Learner Profile Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Learner Identity | DUC-001 | View learner identity | Trainer, Manager, Employee Learner | Learner exists in employee records. | User opens Learner Profiles module. | User selects learner; system loads profile; system displays employee number, name, department, role, and manager. | If learner has no manager, system displays Unassigned. | User can identify the selected learner clearly. | DFR-001 |
| Profile Switching | DUC-002 | Switch learner profile | Trainer, Manager, L&D Administrator | Learner list is available. | User opens header profile dropdown. | User selects another learner; system reloads all profile-specific data; header displays new learner name. | If profile fails to load, system shows error message. | User can compare and review different learners. | DFR-002 |
| Program Summary | DUC-003 | Review learner programs | Trainer, Manager, L&D Administrator | Learner has attendance, assessment, or competency records. | User views learner profile. | System derives programs from learner records; system displays program tags or list. | If no programs exist, system shows empty state. | User understands learner learning scope. | DFR-003 |
| Attendance Summary | DUC-004 | Review attendance summary | Trainer, Manager, L&D Administrator | Attendance records exist or empty summary is available. | User opens learner profile. | System calculates total, attended, missed, average percentage, and latest date. | If no attendance exists, system shows zero/empty summary. | User understands attendance performance. | DFR-004 |
| Assessment Summary | DUC-005 | Review assessment summary | Trainer, Manager, L&D Administrator | Assessment records exist or empty summary is available. | User opens learner profile. | System calculates total assessments, average score, passed count, failed count, and latest date. | If no assessments exist, system shows zero/empty summary. | User understands assessment performance. | DFR-005 |
| Profile Drilldowns | DUC-006 | Drill into learning records | Trainer, Manager, L&D Administrator | Learner profile is loaded. | User reviews detail tables. | System displays attendance and assessment detail rows with program, session/competency, status, value, and date. | If no detail records exist, system shows empty table. | User can inspect source-level learning evidence. | DFR-006 |

## 2. Data Ingestion Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Data Source Management | DUC-007 | Configure data source | System Administrator | User has admin access. | User submits source details. | System checks existing source; creates or updates source; source appears in data source list. | If source details are invalid, system rejects request. | Source can be used for imports. | DFR-007 |
| API Ingestion | DUC-008 | Ingest record through API | System Administrator | Data source exists and payload is available. | External source submits JSON payload. | System validates references; persists valid record; creates import batch; updates profile data. | Invalid payload creates import error and reconciliation issue. | Learning data is stored and traceable. | DFR-008, DFR-011, DFR-012, DFR-013 |
| UI Form Ingestion | DUC-009 | Ingest record through UI form | System Administrator, L&D Administrator | Data source exists and user is on Data Ingestion module. | User fills and submits form. | UI posts payload; API validates and stores record; dashboard refreshes import counts. | Validation failure displays message and records error. | User can manually add or correct learning data. | DFR-009, DFR-011, DFR-012 |
| Excel Ingestion | DUC-010 | Upload Excel import file | System Administrator | Excel file has supported header row and data source exists. | User selects import type and uploads file. | API parses rows; each row is converted to ingestion request; system records success/failure counts. | Bad file or missing columns returns upload error; row failures are summarized. | Batch import is processed from workbook. | DFR-010, DFR-011, DFR-012, DFR-013 |
| Import Batch Tracking | DUC-011 | Review import batches | System Administrator, L&D Administrator | One or more imports exist. | User opens Data Ingestion module. | System lists recent imports with source, type, status, successful records, and failed records. | If no imports exist, system shows empty state. | User can monitor ingestion activity. | DFR-011 |
| Validation Errors | DUC-012 | Review import errors | System Administrator, L&D Administrator | Failed records exist. | User opens import errors section. | System displays field, message, raw value, and resolved status. | User may resolve error after correction. | User can triage data quality issues. | DFR-012 |
| Reconciliation Issues | DUC-013 | Review reconciliation queue | System Administrator, L&D Administrator | Reference mismatches exist. | User opens Validation & Reconciliation module. | System displays entity type, source ID, issue type, description, and resolution state. | User can resolve after source data correction. | Cross-source mismatches are visible and traceable. | DFR-013 |

## 3. Competency Tracking Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Milestone Tracking | DUC-014 | View competency milestones | Trainer, L&D Administrator | Learner has milestone records. | User opens Competency Tracking module. | System lists competency code, name, status, due date, and completed date. | If none exist, system shows empty state. | User understands competency progress. | DFR-014 |
| Milestone Updates | DUC-015 | Update milestone from ingestion | System Administrator | Milestone ingestion payload references known learner, program, and competency. | API or UI submits milestone status. | System finds existing milestone; updates status and completion date; profile refreshes. | If no milestone exists, system creates one. | Milestone data remains current without duplicates. | DFR-015 |
| Overdue Visibility | DUC-016 | Review overdue or incomplete competencies | Trainer, Manager, L&D Administrator | Milestones have due dates and statuses. | User views profile or dashboard. | System calculates incomplete/overdue counts and compliance readiness. | If all complete, system shows compliant status. | User can identify learning gaps. | DFR-016 |

## 4. Risk Rules and Risk Assessment Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Risk Rule Configuration | DUC-017 | Create risk rule | L&D Administrator | User has rule configuration permission. | User submits rule details. | System saves rule with condition, severity, version, and inactive/active status. | Invalid condition is rejected with validation message. | Rule is available for risk assessment. | DFR-017 |
| Risk Assessment | DUC-018 | Recalculate learner risk | L&D Administrator, Trainer | Learner has learning records and active rules exist. | User or worker triggers risk calculation. | System evaluates learning data; applies active rules; stores risk level. | If no rule triggers, risk level is None. | Learner risk status is updated. | DFR-018 |
| Explainability | DUC-019 | Review risk reasons | Trainer, Manager, L&D Administrator | Risk assessment exists. | User opens risk status or history. | System displays risk level, reason, and triggered rules. | If no triggers exist, system shows no active risk reason. | User understands why learner is at risk. | DFR-019 |
| Risk History | DUC-020 | View risk history | L&D Administrator, Compliance Officer | Learner has one or more risk assessments. | User opens Risk Assessment module. | System lists historical assessments by date and severity. | If no history exists, system shows empty history. | Risk trend remains auditable. | DFR-020 |

## 5. Intervention Management Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Intervention Creation | DUC-021 | Create intervention | Trainer, Manager, L&D Administrator | Learner and program exist. | User creates intervention for learner. | System stores type, owner, due date, learner, and program. | Missing learner/program returns validation error. | Intervention appears in learner history. | DFR-021 |
| Intervention Notes | DUC-022 | Add intervention note | Trainer, Manager, L&D Administrator | Intervention exists. | User enters note. | System stores note with author and timestamp. | Empty note is rejected. | Intervention progress is traceable. | DFR-022 |
| Outcome Recording | DUC-023 | Record intervention outcome | Trainer, L&D Administrator | Intervention exists and action has been completed. | User records outcome and measurement. | System stores outcome and updates intervention status. | Missing outcome is rejected. | Outcome is visible in intervention history. | DFR-023 |
| Status Tracking | DUC-024 | Track intervention status | Trainer, Manager, L&D Administrator | Intervention exists. | User views intervention list/profile. | System displays planned, in progress, completed, outcome recorded, closed, or reopened status. | Overdue due date is highlighted in dashboards. | Support actions remain manageable. | DFR-024 |

## 6. Dashboard Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Metric Snapshot | DUC-025 | Capture dashboard metric snapshot | L&D Administrator | Learning or ingestion event occurs. | System records metric snapshot. | Snapshot stores metric name, scope, value, and capture date. | If calculation fails, system logs error for operations. | Dashboard data has a traceable source. | DFR-025 |
| Risk Dashboard | DUC-026 | View risk dashboard | Trainer, Manager, L&D Administrator | Risk assessments exist. | User opens Dashboards module. | System displays learner counts by risk level and drilldown options. | If no risk data exists, system shows zero-state dashboard. | User can identify high-risk learner population. | DFR-026 |
| Compliance Dashboard | DUC-027 | View compliance dashboard | Manager, L&D Administrator | Learning records and milestones exist. | User opens Dashboards module. | System displays compliance readiness, attendance gaps, assessment performance, and open interventions. | Filters may narrow data by scope. | User can monitor compliance readiness. | DFR-027 |

## 7. Compliance Reporting Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Report Templates | DUC-028 | View report templates | Compliance Officer, L&D Administrator | Report templates exist. | User opens Compliance Reporting module. | System lists active templates by code, name, and type. | If no templates exist, system shows setup required message. | User can select report type. | DFR-028 |
| Report Requests | DUC-029 | Request compliance report | Compliance Officer | Template exists and filters are selected. | User submits report request. | System stores requested by, template, filters, status, and timestamps. | Invalid template returns validation error. | Report request is traceable. | DFR-029 |
| Compliance Evidence | DUC-030 | Review compliance evidence | Compliance Officer | Report data exists. | User opens generated report or report preview. | System displays completion, attendance, assessment, competency, exception, and intervention evidence. | Missing evidence is shown as exception. | Compliance decision is supported by traceable data. | DFR-030 |

## 8. Notifications and Escalations Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Notification Queue | DUC-031 | Create risk notification | Trainer, Manager, L&D Administrator | Learner becomes high risk or intervention is overdue. | Risk or intervention event occurs. | System creates notification message with channel, subject, body, and status. | Delivery failure marks notification failed. | Responsible users receive actionable context. | DFR-031 |
| Escalation Rules | DUC-032 | Apply escalation rule | L&D Administrator | Escalation rule exists. | Risk/intervention condition matches rule. | System identifies target role and creates escalation action or notification. | If target role is missing, issue is logged. | Overdue or critical items are escalated. | DFR-032 |

## 9. Administration and Access Control Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| User Management | DUC-033 | Review system users | System Administrator | Users exist or identity sync has run. | User opens Users & Roles module. | System lists display name, email, external subject ID, and active state. | If no users exist, system shows empty state. | Admin can verify users. | DFR-033 |
| Role Management | DUC-034 | Review role assignments | System Administrator | Roles and assignments exist. | User opens Users & Roles module. | System displays roles, scopes, and assignments. | Missing role assignment can be flagged for setup. | Admin can verify access model. | DFR-034 |
| System Configuration | DUC-035 | Review system configuration | System Administrator | Configuration values exist. | User opens Administration module. | System lists key, value, and category. | Invalid configuration is flagged. | Admin can verify operational settings. | DFR-035 |
| Data Source Administration | DUC-036 | Review data source setup | System Administrator | Data sources exist. | User opens Data Ingestion or Administration module. | System displays source name, type, owner, endpoint, and active status. | Inactive source is shown as disabled. | Admin can manage source readiness. | DFR-036 |

## 10. Audit and Operations Module

| Feature | Use Case ID | Use Case | Primary Actor | Preconditions | Trigger | Main Flow | Alternate / Exception Flow | Success Outcome | Related DFRs |
|---|---|---|---|---|---|---|---|---|---|
| Audit Logging | DUC-037 | Review audit log | Compliance Officer, System Administrator | Audit events exist. | User opens Audit module. | System lists actor, action, entity type, entity ID, and timestamp. | If no logs exist, system shows empty state. | Compliance actions are traceable. | DFR-037 |
| Data Correction Traceability | DUC-038 | Review data correction evidence | Compliance Officer | Before/after data is captured. | User opens audit entry details. | System shows original and corrected values where policy allows. | Sensitive values may be masked. | Correction evidence supports audit review. | DFR-038 |
| Health Checks | DUC-039 | Verify application health | System Administrator | App is deployed or running locally. | User calls health endpoint. | System returns healthy status. | If dependency fails, status indicates degraded/unhealthy. | Operator can verify runtime readiness. | DFR-039 |
| Deployment Verification | DUC-040 | Verify setup and deployment | System Administrator | Deployment instructions are available. | User follows setup/build/run steps. | Database is seeded, API starts, frontend loads, and core screen opens. | Failed step is documented as known issue. | Demo or deployment environment is validated. | DFR-040 |
