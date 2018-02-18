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
    }
}
