using ProductManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductManagement.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize);

        // Pagination ve Filtering için gerekli metot imzasi
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    }
}