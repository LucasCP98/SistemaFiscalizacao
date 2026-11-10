
using SistemaFiscalizacao.Application.DTOs.Regularizacao;

namespace SistemaFiscalizacao.Application.Services;

public interface IRegularizacaoService
{
    RegularizacaoDTO Create(
        long denunciaId,
        long notificacaoId,
        CreateRegularizacaoDTO dto);
}
