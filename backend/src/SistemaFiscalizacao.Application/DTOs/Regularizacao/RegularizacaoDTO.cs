
namespace SistemaFiscalizacao.Application.DTOs.Regularizacao;

public class RegularizacaoDTO
{
    public long Id { get; set; }
    public long NotificacaoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInformada { get; set; }
    public bool DentroDoPrazo { get; set; }
    public DateTime DataCriacao { get; set; }
}
