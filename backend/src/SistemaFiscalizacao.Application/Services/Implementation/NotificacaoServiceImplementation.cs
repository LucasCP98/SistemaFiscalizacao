
using SistemaFiscalizacao.Application.DTOs.Notificacao;
using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Domain.Enums;
using SistemaFiscalizacao.Infrastructure.Repositories;

namespace SistemaFiscalizacao.Application.Services.Implementation;

public class NotificacaoServiceImplementation : INotificacaoService
{
    private readonly IRepository<Denuncia> _denunciaRepository;
    private readonly IRepository<Vistoria> _vistoriaRepository;
    private readonly IRepository<Notificacao> _notificacaoRepository;

    public NotificacaoServiceImplementation(
        IRepository<Denuncia> denunciaRepository,
        IRepository<Vistoria> vistoriaRepository,
        IRepository<Notificacao> notificacaoRepository)
    {
        _denunciaRepository = denunciaRepository;
        _vistoriaRepository = vistoriaRepository;
        _notificacaoRepository = notificacaoRepository;
    }

    public NotificacaoDTO Create(long denunciaId, long vistoriaId, CreateNotificacaoDTO dto)
    {
        // Verifica se a denúncia existe
        var denuncia = _denunciaRepository.FindById(denunciaId);

        if (denuncia is null)
            throw new InvalidOperationException("Denúncia não encontrada.");

        // Verifica se a vistoria existe
        var vistoria = _vistoriaRepository.FindById(vistoriaId);

        if (vistoria is null)
            throw new InvalidOperationException("Vistoria não encontrada.");

        // Verifica se a vistoria pertence à denúncia informada
        if (vistoria.DenunciaId != denunciaId)
            throw new InvalidOperationException(
                "A vistoria informada não pertence à denúncia.");

        // Verifica se a vistoria identificou irregularidade
        if (!vistoria.IrregularidadeIdentificada)
            throw new InvalidOperationException(
                "Não é possível emitir uma notificação para uma vistoria sem irregularidade identificada.");

        // Verifica se a denúncia está aguardando regularização
        if (denuncia.Status != StatusDenuncia.AguardandoRegularizacao)
            throw new InvalidOperationException(
                "A denúncia não está no status adequado para receber uma notificação.");

        // Valida os dados recebidos
        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new InvalidOperationException(
                "A descrição da notificação é obrigatória.");

        if (dto.PrazoDias <= 0 || dto.PrazoDias > 365)
            throw new InvalidOperationException(
                "O prazo deve ser maior que zero e não pode ultrapassar 365 dias.");

        // Define as datas da notificação
        var dataEmissao = DateTime.UtcNow;
        var dataLimite = dataEmissao.AddDays(dto.PrazoDias);

        // Cria a entidade
        var notificacao = new Notificacao
        {
            DenunciaId = denunciaId,
            VistoriaId = vistoriaId,
            Descricao = dto.Descricao.Trim(),
            DataEmissao = dataEmissao,
            PrazoDias = dto.PrazoDias,
            DataLimite = dataLimite,
            DataCriacao = DateTime.UtcNow
        };

        // Persiste a notificação
        _notificacaoRepository.Create(notificacao);

        // Retorna o DTO
        return new NotificacaoDTO
        {
            Id = notificacao.Id,
            DenunciaId = notificacao.DenunciaId,
            VistoriaId = notificacao.VistoriaId,
            Descricao = notificacao.Descricao,
            DataEmissao = notificacao.DataEmissao,
            PrazoDias = notificacao.PrazoDias,
            DataLimite = notificacao.DataLimite,
            DataCriacao = notificacao.DataCriacao
        };


    }

    public List<NotificacaoDTO> FindAllByDenunciaId(long denunciaId)
    {
        var denuncia = _denunciaRepository.FindById(denunciaId);

        if (denuncia is null)
            throw new InvalidOperationException("Denúncia não encontrada.");

        var notificacoes = _notificacaoRepository
            .FindAll()
            .Where(n => n.DenunciaId == denunciaId)
            .OrderByDescending(n => n.DataEmissao)
            .ToList();

        return notificacoes.Select(n => new NotificacaoDTO
        {
            Id = n.Id,
            DenunciaId = n.DenunciaId,
            VistoriaId = n.VistoriaId,
            Descricao = n.Descricao,
            DataEmissao = n.DataEmissao,
            PrazoDias = n.PrazoDias,
            DataLimite = n.DataLimite,
            DataCriacao = n.DataCriacao
        }).ToList();
    }
}
