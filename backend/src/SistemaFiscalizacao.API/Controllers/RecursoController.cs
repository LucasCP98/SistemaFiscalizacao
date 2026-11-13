
using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Recurso;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/processos-infracao/{processoInfracaoId:long}/recursos")]
public class RecursoController : ControllerBase
{
    private readonly IRecursoService _recursoService;

    public RecursoController(IRecursoService recursoService)
    {
        _recursoService = recursoService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RecursoDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create(
        long processoInfracaoId,
        [FromBody] CreateRecursoDTO dto)
    {
        try
        {
            var recurso = _recursoService.Create(processoInfracaoId, dto);

            return CreatedAtAction(
                nameof(FindAllByProcessoInfracaoId),
                new { processoInfracaoId },
                recurso);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RecursoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FindAllByProcessoInfracaoId(long processoInfracaoId)
    {
        try
        {
            var recursos = _recursoService
                .FindAllByProcessoInfracaoId(processoInfracaoId);

            return Ok(recursos);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [HttpPut("{recursoId:long}/decisao")]
    [ProducesResponseType(typeof(RecursoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Decidir(
        long processoInfracaoId,
        long recursoId,
        [FromBody] DecidirRecursoDTO dto)
    {
        try
        {
            var recurso = _recursoService.Decidir(
                processoInfracaoId,
                recursoId,
                dto);

            return Ok(recurso);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }

}
