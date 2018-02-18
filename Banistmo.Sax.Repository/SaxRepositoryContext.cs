using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Repository.Structure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//SA: JMMB
namespace Banistmo.Sax.Repository
{
    public class SaxRepositoryContext : IRepositoryContext
    {
        private const string OBJECT_CONTEXT_KEY = "Banistmo.Sax.Repository.Model";
        public IDbSet<T> GetObjectSet<T>()
            where T : class
        {
            return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY).Set<T>();
        }

        /// <summary>
        /// Returns the active object context
        /// </summary>
        public DbContext ObjectContext
        {
            get
            {
                return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY);
            }
        }

        public int SaveChanges()
        {
            return this.ObjectContext.SaveChanges();
        }

        public void Terminate()
        {
            ContextManager.SetRepositoryContext(null, OBJECT_CONTEXT_KEY);
        }

    }

}
