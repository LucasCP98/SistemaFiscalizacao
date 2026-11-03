namespace SistemaFiscalizacao.Infrastructure.Transactions;

public interface ITransactionManager
{
    void ExecuteInTransaction(Action operation);
}