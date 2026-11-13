
using SistemaFiscalizacao.Domain.Enums;

namespace SistemaFiscalizacao.Application.DTOs.Recurso;

public class RecursoDTO
{
    public long Id { get; set; }

    public long ProcessoInfracaoId { get; set; }

    public string Justificativa { get; set; } = string.Empty;

    public DateTime DataApresentacao { get; set; }

    public StatusRecurso Status { get; set; }

    public string? Decisao { get; set; }

    public DateTime? DataDecisao { get; set; }

    public DateTime DataCriacao { get; set; }
}
