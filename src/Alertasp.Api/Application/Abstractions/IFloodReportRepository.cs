using Alertasp.Api.Domain;

namespace Alertasp.Api.Application.Abstractions;

public interface IFloodReportRepository
{
    Task<IReadOnlyList<FloodReport>> SearchAsync(string? district, ReportStatus? status, CancellationToken cancellationToken);
    Task<FloodReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(FloodReport report, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
