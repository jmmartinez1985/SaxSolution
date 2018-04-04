using Banistmo.Sax.Repository.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.Repository.Interfaces
{
    [Injectable]
    public class ProcedureExecuter : IProcedureExecuter 
    {

        private IRepositoryContext _Context;

        public ProcedureExecuter()
            : this(new SaxRepositoryContext())
        {
        }

        public ProcedureExecuter(IRepositoryContext repositoryContext)
        {
            repositoryContext = repositoryContext ?? new SaxRepositoryContext();
            _Context = repositoryContext;
        }

        public DbRawSqlQuery<T> ExecuteProcedure<T>(string spName, params object[] parameters) where T : class
        {
            return this._Context.ObjectContext.Database.SqlQuery<T>(spName, parameters);
        }
    }
}
