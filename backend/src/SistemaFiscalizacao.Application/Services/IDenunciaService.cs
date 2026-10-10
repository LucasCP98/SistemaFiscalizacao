using SistemaFiscalizacao.Application.DTOs.Denuncia;

namespace SistemaFiscalizacao.Application.Services;
public interface IDenunciaService
{
    DenunciaDTO Create(CreateDenunciaDTO denuncia);

    DenunciaDTO? FindById(long id);

    List<DenunciaDTO> FindAll();

    DenunciaDTO? Update(DenunciaDTO denuncia);

    void Delete(long id);
}
