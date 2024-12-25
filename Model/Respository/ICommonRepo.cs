using System.Linq.Expressions;

namespace LibraryMangamentSystem.Model.Respository
{
    public interface ICommonRepo<T>
    {
        Task<T> create(T model);
        Task<List<T>> getAll();
        Task<T> update(T model);
        Task<T> delete(T model);
        Task<T> getUser(Expression<Func<T,bool>> filter);
    }
}
