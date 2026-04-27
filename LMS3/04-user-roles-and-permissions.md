# User Roles and Permissions

## Role Definitions

## Employee Learner

An employee who participates in required or optional learning programs.

Allowed actions:

- View own learning profile.
- View own attendance, assessment, competency, and intervention status.
- View assigned next actions.
- Add learner comments where enabled.

Restricted actions:

- Cannot change risk rules.
- Cannot edit official attendance, assessment, or competency records.
- Cannot view other employees unless explicitly authorized.

## Trainer

A user responsible for training delivery and learner support.

Allowed actions:

- View learners assigned to their programs.
- View risk reasons for assigned learners.
- Create and update interventions.
- Add trainer notes and outcome observations.
- View dashboards for assigned training groups.

Restricted actions:

- Cannot manage system integrations.
- Cannot change global role permissions.
- Cannot approve compliance exceptions unless assigned.

## Manager

A people manager responsible for team-level learning follow-up.

Allowed actions:

- View team learning progress.
- View team risk summary.
- View assigned intervention actions.
- Add manager follow-up notes.

Restricted actions:

- Cannot edit assessment scores or attendance records.
- Cannot configure risk rules.
- Cannot access unrelated teams.

## L&D Administrator

A learning and development operations owner.

Allowed actions:

- Manage risk rules.
- Review learning data quality.
- View organization-wide dashboards.
- Create and manage interventions.
- Generate compliance reports.
- Configure report templates and lookup values.

Restricted actions:

- Cannot manage infrastructure-level settings unless also system administrator.
- Cannot bypass audit logging.

## Compliance Officer

A user responsible for compliance evidence and audit readiness.

Allowed actions:

- View compliance dashboards.
- Generate compliance reports.
- Review exceptions and evidence.
- Export compliance data.

Restricted actions:

- Cannot alter source learning records.
- Cannot modify risk rules unless also assigned L&D administrator permissions.

## System Administrator

A technical administrator responsible for configuration and access management.

Allowed actions:

- Manage users, roles, and access.
- Configure data sources and integrations.
- Monitor imports and system jobs.
- Manage system-level settings.

Restricted actions:

- Should not change learning outcomes or business records unless through controlled admin support workflows.

## Permission Matrix

| Capability | Employee Learner | Trainer | Manager | L&D Administrator | Compliance Officer | System Administrator |
|---|---:|---:|---:|---:|---:|---:|
| View own profile | Yes | No | No | No | No | No |
| View assigned learner profiles | No | Yes | Yes | Yes | Yes | Support only |
| View organization dashboards | No | Limited | Limited | Yes | Yes | Support only |
| Configure risk rules | No | No | No | Yes | No | Technical support |
| Create interventions | No | Yes | Yes | Yes | No | No |
| Update intervention outcome | Limited comments | Yes | Yes | Yes | No | No |
| Generate compliance reports | No | No | Limited | Yes | Yes | No |
| Manage users and roles | No | No | No | No | No | Yes |
| Configure integrations | No | No | No | Limited | No | Yes |
| View audit logs | No | Limited | No | Yes | Yes | Yes |

## Access Control Principles

- Use role-based access control for standard permissions.
- Use attribute-based checks for department, manager, program, and assigned learner access.
- Apply least privilege by default.
- Audit sensitive actions such as exports, rule changes, permission changes, and intervention updates.
