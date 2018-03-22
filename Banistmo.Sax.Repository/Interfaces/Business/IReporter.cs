using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IReporter<T>
    {
        DbRawSqlQuery<T> ExecuteProcedure(string spName, params object[] parameters);
    }
}
