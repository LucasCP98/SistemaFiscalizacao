
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

        // Executa a operação recebida pelo método de transação
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
        // Arrange
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        var dto = new CreateVistoriaDTO
        {
            DataVistoria = DateTime.Now,
            Relato = "Irregularidade identificada.",
            IrregularidadeIdentificada = true
        };

        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(r => r.Create(It.IsAny<Vistoria>()))
            .Returns((Vistoria v) => v);

        _denunciaRepository
            .Setup(r => r.Update(It.IsAny<Denuncia>()))
            .Returns((Denuncia d) => d);

        // Act
        var result = _service.Create(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.DenunciaId);
        Assert.True(result.IrregularidadeIdentificada);

        Assert.Equal(
            StatusDenuncia.AguardandoRegularizacao,
            denuncia.Status);

        Assert.Null(denuncia.Resultado);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Once);

        _denunciaRepository.Verify(
            r => r.Update(denuncia),
            Times.Once);
    }

    [Fact]
    public void Create_WithoutIrregularity_ShouldCloseDenunciaAsImprocedente()
    {
        // Arrange
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        var dto = new CreateVistoriaDTO
        {
            DataVistoria = DateTime.Now,
            Relato = "Nenhuma irregularidade encontrada.",
            IrregularidadeIdentificada = false
        };

        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        _vistoriaRepository
            .Setup(r => r.Create(It.IsAny<Vistoria>()))
            .Returns((Vistoria v) => v);

        _denunciaRepository
            .Setup(r => r.Update(It.IsAny<Denuncia>()))
            .Returns((Denuncia d) => d);

        // Act
        var result = _service.Create(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IrregularidadeIdentificada);

        Assert.Equal(
            StatusDenuncia.Encerrada,
            denuncia.Status);

        Assert.Equal(
            ResultadoDenuncia.Improcedente,
            denuncia.Resultado);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Once);

        _denunciaRepository.Verify(
            r => r.Update(denuncia),
            Times.Once);
    }

    [Fact]
    public void Create_WithNonExistingDenuncia_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns((Denuncia?)null);

        var dto = new CreateVistoriaDTO
        {
            DataVistoria = DateTime.Now,
            Relato = "Relato da vistoria.",
            IrregularidadeIdentificada = true
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        // Assert
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
        // Arrange
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.Recebida
        };

        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        var dto = new CreateVistoriaDTO
        {
            DataVistoria = DateTime.Now,
            Relato = "Relato da vistoria.",
            IrregularidadeIdentificada = true
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        // Assert
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
        // Arrange
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        var dto = new CreateVistoriaDTO
        {
            DataVistoria = default,
            Relato = "Relato da vistoria.",
            IrregularidadeIdentificada = true
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        // Assert
        Assert.Equal("A data da vistoria é obrigatória.", exception.Message);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Never);
    }

    [Fact]
    public void Create_WithEmptyReport_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        _denunciaRepository
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        var dto = new CreateVistoriaDTO
        {
            DataVistoria = DateTime.Now,
            Relato = " ",
            IrregularidadeIdentificada = true
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => _service.Create(1, dto));

        // Assert
        Assert.Equal("O relato da vistoria é obrigatório.", exception.Message);

        _vistoriaRepository.Verify(
            r => r.Create(It.IsAny<Vistoria>()),
            Times.Never);
    }
}
