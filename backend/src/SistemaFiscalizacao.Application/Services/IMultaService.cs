
using SistemaFiscalizacao.Application.DTOs.Multa;

namespace SistemaFiscalizacao.Application.Services;

public interface IMultaService
{
    MultaDTO Create(long processoInfracaoId, CreateMultaDTO dto);

    MultaDTO? FindByProcessoInfracaoId(long processoInfracaoId);
}
