
using Moq;
using SistemaFiscalizacao.Application.DTOs.ProcessoInfracao;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using Xunit;

namespace SistemaFiscalizacao.Tests.Services;

public class ProcessoInfracaoServiceTests
{
    private readonly Mock<IRepository<Denuncia>> _denunciaRepository;
    private readonly Mock<IRepository<Vistoria>> _vistoriaRepository;
    private readonly Mock<IRepository<ProcessoInfracao>> _processoRepository;

    private readonly ProcessoInfracaoServiceImplementation _service;

    public ProcessoInfracaoServiceTests()
    {
        _denunciaRepository = new Mock<IRepository<Denuncia>>();
        _vistoriaRepository = new Mock<IRepository<Vistoria>>();
        _processoRepository = new Mock<IRepository<ProcessoInfracao>>();

        _service = new ProcessoInfracaoServiceImplementation(
            _denunciaRepository.Object,
            _vistoriaRepository.Object,
            _processoRepository.Object);
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateProcessoInfracao()
    {
        // Arrange
        var denuncia = CriarDenuncia();

        var vistoria = CriarVistoria(
            id: 10,
            numeroVistoria: 2,
            irregularidadeIdentificada: true);

        ConfigurarDenuncia(denuncia);
        ConfigurarVistorias(vistoria);
        ConfigurarProcessos();

        var dto = CriarDTO();

        // Act
        var resultado = _service.Create(denuncia.Id, dto);

        // Assert
        Assert.Equal(denuncia.Id, resultado.DenunciaId);
        Assert.Equal(vistoria.Id, resultado.VistoriaId);
        Assert.Equal(StatusProcessoInfracao.Aberto, resultado.Status);
        Assert.Equal("Infração constatada durante a vistoria.", resultado.Descricao);
        Assert.Null(resultado.ValorMulta);
        Assert.StartsWith("PROC-", resultado.NumeroProcesso);

        _processoRepository.Verify(
            x => x.Create(It.Is<ProcessoInfracao>(
                p => p.DenunciaId == denuncia.Id &&
                     p.VistoriaId == vistoria.Id)),
            Times.Once);
    }

    [Fact]
    public void Create_WithNonexistentDenuncia_ShouldThrowException()
    {
        // Arrange
        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns((Denuncia)null!);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarDTO()));

