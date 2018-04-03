using System.Data.Entity.Infrastructure;

namespace Banistmo.Sax.Repository.Interfaces
{
    public interface IProcedureExecuter
    {
        DbRawSqlQuery<T> ExecuteProcedure<T>(string spName, params object[] parameters) where T : class;
    }
}