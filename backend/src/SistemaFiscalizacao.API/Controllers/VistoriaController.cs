
using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Vistoria;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/denuncias/{denunciaId:long}/vistorias")]
public class VistoriaController : ControllerBase
{
    private readonly IVistoriaService _vistoriaService;

    public VistoriaController(IVistoriaService vistoriaService)
    {
        _vistoriaService = vistoriaService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(VistoriaDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Create(
        long denunciaId,
        [FromBody] CreateVistoriaDTO dto)
    {
        try
        {
            var vistoria = _vistoriaService.Create(denunciaId, dto);

            return Created(
                $"/api/denuncias/{denunciaId}/vistorias/{vistoria.Id}",
                vistoria);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == "Denúncia não encontrada.")
                return NotFound(new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}
