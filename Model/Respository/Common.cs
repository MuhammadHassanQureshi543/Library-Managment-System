
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LibraryMangamentSystem.Model.Respository
{
    public class Common<T> : ICommonRepo<T> where T : class
    {
        private readonly DBContexts _dbContext;
        private DbSet<T> _dbSet;
        public Common(DBContexts dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task<T> create(T model)
        {
            await _dbSet.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return model;
        }

        public async Task<T> delete(T model)
        {
            _dbSet.Remove(model);
            await _dbContext.SaveChangesAsync();
            return model;
        }

        public async Task<List<T>> getAll()
        {
            var data = await _dbSet.ToListAsync();
            return data;
        }

        public async Task<T> getUser(Expression<Func<T, bool>> filter)
        {
            var data = await _dbSet.FirstOrDefaultAsync(filter);
            return data;
        }

        public async Task<T> update(T model)
        {
            _dbSet.Update(model);
            await _dbContext.SaveChangesAsync();
            return model;
        }
    }
}
