
using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Regularizacao;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/denuncias/{denunciaId:long}/notificacoes/{notificacaoId:long}/regularizacoes")]
public class RegularizacaoController : ControllerBase
{
    private readonly IRegularizacaoService _regularizacaoService;

    public RegularizacaoController(
        IRegularizacaoService regularizacaoService)
    {
        _regularizacaoService = regularizacaoService;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(RegularizacaoDTO),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create(
        [FromRoute] long denunciaId,
        [FromRoute] long notificacaoId,
        [FromBody] CreateRegularizacaoDTO dto)
    {
        try
        {
            var regularizacao = _regularizacaoService.Create(
                denunciaId,
                notificacaoId,
                dto);

            return StatusCode(
                StatusCodes.Status201Created,
                regularizacao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
