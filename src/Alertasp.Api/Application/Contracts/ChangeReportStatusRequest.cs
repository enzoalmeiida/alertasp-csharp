using Alertasp.Api.Domain;

namespace Alertasp.Api.Application.Contracts;

public sealed record ChangeReportStatusRequest(ReportStatus Status);
