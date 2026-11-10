
using Moq;
using SistemaFiscalizacao.Application.DTOs.Notificacao;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using Xunit;

namespace SistemaFiscalizacao.Tests.Services;

public class NotificacaoServiceTests
{
    private readonly Mock<IRepository<Denuncia>> _denunciaRepository;
    private readonly Mock<IRepository<Vistoria>> _vistoriaRepository;
    private readonly Mock<IRepository<Notificacao>> _notificacaoRepository;

    private readonly NotificacaoServiceImplementation _service;

    public NotificacaoServiceTests()
    {
        _denunciaRepository = new Mock<IRepository<Denuncia>>();
        _vistoriaRepository = new Mock<IRepository<Vistoria>>();
        _notificacaoRepository = new Mock<IRepository<Notificacao>>();

        _service = new NotificacaoServiceImplementation(
            _denunciaRepository.Object,
            _vistoriaRepository.Object,
            _notificacaoRepository.Object);
    }

    // Cenário 1: Criação de notificação com sucesso
    [Fact]
    public void Create_DeveCriarNotificacao_QuandoDadosForemValidos()
    {
        // Arrange
        var denuncia = CriarDenuncia();
        var vistoria = CriarVistoria();

        var dto = new CreateNotificacaoDTO
        {
            Descricao = "Regularizar construção sem autorização.",
            PrazoDias = 15
        };

        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(x => x.FindById(1))
            .Returns(vistoria);

        // Act
        var resultado = _service.Create(1, 1, dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.DenunciaId);
        Assert.Equal(1, resultado.VistoriaId);
        Assert.Equal(dto.Descricao, resultado.Descricao);
        Assert.Equal(15, resultado.PrazoDias);

        Assert.Equal(
            resultado.DataEmissao.AddDays(15),
            resultado.DataLimite);

        _notificacaoRepository.Verify(
            x => x.Create(It.IsAny<Notificacao>()),
            Times.Once);
    }

    // Cenário 2: Denúncia não encontrada
    [Fact]
    public void Create_DeveLancarExcecao_QuandoDenunciaNaoExistir()
    {
        // Arrange
        var dto = CriarDTO();

        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns((Denuncia)null!);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, dto));

        // Assert
        Assert.Equal("Denúncia não encontrada.", exception.Message);

        _notificacaoRepository.Verify(
            x => x.Create(It.IsAny<Notificacao>()),
            Times.Never);
    }

    // Cenário 3: Vistoria não encontrada
    [Fact]
    public void Create_DeveLancarExcecao_QuandoVistoriaNaoExistir()
    {
        // Arrange
        var denuncia = CriarDenuncia();

        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(x => x.FindById(1))
            .Returns((Vistoria)null!);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, CriarDTO()));

        // Assert
        Assert.Equal("Vistoria não encontrada.", exception.Message);
    }

    // Cenário 4: Vistoria não pertence à denúncia
    [Fact]
    public void Create_DeveLancarExcecao_QuandoVistoriaNaoPertencerADenuncia()
    {
        // Arrange
        var denuncia = CriarDenuncia();
        var vistoria = CriarVistoria();
        vistoria.DenunciaId = 2;

        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(x => x.FindById(1))
            .Returns(vistoria);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, CriarDTO()));

        // Assert
        Assert.Equal(
            "A vistoria informada não pertence à denúncia.",
            exception.Message);
    }

    // Cenário 5: Vistoria sem irregularidade
    [Fact]
    public void Create_DeveLancarExcecao_QuandoVistoriaNaoIdentificarIrregularidade()
    {
        // Arrange
        var denuncia = CriarDenuncia();
        var vistoria = CriarVistoria();
        vistoria.IrregularidadeIdentificada = false;

        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(x => x.FindById(1))
            .Returns(vistoria);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, CriarDTO()));

        // Assert
        Assert.Equal(
            "Não é possível emitir uma notificação para uma vistoria sem irregularidade identificada.",
            exception.Message);
    }

    // Cenário 6: Denúncia com status inválido
    [Fact]
    public void Create_DeveLancarExcecao_QuandoDenunciaNaoEstiverAguardandoRegularizacao()
    {
        // Arrange
        var denuncia = CriarDenuncia();
        denuncia.Status = StatusDenuncia.EmTriagem;

        var vistoria = CriarVistoria();

        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(x => x.FindById(1))
            .Returns(vistoria);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, CriarDTO()));

        // Assert
        Assert.Equal(
            "A denúncia não está no status adequado para receber uma notificação.",
            exception.Message);
    }

    // Cenário 7: Descrição vazia
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_DeveLancarExcecao_QuandoDescricaoForVazia(string descricao)
    {
        // Arrange
        ConfigurarDadosValidos();

        var dto = new CreateNotificacaoDTO
        {
            Descricao = descricao,
            PrazoDias = 15
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, dto));

        // Assert
        Assert.Equal(
            "A descrição da notificação é obrigatória.",
            exception.Message);
    }

    // Cenário 8: Prazo inválido
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(366)]
    public void Create_DeveLancarExcecao_QuandoPrazoForInvalido(int prazoDias)
    {
        // Arrange
        ConfigurarDadosValidos();

        var dto = new CreateNotificacaoDTO
        {
            Descricao = "Regularizar construção.",
            PrazoDias = prazoDias
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, 1, dto));

        // Assert
        Assert.Equal(
            "O prazo deve ser maior que zero e não pode ultrapassar 365 dias.",
            exception.Message);
    }

    // Métodos auxiliares

    private static Denuncia CriarDenuncia()
    {
        return new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.AguardandoRegularizacao
        };
    }

    private static Vistoria CriarVistoria()
    {
        return new Vistoria
        {
            Id = 1,
            DenunciaId = 1,
            IrregularidadeIdentificada = true
        };
    }

    private static CreateNotificacaoDTO CriarDTO()
    {
        return new CreateNotificacaoDTO
        {
            Descricao = "Regularizar construção.",
            PrazoDias = 15
        };
    }

    private void ConfigurarDadosValidos()
    {
        _denunciaRepository
            .Setup(x => x.FindById(1))
            .Returns(CriarDenuncia());

        _vistoriaRepository
            .Setup(x => x.FindById(1))
            .Returns(CriarVistoria());
    }
}
