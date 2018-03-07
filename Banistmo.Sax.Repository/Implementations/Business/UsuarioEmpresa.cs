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
    public class UsuarioEmpresa : RepositoryBase<SAX_USUARIO_EMPRESA>, IUsuarioEmpresa
    {
        public UsuarioEmpresa()
            : this(new SaxRepositoryContext())
        {
        }
        public UsuarioEmpresa(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_USUARIO_EMPRESA, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_USUARIO_EMPRESA, bool>> SearchFilters(SAX_USUARIO_EMPRESA obj)
        {
            return x => x.UE_ID_USUARIO_EMPRESA == obj.UE_ID_USUARIO_EMPRESA;
        }

        public void CreateAndRemove(List<SAX_USUARIO_EMPRESA> create, List<int> remove)
        {
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {
                    var countdelete = EFBatchOperation.For(db, db.SAX_USUARIO_EMPRESA).Where(b => remove.Any(c => c == b.CE_ID_EMPRESA)).Delete();
                    EFBatchOperation.For(db, db.SAX_USUARIO_EMPRESA).InsertAll(create);
                }
                trx.Complete();
            }
        }
    }
}
