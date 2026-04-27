# Data, Rules, and Interventions

## Core Data Entities

## Employee

Represents an employee learner.

Key fields:

- employee_id
- full_name
- email
- department
- job_role
- manager_id
- location
- employment_status

## Learning Program

Represents a training program or learning requirement.

Key fields:

- program_id
- name
- description
- owner
- mandatory_flag
- compliance_category
- start_date
- end_date

## Training Attendance

Represents attendance for a learning session.

Key fields:

- attendance_id
- employee_id
- program_id
- session_id
- attendance_date
- status
- attendance_percentage
- source_system
- imported_at

## Assessment Result

Represents a periodic assessment or evaluation result.

Key fields:

- assessment_result_id
- employee_id
- program_id
- competency_id
- assessment_type
- score
- score_type
- pass_fail_status
- attempt_number
- assessment_date
- source_system

## Competency Milestone

Represents progress against a competency-level requirement.

Key fields:

- milestone_id
- employee_id
- competency_id
- program_id
- status
- due_date
- completion_date
- evidence_reference
- source_system

## Risk Rule

Represents a configurable business rule used to classify learners.

Key fields:

- rule_id
- rule_name
- description
- target_population
- condition_expression
- severity
- active_from
- active_to
- version
- owner
- status

## Risk Assessment Result

Represents the outcome of applying risk rules to learners.

Key fields:

- risk_result_id
- employee_id
- program_id
- competency_id
- triggered_rule_ids
- risk_level
- risk_reason
- assessed_at

## Intervention

Represents an action taken to help an at-risk learner.

Key fields:

- intervention_id
- employee_id
- program_id
- competency_id
- intervention_type
- owner_id
- due_date
- status
- planned_action
- actual_action
- outcome
- created_at
- completed_at

## Recommended Risk Rule Format

Example JSON-style rule definition:

```json
{
  "ruleName": "Low Attendance Risk",
  "targetPopulation": {
    "programId": "mandatory-security-training",
    "employeeStatus": "active"
  },
  "conditions": {
    "all": [
      {
        "field": "attendancePercentage",
        "operator": "lessThan",
        "value": 75
      },
      {
        "field": "programDueDate",
        "operator": "withinDays",
        "value": 14
      }
    ]
  },
  "severity": "high",
  "riskReason": "Attendance is below required threshold close to due date"
}
```

## Sample Risk Rules

| Rule | Condition | Risk Level |
|---|---|---|
| Low attendance | Attendance below 75% | High |
| Critical attendance gap | Attendance below 50% | Critical |
| Low assessment score | Latest score below passing threshold | Medium |
| Repeated assessment failure | Two or more failed attempts | High |
| Overdue competency | Competency due date passed and status not completed | Critical |
| Slow competency progress | Milestone in progress for more than configured days | Medium |
| Composite learning risk | Low attendance and low assessment score | High |

## Intervention Lifecycle

1. Recommended: System suggests intervention based on risk result.
2. Planned: Trainer or L&D administrator assigns owner and due date.
3. In Progress: Intervention activity has started.
4. Completed: Action has been performed.
5. Outcome Recorded: Result is documented.
6. Closed: No further action required.
7. Reopened: Learner remains at risk or risk increases again.

## Intervention Outcomes

Possible outcomes:

- Improved
- No improvement
- Partially improved
- Reassessment required
- Escalated to manager
- Exempted or waived
- Invalid risk classification

## Data Quality Rules

- Employee ID must exist before learning records are linked.
- Program ID and competency ID must be valid.
- Attendance percentage must be between 0 and 100.
- Assessment score must match configured score type.
- Due date cannot be earlier than program start date unless explicitly allowed.
- Duplicate records should be flagged using source system reference and event timestamp.
