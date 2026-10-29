namespace SistemaFiscalizacao.Domain.Entities;

public class Vistoria : BaseEntity
{
    public long DenunciaId { get; set; }

    public DateTime DataVistoria { get; set; }

    public string Relato { get; set; } = string.Empty;

    public bool IrregularidadeIdentificada { get; set; }

    public DateTime DataCriacao { get; set; }

    public Denuncia Denuncia { get; set; } = null!;
}