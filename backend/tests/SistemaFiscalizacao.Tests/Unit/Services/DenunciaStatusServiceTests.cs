using Moq;
using SistemaFiscalizacao.Application.DTOs.Denuncia;
using SistemaFiscalizacao.Application.Services.Implementation;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;

namespace SistemaFiscalizacao.Tests.Unit.Services;

public class DenunciaStatusServiceTests
{
    private readonly Mock<IRepository<Denuncia>> _repositoryMock;
    private readonly DenunciaServiceImplementation _service;

    public DenunciaStatusServiceTests()
    {
        _repositoryMock = new Mock<IRepository<Denuncia>>();
        _service = new DenunciaServiceImplementation(
            _repositoryMock.Object);
    }

    [Fact]
    public void AlterarStatus_TransicaoValida_AtualizaDenuncia()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.Recebida,
            Resultado = null
        };

        _repositoryMock
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        _repositoryMock
            .Setup(r => r.Update(It.IsAny<Denuncia>()))
            .Returns((Denuncia d) => d);

        var dto = new AlterarStatusDenunciaDTO
        {
            Status = StatusDenuncia.EmTriagem,
            Resultado = null
        };

        var resultado = _service.AlterarStatus(1, dto);

        Assert.NotNull(resultado);
        Assert.Equal(StatusDenuncia.EmTriagem, resultado.Status);
        Assert.Null(resultado.Resultado);

        _repositoryMock.Verify(
            r => r.Update(It.IsAny<Denuncia>()),
            Times.Once);
    }
    [Fact]
    public void AlterarStatus_TransicaoInvalida_LancaExcecao()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.Recebida
        };

        _repositoryMock
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        var dto = new AlterarStatusDenunciaDTO
        {
            Status = StatusDenuncia.Encerrada,
            Resultado = ResultadoDenuncia.Regularizada
        };

        Assert.Throws<InvalidOperationException>(
            () => _service.AlterarStatus(1, dto));

        _repositoryMock.Verify(
            r => r.Update(It.IsAny<Denuncia>()),
            Times.Never);
    }

    [Fact]
    public void AlterarStatus_EncerramentoSemResultado_LancaExcecao()
    {
        var denuncia = new Denuncia
        {
            Id = 1,
            Status = StatusDenuncia.EmVistoria
        };

        _repositoryMock
            .Setup(r => r.FindById(1))
            .Returns(denuncia);

        var dto = new AlterarStatusDenunciaDTO
        {
            Status = StatusDenuncia.Encerrada,
            Resultado = null
        };

        Assert.Throws<InvalidOperationException>(
            () => _service.AlterarStatus(1, dto));

        _repositoryMock.Verify(
            r => r.Update(It.IsAny<Denuncia>()),
            Times.Never);
    }
}