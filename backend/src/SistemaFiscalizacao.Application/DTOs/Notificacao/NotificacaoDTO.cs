
namespace SistemaFiscalizacao.Application.DTOs.Notificacao;

public class NotificacaoDTO
{
    public long Id { get; set; }

    public long DenunciaId { get; set; }

    public long VistoriaId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataEmissao { get; set; }

    public int PrazoDias { get; set; }

    public DateTime DataLimite { get; set; }

    public DateTime DataCriacao { get; set; }
}
