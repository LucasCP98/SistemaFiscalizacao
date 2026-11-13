
using SistemaFiscalizacao.Application.DTOs.Multa;
using SistemaFiscalizacao.Application.Services;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Application.Services.Implementation;

public class MultaServiceImplementation : IMultaService
{
    private readonly IRepository<ProcessoInfracao> _processoRepository;
    private readonly IRepository<Multa> _multaRepository;
    private readonly ITransactionManager _transactionManager;

    private const decimal ValorMinimoPermitido = 500m;
    private const decimal ValorMaximoPermitido = 5000m;

    public MultaServiceImplementation(
        IRepository<ProcessoInfracao> processoRepository,
        IRepository<Multa> multaRepository,
        ITransactionManager transactionManager)
    {
        _processoRepository = processoRepository;
        _multaRepository = multaRepository;
        _transactionManager = transactionManager;
    }

    public MultaDTO Create(long processoInfracaoId, CreateMultaDTO dto)
    {
        var processo = _processoRepository.FindById(processoInfracaoId);

        if (processo == null)
            throw new InvalidOperationException("Processo de infração não encontrado.");

        if (processo.Status != StatusProcessoInfracao.Aberto)
            throw new InvalidOperationException(
                "A multa só pode ser aplicada em processos com status Aberto.");

        var multaExistente = _multaRepository.FindAll()
            .FirstOrDefault(m => m.ProcessoInfracaoId == processoInfracaoId);

        if (multaExistente != null)
            throw new InvalidOperationException(
                "Já existe uma multa vinculada a este processo.");

        if (dto.Valor < ValorMinimoPermitido ||
            dto.Valor > ValorMaximoPermitido)
        {
            throw new InvalidOperationException(
                $"O valor da multa deve estar entre R$ {ValorMinimoPermitido:N2} e R$ {ValorMaximoPermitido:N2}.");
        }

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new InvalidOperationException("A descrição da multa é obrigatória.");

        var dataAplicacao = DateTime.UtcNow;

        var multa = new Multa
        {
            ProcessoInfracaoId = processoInfracaoId,
            Valor = dto.Valor,
            ValorMinimo = ValorMinimoPermitido,
            ValorMaximo = ValorMaximoPermitido,
            Descricao = dto.Descricao.Trim(),
            DataAplicacao = dataAplicacao,
            DataCriacao = dataAplicacao
        };

        _transactionManager.ExecuteInTransaction(() =>
        {
            _multaRepository.Create(multa);

            processo.ValorMulta = multa.Valor;
            processo.Status = StatusProcessoInfracao.AguardandoPagamento;

            _processoRepository.Update(processo);
        });

        return MapearDTO(multa);
    }

    public MultaDTO? FindByProcessoInfracaoId(long processoInfracaoId)
    {
        var multa = _multaRepository.FindAll()
            .FirstOrDefault(m => m.ProcessoInfracaoId == processoInfracaoId);

        if (multa == null)
            return null;

        return MapearDTO(multa);
    }

    private static MultaDTO MapearDTO(Multa multa)
    {
        return new MultaDTO
        {
            Id = multa.Id,
            ProcessoInfracaoId = multa.ProcessoInfracaoId,
            Valor = multa.Valor,
            ValorMinimo = multa.ValorMinimo,
            ValorMaximo = multa.ValorMaximo,
            Descricao = multa.Descricao,
            DataAplicacao = multa.DataAplicacao,
            DataCriacao = multa.DataCriacao
        };
    }
}
