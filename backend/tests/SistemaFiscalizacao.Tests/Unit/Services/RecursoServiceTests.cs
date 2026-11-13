
using Moq;
using SistemaFiscalizacao.Application.DTOs.Recurso;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Tests.Services;

public class RecursoServiceTests
{
    private readonly Mock<IRepository<ProcessoInfracao>> _processoRepository;
    private readonly Mock<IRepository<Multa>> _multaRepository;
    private readonly Mock<IRepository<Recurso>> _recursoRepository;
    private readonly Mock<ITransactionManager> _transactionManager;
    private readonly RecursoServiceImplementation _service;

    public RecursoServiceTests()
    {
        _processoRepository = new Mock<IRepository<ProcessoInfracao>>();
        _multaRepository = new Mock<IRepository<Multa>>();
        _recursoRepository = new Mock<IRepository<Recurso>>();
        _transactionManager = new Mock<ITransactionManager>();

        _transactionManager
            .Setup(x => x.ExecuteInTransaction(It.IsAny<Action>()))
            .Callback<Action>(operation => operation());

        _service = new RecursoServiceImplementation(
            _processoRepository.Object,
            _multaRepository.Object,
            _recursoRepository.Object,
            _transactionManager.Object);
    }

    // CADASTRO DE RECURSO

    [Fact]
    public void Create_WithValidData_ShouldCreatePendingAppeal()
    {
        var processo = CriarProcesso();

        ConfigurarProcesso(processo);
        ConfigurarMultas(new Multa
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Valor = 1500m
        });
        ConfigurarRecursos();
        ConfigurarPersistencia();

        var result = _service.Create(1, CriarCreateDTO());

        Assert.NotNull(result);
        Assert.Equal(1, result.ProcessoInfracaoId);
        Assert.Equal("Justificativa do recurso.", result.Justificativa);
        Assert.Equal(StatusRecurso.Pendente, result.Status);
        Assert.Null(result.Decisao);
        Assert.Null(result.DataDecisao);

        Assert.Equal(
            StatusProcessoInfracao.EmRecurso,
            processo.Status);

        _recursoRepository.Verify(
            r => r.Create(It.Is<Recurso>(
                x => x.Status == StatusRecurso.Pendente)),
            Times.Once);

        _processoRepository.Verify(
            r => r.Update(processo),
            Times.Once);

