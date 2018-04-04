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
using EntityFramework.Utilities;
using System.Transactions;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class ModuloRol : RepositoryBase<SAX_MODULO_ROL>, IModuloRol
    {
        public ModuloRol()
            : this(new SaxRepositoryContext())
        {
        }
        public ModuloRol(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_MODULO_ROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_MODULO_ROL, bool>> SearchFilters(SAX_MODULO_ROL obj)
        {
            return x => x.MR_ID_MODULO_ROL == obj.MR_ID_MODULO_ROL;
        }

        public void CreateAndRemove(List<SAX_MODULO_ROL> create)
        {
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {
                    List<String> listDeleted= (List<String>)create.GroupBy(x => x.RL_ID_ROL).Select(grp => grp.First().RL_ID_ROL).ToList();
                    EFBatchOperation.For(db, db.SAX_MODULO_ROL).Where(b => listDeleted.Any(c => c == b.RL_ID_ROL)).Delete();
                    EFBatchOperation.For(db, db.SAX_MODULO_ROL).InsertAll(create);
                }
                trx.Complete();
            }
        }
    }
}
