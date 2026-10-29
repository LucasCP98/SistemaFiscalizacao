using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Domain.Rules;
using Xunit;

namespace SistemaFiscalizacao.Tests.Unit.Rules;

public class DenunciaStatusRulesTests
{
    [Theory]
    [InlineData(
        StatusDenuncia.Recebida,
        StatusDenuncia.EmTriagem)]
    [InlineData(
        StatusDenuncia.EmTriagem,
        StatusDenuncia.AguardandoAtribuicao)]
    [InlineData(
        StatusDenuncia.AguardandoAtribuicao,
        StatusDenuncia.EmVistoria)]
    [InlineData(
        StatusDenuncia.EmVistoria,
        StatusDenuncia.AguardandoRegularizacao)]
    public void PodeTransicionar_QuandoTransicaoForPermitida_RetornaTrue(
        StatusDenuncia atual,
        StatusDenuncia proximo)
    {
        var resultado = DenunciaStatusRules.PodeTransicionar(
            atual, proximo);

        Assert.True(resultado);
    }

    [Theory]
    [InlineData(
        StatusDenuncia.Recebida,
        StatusDenuncia.Encerrada)]
    [InlineData(
        StatusDenuncia.Encerrada,
        StatusDenuncia.Recebida)]
    [InlineData(
        StatusDenuncia.EmTriagem,
        StatusDenuncia.EmVistoria)]
    public void PodeTransicionar_QuandoTransicaoForInvalida_RetornaFalse(
        StatusDenuncia atual,
        StatusDenuncia proximo)
    {
        var resultado = DenunciaStatusRules.PodeTransicionar(
            atual, proximo);

        Assert.False(resultado);
    }
}