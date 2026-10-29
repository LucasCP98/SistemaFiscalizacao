using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Denuncia;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/denuncias")]
public class DenunciaController : ControllerBase
{
    private readonly IDenunciaService _denunciaService;
    private readonly ILogger<DenunciaController> _logger;

    public DenunciaController(
        IDenunciaService denunciaService,
        ILogger<DenunciaController> logger)
    {
        _denunciaService = denunciaService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(List<DenunciaDTO>))]
    [ProducesResponseType(401)]
    public IActionResult Get()
    {
        _logger.LogInformation("Buscando todas as denúncias");

        return Ok(_denunciaService.FindAll());
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(200, Type = typeof(DenunciaDTO))]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public IActionResult Get(long id)
    {
        _logger.LogInformation(
            "Buscando denúncia com ID: {Id}",
            id);

        var denuncia = _denunciaService.FindById(id);

        if (denuncia == null)
        {
            _logger.LogWarning(
                "Denúncia com ID {Id} não encontrada",
                id);

            return NotFound();
        }

        return Ok(denuncia);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(DenunciaDTO))]
    [ProducesResponseType(400)]
    public IActionResult Post(
        [FromBody] CreateDenunciaDTO denuncia)
    {
        _logger.LogInformation(
            "Criando nova denúncia");

        var createdDenuncia =
            _denunciaService.Create(denuncia);

        return CreatedAtAction(
            nameof(Get),
            new { id = createdDenuncia.Id },
            createdDenuncia);
    }

    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(typeof(DenunciaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AlterarStatus(
    long id,
    [FromBody] AlterarStatusDenunciaDTO dto)
    {
        try
        {
            var denuncia = _denunciaService.AlterarStatus(id, dto);

            if (denuncia is null)
                return NotFound();

            return Ok(denuncia);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
 