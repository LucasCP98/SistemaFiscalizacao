using SistemaFiscalizacao.Infrastructure.Data;
namespace SistemaFiscalizacao.Infrastructure.Transactions;

public class TransactionManager : ITransactionManager
{
    private readonly SistemaFiscalizacaoDbContext _context;

    public TransactionManager(SistemaFiscalizacaoDbContext context)
    {
        _context = context;
    }

    public void ExecuteInTransaction(Action operation)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            operation();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}