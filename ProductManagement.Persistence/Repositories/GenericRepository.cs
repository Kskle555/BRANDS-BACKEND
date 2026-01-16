using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductManagement.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _dbContext;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Set<T>()
                .Skip((pageNumber - 1) * pageSize) // Önceki sayfaları atla
                .Take(pageSize)                    // İstenen kadar al
                .AsNoTracking()                    // Okuma işlemi olduğu için hızlandırır (Cache yok)
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            // Soft Delete Mantığı: Veriyi silmiyoruz, durumunu güncelliyoruz. taskta özellikle istendiği için ekliyorum.
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.UtcNow;
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        // IQueryable dönerek veritabanına sorgu atılmadan önce filtreleme/sayfalama
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public async Task<T> GetByFilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }
    }
}