
using SistemaFiscalizacao.Application.DTOs.Recurso;

namespace SistemaFiscalizacao.Application.Services;

public interface IRecursoService
{
    RecursoDTO Create(long processoInfracaoId, CreateRecursoDTO dto);

    List<RecursoDTO> FindAllByProcessoInfracaoId(long processoInfracaoId);

    RecursoDTO Decidir(
        long processoInfracaoId,
        long recursoId,
        DecidirRecursoDTO dto);
}
