using Alertasp.Api.Application.Contracts;
using Alertasp.Api.Domain;

namespace Alertasp.Api.Application.Abstractions;

public interface IFloodReportService
{
    Task<IReadOnlyList<ReportResponse>> SearchAsync(string? district, ReportStatus? status, CancellationToken cancellationToken);
    Task<ReportResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ReportResponse> CreateAsync(CreateReportRequest request, CancellationToken cancellationToken);
    Task<ReportResponse?> ChangeStatusAsync(Guid id, ReportStatus status, CancellationToken cancellationToken);
}
