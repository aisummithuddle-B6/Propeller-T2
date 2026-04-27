import React, { useEffect, useState } from "react";
import { createRoot } from "react-dom/client";
import "./styles.css";

type LearnerProfile = {
  employeeId: string;
  employeeNumber: string;
  fullName: string;
  department: string;
  jobRole: string;
  managerName: string;
  assignedPrograms: string[];
  attendanceSummary: AttendanceSummary;
  assessmentSummary: AssessmentSummary;
  competencyProgress: CompetencyProgress[];
  riskStatus: RiskStatus;
  complianceReadiness: ComplianceReadiness;
  interventionHistory: InterventionHistoryItem[];
  riskHistory: RiskHistoryItem[];
  attendanceRecords: AttendanceRecord[];
  assessmentRecords: AssessmentRecord[];
};

type LearnerProfileSummary = {
  employeeId: string;
  employeeNumber: string;
  fullName: string;
  department: string;
  jobRole: string;
  currentRiskLevel: number;
  openInterventions: number;
  completedCompetencies: number;
  overdueCompetencies: number;
};

type AttendanceSummary = {
  totalSessions: number;
  attendedSessions: number;
  attendancePercentage: number;
  missedSessions: number;
  lastAttendanceDate: string;
};

type AssessmentSummary = {
  totalAssessments: number;
  averageScore: number;
  passedAssessments: number;
  failedAssessments: number;
  lastAssessmentDate: string;
};

type CompetencyProgress = {
  competencyCode: string;
  competencyName: string;
  status: string;
  dueDate: string;
  completedOn?: string;
};

type RiskStatus = {
  currentRiskLevel: number;
  reason: string;
  triggeredRules: string[];
  assessedAt: string;
};

type ComplianceReadiness = {
  isCompliant: boolean;
  requiredCompetencies: number;
  completedRequiredCompetencies: number;
  overdueCompetencies: number;
  summary: string;
};

type InterventionHistoryItem = {
  interventionId: string;
  type: string;
  ownerName: string;
  status: string;
  dueDate: string;
  outcome?: string;
};

type RiskHistoryItem = {
  riskAssessmentId: string;
  riskLevel: number;
  reason: string;
  assessedAt: string;
  triggeredRules: string[];
};

type AttendanceRecord = {
  attendanceRecordId: string;
  programName: string;
  sessionTitle: string;
  status: string;
  attendancePercentage: number;
  attendanceDate: string;
};

type AssessmentRecord = {
  assessmentResultId: string;
  programName: string;
  competencyName: string;
  assessmentType: string;
  score: number;
  status: string;
  attemptNumber: number;
  assessmentDate: string;
};

type IngestionStatus = {
  dataSources: number;
  importBatches: number;
  failedRecords: number;
  openErrors: number;
  openReconciliationIssues: number;
};

type DataSource = {
  id: string;
  name: string;
  sourceType: string;
  owner: string;
  endpoint?: string;
  isActive: boolean;
};

type ImportBatch = {
  id: string;
  dataSourceName: string;
  importType: string;
  fileName: string;
  status: string;
  totalRecords: number;
  successfulRecords: number;
  failedRecords: number;
};

type ImportError = {
  id: string;
  importBatchId: string;
  rowNumber: number;
  fieldName: string;
  errorMessage: string;
  rawValue?: string;
  isResolved: boolean;
};

type ReconciliationIssue = {
  id: string;
  entityType: string;
  sourceRecordId: string;
  issueType: string;
  description: string;
  isResolved: boolean;
};

type IngestionDashboard = {
  status: IngestionStatus;
  sources: DataSource[];
  imports: ImportBatch[];
  errors: ImportError[];
  issues: ReconciliationIssue[];
};

const apiBaseUrl = "http://localhost:5080";

