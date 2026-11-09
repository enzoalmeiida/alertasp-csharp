using Alertasp.Api.Application;
using Alertasp.Api.Application.Abstractions;
using Alertasp.Api.Application.Contracts;
using Alertasp.Api.Domain;
using Xunit;

namespace Alertasp.Tests;

public sealed class FloodReportServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 8, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateAsync_SavesReportWithInitialStatusAndTimestamps()
    {
        var repository = new FakeFloodReportRepository();
        var service = new FloodReportService(repository, new FixedTimeProvider(Now));
        var request = new CreateReportRequest("Av. Paulista", "Bela Vista", -23.5617m, -46.6559m,
            Now.AddMinutes(-15), ReportSource.SyntheticDemo);

        var created = await service.CreateAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(ReportStatus.Reported, created.Status);
        Assert.Equal(Now, created.CreatedAt);
        Assert.Equal(Now, created.UpdatedAt);
        Assert.Equal(1, repository.SaveCount);
        Assert.Single(repository.Reports);
    }

    [Fact]
    public async Task ChangeStatusAsync_ReturnsNullWhenReportDoesNotExist()
    {
        var repository = new FakeFloodReportRepository();
        var service = new FloodReportService(repository, new FixedTimeProvider(Now));

        var updated = await service.ChangeStatusAsync(Guid.NewGuid(), ReportStatus.Confirmed, CancellationToken.None);

        Assert.Null(updated);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public void FloodReport_RejectsCoordinatesOutsideGeographicRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FloodReport(
            "Rua Um", "Sé", 91m, -46m, Now.AddMinutes(-1), ReportSource.Citizen, Now));
    }

    [Fact]
    public void FloodReport_RejectsObservationInTheFuture()
    {
        Assert.Throws<ArgumentException>(() => new FloodReport(
            "Rua Um", "Sé", -23m, -46m, Now.AddMinutes(1), ReportSource.Citizen, Now));
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FakeFloodReportRepository : IFloodReportRepository
    {
        public List<FloodReport> Reports { get; } = [];
        public int SaveCount { get; private set; }

        public Task<IReadOnlyList<FloodReport>> SearchAsync(string? district, ReportStatus? status, CancellationToken cancellationToken)
        {
            IEnumerable<FloodReport> query = Reports;
            if (!string.IsNullOrWhiteSpace(district)) query = query.Where(report => report.District == district);
            if (status.HasValue) query = query.Where(report => report.Status == status.Value);
            return Task.FromResult<IReadOnlyList<FloodReport>>(query.ToArray());
        }

        public Task<FloodReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(Reports.SingleOrDefault(report => report.Id == id));

        public Task AddAsync(FloodReport report, CancellationToken cancellationToken)
        {
            Reports.Add(report);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCount++;
            return Task.CompletedTask;
        }
    }
}