        // Assert
        Assert.Equal("Denúncia não encontrada.", exception.Message);
    }

    [Fact]
    public void Create_WithInvalidStatus_ShouldThrowException()
    {
        // Arrange
        var denuncia = CriarDenuncia();
        denuncia.Status = StatusDenuncia.Encerrada;

        ConfigurarDenuncia(denuncia);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(denuncia.Id, CriarDTO()));

        // Assert
        Assert.Contains("status adequado", exception.Message);
    }

    [Fact]
    public void Create_WhenProcessAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var denuncia = CriarDenuncia();
        ConfigurarDenuncia(denuncia);

        _processoRepository
            .Setup(x => x.FindAll())
            .Returns(new List<ProcessoInfracao>
            {
                new()
                {
                    Id = 1,
                    DenunciaId = denuncia.Id
                }
            });

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(denuncia.Id, CriarDTO()));

        // Assert
        Assert.Contains("Já existe um processo", exception.Message);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldThrowException()
    {
        // Arrange
        var denuncia = CriarDenuncia();

        ConfigurarDenuncia(denuncia);
        ConfigurarProcessos();

        var dto = new CreateProcessoInfracaoDTO
        {
            Descricao = " "
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(denuncia.Id, dto));

        // Assert
        Assert.Equal("A descrição da infração é obrigatória.", exception.Message);
    }

    [Fact]
    public void Create_WithoutInspection_ShouldThrowException()
    {
        // Arrange
        var denuncia = CriarDenuncia();

        ConfigurarDenuncia(denuncia);
        ConfigurarProcessos();
        ConfigurarVistorias();

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(denuncia.Id, CriarDTO()));

        // Assert
        Assert.Contains("Nenhuma vistoria", exception.Message);
    }

    [Fact]
    public void Create_WhenLastInspectionHasNoIrregularity_ShouldThrowException()
    {
        // Arrange
        var denuncia = CriarDenuncia();

        var vistoria = CriarVistoria(
            id: 10,
            numeroVistoria: 2,
            irregularidadeIdentificada: false);

        ConfigurarDenuncia(denuncia);
        ConfigurarProcessos();
        ConfigurarVistorias(vistoria);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(denuncia.Id, CriarDTO()));

        // Assert
        Assert.Contains("não identificou irregularidade", exception.Message);
    }

    [Fact]
    public void FindById_WithExistingProcess_ShouldReturnDTO()
    {
        // Arrange
        var processo = CriarProcesso();

        _processoRepository
            .Setup(x => x.FindById(processo.Id))
            .Returns(processo);

        // Act
        var resultado = _service.FindById(processo.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(processo.Id, resultado.Id);
        Assert.Equal(processo.DenunciaId, resultado.DenunciaId);
    }

    [Fact]
    public void FindById_WithNonexistentProcess_ShouldReturnNull()
    {
        // Arrange
        _processoRepository
            .Setup(x => x.FindById(999))
            .Returns((ProcessoInfracao)null!);

        // Act
        var resultado = _service.FindById(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void FindByDenunciaId_WithExistingProcess_ShouldReturnDTO()
    {
        // Arrange
        var processo = CriarProcesso();

        _processoRepository
            .Setup(x => x.FindAll())
            .Returns(new List<ProcessoInfracao> { processo });

        // Act
        var resultado = _service.FindByDenunciaId(processo.DenunciaId);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(processo.Id, resultado.Id);
        Assert.Equal(processo.DenunciaId, resultado.DenunciaId);
    }

    [Fact]
    public void FindByDenunciaId_WithoutProcess_ShouldReturnNull()
    {
        // Arrange
        _processoRepository
            .Setup(x => x.FindAll())
            .Returns(new List<ProcessoInfracao>());

        // Act
        var resultado = _service.FindByDenunciaId(999);

        // Assert
        Assert.Null(resultado);
    }

    private void ConfigurarDenuncia(Denuncia denuncia)
    {
        _denunciaRepository
            .Setup(x => x.FindById(denuncia.Id))
            .Returns(denuncia);
    }

    private void ConfigurarVistorias(params Vistoria[] vistorias)
    {
        _vistoriaRepository
            .Setup(x => x.FindAll())
            .Returns(vistorias.ToList());
    }

    private void ConfigurarProcessos()
    {
        _processoRepository
            .Setup(x => x.FindAll())
            .Returns(new List<ProcessoInfracao>());
    }

    private static Denuncia CriarDenuncia()
    {
        return new Denuncia
        {
            Id = 9,
            Status = StatusDenuncia.EmProcessoDeInfracao
        };
    }

    private static Vistoria CriarVistoria(
        long id,
        int numeroVistoria,
        bool irregularidadeIdentificada)
    {
        return new Vistoria
        {
            Id = id,
            DenunciaId = 9,
            NumeroVistoria = numeroVistoria,
            IrregularidadeIdentificada = irregularidadeIdentificada,
            DataVistoria = DateTime.UtcNow,
            Relato = "Vistoria realizada."
        };
    }

    private static CreateProcessoInfracaoDTO CriarDTO()
    {
        return new CreateProcessoInfracaoDTO
        {
            Descricao = "Infração constatada durante a vistoria."
        };
    }

    private static ProcessoInfracao CriarProcesso()
    {
        return new ProcessoInfracao
        {
            Id = 1,
            DenunciaId = 9,
            VistoriaId = 10,
            NumeroProcesso = "PROC-2026-000009",
            Descricao = "Infração constatada.",
            DataAbertura = DateTime.UtcNow,
            Status = StatusProcessoInfracao.Aberto,
            DataCriacao = DateTime.UtcNow
        };
    }
}
