
using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Multa;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/processos-infracao/{processoInfracaoId:long}/multas")]
public class MultaController : ControllerBase
{
    private readonly IMultaService _multaService;

    public MultaController(IMultaService multaService)
    {
        _multaService = multaService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(MultaDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create(
        long processoInfracaoId,
        [FromBody] CreateMultaDTO dto)
    {
        try
        {
            var multa = _multaService.Create(processoInfracaoId, dto);

            return CreatedAtAction(
                nameof(FindByProcessoInfracaoId),
                new { processoInfracaoId },
                multa);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(MultaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FindByProcessoInfracaoId(long processoInfracaoId)
    {
        var multa = _multaService.FindByProcessoInfracaoId(processoInfracaoId);

        if (multa == null)
            return NotFound(new { message = "Multa não encontrada para este processo." });

        return Ok(multa);
    }
}
