using Alertasp.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Alertasp.Api.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<FloodReport> FloodReports => Set<FloodReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var report = modelBuilder.Entity<FloodReport>();
        report.HasKey(item => item.Id);
        report.Property(item => item.RoadName).HasMaxLength(120).IsRequired();
        report.Property(item => item.District).HasMaxLength(120).IsRequired();
        report.Property(item => item.Source).HasConversion<string>().HasMaxLength(32);
        report.Property(item => item.Status).HasConversion<string>().HasMaxLength(32);
        report.Property(item => item.ObservedAt).HasConversion(
            value => value.ToUnixTimeMilliseconds(), value => DateTimeOffset.FromUnixTimeMilliseconds(value));
        report.Property(item => item.CreatedAt).HasConversion(
            value => value.ToUnixTimeMilliseconds(), value => DateTimeOffset.FromUnixTimeMilliseconds(value));
        report.Property(item => item.UpdatedAt).HasConversion(
            value => value.ToUnixTimeMilliseconds(), value => DateTimeOffset.FromUnixTimeMilliseconds(value));
        report.HasIndex(item => item.District);
        report.HasIndex(item => item.Status);
        report.HasIndex(item => item.ObservedAt);
    }
}
