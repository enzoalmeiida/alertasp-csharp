using System.ComponentModel.DataAnnotations;
using Alertasp.Api.Domain;

namespace Alertasp.Api.Application.Contracts;

public sealed record CreateReportRequest(
    [Required, StringLength(120, MinimumLength = 2)] string RoadName,
    [Required, StringLength(120, MinimumLength = 2)] string District,
    [Range(typeof(decimal), "-90", "90")] decimal Latitude,
    [Range(typeof(decimal), "-180", "180")] decimal Longitude,
    DateTimeOffset ObservedAt,
    ReportSource Source);
