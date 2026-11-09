using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Alertasp.Tests;

public sealed class ReportsApiIntegrationTests
{
    [Fact]
    public async Task ReportsFlow_WritesFiltersUpdatesAndSurvivesRestart()
    {
        var databasePath = NewDatabasePath();
        try
        {
            Guid reportId;
            using (var factory = new AlertaspApiFactory(databasePath))
            using (var client = factory.CreateClient())
            {
                var swagger = await client.GetAsync("/swagger/v1/swagger.json");
                Assert.Equal(HttpStatusCode.OK, swagger.StatusCode);

                using var createResponse = await client.PostAsync("/api/reports", JsonBody(new
                {
                    roadName = "Rua de teste",
                    district = "Sé",
                    latitude = -23.5505m,
                    longitude = -46.6333m,
                    observedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
                    source = "SyntheticDemo"
                }));

                Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
                Assert.NotNull(createResponse.Headers.Location);
                using var created = await JsonDocument.ParseAsync(await createResponse.Content.ReadAsStreamAsync());
                reportId = created.RootElement.GetProperty("id").GetGuid();
                Assert.Equal("Reported", created.RootElement.GetProperty("status").GetString());

                var listResponse = await client.GetAsync("/api/reports?district=S%C3%A9&status=Reported");
                Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
                using var list = await JsonDocument.ParseAsync(await listResponse.Content.ReadAsStreamAsync());
                Assert.Equal(1, list.RootElement.GetArrayLength());
                Assert.Equal(reportId, list.RootElement[0].GetProperty("id").GetGuid());

                var noMatchResponse = await client.GetAsync("/api/reports?district=Pinheiros&status=Reported");
                using var noMatch = await JsonDocument.ParseAsync(await noMatchResponse.Content.ReadAsStreamAsync());
                Assert.Equal(0, noMatch.RootElement.GetArrayLength());

                using var updateResponse = await client.PatchAsync(
                    $"/api/reports/{reportId}/status", JsonBody(new { status = "Confirmed" }));
                Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

                var missingResponse = await client.GetAsync($"/api/reports/{Guid.Empty}");
                Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
            }

            // A new host and DbContext use the same on-disk SQLite file.
            using (var restartedFactory = new AlertaspApiFactory(databasePath))
            using (var restartedClient = restartedFactory.CreateClient())
            using (var persistedResponse = await restartedClient.GetAsync($"/api/reports/{reportId}"))
            {
                Assert.Equal(HttpStatusCode.OK, persistedResponse.StatusCode);
                using var persisted = await JsonDocument.ParseAsync(await persistedResponse.Content.ReadAsStreamAsync());
                Assert.Equal("Confirmed", persisted.RootElement.GetProperty("status").GetString());
                Assert.Equal("Rua de teste", persisted.RootElement.GetProperty("roadName").GetString());
            }
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    [Fact]
    public async Task Create_ReturnsBadRequestForInvalidCoordinatesAndFutureObservation()
    {
        var databasePath = NewDatabasePath();
        try
        {
            using var factory = new AlertaspApiFactory(databasePath);
            using var client = factory.CreateClient();

            using var invalidCoordinateResponse = await client.PostAsync("/api/reports", JsonBody(new
            {
                roadName = "Rua de teste",
                district = "Sé",
                latitude = 91m,
                longitude = -46m,
                observedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
                source = "Citizen"
            }));
            Assert.Equal(HttpStatusCode.BadRequest, invalidCoordinateResponse.StatusCode);

            using var futureObservationResponse = await client.PostAsync("/api/reports", JsonBody(new
            {
                roadName = "Rua de teste",
                district = "Sé",
                latitude = -23m,
                longitude = -46m,
                observedAt = DateTimeOffset.UtcNow.AddHours(1),
                source = "Citizen"
            }));
            Assert.Equal(HttpStatusCode.BadRequest, futureObservationResponse.StatusCode);
            using var problem = await JsonDocument.ParseAsync(await futureObservationResponse.Content.ReadAsStreamAsync());
            Assert.Equal("Dados inválidos", problem.RootElement.GetProperty("title").GetString());
        }
        finally
        {
            DeleteDatabaseFiles(databasePath);
        }
    }

    private static StringContent JsonBody<T>(T value) =>
        new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

    private static string NewDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"alertasp-integration-{Guid.NewGuid():N}.db");

    private static void DeleteDatabaseFiles(string path)
    {
        foreach (var candidate in new[] { path, path + "-shm", path + "-wal" })
            if (File.Exists(candidate)) File.Delete(candidate);
    }

    private sealed class AlertaspApiFactory(string databasePath) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Alertasp"] = $"Data Source={databasePath}"
                }));
        }
    }
}
