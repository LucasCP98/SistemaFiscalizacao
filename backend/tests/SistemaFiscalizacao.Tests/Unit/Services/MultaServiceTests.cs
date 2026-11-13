
using Moq;
using SistemaFiscalizacao.Application.DTOs.Multa;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Tests.Services;

public class MultaServiceTests
{
    private readonly Mock<IRepository<ProcessoInfracao>> _processoRepository;
    private readonly Mock<IRepository<Multa>> _multaRepository;
    private readonly Mock<ITransactionManager> _transactionManager;
    private readonly MultaServiceImplementation _service;

    public MultaServiceTests()
    {
        _processoRepository = new Mock<IRepository<ProcessoInfracao>>();
        _multaRepository = new Mock<IRepository<Multa>>();
        _transactionManager = new Mock<ITransactionManager>();

        _transactionManager
            .Setup(x => x.ExecuteInTransaction(It.IsAny<Action>()))
            .Callback<Action>(operation => operation());

        _service = new MultaServiceImplementation(
            _processoRepository.Object,
            _multaRepository.Object,
            _transactionManager.Object);
    }

    // CADASTRO DE MULTA

    [Fact]
    public void Create_WithValidData_ShouldCreateFineAndUpdateProcess()
    {
        var processo = CriarProcesso();

        ConfigurarProcesso(processo);
        ConfigurarMultas();
        ConfigurarPersistencia();

        var result = _service.Create(1, CriarDTO(1500m));

        Assert.NotNull(result);
        Assert.Equal(1, result.ProcessoInfracaoId);
        Assert.Equal(1500m, result.Valor);
        Assert.Equal(500m, result.ValorMinimo);
        Assert.Equal(5000m, result.ValorMaximo);
        Assert.Equal("Multa aplicada por irregularidade.", result.Descricao);

        Assert.Equal(1500m, processo.ValorMulta);

        Assert.Equal(
            StatusProcessoInfracao.AguardandoPagamento,
            processo.Status);

        _multaRepository.Verify(
            r => r.Create(It.Is<Multa>(
                m => m.Valor == 1500m &&
                     m.ProcessoInfracaoId == 1)),
            Times.Once);

        _processoRepository.Verify(
            r => r.Update(processo),
            Times.Once);

        _transactionManager.Verify(
            t => t.ExecuteInTransaction(It.IsAny<Action>()),
            Times.Once);
    }

    [Fact]
    public void Create_WithMinimumAllowedValue_ShouldCreateFine()
    {
        var processo = CriarProcesso();

        ConfigurarProcesso(processo);
        ConfigurarMultas();
        ConfigurarPersistencia();

        var result = _service.Create(1, CriarDTO(500m));

        Assert.Equal(500m, result.Valor);

        _multaRepository.Verify(
            r => r.Create(It.Is<Multa>(
                m => m.Valor == 500m)),
            Times.Once);
    }

    [Fact]
    public void Create_WithMaximumAllowedValue_ShouldCreateFine()
    {
        var processo = CriarProcesso();

        ConfigurarProcesso(processo);
        ConfigurarMultas();
        ConfigurarPersistencia();

        var result = _service.Create(1, CriarDTO(5000m));

        Assert.Equal(5000m, result.Valor);

        _multaRepository.Verify(
            r => r.Create(It.Is<Multa>(
                m => m.Valor == 5000m)),
            Times.Once);
    }

    [Fact]
    public void Create_WithValueBelowMinimum_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());
        ConfigurarMultas();

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarDTO(499.99m)));

        Assert.Contains("O valor da multa deve estar entre", exception.Message);

        _multaRepository.Verify(
            r => r.Create(It.IsAny<Multa>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithValueAboveMaximum_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());
        ConfigurarMultas();

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarDTO(5000.01m)));

        Assert.Contains("O valor da multa deve estar entre", exception.Message);

        _multaRepository.Verify(
            r => r.Create(It.IsAny<Multa>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithNonExistingProcess_ShouldThrowInvalidOperationException()
    {
        _processoRepository
            .Setup(r => r.FindById(1))
            .Returns((ProcessoInfracao?)null);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarDTO(1500m)));

        Assert.Equal(
            "Processo de infração não encontrado.",
            exception.Message);

        _multaRepository.Verify(
            r => r.Create(It.IsAny<Multa>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithInvalidProcessStatus_ShouldThrowInvalidOperationException()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.AguardandoPagamento;

        ConfigurarProcesso(processo);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarDTO(1500m)));

        Assert.Equal(
            "A multa só pode ser aplicada em processos com status Aberto.",
            exception.Message);

        _multaRepository.Verify(
            r => r.Create(It.IsAny<Multa>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithExistingFine_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());

        ConfigurarMultas(new Multa
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Valor = 1500m
        });

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarDTO(1500m)));

        Assert.Equal(
            "Já existe uma multa vinculada a este processo.",
            exception.Message);

        _multaRepository.Verify(
            r => r.Create(It.IsAny<Multa>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());
        ConfigurarMultas();

        var dto = CriarDTO(1500m);
        dto.Descricao = " ";

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        Assert.Equal(
            "A descrição da multa é obrigatória.",
            exception.Message);

        _multaRepository.Verify(
            r => r.Create(It.IsAny<Multa>()),
            Times.Never);
    }

    // CONSULTA DE MULTA

    [Fact]
    public void FindByProcessoInfracaoId_WithExistingFine_ShouldReturnFineDTO()
    {
        var multa = CriarMulta();

        ConfigurarMultas(multa);

        var result = _service.FindByProcessoInfracaoId(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.ProcessoInfracaoId);
        Assert.Equal(1500m, result.Valor);
        Assert.Equal(500m, result.ValorMinimo);
        Assert.Equal(5000m, result.ValorMaximo);
    }

    [Fact]
    public void FindByProcessoInfracaoId_WithNonExistingFine_ShouldReturnNull()
    {
        ConfigurarMultas();

        var result = _service.FindByProcessoInfracaoId(1);

        Assert.Null(result);
    }

    // MÉTODOS AUXILIARES

    private void ConfigurarProcesso(ProcessoInfracao processo)
    {
        _processoRepository
            .Setup(r => r.FindById(processo.Id))
            .Returns(processo);
    }

    private void ConfigurarMultas(params Multa[] multas)
    {
        _multaRepository
            .Setup(r => r.FindAll())
            .Returns(multas.ToList());
    }

    private void ConfigurarPersistencia()
    {
        _multaRepository
            .Setup(r => r.Create(It.IsAny<Multa>()))
            .Returns((Multa m) => m);

        _processoRepository
            .Setup(r => r.Update(It.IsAny<ProcessoInfracao>()))
            .Returns((ProcessoInfracao p) => p);
    }

    private static ProcessoInfracao CriarProcesso()
    {
        return new ProcessoInfracao
        {
            Id = 1,
            DenunciaId = 1,
            Status = StatusProcessoInfracao.Aberto
        };
    }

    private static Multa CriarMulta()
    {
        return new Multa
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Valor = 1500m,
            ValorMinimo = 500m,
            ValorMaximo = 5000m,
            Descricao = "Multa aplicada por irregularidade."
        };
    }

    private static CreateMultaDTO CriarDTO(decimal valor)
    {
        return new CreateMultaDTO
        {
            Valor = valor,
            Descricao = "Multa aplicada por irregularidade."
        };
    }
}
