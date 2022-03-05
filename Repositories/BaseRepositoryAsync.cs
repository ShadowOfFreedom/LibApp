using LibApp.Data;
using LibApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibApp.Repositories {
    public abstract class BaseRepositoryAsync<T> : IRepositoryAsync<T> where T : class{
        private readonly ApplicationDbContext _context;
        protected DbSet<T> _entities;

        protected BaseRepositoryAsync(ApplicationDbContext context) => _context = context;

        public virtual async Task<List<T>> GetAll() =>
            await _entities.ToListAsync();

        public virtual async Task<T> GetById(int id) =>
            await _entities.FindAsync(id);

        public virtual async Task<int> Add(T entity) {
            _entities.Add(entity);
            return await Save();
        }

        public virtual async Task<int> Update(T entity) {
            _entities.Update(entity);
            return await Save();
        }

        public virtual async Task<int> Delete(int id) {
            _entities.Remove(await GetById(id));
            return await Save();
        }

        private async Task<int> Save() =>
            await _context.SaveChangesAsync();
    }
}
