using Banistmo.Sax.Repository.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    public abstract class Reporter<T> : IReporter<T>
                where T : class
    {

        public Reporter()
            : this(new SaxRepositoryContext())
        {
        }

        private IDbSet<T> _objectSet;
        private IRepositoryContext _Context;
        public IDbSet<T> ObjectSet
        {
            get
            {
                return _objectSet;
            }
        }
        public Reporter(IRepositoryContext repositoryContext)
        {
            repositoryContext = repositoryContext ?? new SaxRepositoryContext();
            _objectSet = repositoryContext.GetObjectSet<T>();
            _Context = repositoryContext;
        }
        public DbRawSqlQuery<T> ExecuteProcedure(string spName, params object[] parameters)
        {
            return this._Context.ObjectContext.Database.SqlQuery<T>(spName, parameters);
        }
    }
}
