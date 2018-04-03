using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IProcedureExecuter
    {
        DbRawSqlQuery<T> ExecuteProcedure<T>(string spName, params object[] parameters) where T : class;
    }
}
