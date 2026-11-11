
using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.ProcessoInfracao;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api")]
public class ProcessoInfracaoController : ControllerBase
{
    private readonly IProcessoInfracaoService _processoInfracaoService;

    public ProcessoInfracaoController(
        IProcessoInfracaoService processoInfracaoService)
    {
        _processoInfracaoService = processoInfracaoService;
    }

    [HttpPost("denuncias/{denunciaId:long}/processos-infracao")]
    [ProducesResponseType(typeof(ProcessoInfracaoDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create(
        long denunciaId,
        [FromBody] CreateProcessoInfracaoDTO dto)
    {
        try
        {
            var processo = _processoInfracaoService.Create(denunciaId, dto);

            return CreatedAtAction(
                nameof(FindById),
                new { id = processo.Id },
                processo);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == "Denúncia não encontrada.")
                return NotFound(new { mensagem = ex.Message });

            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("processos-infracao/{id:long}")]
    [ProducesResponseType(typeof(ProcessoInfracaoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FindById(long id)
    {
        var processo = _processoInfracaoService.FindById(id);

        if (processo is null)
            return NotFound(new { mensagem = "Processo de infração não encontrado." });

        return Ok(processo);
    }

    [HttpGet("denuncias/{denunciaId:long}/processos-infracao")]
    [ProducesResponseType(typeof(ProcessoInfracaoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FindByDenunciaId(long denunciaId)
    {
        var processo = _processoInfracaoService.FindByDenunciaId(denunciaId);

        if (processo is null)
            return NotFound(new { mensagem = "Processo de infração não encontrado para esta denúncia." });

        return Ok(processo);
    }
}