        _transactionManager.Verify(
            t => t.ExecuteInTransaction(It.IsAny<Action>()),
            Times.Once);
    }

    [Fact]
    public void Create_WithNonExistingProcess_ShouldThrowInvalidOperationException()
    {
        _processoRepository
            .Setup(r => r.FindById(1))
            .Returns((ProcessoInfracao?)null);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarCreateDTO()));

        Assert.Equal(
            "Processo de infração não encontrado.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Create(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithInvalidProcessStatus_ShouldThrowInvalidOperationException()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.Aberto;

        ConfigurarProcesso(processo);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarCreateDTO()));

        Assert.Equal(
            "O recurso só pode ser apresentado em processos aguardando pagamento.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Create(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithoutFine_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());
        ConfigurarMultas();

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarCreateDTO()));

        Assert.Equal(
            "Não existe multa vinculada a este processo.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Create(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithPendingAppeal_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());

        ConfigurarMultas(new Multa
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Valor = 1500m
        });

        ConfigurarRecursos(new Recurso
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Status = StatusRecurso.Pendente
        });

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, CriarCreateDTO()));

        Assert.Equal(
            "Já existe um recurso pendente para este processo.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Create(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithEmptyJustification_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());

        ConfigurarMultas(new Multa
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Valor = 1500m
        });

        ConfigurarRecursos();

        var dto = CriarCreateDTO();
        dto.Justificativa = " ";

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        Assert.Equal(
            "A justificativa do recurso é obrigatória.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Create(It.IsAny<Recurso>()),
            Times.Never);
    }

    // DECISÃO DO RECURSO

    [Fact]
    public void Decidir_WithDeferimento_ShouldCancelProcess()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.EmRecurso;

        var recurso = CriarRecurso();

        ConfigurarProcesso(processo);
        ConfigurarRecurso(recurso);
        ConfigurarPersistencia();

        var result = _service.Decidir(
            1,
            1,
            CriarDecidirDTO(StatusRecurso.Deferido));

        Assert.Equal(StatusRecurso.Deferido, result.Status);

        Assert.Equal(
            StatusProcessoInfracao.Cancelado,
            processo.Status);

        Assert.Equal(
            "Recurso analisado pela administração.",
            result.Decisao);

        Assert.NotNull(result.DataDecisao);

        _recursoRepository.Verify(
            r => r.Update(recurso),
            Times.Once);

        _processoRepository.Verify(
            r => r.Update(processo),
            Times.Once);

        _transactionManager.Verify(
            t => t.ExecuteInTransaction(It.IsAny<Action>()),
            Times.Once);
    }

    [Fact]
    public void Decidir_WithIndeferimento_ShouldReturnProcessToAwaitingPayment()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.EmRecurso;

        var recurso = CriarRecurso();

        ConfigurarProcesso(processo);
        ConfigurarRecurso(recurso);
        ConfigurarPersistencia();

        var result = _service.Decidir(
            1,
            1,
            CriarDecidirDTO(StatusRecurso.Indeferido));

        Assert.Equal(StatusRecurso.Indeferido, result.Status);

        Assert.Equal(
            StatusProcessoInfracao.AguardandoPagamento,
            processo.Status);

        Assert.NotNull(result.DataDecisao);

        _recursoRepository.Verify(
            r => r.Update(recurso),
            Times.Once);

        _processoRepository.Verify(
            r => r.Update(processo),
            Times.Once);
    }

    [Fact]
    public void Decidir_WithNonExistingProcess_ShouldThrowInvalidOperationException()
    {
        _processoRepository
            .Setup(r => r.FindById(1))
            .Returns((ProcessoInfracao?)null);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(
                1,
                1,
                CriarDecidirDTO(StatusRecurso.Deferido)));

        Assert.Equal(
            "Processo de infração não encontrado.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Update(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Decidir_WithNonExistingAppeal_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());

        _recursoRepository
            .Setup(r => r.FindById(1))
            .Returns((Recurso?)null);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(
                1,
                1,
                CriarDecidirDTO(StatusRecurso.Deferido)));

        Assert.Equal(
            "Recurso não encontrado para este processo.",
            exception.Message);

        _processoRepository.Verify(
            r => r.Update(It.IsAny<ProcessoInfracao>()),
            Times.Never);
    }

    [Fact]
    public void Decidir_WithAppealFromAnotherProcess_ShouldThrowInvalidOperationException()
    {
        ConfigurarProcesso(CriarProcesso());

        var recurso = CriarRecurso();
        recurso.ProcessoInfracaoId = 2;

        ConfigurarRecurso(recurso);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(
                1,
                1,
                CriarDecidirDTO(StatusRecurso.Deferido)));

        Assert.Equal(
            "Recurso não encontrado para este processo.",
            exception.Message);
    }

    [Fact]
    public void Decidir_WithInvalidProcessStatus_ShouldThrowInvalidOperationException()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.AguardandoPagamento;

        ConfigurarProcesso(processo);
        ConfigurarRecurso(CriarRecurso());

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(
                1,
                1,
                CriarDecidirDTO(StatusRecurso.Deferido)));

        Assert.Equal(
            "O processo não está em análise de recurso.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Update(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Decidir_WithAlreadyAnalyzedAppeal_ShouldThrowInvalidOperationException()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.EmRecurso;

        var recurso = CriarRecurso();
        recurso.Status = StatusRecurso.Indeferido;

        ConfigurarProcesso(processo);
        ConfigurarRecurso(recurso);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(
                1,
                1,
                CriarDecidirDTO(StatusRecurso.Deferido)));

        Assert.Equal(
            "Este recurso já foi analisado.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Update(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Decidir_WithInvalidStatus_ShouldThrowInvalidOperationException()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.EmRecurso;

        ConfigurarProcesso(processo);
        ConfigurarRecurso(CriarRecurso());

        var dto = CriarDecidirDTO((StatusRecurso)99);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(1, 1, dto));

        Assert.Equal(
            "A decisão deve ser Deferido ou Indeferido.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Update(It.IsAny<Recurso>()),
            Times.Never);
    }

    [Fact]
    public void Decidir_WithEmptyDecision_ShouldThrowInvalidOperationException()
    {
        var processo = CriarProcesso();
        processo.Status = StatusProcessoInfracao.EmRecurso;

        ConfigurarProcesso(processo);
        ConfigurarRecurso(CriarRecurso());

        var dto = CriarDecidirDTO(StatusRecurso.Deferido);
        dto.Decisao = " ";

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Decidir(1, 1, dto));

        Assert.Equal(
            "A justificativa da decisão é obrigatória.",
            exception.Message);

        _recursoRepository.Verify(
            r => r.Update(It.IsAny<Recurso>()),
            Times.Never);
    }

    // MÉTODOS AUXILIARES

    private void ConfigurarProcesso(ProcessoInfracao processo)
    {
        _processoRepository
            .Setup(r => r.FindById(processo.Id))
            .Returns(processo);
    }

    private void ConfigurarRecurso(Recurso recurso)
    {
        _recursoRepository
            .Setup(r => r.FindById(recurso.Id))
            .Returns(recurso);
    }

    private void ConfigurarMultas(params Multa[] multas)
    {
        _multaRepository
            .Setup(r => r.FindAll())
            .Returns(multas.ToList());
    }

    private void ConfigurarRecursos(params Recurso[] recursos)
    {
        _recursoRepository
            .Setup(r => r.FindAll())
            .Returns(recursos.ToList());
    }

    private void ConfigurarPersistencia()
    {
        _recursoRepository
            .Setup(r => r.Create(It.IsAny<Recurso>()))
            .Returns((Recurso r) => r);

        _recursoRepository
            .Setup(r => r.Update(It.IsAny<Recurso>()))
            .Returns((Recurso r) => r);

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
            Status = StatusProcessoInfracao.AguardandoPagamento
        };
    }

    private static Recurso CriarRecurso()
    {
        return new Recurso
        {
            Id = 1,
            ProcessoInfracaoId = 1,
            Justificativa = "Justificativa do recurso.",
            Status = StatusRecurso.Pendente
        };
    }

    private static CreateRecursoDTO CriarCreateDTO()
    {
        return new CreateRecursoDTO
        {
            Justificativa = "Justificativa do recurso."
        };
    }

    private static DecidirRecursoDTO CriarDecidirDTO(StatusRecurso status)
    {
        return new DecidirRecursoDTO
        {
            Status = status,
            Decisao = "Recurso analisado pela administração."
        };
    }
}
