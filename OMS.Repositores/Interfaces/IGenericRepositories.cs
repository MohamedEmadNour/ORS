
using Microsoft.EntityFrameworkCore.Query;
using OMS.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.Interfaces
{
    public interface IGenericRepositories<T, TKey> where T : BaseEntity<TKey>
    {
        Task<T> GetByIdAsync(TKey id);
        Task<T> GetByIdAsync(TKey id, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<IReadOnlyList<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<Customer> FindCustomerByEmail(string email);

    }
}
