using Alertasp.Api.Application.Abstractions;
using Alertasp.Api.Application.Contracts;
using Alertasp.Api.Domain;

namespace Alertasp.Api.Application;

public sealed class FloodReportService(IFloodReportRepository repository, TimeProvider timeProvider) : IFloodReportService
{
    public async Task<IReadOnlyList<ReportResponse>> SearchAsync(string? district, ReportStatus? status, CancellationToken cancellationToken)
    {
        var reports = await repository.SearchAsync(district, status, cancellationToken);
        return reports.Select(ReportResponse.From).ToArray();
    }

    public async Task<ReportResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var report = await repository.GetByIdAsync(id, cancellationToken);
        return report is null ? null : ReportResponse.From(report);
    }

    public async Task<ReportResponse> CreateAsync(CreateReportRequest request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var report = new FloodReport(request.RoadName, request.District, request.Latitude, request.Longitude,
            request.ObservedAt, request.Source, now);
        await repository.AddAsync(report, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ReportResponse.From(report);
    }

    public async Task<ReportResponse?> ChangeStatusAsync(Guid id, ReportStatus status, CancellationToken cancellationToken)
    {
        var report = await repository.GetByIdAsync(id, cancellationToken);
        if (report is null) return null;
        report.ChangeStatus(status, timeProvider.GetUtcNow());
        await repository.SaveChangesAsync(cancellationToken);
        return ReportResponse.From(report);
    }
}
