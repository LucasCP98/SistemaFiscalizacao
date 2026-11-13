
using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Domain.Entities;

public class Recurso : BaseEntity
{
    public long ProcessoInfracaoId { get; set; }

    public string Justificativa { get; set; } = string.Empty;

    public DateTime DataApresentacao { get; set; }

    public StatusRecurso Status { get; set; }

    public string? Decisao { get; set; }

    public DateTime? DataDecisao { get; set; }

    public DateTime DataCriacao { get; set; }

    public ProcessoInfracao ProcessoInfracao { get; set; } = null!;
}
