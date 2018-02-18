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
    }
}
