using SistemaFiscalizacao.Application.DTOs.Denuncia;
using Mapster;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Repositories;

namespace SistemaFiscalizacao.Application.Services.Implementation;
public class DenunciaServiceImplementation : IDenunciaService
{
    private readonly IRepository<Denuncia> _repository;

    public DenunciaServiceImplementation(IRepository<Denuncia> repository)
    {
        _repository = repository;
    }

    public List<DenunciaDTO> FindAll()
    {
        return _repository
            .FindAll()
            .Adapt<List<DenunciaDTO>>();
    }

    public DenunciaDTO? FindById(long id)
    {
        var entity = _repository.FindById(id);

        return entity?.Adapt<DenunciaDTO>();
    }

    public DenunciaDTO Create(CreateDenunciaDTO denuncia)
    {
        var entity = denuncia.Adapt<Denuncia>();

        entity.Protocolo = GerarProtocolo();

        entity.DataCriacao = DateTime.UtcNow;

        entity = _repository.Create(entity);

        return entity.Adapt<DenunciaDTO>();
    }

    public DenunciaDTO? Update(DenunciaDTO denuncia)
    {
        var entity = _repository.FindById(denuncia.Id);

        if (entity == null)
        {
            return null;
        }

        denuncia.Adapt(entity);

        entity = _repository.Update(entity);

        return entity.Adapt<DenunciaDTO>();
    }

    public void Delete(long id)
    {
        _repository.Delete(id);
    }

    private string GerarProtocolo()
    {
        return $"DEN-{DateTime.Now:yyyyMMddHHmmss}";
    }
}
