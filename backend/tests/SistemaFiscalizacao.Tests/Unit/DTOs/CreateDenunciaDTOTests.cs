
using System.ComponentModel.DataAnnotations;
using SistemaFiscalizacao.Application.DTOs.Denuncia;

namespace SistemaFiscalizacao.Tests.Unit.DTOs;

public class CreateDenunciaDTOTests
{
    [Fact]
    public void Validar_DTOValido_DeveRetornarSemErros()
    {
        // Arrange
        var dto = new CreateDenunciaDTO
        {
            Descricao = "Construção irregular em um terreno",
            Canal = "Site",
            Anonima = true
        };

        // Act
        var resultados = Validar(dto);

        // Assert
        Assert.Empty(resultados);
    }

    [Fact]
    public void Validar_DescricaoVazia_DeveRetornarErro()
    {
        var dto = new CreateDenunciaDTO
        {
            Descricao = "",
            Canal = "Site",
            Anonima = true
        };

        var resultados = Validar(dto);

        Assert.Contains(resultados, erro =>
            erro.MemberNames.Contains(nameof(dto.Descricao)));
    }

    [Fact]
    public void Validar_CanalVazio_DeveRetornarErro()
    {
        var dto = new CreateDenunciaDTO
        {
            Descricao = "Construção irregular",
            Canal = "",
            Anonima = false
        };

        var resultados = Validar(dto);

        Assert.Contains(resultados, erro =>
            erro.MemberNames.Contains(nameof(dto.Canal)));
    }

    [Fact]
    public void Validar_DescricaoAcimaDoLimite_DeveRetornarErro()
    {
        var dto = new CreateDenunciaDTO
        {
            Descricao = new string('A', 2001),
            Canal = "Site",
            Anonima = false
        };

        var resultados = Validar(dto);

        Assert.Contains(resultados, erro =>
            erro.MemberNames.Contains(nameof(dto.Descricao)));
    }

    [Fact]
    public void Validar_CanalAcimaDoLimite_DeveRetornarErro()
    {
        var dto = new CreateDenunciaDTO
        {
            Descricao = "Construção irregular",
            Canal = new string('A', 51),
            Anonima = false
        };

        var resultados = Validar(dto);

        Assert.Contains(resultados, erro =>
            erro.MemberNames.Contains(nameof(dto.Canal)));
    }

    private static List<ValidationResult> Validar(object objeto)
    {
        var resultados = new List<ValidationResult>();

        Validator.TryValidateObject(
            objeto,
            new ValidationContext(objeto),
            resultados,
            validateAllProperties: true);

        return resultados;
    }
}
