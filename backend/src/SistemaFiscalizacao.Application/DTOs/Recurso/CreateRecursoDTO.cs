
using System.ComponentModel.DataAnnotations;

namespace SistemaFiscalizacao.Application.DTOs.Recurso;

public class CreateRecursoDTO
{
    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Justificativa { get; set; } = string.Empty;
}