const moduleLinks = [
  { id: "learner-profile", label: "Learner Profiles" },
  { id: "data-ingestion", label: "Data Ingestion" },
  { id: "validation", label: "Validation & Reconciliation" },
  { id: "competency-tracking", label: "Competency Tracking" },
  { id: "risk-rules", label: "Risk Rules Engine" },
  { id: "risk-assessment", label: "Risk Assessment" },
  { id: "interventions", label: "Intervention Management" },
  { id: "notifications", label: "Notifications & Escalations" },
  { id: "dashboards", label: "Dashboards" },
  { id: "compliance-reporting", label: "Compliance Reporting" },
  { id: "roles", label: "Users & Roles" },
  { id: "audit", label: "Audit & History" },
  { id: "administration", label: "Administration" }
];

function riskLabel(level: number) {
  return ["None", "Low", "Medium", "High", "Critical"][level] ?? "Unknown";
}

function App() {
  const [activeModule, setActiveModule] = useState(moduleLinks[0].id);
  const [learners, setLearners] = useState<LearnerProfileSummary[]>([]);
  const [selectedLearnerId, setSelectedLearnerId] = useState<string>("");
  const [profile, setProfile] = useState<LearnerProfile | null>(null);
  const [ingestion, setIngestion] = useState<IngestionDashboard | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [ingestionMessage, setIngestionMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function loadLearners() {
      try {
        const data = await getJson<LearnerProfileSummary[]>("/api/learners");
        setLearners(data);
        setSelectedLearnerId(data[0]?.employeeId ?? "");
      } catch (err) {
        setError(err instanceof Error ? err.message : "Unable to load learner list.");
      } finally {
        setIsLoading(false);
      }
    }

    void loadLearners();
  }, []);

  useEffect(() => {
    void loadIngestionDashboard();
  }, []);

  useEffect(() => {
    if (!selectedLearnerId) return;
    void reloadProfile(selectedLearnerId);
  }, [selectedLearnerId]);

  const selectedLearner = learners.find((learner) => learner.employeeId === selectedLearnerId);
  const activeModuleLabel = moduleLinks.find((module) => module.id === activeModule)?.label ?? "Module";

  async function loadIngestionDashboard() {
    const [status, sources, imports, errors, issues] = await Promise.all([
      getJson<IngestionStatus>("/api/data-ingestion/status"),
      getJson<DataSource[]>("/api/data-ingestion/sources"),
      getJson<ImportBatch[]>("/api/data-ingestion/imports"),
      getJson<ImportError[]>("/api/data-ingestion/errors?unresolvedOnly=true"),
      getJson<ReconciliationIssue[]>("/api/data-ingestion/reconciliation-issues?unresolvedOnly=true")
    ]);

    setIngestion({ status, sources, imports, errors, issues });
  }

  async function postJson<TRequest>(path: string, body: TRequest) {
    const response = await fetch(`${apiBaseUrl}${path}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body)
    });

    if (!response.ok) {
      throw new Error(`API returned ${response.status}`);
    }

    return response.json();
  }

  async function submitSampleAttendance() {
    if (!ingestion?.sources[0]) return;
    try {
      const response = await postJson("/api/data-ingestion/attendance", {
        dataSourceId: ingestion.sources[0].id,
        employeeNumber: profile?.employeeNumber ?? "EMP-0001",
        programCode: "SEC-AWARE-2026",
        sessionCode: "SEC-S2",
        status: "Present",
        attendancePercentage: 100,
        attendanceDate: new Date().toISOString().slice(0, 10),
        sourceRecordId: `ui-att-${Date.now()}`
      });
      setIngestionMessage(response.message);
      await Promise.all([loadIngestionDashboard(), selectedLearnerId ? reloadProfile(selectedLearnerId) : Promise.resolve()]);
    } catch (err) {
      setIngestionMessage(err instanceof Error ? err.message : "Unable to ingest attendance.");
    }
  }

  async function submitSampleAssessment() {
    if (!ingestion?.sources[0]) return;
    try {
      const response = await postJson("/api/data-ingestion/assessments", {
        dataSourceId: ingestion.sources[0].id,
        employeeNumber: profile?.employeeNumber ?? "EMP-0001",
        programCode: "DATA-PRIV-2026",
        competencyCode: "PRV-201",
        score: 82,
        status: "Passed",
        assessmentType: "Reassessment",
        scoreType: "Percentage",
        attemptNumber: 2,
        assessmentDate: new Date().toISOString().slice(0, 10),
        sourceRecordId: `ui-assess-${Date.now()}`
      });
      setIngestionMessage(response.message);
      await Promise.all([loadIngestionDashboard(), selectedLearnerId ? reloadProfile(selectedLearnerId) : Promise.resolve()]);
    } catch (err) {
      setIngestionMessage(err instanceof Error ? err.message : "Unable to ingest assessment.");
    }
  }

  async function submitSampleMilestone() {
    if (!ingestion?.sources[0]) return;
    try {
      const today = new Date().toISOString().slice(0, 10);
      const response = await postJson("/api/data-ingestion/competency-milestones", {
        dataSourceId: ingestion.sources[0].id,
        employeeNumber: profile?.employeeNumber ?? "EMP-0001",
        programCode: "DATA-PRIV-2026",
        competencyCode: "PRV-201",
        status: "Completed",
        dueDate: today,
        completedOn: today,
        sourceRecordId: `ui-comp-${Date.now()}`
      });
      setIngestionMessage(response.message);
      await Promise.all([loadIngestionDashboard(), selectedLearnerId ? reloadProfile(selectedLearnerId) : Promise.resolve()]);
    } catch (err) {
      setIngestionMessage(err instanceof Error ? err.message : "Unable to ingest competency milestone.");
    }
  }

  async function submitInvalidAttendance() {
    if (!ingestion?.sources[0]) return;
    try {
      const response = await postJson("/api/data-ingestion/attendance", {
        dataSourceId: ingestion.sources[0].id,
        employeeNumber: "UNKNOWN-EMP",
        programCode: "SEC-AWARE-2026",
        sessionCode: "SEC-S1",
        status: "Present",
        attendancePercentage: 100,
        attendanceDate: new Date().toISOString().slice(0, 10),
        sourceRecordId: `ui-invalid-${Date.now()}`
      });
      setIngestionMessage(response.message);
      await loadIngestionDashboard();
    } catch (err) {
      setIngestionMessage(err instanceof Error ? err.message : "Unable to submit invalid sample.");
    }
  }

  async function reloadProfile(employeeId: string) {
    try {
      setError(null);
      const data = await getJson<LearnerProfile>(`/api/learners/${employeeId}/profile`);
      setProfile(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to load learner profile.");
    }
  }

  return (
    <main className="shell">
      <section className="hero">
        <div>
          <p className="eyebrow">Corporate Learning Tracking</p>
          <h1>Learning Progress, Intervention, and Compliance</h1>
          <p>Module navigation for the full in-scope application, with SQL-backed Learner Profile and Data Ingestion features implemented.</p>
        </div>
        <div className="header-controls">
          <div className="current-user">
            <span>Viewing learner</span>
            <strong>{profile?.fullName ?? selectedLearner?.fullName ?? "Loading learners"}</strong>
          </div>
          <label>
            Switch profile
            <select
              value={selectedLearnerId}
              onChange={(event) => setSelectedLearnerId(event.target.value)}
            >
              {learners.map((learner) => (
                <option value={learner.employeeId} key={learner.employeeId}>
                  {learner.fullName} ({learner.employeeNumber})
                </option>
              ))}
            </select>
          </label>
        </div>
      </section>

      <nav className="module-nav" aria-label="Application modules">
        {moduleLinks.map((module) => (
          <button
            className={module.id === activeModule ? "active" : undefined}
            type="button"
            onClick={() => setActiveModule(module.id)}
            key={module.id}
          >
            {module.label}
          </button>
        ))}
      </nav>

      {isLoading && <section className="panel">Loading learner profile...</section>}
      {error && <section className="panel error">Unable to load learner profile: {error}</section>}

      {profile && (
        <>
          <section className="active-module-summary">
            Showing only <strong>{activeModuleLabel}</strong> features for <strong>{profile.fullName}</strong>.
          </section>

          {activeModule === "learner-profile" && (
            <>
              <section className="profile-header" id="learner-profile">
                <div>
                  <p className="eyebrow">Employee Identity</p>
                  <h2>{profile.fullName}</h2>
                  <p>{profile.employeeNumber} · {profile.department} · {profile.jobRole}</p>
                  <p>Manager: {profile.managerName}</p>
                </div>
                <div className={`risk-badge risk-${riskLabel(profile.riskStatus.currentRiskLevel).toLowerCase()}`}>
                  {riskLabel(profile.riskStatus.currentRiskLevel)}
                </div>
              </section>

              <section className="grid">
                <MetricCard label="Attendance" value={`${profile.attendanceSummary.attendancePercentage}%`} detail={`${profile.attendanceSummary.attendedSessions}/${profile.attendanceSummary.totalSessions} sessions attended`} />
                <MetricCard label="Assessments" value={`${profile.assessmentSummary.averageScore}%`} detail={`${profile.assessmentSummary.passedAssessments} passed, ${profile.assessmentSummary.failedAssessments} failed`} />
                <MetricCard label="Compliance" value={profile.complianceReadiness.isCompliant ? "Ready" : "Gaps"} detail={profile.complianceReadiness.summary} />
                <MetricCard label="Interventions" value={String(profile.interventionHistory.length)} detail="Tracked support actions" />
              </section>

              <Section title="Assigned Learning Programs">
                <div className="tags">
                  {profile.assignedPrograms.map((program) => <span className="tag" key={program}>{program}</span>)}
                </div>
              </Section>

              <Section title="Current Risk Status">
                <p>{profile.riskStatus.reason}</p>
                <div className="tags">
                  {profile.riskStatus.triggeredRules.map((rule) => <span className="tag danger" key={rule}>{rule}</span>)}
                </div>
              </Section>

              <Section title="Attendance Drill-Down">
                <table>
                  <thead><tr><th>Program</th><th>Session</th><th>Status</th><th>Attendance</th><th>Date</th></tr></thead>
                  <tbody>
                    {profile.attendanceRecords.map((record) => (
                      <tr key={record.attendanceRecordId}>
                        <td>{record.programName}</td>
                        <td>{record.sessionTitle}</td>
                        <td>{record.status}</td>
                        <td>{record.attendancePercentage}%</td>
                        <td>{record.attendanceDate}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </Section>

              <Section title="Assessment Drill-Down">
                <table>
                  <thead><tr><th>Program</th><th>Competency</th><th>Type</th><th>Score</th><th>Status</th><th>Attempt</th></tr></thead>
                  <tbody>
                    {profile.assessmentRecords.map((record) => (
                      <tr key={record.assessmentResultId}>
                        <td>{record.programName}</td>
                        <td>{record.competencyName}</td>
                        <td>{record.assessmentType || "Assessment"}</td>
                        <td>{record.score}%</td>
                        <td>{record.status}</td>
                        <td>{record.attemptNumber}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </Section>
            </>
          )}

          {activeModule === "competency-tracking" && (
            <Section id="competency-tracking" title="Competency Tracking">
            <table>
              <thead><tr><th>Code</th><th>Competency</th><th>Status</th><th>Due</th><th>Completed</th></tr></thead>
              <tbody>
                {profile.competencyProgress.map((item) => (
                  <tr key={item.competencyCode}>
                    <td>{item.competencyCode}</td>
                    <td>{item.competencyName}</td>
                    <td>{item.status}</td>
                    <td>{item.dueDate}</td>
                    <td>{item.completedOn ?? "-"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            </Section>
          )}

          {activeModule === "interventions" && (
            <Section id="interventions" title="Intervention Management">
            <table>
              <thead><tr><th>Type</th><th>Owner</th><th>Status</th><th>Due</th><th>Outcome</th></tr></thead>
              <tbody>
                {profile.interventionHistory.map((item) => (
                  <tr key={item.interventionId}>
                    <td>{item.type}</td>
                    <td>{item.ownerName}</td>
                    <td>{item.status}</td>
                    <td>{item.dueDate}</td>
                    <td>{item.outcome ?? "Pending"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            </Section>
          )}

          {activeModule === "risk-assessment" && (
            <Section id="risk-assessment" title="Risk Assessment">
            <table>
              <thead><tr><th>Risk</th><th>Reason</th><th>Triggered Rules</th><th>Assessed</th></tr></thead>
              <tbody>
                {profile.riskHistory.map((item) => (
                  <tr key={item.riskAssessmentId}>
                    <td>{riskLabel(item.riskLevel)}</td>
                    <td>{item.reason}</td>
                    <td>{item.triggeredRules.join(", ") || "-"}</td>
                    <td>{new Date(item.assessedAt).toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            </Section>
          )}

          {activeModule === "data-ingestion" && ingestion && (
            <Section id="data-ingestion" title="Data Ingestion">
              <section className="grid compact">
                <MetricCard label="Sources" value={String(ingestion.status.dataSources)} detail="Configured source systems" />
                <MetricCard label="Imports" value={String(ingestion.status.importBatches)} detail="Tracked import batches" />
                <MetricCard label="Open Errors" value={String(ingestion.status.openErrors)} detail="Validation failures" />
                <MetricCard label="Open Issues" value={String(ingestion.status.openReconciliationIssues)} detail="Reconciliation queue" />
              </section>

              <div className="actions">
                <button onClick={submitSampleAttendance}>Ingest attendance</button>
                <button onClick={submitSampleAssessment}>Ingest assessment</button>
                <button onClick={submitSampleMilestone}>Complete milestone</button>
                <button className="secondary" onClick={submitInvalidAttendance}>Create validation error</button>
              </div>
              {ingestionMessage && <p className="notice">{ingestionMessage}</p>}

              <h3>Data Sources</h3>
              <table>
                <thead><tr><th>Name</th><th>Type</th><th>Owner</th><th>Active</th></tr></thead>
                <tbody>
                  {ingestion.sources.map((source) => (
                    <tr key={source.id}>
                      <td>{source.name}</td>
                      <td>{source.sourceType}</td>
                      <td>{source.owner}</td>
                      <td>{source.isActive ? "Yes" : "No"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>

              <h3>Recent Imports</h3>
              <table>
                <thead><tr><th>Source</th><th>Type</th><th>Status</th><th>Success</th><th>Failed</th></tr></thead>
                <tbody>
                  {ingestion.imports.map((batch) => (
                    <tr key={batch.id}>
                      <td>{batch.dataSourceName}</td>
                      <td>{batch.importType}</td>
                      <td>{batch.status}</td>
                      <td>{batch.successfulRecords}</td>
                      <td>{batch.failedRecords}</td>
                    </tr>
                  ))}
                </tbody>
              </table>

              <h3>Open Import Errors</h3>
              <table>
                <thead><tr><th>Field</th><th>Error</th><th>Raw Value</th></tr></thead>
                <tbody>
                  {ingestion.errors.map((item) => (
                    <tr key={item.id}>
                      <td>{item.fieldName}</td>
                      <td>{item.errorMessage}</td>
                      <td>{item.rawValue ?? "-"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </Section>
          )}

          {activeModule === "validation" && ingestion && (
            <Section id="validation" title="Validation & Reconciliation">
              <p>Open validation errors and reconciliation issues from data ingestion.</p>
              <section className="grid compact">
                <MetricCard label="Open Import Errors" value={String(ingestion.status.openErrors)} detail="Rows requiring correction" />
                <MetricCard label="Open Reconciliation Issues" value={String(ingestion.status.openReconciliationIssues)} detail="Cross-source mismatches" />
              </section>
              <table>
                <thead><tr><th>Entity</th><th>Issue Type</th><th>Description</th><th>Resolved</th></tr></thead>
                <tbody>
                  {ingestion.issues.map((issue) => (
                    <tr key={issue.id}>
                      <td>{issue.entityType}</td>
                      <td>{issue.issueType}</td>
                      <td>{issue.description}</td>
                      <td>{issue.isResolved ? "Yes" : "No"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </Section>
          )}

          {activeModule === "risk-rules" && <ModulePlaceholder
            id="risk-rules"
            title="Risk Rules Engine"
            features={[
              "Configure attendance, assessment, milestone, and composite rules",
              "Version and activate/deactivate rules",
              "Show triggered rule reasons on learner profiles"
            ]}
          />}
          {activeModule === "notifications" && <ModulePlaceholder
            id="notifications"
            title="Notifications & Escalations"
            features={[
              "Notify trainers and managers when learners become high risk",
              "Escalate overdue interventions",
              "Track notification status and delivery channel"
            ]}
          />}
          {activeModule === "dashboards" && <ModulePlaceholder
            id="dashboards"
            title="Dashboards"
            features={[
              "Risk overview",
              "Attendance gaps",
              "Assessment performance",
              "Intervention tracker",
              "Compliance readiness"
            ]}
          />}
          {activeModule === "compliance-reporting" && <ModulePlaceholder
            id="compliance-reporting"
            title="Compliance Reporting"
            features={[
              "Mandatory training compliance report",
              "At-risk learner report",
              "Intervention effectiveness report",
              "Audit evidence export"
            ]}
          />}
          {activeModule === "roles" && <ModulePlaceholder
            id="roles"
            title="Users & Roles"
            features={[
              "Employee Learner",
              "Trainer",
              "Manager",
              "L&D Administrator",
              "Compliance Officer",
              "System Administrator"
            ]}
          />}
          {activeModule === "audit" && <ModulePlaceholder
            id="audit"
            title="Audit & History"
            features={[
              "Track rule changes, intervention updates, imports, report exports, and admin actions",
              "Preserve historical risk status and data correction evidence"
            ]}
          />}
          {activeModule === "administration" && <ModulePlaceholder
            id="administration"
            title="Administration"
            features={[
              "Configure data sources, lookup values, report settings, and system configuration",
              "Monitor imports and data quality queues"
            ]}
          />}
        </>
      )}
    </main>
  );
}

async function getJson<T>(path: string): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`);
  if (!response.ok) {
    throw new Error(`API returned ${response.status}`);
  }
  return response.json() as Promise<T>;
}

function MetricCard({ label, value, detail }: { label: string; value: string; detail: string }) {
  return (
    <article className="card metric">
      <p>{label}</p>
      <strong>{value}</strong>
      <span>{detail}</span>
    </article>
  );
}

function ModulePlaceholder({ id, title, features }: { id: string; title: string; features: string[] }) {
  return (
    <Section id={id} title={title}>
      <p className="module-status">Module link is available. Feature implementation is planned after the currently implemented Learner Profile and Data Ingestion modules.</p>
      <ul className="feature-list">
        {features.map((feature) => <li key={feature}>{feature}</li>)}
      </ul>
    </Section>
  );
}

function Section({ id, title, children }: { id?: string; title: string; children: React.ReactNode }) {
  return (
    <section className="panel" id={id}>
      <h2>{title}</h2>
      {children}
    </section>
  );
}

createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
