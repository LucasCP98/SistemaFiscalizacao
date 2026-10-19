
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaFiscalizacao.API.Controllers;
using SistemaFiscalizacao.Application.DTOs.Denuncia;
using SistemaFiscalizacao.Application.Services;

namespace SistemaFiscalizacao.Tests.Unit.Controllers;

public class DenunciaControllerTests
{
    private readonly Mock<IDenunciaService> _serviceMock;
    private readonly Mock<ILogger<DenunciaController>> _loggerMock;
    private readonly DenunciaController _controller;

    public DenunciaControllerTests()
    {
        _serviceMock = new Mock<IDenunciaService>();
        _loggerMock = new Mock<ILogger<DenunciaController>>();

        _controller = new DenunciaController(
            _serviceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Get_SemParametros_DeveRetornarOkComListaDeDenuncias()
    {
        // Arrange
        var denuncias = new List<DenunciaDTO>
        {
            new()
            {
                Id = 1,
                Protocolo = "DEN-TESTE-001",
                Descricao = "Construção irregular",
                Canal = "Site",
                Anonima = true,
                DataCriacao = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Protocolo = "DEN-TESTE-002",
                Descricao = "Descarte irregular de resíduos",
                Canal = "Telefone",
                Anonima = false,
                DataCriacao = DateTime.UtcNow
            }
        };

        _serviceMock
            .Setup(service => service.FindAll())
            .Returns(denuncias);

        // Act
        var resultado = _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var listaRetornada =
            Assert.IsType<List<DenunciaDTO>>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(2, listaRetornada.Count);

        _serviceMock.Verify(
            service => service.FindAll(),
            Times.Once);
    }

    [Fact]
    public void Get_ComIdExistente_DeveRetornarOkComDenuncia()
    {
        // Arrange
        var denuncia = new DenunciaDTO
        {
            Id = 1,
            Protocolo = "DEN-TESTE-001",
            Descricao = "Construção irregular",
            Canal = "Site",
            Anonima = true,
            DataCriacao = DateTime.UtcNow
        };

        _serviceMock
            .Setup(service => service.FindById(1))
            .Returns(denuncia);

        // Act
        var resultado = _controller.Get(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var denunciaRetornada =
            Assert.IsType<DenunciaDTO>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(1, denunciaRetornada.Id);
        Assert.Equal(
            denuncia.Protocolo,
            denunciaRetornada.Protocolo);

        _serviceMock.Verify(
            service => service.FindById(1),
            Times.Once);
    }

    [Fact]
    public void Get_ComIdInexistente_DeveRetornarNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(service => service.FindById(999))
            .Returns((DenunciaDTO?)null);

        // Act
        var resultado = _controller.Get(999);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);

        _serviceMock.Verify(
            service => service.FindById(999),
            Times.Once);
    }

    [Fact]
    public void Post_ComDenunciaValida_DeveRetornarCreatedAtAction()
    {
        // Arrange
        var dto = new CreateDenunciaDTO
        {
            Descricao = "Construção irregular",
            Canal = "Site",
            Anonima = true
        };

        var denunciaCriada = new DenunciaDTO
        {
            Id = 1,
            Protocolo = "DEN-TESTE-001",
            Descricao = dto.Descricao,
            Canal = dto.Canal,
            Anonima = dto.Anonima,
            DataCriacao = DateTime.UtcNow
        };

        _serviceMock
            .Setup(service => service.Create(dto))
            .Returns(denunciaCriada);

        // Act
        var resultado = _controller.Post(dto);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(resultado);

        var retorno =
            Assert.IsType<DenunciaDTO>(createdResult.Value);

        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(nameof(DenunciaController.Get),
            createdResult.ActionName);
        Assert.Equal(1L, createdResult.RouteValues!["id"]);
        Assert.Equal(denunciaCriada.Id, retorno.Id);

        _serviceMock.Verify(
            service => service.Create(dto),
            Times.Once);
    }
}
