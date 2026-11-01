
namespace SistemaFiscalizacao.Application.DTOs.Vistoria;

public class VistoriaDTO
{
    public long Id { get; set; }
    public long DenunciaId { get; set; }
    public DateTime DataVistoria { get; set; }
    public string Relato { get; set; } = string.Empty;
    public bool IrregularidadeIdentificada { get; set; }
    public DateTime DataCriacao { get; set; }
}
