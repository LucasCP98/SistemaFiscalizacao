namespace SistemaFiscalizacao.Domain.Entities;

public class Regularizacao : BaseEntity
{
    public long NotificacaoId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataInformada { get; set; }

    public bool DentroDoPrazo { get; set; }

    public DateTime DataCriacao { get; set; }

    public Notificacao Notificacao { get; set; } = null!;
}