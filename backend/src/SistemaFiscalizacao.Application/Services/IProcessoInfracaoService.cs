
using SistemaFiscalizacao.Application.DTOs.ProcessoInfracao;

namespace SistemaFiscalizacao.Application.Services;

public interface IProcessoInfracaoService
{
    ProcessoInfracaoDTO Create(
        long denunciaId,
        CreateProcessoInfracaoDTO dto);

    ProcessoInfracaoDTO? FindById(long id);

    ProcessoInfracaoDTO? FindByDenunciaId(long denunciaId);
}
