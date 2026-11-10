using SistemaFiscalizacao.Application.DTOs.Notificacao;

namespace SistemaFiscalizacao.Application.Services;

public interface INotificacaoService
{
    NotificacaoDTO Create(
        long denunciaId,
        long vistoriaId,
        CreateNotificacaoDTO dto);

    List<NotificacaoDTO> FindAllByDenunciaId(long denunciaId);
}