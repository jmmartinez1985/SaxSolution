using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
//SA: JMMB
namespace Banistmo.Sax.Repository.Implementations
{
    public abstract class RepositoryBase<T> : IRepository<T>
        where T : class
    {


        public RepositoryBase()
            : this(new SaxRepositoryContext())
        {
        }

        public RepositoryBase(IRepositoryContext repositoryContext)
        {
            repositoryContext = repositoryContext ?? new SaxRepositoryContext();
            _objectSet = repositoryContext.GetObjectSet<T>();
            _Context = repositoryContext;
        }
        public abstract Expression<Func<T, bool>> SearchFilters(T obj);
        public abstract Expression<Func<T, bool>> GetFilters();
        private IDbSet<T> _objectSet;
        private IRepositoryContext _Context;
        public IDbSet<T> ObjectSet
        {
            get
            {
                return _objectSet;
            }
        }

        #region IRepository Members

        public void Add(T entity)
        {
            this.ObjectSet.Add(entity);
        }

        public void Insert(T entity)
        {
            this.ObjectSet.Add(entity);
            _Context.SaveChanges();
        }

        public void Update(T entity, T NewEntity)
        {
            this.ObjectSet.Attach(entity);
            _Context.ObjectContext.Entry(entity).CurrentValues.SetValues(NewEntity);
            _Context.ObjectContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            _Context.SaveChanges();
        }

        public void Delete(T entity)
        {
            this.ObjectSet.Remove(entity);
            _Context.SaveChanges();
        }


        public IList<T> GetAll()
        {
            return this.ObjectSet.ToList<T>();
        }

        public IList<T> GetAll(Expression<Func<T, bool>> whereCondition)
        {
            return this.ObjectSet.Where(whereCondition).ToList<T>();
        }

        public T GetSingle(Expression<Func<T, bool>> whereCondition)
        {
            return this.ObjectSet.Where(whereCondition).FirstOrDefault<T>();
        }

        public void Attach(T entity)
        {
            this.ObjectSet.Attach(entity);
        }

        public IQueryable<T> GetQueryable()
        {
            return this.ObjectSet.AsQueryable<T>();
        }

        public long Count()
        {
            return this.ObjectSet.LongCount<T>();
        }

        public long Count(Expression<Func<T, bool>> whereCondition)
        {
            return this.ObjectSet.Where(whereCondition).LongCount<T>();
        }

        #endregion

        #region IRepository Member witn Return Entities
        public T Insert(T entity, bool status)
        {
            this.ObjectSet.Add(entity);
            _Context.SaveChanges();
            return entity;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await this.ObjectSet.Where(whereCondition).FirstOrDefaultAsync<T>();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await this.ObjectSet.ToListAsync<T>();
        }

        public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition)
        {
            return await this.ObjectSet.Where(whereCondition).ToListAsync<T>();
        }

        public async Task<int> CountAsync()
        {
            return await this.ObjectSet.CountAsync<T>();
        }

        public IList<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetQueryable();
            foreach (Expression<Func<T, object>> include in includes)
                if (include != null)
                    query = query.Include(include);
            if (filter != null)
                query = query.Where(filter);
            if (orderBy != null)
                query = orderBy(query);
            return query.ToList();
        }

        public T GetSingle(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetQueryable();
            foreach (Expression<Func<T, object>> include in includes)
                if (include != null)
                    query = query.Include(include);
            if (filter != null)
                query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetQueryable();
            foreach (Expression<Func<T, object>> include in includes)
                if (include != null)
                    query = query.Include(include);
            if (whereCondition != null)
                query = query.Where(whereCondition);

            return await query.FirstOrDefaultAsync<T>();
        }

        public async Task<ICollection<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetQueryable();
            foreach (Expression<Func<T, object>> include in includes)
                if (include != null)
                    query = query.Include(include);
            return await query.ToListAsync<T>();
        }

        public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> whereCondition, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = GetQueryable();
            foreach (Expression<Func<T, object>> include in includes)
                if (include != null)
                    query = query.Include(include);
            if (whereCondition != null)
                query = query.Where(whereCondition);
            return await query.ToListAsync<T>();
        }
        #endregion
    }
}
