using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

//SA: JMMB
namespace Banistmo.Sax.Services.Interfaces
{
    public interface IService<M, T, E>
    {
        M GetSingle(Expression<Func<T, bool>> whereCondition);
        void Add(M entity);
        void Insert(M entity);
        M Insert(M entity, bool status);
        void Delete(M entity);
        void Update(M entity);
        List<M> GetAll(Expression<Func<T, bool>> whereCondition);
        List<M> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> whereCondition);
        long Count(Expression<Func<T, bool>> whereCondition);
        long Count();
        Task<M> GetSingleAsync(Expression<Func<T, bool>> whereCondition);
        Task<ICollection<M>> GetAllAsync();
        Task<ICollection<M>> GetAllAsync(Expression<Func<T, bool>> whereCondition);
        Task<M> GetSingleAsync(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] includes);
        Task<ICollection<M>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<ICollection<M>> GetAllAsync(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] includes);
        Task<int> CountAsync();
        IList<M> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes);
        M GetSingle(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
        List<M> ExecuteProcedure(string spName, params object[] parameters);
        List<NewObject> GetAllFlatten<NewObject>(Expression<Func<T, bool>> whereCondition) where NewObject : class;
        List<NewObject> GetAllFlatten<NewObject>() where NewObject : class;
    }
}
