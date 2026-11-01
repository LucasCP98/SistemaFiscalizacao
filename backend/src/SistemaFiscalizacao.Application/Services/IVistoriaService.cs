
using SistemaFiscalizacao.Application.DTOs.Vistoria;

namespace SistemaFiscalizacao.Application.Services;

public interface IVistoriaService
{
    VistoriaDTO Create(long denunciaId, CreateVistoriaDTO dto);
}
