using Microsoft.EntityFrameworkCore;
using OMS.Data.DBCOntext;
using OMS.Data.Entites;
using OMS.Repositores.Interfaces;
using System.Linq.Expressions;

namespace OMS.Repositores.Repositories
{
    public class GenericRepositories<T , TKey> : IGenericRepositories<T , TKey> where T : BaseEntity<TKey>
    {
        private readonly OSMDBContext _context;

        public GenericRepositories(OSMDBContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);


        public void Update(T entity)
            => _context.Set<T>().Update(entity);
        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);

    }
}
