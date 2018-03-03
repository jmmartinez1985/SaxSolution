using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//JMMB
namespace Banistmo.Sax.Common
{
    public static class Extension
    {
        public static DbContext BulkInsert<T>(this DbContext context, T entity, int count, int batchSize) where T : class
        {
            context.Set<T>().Add(entity);

            if (count % batchSize == 0)
            {
                context.SaveChanges();
                context.Dispose();
                context = new DbContext("DefaultConnection");

                // This is optional
                context.Configuration.AutoDetectChangesEnabled = false;
            }
            return context;
        }
    }
}
