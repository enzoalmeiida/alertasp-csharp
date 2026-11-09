using Alertasp.Api.Domain;

namespace Alertasp.Api.Application.Contracts;

public sealed record ReportResponse(
    Guid Id,
    string RoadName,
    string District,
    decimal Latitude,
    decimal Longitude,
    DateTimeOffset ObservedAt,
    ReportSource Source,
    ReportStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static ReportResponse From(FloodReport report) => new(
        report.Id, report.RoadName, report.District, report.Latitude, report.Longitude,
        report.ObservedAt, report.Source, report.Status, report.CreatedAt, report.UpdatedAt);
}
