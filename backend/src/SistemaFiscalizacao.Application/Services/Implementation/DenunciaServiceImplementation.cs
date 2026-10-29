using SistemaFiscalizacao.Application.DTOs.Denuncia;
using Mapster;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Domain.Rules;

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

    private string GerarProtocolo()
    {
        return $"DEN-{DateTime.Now:yyyyMMddHHmmss}";
    }

    public DenunciaDTO? AlterarStatus(
    long id,
    AlterarStatusDenunciaDTO dto)
    {
        var denuncia = _repository.FindById(id);

        if (denuncia is null)
            return null;

        if (!DenunciaStatusRules.PodeTransicionar(
                denuncia.Status, dto.Status))
        {
            throw new InvalidOperationException(
                $"Não é permitida a transição de " +
                $"{denuncia.Status} para {dto.Status}.");
        }

        if (dto.Status == StatusDenuncia.Encerrada)
        {
            if (dto.Resultado is null ||
                !Enum.IsDefined(typeof(ResultadoDenuncia), dto.Resultado.Value))
            {
                throw new InvalidOperationException(
                    "Para encerrar uma denúncia, informe um resultado final válido.");
            }
        }
        else if (dto.Resultado is not null)
        {
            throw new InvalidOperationException(
                "O resultado só pode ser informado ao encerrar a denúncia.");
        }

        denuncia.Status = dto.Status;
        denuncia.Resultado = dto.Resultado;

        _repository.Update(denuncia);

        return denuncia.Adapt<DenunciaDTO>();
    }
}
