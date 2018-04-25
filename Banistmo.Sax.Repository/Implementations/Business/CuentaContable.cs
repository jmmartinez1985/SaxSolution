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

namespace Banistmo.Sax.Repository.Implementations.Business
{

    [Injectable]
    public class CuentaContable : RepositoryBase<SAX_CUENTA_CONTABLE>, ICuentaContable
    {
        public CuentaContable()
            : this(new SaxRepositoryContext())
        {
        }
        public CuentaContable(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_CUENTA_CONTABLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_CUENTA_CONTABLE, bool>> SearchFilters(SAX_CUENTA_CONTABLE obj)
        {
            return x => x.CO_ID_CUENTA_CONTABLE == obj.CO_ID_CUENTA_CONTABLE;
        }

        
        
    }
}
