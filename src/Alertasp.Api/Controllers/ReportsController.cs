using Alertasp.Api.Application.Abstractions;
using Alertasp.Api.Application.Contracts;
using Alertasp.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Alertasp.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Produces("application/json")]
public sealed class ReportsController(IFloodReportService service) : ControllerBase
{
    /// <summary>Lista ocorrências mais recentes, com filtros opcionais por distrito e situação.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ReportResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReportResponse>>> Search(
        [FromQuery] string? district,
        [FromQuery] ReportStatus? status,
        CancellationToken cancellationToken) =>
        Ok(await service.SearchAsync(district, status, cancellationToken));

    /// <summary>Consulta uma ocorrência pelo identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var report = await service.GetByIdAsync(id, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    /// <summary>Registra uma ocorrência sem armazenar dados pessoais do relator.</summary>
    [HttpPost]
    [ProducesResponseType<ReportResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportResponse>> Create(CreateReportRequest request, CancellationToken cancellationToken)
    {
        var report = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    /// <summary>Atualiza a situação de uma ocorrência.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType<ReportResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportResponse>> ChangeStatus(Guid id, ChangeReportStatusRequest request, CancellationToken cancellationToken)
    {
        var report = await service.ChangeStatusAsync(id, request.Status, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }
}
