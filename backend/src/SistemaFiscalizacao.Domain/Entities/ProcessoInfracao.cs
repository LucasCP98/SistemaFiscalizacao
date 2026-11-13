
using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Domain.Entities;

public class ProcessoInfracao : BaseEntity
{
    public long DenunciaId { get; set; }

    public long VistoriaId { get; set; }

    public string NumeroProcesso { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataAbertura { get; set; }

    public StatusProcessoInfracao Status { get; set; }

    public decimal? ValorMulta { get; set; }

    public DateTime DataCriacao { get; set; }

    public Denuncia Denuncia { get; set; } = null!;

    public Vistoria Vistoria { get; set; } = null!;
    
    public Multa? Multa { get; set; }

    public ICollection<Recurso> Recursos { get; set; } = new List<Recurso>();
}
