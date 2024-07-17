using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
        public async Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }


        public async Task<T> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }


        public async Task<T> GetByIdAsync(TKey id, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
            {
                query = include(query);
            }

            var keyProperty = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.First();
            return await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyProperty.Name).Equals(id));
        }

        public async Task AddAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);


        public void Update(T entity)
            => _context.Set<T>().Update(entity);
        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);

        public async Task<Customer> FindCustomerByEmail(string email)
        {
            return await _context.Set<Customer>()
                                 .Where(e => e.Email.ToLower() == email.ToLower())
                                 .FirstOrDefaultAsync();
        }
    }
}
