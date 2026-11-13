
using System.ComponentModel.DataAnnotations;

namespace SistemaFiscalizacao.Application.DTOs.Multa;

public class CreateMultaDTO
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Descricao { get; set; } = string.Empty;
}
