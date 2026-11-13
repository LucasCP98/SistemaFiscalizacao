
namespace SistemaFiscalizacao.Application.DTOs.Multa;

public class MultaDTO
{
    public long Id { get; set; }

    public long ProcessoInfracaoId { get; set; }

    public decimal Valor { get; set; }

    public decimal ValorMinimo { get; set; }

    public decimal ValorMaximo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataAplicacao { get; set; }

    public DateTime DataCriacao { get; set; }
}
