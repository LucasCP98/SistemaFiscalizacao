namespace SistemaFiscalizacao.Application.DTOs.Denuncia;

public class DenunciaDTO
{
    public long Id { get; set; }

    public string Protocolo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string Canal { get; set; } = string.Empty;

    public bool Anonima { get; set; }

    public DateTime DataCriacao { get; set; }
}
