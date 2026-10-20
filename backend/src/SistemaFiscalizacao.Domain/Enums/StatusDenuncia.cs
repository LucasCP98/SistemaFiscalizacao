namespace SistemaFiscalizacao.Domain.Enums;

public enum StatusDenuncia
{
    Recebida = 1,
    EmTriagem = 2,
    AguardandoAtribuicao = 3,
    EmVistoria = 4,
    AguardandoRegularizacao = 5,
    AguardandoNovaVistoria = 6,
    EmProcessoDeInfracao = 7,
    Encerrada = 8
}