
using System.ComponentModel.DataAnnotations;
using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Application.DTOs.Recurso;

public class DecidirRecursoDTO
{
    [Required]
    public StatusRecurso Status { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Decisao { get; set; } = string.Empty;
}
