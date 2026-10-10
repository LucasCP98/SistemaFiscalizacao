namespace SistemaFiscalizacao.Application.DTOs.Denuncia;

public class CreateDenunciaDTO
{
    public string Descricao { get; set; } = string.Empty;

    public string Canal { get; set; } = string.Empty;

    public bool Anonima { get; set; }
}
