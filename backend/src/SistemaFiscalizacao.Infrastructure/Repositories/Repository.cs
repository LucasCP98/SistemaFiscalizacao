using Microsoft.EntityFrameworkCore;
using SistemaFiscalizacao.Infrastructure.Data;

namespace SistemaFiscalizacao.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SistemaFiscalizacaoDbContext _context;

    public Repository(SistemaFiscalizacaoDbContext context)
    {
        _context = context;
    }

    public List<T> FindAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? FindById(long id)
    {
        return _context.Set<T>().Find(id);
    }

    public T Create(T entity)
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();

        return entity;
    }

    public T Update(T entity)
    {
        _context.Set<T>().Update(entity);
        _context.SaveChanges();

        return entity;
    }

    public void Delete(long id)
    {
        var entity = _context.Set<T>().Find(id);

        if (entity == null)
        {
            return;
        }

        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
    }
}