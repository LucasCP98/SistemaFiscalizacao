namespace SistemaFiscalizacao.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    List<T> FindAll();

    T? FindById(long id);

    T Create(T entity);

    T Update(T entity);
}