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

type ExcelUploadResult = {
  fileName: string;
  importType: string;
  totalRecords: number;
  successfulRecords: number;
  failedRecords: number;
  messages: string[];
};

type IngestionFormType = "employee" | "attendance" | "assessment" | "competency";

type IngestionFormState = {
  employeeNumber: string;
  fullName: string;
  email: string;
  department: string;
  jobRole: string;
  managerEmail: string;
  programCode: string;
  sessionCode: string;
  status: string;
  attendancePercentage: string;
  attendanceDate: string;
  competencyCode: string;
  score: string;
  assessmentType: string;
  scoreType: string;
  attemptNumber: string;
  assessmentDate: string;
  dueDate: string;
  completedOn: string;
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

function defaultIngestionForm(): IngestionFormState {
  const today = new Date().toISOString().slice(0, 10);
  return {
    employeeNumber: "EMP-0101",
    fullName: "Nisha Varma",
    email: "nisha.varma@example.com",
    department: "Engineering",
    jobRole: "Engineer",
    managerEmail: "priya.manager@example.com",
    programCode: "SEC-AWARE-2026",
    sessionCode: "SEC-S1",
    status: "Present",
    attendancePercentage: "100",
    attendanceDate: today,
    competencyCode: "SEC-101",
    score: "85",
    assessmentType: "Quiz",
    scoreType: "Percentage",
    attemptNumber: "1",
    assessmentDate: today,
    dueDate: today,
    completedOn: today
  };
}

function formEndpoint(formType: IngestionFormType) {
  return {
    employee: "/api/data-ingestion/employees",
    attendance: "/api/data-ingestion/attendance",
    assessment: "/api/data-ingestion/assessments",
    competency: "/api/data-ingestion/competency-milestones"
  }[formType];
}

function formPayload(
  formType: IngestionFormType,
  dataSourceId: string,
  sourceRecordId: string,
  form: IngestionFormState
) {
  if (formType === "employee") {
    return {
      dataSourceId,
      employeeNumber: form.employeeNumber,
      fullName: form.fullName,
      email: form.email,
      department: form.department,
      jobRole: form.jobRole,
      managerEmail: form.managerEmail || null,
      sourceRecordId
    };
  }

  if (formType === "attendance") {
    return {
      dataSourceId,
      employeeNumber: form.employeeNumber,
      programCode: form.programCode,
      sessionCode: form.sessionCode,
      status: form.status,
      attendancePercentage: Number(form.attendancePercentage),
      attendanceDate: form.attendanceDate,
      sourceRecordId
    };
  }

  if (formType === "assessment") {
    return {
      dataSourceId,
      employeeNumber: form.employeeNumber,
      programCode: form.programCode,
      competencyCode: form.competencyCode,
      score: Number(form.score),
      status: form.status,
      assessmentType: form.assessmentType,
      scoreType: form.scoreType,
      attemptNumber: Number(form.attemptNumber),
      assessmentDate: form.assessmentDate,
      sourceRecordId
    };
  }

  return {
    dataSourceId,
    employeeNumber: form.employeeNumber,
    programCode: form.programCode,
    competencyCode: form.competencyCode,
    status: form.status,
    dueDate: form.dueDate,
    completedOn: form.completedOn || null,
    sourceRecordId
  };
}

function App() {
  const [activeModule, setActiveModule] = useState(moduleLinks[0].id);
  const [learners, setLearners] = useState<LearnerProfileSummary[]>([]);
  const [selectedLearnerId, setSelectedLearnerId] = useState<string>("");
  const [profile, setProfile] = useState<LearnerProfile | null>(null);
  const [ingestion, setIngestion] = useState<IngestionDashboard | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [ingestionMessage, setIngestionMessage] = useState<string | null>(null);
  const [excelResult, setExcelResult] = useState<ExcelUploadResult | null>(null);
  const [excelImportType, setExcelImportType] = useState("Attendance");
  const [selectedExcelFile, setSelectedExcelFile] = useState<File | null>(null);
  const [formType, setFormType] = useState<IngestionFormType>("attendance");
  const [ingestionForm, setIngestionForm] = useState<IngestionFormState>(() => defaultIngestionForm());
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

  async function uploadExcelFile() {
    if (!ingestion?.sources[0] || !selectedExcelFile) {
      setIngestionMessage("Choose a data source and Excel file before uploading.");
      return;
    }

    try {
      const body = new FormData();
      body.append("dataSourceId", ingestion.sources[0].id);
      body.append("importType", excelImportType);
      body.append("file", selectedExcelFile);

      const response = await fetch(`${apiBaseUrl}/api/data-ingestion/excel-upload`, {
        method: "POST",
        body
      });

      if (!response.ok) {
        throw new Error(`API returned ${response.status}`);
      }

      const result = (await response.json()) as ExcelUploadResult;
      setExcelResult(result);
      setIngestionMessage(`Excel upload complete: ${result.successfulRecords} succeeded, ${result.failedRecords} failed.`);
      await Promise.all([loadIngestionDashboard(), selectedLearnerId ? reloadProfile(selectedLearnerId) : Promise.resolve()]);
    } catch (err) {
      setIngestionMessage(err instanceof Error ? err.message : "Unable to upload Excel file.");
    }
  }

  async function submitIngestionForm() {
    if (!ingestion?.sources[0]) return;

    try {
      const dataSourceId = ingestion.sources[0].id;
      const sourceRecordId = `ui-form-${formType}-${Date.now()}`;
      const response = await postJson(formEndpoint(formType), formPayload(formType, dataSourceId, sourceRecordId, ingestionForm));
      setIngestionMessage(response.message);
      await Promise.all([loadIngestionDashboard(), selectedLearnerId ? reloadProfile(selectedLearnerId) : Promise.resolve()]);
    } catch (err) {
      setIngestionMessage(err instanceof Error ? err.message : "Unable to submit ingestion form.");
    }
  }

  function updateIngestionForm(field: keyof IngestionFormState, value: string) {
    setIngestionForm((current) => ({ ...current, [field]: value }));
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

              <div className="ingestion-panels">
                <section className="card">
                  <h3>Ingest via Excel File</h3>
                  <p className="module-status">
                    Upload an `.xlsx` file with a header row. Supported import types: Employee, Attendance, Assessment, and CompetencyMilestone.
                  </p>
                  <div className="form-grid">
                    <label>
                      Import type
                      <select value={excelImportType} onChange={(event) => setExcelImportType(event.target.value)}>
                        <option>Employee</option>
                        <option>Attendance</option>
                        <option>Assessment</option>
                        <option>CompetencyMilestone</option>
                      </select>
                    </label>
                    <label>
                      Excel file
                      <input
                        accept=".xlsx,.xls"
                        type="file"
                        onChange={(event) => setSelectedExcelFile(event.target.files?.[0] ?? null)}
                      />
                    </label>
                  </div>
                  <div className="actions">
                    <button onClick={uploadExcelFile}>Upload Excel</button>
                  </div>
                  {excelResult && (
                    <div className="notice">
                      <strong>{excelResult.fileName}</strong>: {excelResult.successfulRecords}/{excelResult.totalRecords} records ingested.
                      {excelResult.messages.length > 0 && (
                        <ul>
                          {excelResult.messages.slice(0, 5).map((message) => <li key={message}>{message}</li>)}
                        </ul>
                      )}
                    </div>
                  )}
                </section>

                <section className="card">
                  <h3>Ingest via UI Form</h3>
                  <div className="form-grid">
                    <label>
                      Record type
                      <select value={formType} onChange={(event) => setFormType(event.target.value as IngestionFormType)}>
                        <option value="employee">Employee</option>
                        <option value="attendance">Attendance</option>
                        <option value="assessment">Assessment</option>
                        <option value="competency">Competency Milestone</option>
                      </select>
                    </label>
                    <label>
                      Employee number
                      <input value={ingestionForm.employeeNumber} onChange={(event) => updateIngestionForm("employeeNumber", event.target.value)} />
                    </label>
                    {formType === "employee" && (
                      <>
                        <label>Full name<input value={ingestionForm.fullName} onChange={(event) => updateIngestionForm("fullName", event.target.value)} /></label>
                        <label>Email<input value={ingestionForm.email} onChange={(event) => updateIngestionForm("email", event.target.value)} /></label>
                        <label>Department<input value={ingestionForm.department} onChange={(event) => updateIngestionForm("department", event.target.value)} /></label>
                        <label>Job role<input value={ingestionForm.jobRole} onChange={(event) => updateIngestionForm("jobRole", event.target.value)} /></label>
                        <label>Manager email<input value={ingestionForm.managerEmail} onChange={(event) => updateIngestionForm("managerEmail", event.target.value)} /></label>
                      </>
                    )}
                    {formType !== "employee" && (
                      <>
                        <label>Program code<input value={ingestionForm.programCode} onChange={(event) => updateIngestionForm("programCode", event.target.value)} /></label>
                        <label>Status<input value={ingestionForm.status} onChange={(event) => updateIngestionForm("status", event.target.value)} /></label>
                      </>
                    )}
                    {formType === "attendance" && (
                      <>
                        <label>Session code<input value={ingestionForm.sessionCode} onChange={(event) => updateIngestionForm("sessionCode", event.target.value)} /></label>
                        <label>Attendance %<input type="number" value={ingestionForm.attendancePercentage} onChange={(event) => updateIngestionForm("attendancePercentage", event.target.value)} /></label>
                        <label>Attendance date<input type="date" value={ingestionForm.attendanceDate} onChange={(event) => updateIngestionForm("attendanceDate", event.target.value)} /></label>
                      </>
                    )}
                    {formType === "assessment" && (
                      <>
                        <label>Competency code<input value={ingestionForm.competencyCode} onChange={(event) => updateIngestionForm("competencyCode", event.target.value)} /></label>
                        <label>Score<input type="number" value={ingestionForm.score} onChange={(event) => updateIngestionForm("score", event.target.value)} /></label>
                        <label>Assessment type<input value={ingestionForm.assessmentType} onChange={(event) => updateIngestionForm("assessmentType", event.target.value)} /></label>
                        <label>Score type<input value={ingestionForm.scoreType} onChange={(event) => updateIngestionForm("scoreType", event.target.value)} /></label>
                        <label>Attempt<input type="number" value={ingestionForm.attemptNumber} onChange={(event) => updateIngestionForm("attemptNumber", event.target.value)} /></label>
                        <label>Assessment date<input type="date" value={ingestionForm.assessmentDate} onChange={(event) => updateIngestionForm("assessmentDate", event.target.value)} /></label>
                      </>
                    )}
                    {formType === "competency" && (
                      <>
                        <label>Competency code<input value={ingestionForm.competencyCode} onChange={(event) => updateIngestionForm("competencyCode", event.target.value)} /></label>
                        <label>Due date<input type="date" value={ingestionForm.dueDate} onChange={(event) => updateIngestionForm("dueDate", event.target.value)} /></label>
                        <label>Completed on<input type="date" value={ingestionForm.completedOn} onChange={(event) => updateIngestionForm("completedOn", event.target.value)} /></label>
                      </>
                    )}
                  </div>
                  <div className="actions">
                    <button onClick={submitIngestionForm}>Submit form record</button>
                  </div>
                </section>
              </div>

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
