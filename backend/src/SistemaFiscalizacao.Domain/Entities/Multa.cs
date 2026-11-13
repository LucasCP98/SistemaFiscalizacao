
namespace SistemaFiscalizacao.Domain.Entities;

public class Multa : BaseEntity
{
    public long ProcessoInfracaoId { get; set; }

    public decimal Valor { get; set; }

    public decimal ValorMinimo { get; set; }

    public decimal ValorMaximo { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataAplicacao { get; set; }

    public DateTime DataCriacao { get; set; }

    public ProcessoInfracao ProcessoInfracao { get; set; } = null!;
}
