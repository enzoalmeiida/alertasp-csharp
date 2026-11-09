using Alertasp.Api.Application.Abstractions;
using Alertasp.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Alertasp.Api.Infrastructure;

public sealed class EfFloodReportRepository(AppDbContext dbContext) : IFloodReportRepository
{
    public async Task<IReadOnlyList<FloodReport>> SearchAsync(string? district, ReportStatus? status, CancellationToken cancellationToken)
    {
        IQueryable<FloodReport> query = dbContext.FloodReports.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(district))
            query = query.Where(report => report.District == district.Trim());
        if (status.HasValue)
            query = query.Where(report => report.Status == status.Value);

        return await query.OrderByDescending(report => report.ObservedAt)
            .ThenByDescending(report => report.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<FloodReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.FloodReports.SingleOrDefaultAsync(report => report.Id == id, cancellationToken);

    public async Task AddAsync(FloodReport report, CancellationToken cancellationToken) =>
        await dbContext.FloodReports.AddAsync(report, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
