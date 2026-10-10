namespace SistemaFiscalizacao.Domain.Entities;

public class Denuncia : BaseEntity
{
    public string Protocolo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Canal { get; set; } = string.Empty;

    public bool Anonima { get; set; }

    public DateTime DataCriacao { get; set; }
}
