using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//SA: JMMB
namespace Banistmo.Sax.Repository.Interfaces
{
    public interface IRepositoryContext
    {
        IDbSet<T> GetObjectSet<T>() where T : class;
        DbContext ObjectContext { get; }
        int SaveChanges();
        void Terminate();
    }
}
