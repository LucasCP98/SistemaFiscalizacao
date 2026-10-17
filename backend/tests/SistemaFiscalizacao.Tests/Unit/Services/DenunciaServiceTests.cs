
using Moq;
using SistemaFiscalizacao.Application.DTOs.Denuncia;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Repositories;

namespace SistemaFiscalizacao.Tests.Unit.Services;

public class DenunciaServiceTests
{
    private readonly Mock<IRepository<Denuncia>> _repositoryMock;
    private readonly DenunciaServiceImplementation _service;

    public DenunciaServiceTests()
    {
        _repositoryMock = new Mock<IRepository<Denuncia>>();
        _service = new DenunciaServiceImplementation(_repositoryMock.Object);
    }

    [Fact]
    public void Create_DenunciaValida_DeveRetornarDenunciaCriada()
    {
        // Arrange
        var dto = new CreateDenunciaDTO
        {
            Descricao = "Construção irregular",
            Canal = "Site",
            Anonima = true
        };

        _repositoryMock
            .Setup(repository => repository.Create(
                It.IsAny<Denuncia>()))
            .Returns((Denuncia denuncia) =>
            {
                denuncia.Id = 1;
                return denuncia;
            });

        // Act
        var resultado = _service.Create(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.Id);
        Assert.Equal(dto.Descricao, resultado.Descricao);
        Assert.Equal(dto.Canal, resultado.Canal);
        Assert.Equal(dto.Anonima, resultado.Anonima);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Protocolo));

        _repositoryMock.Verify(
            repository => repository.Create(
                It.Is<Denuncia>(denuncia =>
                    denuncia.Descricao == dto.Descricao &&
                    denuncia.Canal == dto.Canal &&
                    denuncia.Anonima == dto.Anonima)),
            Times.Once);
    }

    [Fact]
    public void FindById_IdExistente_DeveRetornarDenuncia()
    {
        // Arrange
        var denuncia = new Denuncia
        {
            Id = 1,
            Protocolo = "DEN-TESTE-001",
            Descricao = "Construção irregular",
            Canal = "Site",
            Anonima = true,
            DataCriacao = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(repository => repository.FindById(1))
            .Returns(denuncia);

        // Act
        var resultado = _service.FindById(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(denuncia.Id, resultado.Id);
        Assert.Equal(denuncia.Protocolo, resultado.Protocolo);
        Assert.Equal(denuncia.Descricao, resultado.Descricao);

        _repositoryMock.Verify(
            repository => repository.FindById(1),
            Times.Once);
    }

    [Fact]
    public void FindById_IdInexistente_DeveRetornarNull()
    {
        // Arrange
        _repositoryMock
            .Setup(repository => repository.FindById(999))
            .Returns((Denuncia?)null);

        // Act
        var resultado = _service.FindById(999);

        // Assert
        Assert.Null(resultado);

        _repositoryMock.Verify(
            repository => repository.FindById(999),
            Times.Once);
    }

    [Fact]
    public void FindAll_ComDenunciasCadastradas_DeveRetornarLista()
    {
        // Arrange
        var denuncias = new List<Denuncia>
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

        _repositoryMock
            .Setup(repository => repository.FindAll())
            .Returns(denuncias);

        // Act
        var resultado = _service.FindAll();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        Assert.Equal(1, resultado[0].Id);
        Assert.Equal(2, resultado[1].Id);

        _repositoryMock.Verify(
            repository => repository.FindAll(),
            Times.Once);
    }

    [Fact]
    public void FindAll_SemDenuncias_DeveRetornarListaVazia()
    {
        // Arrange
        _repositoryMock
            .Setup(repository => repository.FindAll())
            .Returns(new List<Denuncia>());

        // Act
        var resultado = _service.FindAll();

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);

        _repositoryMock.Verify(
            repository => repository.FindAll(),
            Times.Once);
    }
}
