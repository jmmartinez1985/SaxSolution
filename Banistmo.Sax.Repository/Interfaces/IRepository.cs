using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

//SA: JMMB
namespace Banistmo.Sax.Repository.Interfaces
{
    public interface IRepository<T>
    {
        T GetSingle(Expression<Func<T, bool>> whereCondition);
        void Add(T entity);
        void Insert(T entity);
        T Insert(T entity, bool status);
        void Update(T entity, T NewEntity);
        void Delete(T entity);
        void Attach(T entity);
        IList<T> GetAll(Expression<Func<T, bool>> whereCondition);
        IList<T> GetAll();
        IQueryable<T> GetQueryable();
        long Count(Expression<Func<T, bool>> whereCondition);
        long Count();
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereCondition);
        Task<ICollection<T>> GetAllAsync();
        Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] includes);
        Task<ICollection<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] includes);
        Task<int> CountAsync();
        IList<T> GetAll( Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,params Expression<Func<T, object>>[] includes);
        T GetSingle( Expression<Func<T, bool>> filter = null,params Expression<Func<T, object>>[] includes);

    }
}
