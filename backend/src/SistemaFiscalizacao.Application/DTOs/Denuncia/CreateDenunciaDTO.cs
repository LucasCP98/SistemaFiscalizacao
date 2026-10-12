using System.ComponentModel.DataAnnotations;

namespace SistemaFiscalizacao.Application.DTOs.Denuncia;

public class CreateDenunciaDTO
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O canal de recebimento é obrigatório.")]
    [StringLength(50, ErrorMessage = "O canal deve ter no máximo 50 caracteres.")]
    public string Canal { get; set; } = string.Empty;

    public bool Anonima { get; set; }
}