
using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Notificacao;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/denuncias/{denunciaId:long}/vistorias/{vistoriaId:long}/notificacoes")]
public class NotificacaoController : ControllerBase
{
    private readonly INotificacaoService _notificacaoService;

    public NotificacaoController(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(NotificacaoDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create(
        [FromRoute] long denunciaId,
        [FromRoute] long vistoriaId,
        [FromBody] CreateNotificacaoDTO dto)
    {
        try
        {
            var notificacao = _notificacaoService.Create(
                denunciaId,
                vistoriaId,
                dto);

            return StatusCode(StatusCodes.Status201Created, notificacao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("/api/denuncias/{denunciaId:long}/notificacoes")]
    [ProducesResponseType(typeof(List<NotificacaoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult FindAllByDenunciaId(
    [FromRoute] long denunciaId)
    {
        try
        {
            var notificacoes = _notificacaoService
                .FindAllByDenunciaId(denunciaId);

            return Ok(notificacoes);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
