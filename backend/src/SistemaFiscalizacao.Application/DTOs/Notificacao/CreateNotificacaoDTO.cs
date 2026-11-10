using System.ComponentModel.DataAnnotations;

namespace SistemaFiscalizacao.Application.DTOs.Notificacao;

public class CreateNotificacaoDTO
{
    [Required]
    [StringLength(2000)]
    public string Descricao { get; set; } = string.Empty;

    [Range(1, 365)]
    public int PrazoDias { get; set; }
}
