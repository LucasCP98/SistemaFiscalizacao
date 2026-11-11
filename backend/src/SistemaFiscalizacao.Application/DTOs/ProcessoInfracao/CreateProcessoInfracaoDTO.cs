
using System.ComponentModel.DataAnnotations;

namespace SistemaFiscalizacao.Application.DTOs.ProcessoInfracao;

public class CreateProcessoInfracaoDTO
{
    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Descricao { get; set; } = string.Empty;
}
