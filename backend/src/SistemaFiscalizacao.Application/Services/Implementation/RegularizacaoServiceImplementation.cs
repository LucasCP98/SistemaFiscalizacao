
using SistemaFiscalizacao.Application.DTOs.Regularizacao;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;
using SistemaFiscalizacao.Infrastructure.Transactions;

namespace SistemaFiscalizacao.Application.Services.Implementation;

public class RegularizacaoServiceImplementation : IRegularizacaoService
{
    private readonly IRepository<Denuncia> _denunciaRepository;
    private readonly IRepository<Notificacao> _notificacaoRepository;
    private readonly IRepository<Regularizacao> _regularizacaoRepository;
    private readonly ITransactionManager _transactionManager;

    public RegularizacaoServiceImplementation(
        IRepository<Denuncia> denunciaRepository,
        IRepository<Notificacao> notificacaoRepository,
        IRepository<Regularizacao> regularizacaoRepository,
        ITransactionManager transactionManager)
    {
        _denunciaRepository = denunciaRepository;
        _notificacaoRepository = notificacaoRepository;
        _regularizacaoRepository = regularizacaoRepository;
        _transactionManager = transactionManager;
    }

    public RegularizacaoDTO Create(
        long denunciaId,
        long notificacaoId,
        CreateRegularizacaoDTO dto)
    {
        var denuncia = _denunciaRepository.FindById(denunciaId);

        if (denuncia is null)
            throw new InvalidOperationException("Denúncia não encontrada.");

        var notificacao = _notificacaoRepository.FindById(notificacaoId);

        if (notificacao is null)
            throw new InvalidOperationException("Notificação não encontrada.");

        if (notificacao.DenunciaId != denunciaId)
            throw new InvalidOperationException(
                "A notificação informada não pertence à denúncia.");

        if (denuncia.Status != StatusDenuncia.AguardandoRegularizacao)
            throw new InvalidOperationException(
                "A denúncia não está aguardando regularização.");

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new InvalidOperationException(
                "A descrição da regularização é obrigatória.");

        var dataInformada = DateTime.UtcNow;

        var regularizacao = new Regularizacao
        {
            NotificacaoId = notificacaoId,
            Descricao = dto.Descricao.Trim(),
            DataInformada = dataInformada,
            DentroDoPrazo = dataInformada <= notificacao.DataLimite,
            DataCriacao = dataInformada
        };

        _transactionManager.ExecuteInTransaction(() =>
        {
            _regularizacaoRepository.Create(regularizacao);

            denuncia.Status = StatusDenuncia.AguardandoNovaVistoria;

            _denunciaRepository.Update(denuncia);
        });

        return new RegularizacaoDTO
        {
            Id = regularizacao.Id,
            NotificacaoId = regularizacao.NotificacaoId,
            Descricao = regularizacao.Descricao,
            DataInformada = regularizacao.DataInformada,
            DentroDoPrazo = regularizacao.DentroDoPrazo,
            DataCriacao = regularizacao.DataCriacao
        };
    }
}
