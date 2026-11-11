
using Moq;
using SistemaFiscalizacao.Application.DTOs.Vistoria;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Tests.Services;

public class VistoriaServiceTests
{
    private readonly Mock<IRepository<Denuncia>> _denunciaRepository;
    private readonly Mock<IRepository<Vistoria>> _vistoriaRepository;
    private readonly VistoriaServiceImplementation _service;
    private readonly Mock<ITransactionManager> _transactionManager;

    public VistoriaServiceTests()
    {
        _denunciaRepository = new Mock<IRepository<Denuncia>>();
        _vistoriaRepository = new Mock<IRepository<Vistoria>>();
        _transactionManager = new Mock<ITransactionManager>();

        _transactionManager
            .Setup(x => x.ExecuteInTransaction(It.IsAny<Action>()))
            .Callback<Action>(operation => operation());

        _service = new VistoriaServiceImplementation(
            _denunciaRepository.Object,
            _vistoriaRepository.Object,
            _transactionManager.Object);
    }

    [Fact]
    public void Create_WithIrregularity_ShouldUpdateDenunciaToAguardandoRegularizacao()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        ConfigurarDenuncia(denuncia);
        ConfigurarVistoriasExistentes();
        ConfigurarPersistencia();

        var dto = CriarDTO(true);

        var result = _service.Create(1, dto);

        Assert.NotNull(result);
        Assert.Equal(1, result.DenunciaId);
        Assert.True(result.IrregularidadeIdentificada);

        Assert.Equal(
            StatusDenuncia.AguardandoRegularizacao,
            denuncia.Status);

        Assert.Null(denuncia.Resultado);

        _vistoriaRepository.Verify(
            r => r.Create(It.Is<Vistoria>(
                v => v.NumeroVistoria == 1)),
            Times.Once);

        _denunciaRepository.Verify(
            r => r.Update(denuncia),
            Times.Once);
    }

    [Fact]
    public void Create_WithoutIrregularity_ShouldCloseDenunciaAsImprocedente()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        ConfigurarDenuncia(denuncia);
        ConfigurarVistoriasExistentes();
        ConfigurarPersistencia();

        var dto = CriarDTO(false);

        var result = _service.Create(1, dto);

        Assert.NotNull(result);
        Assert.False(result.IrregularidadeIdentificada);

        Assert.Equal(
            StatusDenuncia.Encerrada,
            denuncia.Status);

        Assert.Equal(
            ResultadoDenuncia.Improcedente,
            denuncia.Resultado);

        _vistoriaRepository.Verify(
            r => r.Create(It.Is<Vistoria>(
                v => v.NumeroVistoria == 1)),
            Times.Once);

        _denunciaRepository.Verify(
            r => r.Update(denuncia),
            Times.Once);
    }

    [Fact]
    public void Create_SecondInspectionWithIrregularity_ShouldUpdateDenunciaToEmProcessoDeInfracao()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.AguardandoNovaVistoria
        };

        ConfigurarDenuncia(denuncia);

        ConfigurarVistoriasExistentes(
            new Vistoria
            {
                Id = 1,
                DenunciaId = 1,
                NumeroVistoria = 1
            });

        ConfigurarPersistencia();

        var dto = CriarDTO(true);

        var result = _service.Create(1, dto);

        Assert.NotNull(result);
        Assert.True(result.IrregularidadeIdentificada);

        Assert.Equal(
            StatusDenuncia.EmProcessoDeInfracao,
            denuncia.Status);

        Assert.Null(denuncia.Resultado);

        _vistoriaRepository.Verify(
            r => r.Create(It.Is<Vistoria>(
                v => v.NumeroVistoria == 2)),
            Times.Once);

        _denunciaRepository.Verify(
            r => r.Update(denuncia),
            Times.Once);
    }

    [Fact]
    public void Create_SecondInspectionWithoutIrregularity_ShouldCloseDenunciaAsRegularizada()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.AguardandoNovaVistoria
        };

        ConfigurarDenuncia(denuncia);

        ConfigurarVistoriasExistentes(
            new Vistoria
            {
                Id = 1,
                DenunciaId = 1,
                NumeroVistoria = 1
            });

        ConfigurarPersistencia();

        var dto = CriarDTO(false);

        var result = _service.Create(1, dto);

        Assert.NotNull(result);
        Assert.False(result.IrregularidadeIdentificada);

        Assert.Equal(
            StatusDenuncia.Encerrada,
            denuncia.Status);

        Assert.Equal(
            ResultadoDenuncia.Regularizada,
            denuncia.Resultado);

        _vistoriaRepository.Verify(
            r => r.Create(It.Is<Vistoria>(
                v => v.NumeroVistoria == 2)),
            Times.Once);

        _denunciaRepository.Verify(
            r => r.Update(denuncia),
            Times.Once);
    }

    [Fact]
    public void Create_WithNonExistingDenuncia_ShouldThrowInvalidOperationException()
    {
        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns((Denuncia?)null);

        var dto = CriarDTO(true);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        Assert.Equal("Denúncia não encontrada.", exception.Message);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Never);

        _denunciaRepository.Verify(
            r => r.Update(It.IsAny<Denuncia>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithInvalidDenunciaStatus_ShouldThrowInvalidOperationException()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.Recebida
        };

        ConfigurarDenuncia(denuncia);

        var dto = CriarDTO(true);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        Assert.Contains("status adequado", exception.Message);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Never);

        _denunciaRepository.Verify(
            r => r.Update(It.IsAny<Denuncia>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithDefaultDate_ShouldThrowInvalidOperationException()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        ConfigurarDenuncia(denuncia);

        var dto = CriarDTO(true);
        dto.DataVistoria = default;

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        Assert.Equal("A data da vistoria é obrigatória.", exception.Message);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithEmptyReport_ShouldThrowInvalidOperationException()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        ConfigurarDenuncia(denuncia);

        var dto = CriarDTO(true);
        dto.Relato = " ";

        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        Assert.Equal("O relato da vistoria é obrigatório.", exception.Message);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Never);
    }

    private void ConfigurarDenuncia(Denuncia denuncia)
    {
        _denunciaRepository
            .Setup(r => r.FindById(denuncia.Id))
            .Returns(denuncia);
    }

    private void ConfigurarVistoriasExistentes(params Vistoria[] vistorias)
    {
        _vistoriaRepository
            .Setup(r => r.FindAll())
            .Returns(vistorias.ToList());
    }

    private void ConfigurarPersistencia()
    {
        _vistoriaRepository
            .Setup(r => r.Create(It.IsAny<Vistoria>()))
            .Returns((Vistoria v) => v);

        _denunciaRepository
            .Setup(r => r.Update(It.IsAny<Denuncia>()))
            .Returns((Denuncia d) => d);
    }

    private static CreateVistoriaDTO CriarDTO(bool irregularidade)
    {
        return new CreateVistoriaDTO
        {
            DataVistoria = DateTime.Now,
            Relato = "Relato da vistoria.",
            IrregularidadeIdentificada = irregularidade
        };
    }
}
