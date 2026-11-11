
using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Application.DTOs.ProcessoInfracao;

public class ProcessoInfracaoDTO
{
    public long Id { get; set; }
    public long DenunciaId { get; set; }
    public long VistoriaId { get; set; }
    public string NumeroProcesso { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public StatusProcessoInfracao Status { get; set; }
    public decimal? ValorMulta { get; set; }
    public DateTime DataCriacao { get; set; }
}
