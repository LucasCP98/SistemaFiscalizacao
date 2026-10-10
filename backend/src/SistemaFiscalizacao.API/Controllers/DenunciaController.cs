using Microsoft.AspNetCore.Mvc;
using SistemaFiscalizacao.Application.DTOs.Denuncia;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPut("{id:long}")]
    [ProducesResponseType(200, Type = typeof(DenunciaDTO))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Put(
        long id,
        [FromBody] DenunciaDTO denuncia)
    {
        if (id != denuncia.Id)
        {
            return BadRequest(
                "O ID da URL é diferente do ID da denúncia.");
        }

        _logger.LogInformation(
            "Atualizando denúncia com ID: {Id}",
            id);

        var updatedDenuncia =
            _denunciaService.Update(denuncia);

        if (updatedDenuncia == null)
        {
            _logger.LogWarning(
                "Denúncia com ID {Id} não encontrada",
                id);

            return NotFound();
        }

        return Ok(updatedDenuncia);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult Delete(long id)
    {
        var denuncia = _denunciaService.FindById(id);

        if (denuncia == null)
        {
            _logger.LogWarning(
                "Tentativa de excluir denúncia inexistente. ID: {Id}",
                id);

            return NotFound();
        }

        _logger.LogInformation(
            "Excluindo denúncia com ID: {Id}",
            id);

        _denunciaService.Delete(id);

        return NoContent();
    }
}