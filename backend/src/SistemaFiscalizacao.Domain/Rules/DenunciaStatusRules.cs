using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Domain.Rules;

public static class DenunciaStatusRules
{
    public static bool PodeTransicionar(
        StatusDenuncia atual,
        StatusDenuncia proximo)
    {
        return atual switch
        {
            StatusDenuncia.Recebida =>
                proximo == StatusDenuncia.EmTriagem,

            StatusDenuncia.EmTriagem =>
                proximo == StatusDenuncia.AguardandoAtribuicao,

            StatusDenuncia.AguardandoAtribuicao =>
                proximo == StatusDenuncia.EmVistoria,

            StatusDenuncia.EmVistoria =>
                proximo == StatusDenuncia.AguardandoRegularizacao
                || proximo == StatusDenuncia.Encerrada,

            StatusDenuncia.AguardandoRegularizacao =>
                proximo == StatusDenuncia.AguardandoNovaVistoria,

            StatusDenuncia.AguardandoNovaVistoria =>
                proximo == StatusDenuncia.EmVistoria
                || proximo == StatusDenuncia.EmProcessoDeInfracao,

            StatusDenuncia.EmProcessoDeInfracao =>
                proximo == StatusDenuncia.Encerrada,

            StatusDenuncia.Encerrada => false,

            _ => false
        };
    }
}