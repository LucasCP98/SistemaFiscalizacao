using SistemaFiscalizacao.Domain.Entities;
using SistemaFiscalizacao.Infrastructure.Data;

namespace SistemaFiscalizacao.Infrastructure.Repositories;

public class DenunciaRepository : Repository<Denuncia>
{
    public DenunciaRepository(SistemaFiscalizacaoDbContext context)
        : base(context)
    {
    }
}