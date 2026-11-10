
namespace SistemaFiscalizacao.Domain.Entities;

public class Notificacao : BaseEntity
{
    public long DenunciaId { get; set; }

    public long VistoriaId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataEmissao { get; set; }

    public int PrazoDias { get; set; }

    public DateTime DataLimite { get; set; }

    public DateTime DataCriacao { get; set; }

    public Denuncia Denuncia { get; set; } = null!;

    public Vistoria Vistoria { get; set; } = null!;
}
