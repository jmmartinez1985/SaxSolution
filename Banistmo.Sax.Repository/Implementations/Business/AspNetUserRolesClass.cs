using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
using System.Linq.Expressions;
using System.Transactions;
using EntityFramework.Utilities;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class AspNetUserRolesClass : RepositoryBase<AspNetUserRoles>, IAspNetUserRoles
    {
        public AspNetUserRolesClass()
            : this(new SaxRepositoryContext())
        {

        }

        public AspNetUserRolesClass(IRepositoryContext repositoryContext)
            : base (repositoryContext)
        {

        }

        public override Expression<Func<AspNetUserRoles, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<AspNetUserRoles, bool>> SearchFilters(AspNetUserRoles obj)
        {
            return x => x.IDAspNetUserRol == obj.IDAspNetUserRol;
        }

        public void CreateAndRemove(List<AspNetUserRoles> create, List<int> remove)
        {
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {
                    if (remove.Count() != 0)
                    {
                        var countdelete = EFBatchOperation.For(db, db.AspNetUserRoles).Where(b => remove.Any(c => c.Equals(b.IDAspNetUserRol))).Delete();
                        EFBatchOperation.For(db, db.AspNetUserRoles).InsertAll(create);
                    } else
                    { EFBatchOperation.For(db, db.AspNetUserRoles).InsertAll(create); }
                }
                trx.Complete();
            }
        }
    }
}
