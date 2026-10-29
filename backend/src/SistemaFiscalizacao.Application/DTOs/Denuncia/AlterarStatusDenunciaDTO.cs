using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Application.DTOs.Denuncia;

public class AlterarStatusDenunciaDTO
{
    public StatusDenuncia Status { get; set; }

    public ResultadoDenuncia? Resultado { get; set; }
}