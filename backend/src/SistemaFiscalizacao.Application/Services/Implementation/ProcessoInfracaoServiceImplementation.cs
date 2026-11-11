
using SistemaFiscalizacao.Application.DTOs.ProcessoInfracao;
using SistemaFiscalizacao.Application.Services;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;

namespace SistemaFiscalizacao.Application.Services.Implementation;

public class ProcessoInfracaoServiceImplementation : IProcessoInfracaoService
{
    private readonly IRepository<Denuncia> _denunciaRepository;
    private readonly IRepository<Vistoria> _vistoriaRepository;
    private readonly IRepository<ProcessoInfracao> _processoRepository;

    public ProcessoInfracaoServiceImplementation(
        IRepository<Denuncia> denunciaRepository,
        IRepository<Vistoria> vistoriaRepository,
        IRepository<ProcessoInfracao> processoRepository)
    {
        _denunciaRepository = denunciaRepository;
        _vistoriaRepository = vistoriaRepository;
        _processoRepository = processoRepository;
    }

    public ProcessoInfracaoDTO Create(
        long denunciaId,
        CreateProcessoInfracaoDTO dto)
    {
        var denuncia = _denunciaRepository.FindById(denunciaId);

        if (denuncia is null)
            throw new InvalidOperationException("Denúncia não encontrada.");

        if (denuncia.Status != StatusDenuncia.EmProcessoDeInfracao)
        {
            throw new InvalidOperationException(
                "A denúncia não está no status adequado para abertura de processo de infração.");
        }

        var processoExistente = _processoRepository
            .FindAll()
            .Any(p => p.DenunciaId == denunciaId);

        if (processoExistente)
        {
            throw new InvalidOperationException(
                "Já existe um processo de infração para esta denúncia.");
        }

        if (string.IsNullOrWhiteSpace(dto.Descricao))
        {
            throw new InvalidOperationException(
                "A descrição da infração é obrigatória.");
        }

        var ultimaVistoria = _vistoriaRepository
            .FindAll()
            .Where(v => v.DenunciaId == denunciaId)
            .OrderByDescending(v => v.NumeroVistoria)
            .FirstOrDefault();

        if (ultimaVistoria is null)
        {
            throw new InvalidOperationException(
                "Nenhuma vistoria foi encontrada para esta denúncia.");
        }

        if (!ultimaVistoria.IrregularidadeIdentificada)
        {
            throw new InvalidOperationException(
                "A última vistoria não identificou irregularidade.");
        }

        var dataAbertura = DateTime.UtcNow;

        var processo = new ProcessoInfracao
        {
            DenunciaId = denunciaId,
            VistoriaId = ultimaVistoria.Id,
            NumeroProcesso = $"PROC-{dataAbertura:yyyy}-{denunciaId:D6}",
            Descricao = dto.Descricao.Trim(),
            DataAbertura = dataAbertura,
            Status = StatusProcessoInfracao.Aberto,
            ValorMulta = null,
            DataCriacao = dataAbertura
        };

        _processoRepository.Create(processo);

        return MapearDTO(processo);
    }

    public ProcessoInfracaoDTO? FindById(long id)
    {
        var processo = _processoRepository.FindById(id);

        if (processo is null)
            return null;

        return MapearDTO(processo);
    }

    public ProcessoInfracaoDTO? FindByDenunciaId(long denunciaId)
    {
        var processo = _processoRepository
            .FindAll()
            .FirstOrDefault(p => p.DenunciaId == denunciaId);

        if (processo is null)
            return null;

        return MapearDTO(processo);
    }

    private static ProcessoInfracaoDTO MapearDTO(ProcessoInfracao processo)
    {
        return new ProcessoInfracaoDTO
        {
            Id = processo.Id,
            DenunciaId = processo.DenunciaId,
            VistoriaId = processo.VistoriaId,
            NumeroProcesso = processo.NumeroProcesso,
            Descricao = processo.Descricao,
            DataAbertura = processo.DataAbertura,
            Status = processo.Status,
            ValorMulta = processo.ValorMulta,
            DataCriacao = processo.DataCriacao
        };
    }
}
