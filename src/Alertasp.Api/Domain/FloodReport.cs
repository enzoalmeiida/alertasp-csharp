namespace Alertasp.Api.Domain;

public sealed class FloodReport
{
    private FloodReport() { }

    public FloodReport(
        string roadName,
        string district,
        decimal latitude,
        decimal longitude,
        DateTimeOffset observedAt,
        ReportSource source,
        DateTimeOffset createdAt)
    {
        RoadName = RequireText(roadName, nameof(roadName));
        District = RequireText(district, nameof(district));
        if (latitude is < -90 or > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180) throw new ArgumentOutOfRangeException(nameof(longitude));
        if (observedAt > createdAt) throw new ArgumentException("O horário observado não pode estar no futuro.", nameof(observedAt));
        if (!Enum.IsDefined(source)) throw new ArgumentOutOfRangeException(nameof(source));

        Id = Guid.NewGuid();
        Latitude = latitude;
        Longitude = longitude;
        ObservedAt = observedAt;
        Source = source;
        Status = ReportStatus.Reported;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string RoadName { get; private set; } = string.Empty;
    public string District { get; private set; } = string.Empty;
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public DateTimeOffset ObservedAt { get; private set; }
    public ReportSource Source { get; private set; }
    public ReportStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void ChangeStatus(ReportStatus status, DateTimeOffset updatedAt)
    {
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        if (updatedAt < UpdatedAt) throw new ArgumentException("A atualização não pode anteceder a atualização anterior.", nameof(updatedAt));
        Status = status;
        UpdatedAt = updatedAt;
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("O campo é obrigatório.", parameterName);
        var cleaned = value.Trim();
        if (cleaned.Length > 120) throw new ArgumentException("O campo deve ter no máximo 120 caracteres.", parameterName);
        return cleaned;
    }
}
