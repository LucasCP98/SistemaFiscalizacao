
using System.ComponentModel.DataAnnotations;

namespace SistemaFiscalizacao.Application.DTOs.Vistoria;

public class CreateVistoriaDTO
{
    [Required]
    public DateTime DataVistoria { get; set; }

    [Required]
    [StringLength(2000)]
    public string Relato { get; set; } = string.Empty;

    public bool IrregularidadeIdentificada { get; set; }
}
