
using SistemaFiscalizacao.Application.DTOs.Recurso;
using SistemaFiscalizacao.Application.Services;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Application.Services.Implementation;

public class RecursoServiceImplementation : IRecursoService
{
    private readonly IRepository<ProcessoInfracao> _processoRepository;
    private readonly IRepository<Multa> _multaRepository;
    private readonly IRepository<Recurso> _recursoRepository;
    private readonly ITransactionManager _transactionManager;

    public RecursoServiceImplementation(
        IRepository<ProcessoInfracao> processoRepository,
        IRepository<Multa> multaRepository,
        IRepository<Recurso> recursoRepository,
        ITransactionManager transactionManager)
    {
        _processoRepository = processoRepository;
        _multaRepository = multaRepository;
        _recursoRepository = recursoRepository;
        _transactionManager = transactionManager;
    }

    public RecursoDTO Create(long processoInfracaoId, CreateRecursoDTO dto)
    {
        var processo = _processoRepository.FindById(processoInfracaoId);

        if (processo == null)
            throw new InvalidOperationException(
                "Processo de infração não encontrado.");

        if (processo.Status != StatusProcessoInfracao.AguardandoPagamento)
            throw new InvalidOperationException(
                "O recurso só pode ser apresentado em processos aguardando pagamento.");

        var multa = _multaRepository.FindAll()
            .FirstOrDefault(m => m.ProcessoInfracaoId == processoInfracaoId);

        if (multa == null)
            throw new InvalidOperationException(
                "Não existe multa vinculada a este processo.");

        var recursoPendente = _recursoRepository.FindAll()
            .Any(r =>
                r.ProcessoInfracaoId == processoInfracaoId &&
                r.Status == StatusRecurso.Pendente);

        if (recursoPendente)
            throw new InvalidOperationException(
                "Já existe um recurso pendente para este processo.");

        if (string.IsNullOrWhiteSpace(dto.Justificativa))
            throw new InvalidOperationException(
                "A justificativa do recurso é obrigatória.");

        var dataApresentacao = DateTime.UtcNow;

        var recurso = new Recurso
        {
            ProcessoInfracaoId = processoInfracaoId,
            Justificativa = dto.Justificativa.Trim(),
            DataApresentacao = dataApresentacao,
            Status = StatusRecurso.Pendente,
            DataCriacao = dataApresentacao
        };

        _transactionManager.ExecuteInTransaction(() =>
        {
            _recursoRepository.Create(recurso);

            processo.Status = StatusProcessoInfracao.EmRecurso;

            _processoRepository.Update(processo);
        });

        return MapearDTO(recurso);
    }

    public List<RecursoDTO> FindAllByProcessoInfracaoId(long processoInfracaoId)
    {
        var processo = _processoRepository.FindById(processoInfracaoId);

        if (processo == null)
            throw new InvalidOperationException(
                "Processo de infração não encontrado.");

        return _recursoRepository.FindAll()
            .Where(r => r.ProcessoInfracaoId == processoInfracaoId)
            .OrderByDescending(r => r.DataApresentacao)
            .Select(MapearDTO)
            .ToList();
    }

    private static RecursoDTO MapearDTO(Recurso recurso)
    {
        return new RecursoDTO
        {
            Id = recurso.Id,
            ProcessoInfracaoId = recurso.ProcessoInfracaoId,
            Justificativa = recurso.Justificativa,
            DataApresentacao = recurso.DataApresentacao,
            Status = recurso.Status,
            Decisao = recurso.Decisao,
            DataDecisao = recurso.DataDecisao,
            DataCriacao = recurso.DataCriacao
        };
    }


    public RecursoDTO Decidir(
        long processoInfracaoId,
        long recursoId,
        DecidirRecursoDTO dto)
    {
        var processo = _processoRepository.FindById(processoInfracaoId);

        if (processo == null)
            throw new InvalidOperationException(
                "Processo de infração não encontrado.");

        var recurso = _recursoRepository.FindById(recursoId);

        if (recurso == null || recurso.ProcessoInfracaoId != processoInfracaoId)
            throw new InvalidOperationException(
                "Recurso não encontrado para este processo.");

        if (processo.Status != StatusProcessoInfracao.EmRecurso)
            throw new InvalidOperationException(
                "O processo não está em análise de recurso.");

        if (recurso.Status != StatusRecurso.Pendente)
            throw new InvalidOperationException(
                "Este recurso já foi analisado.");

        if (dto.Status != StatusRecurso.Deferido &&
            dto.Status != StatusRecurso.Indeferido)
            throw new InvalidOperationException(
                "A decisão deve ser Deferido ou Indeferido.");

        if (string.IsNullOrWhiteSpace(dto.Decisao))
            throw new InvalidOperationException(
                "A justificativa da decisão é obrigatória.");

        recurso.Status = dto.Status;
        recurso.Decisao = dto.Decisao.Trim();
        recurso.DataDecisao = DateTime.UtcNow;

        if (dto.Status == StatusRecurso.Deferido)
        {
            processo.Status = StatusProcessoInfracao.Cancelado;
        }
        else
        {
            processo.Status = StatusProcessoInfracao.AguardandoPagamento;
        }

        _transactionManager.ExecuteInTransaction(() =>
        {
            _recursoRepository.Update(recurso);
            _processoRepository.Update(processo);
        });

        return MapearDTO(recurso);
    }

}
