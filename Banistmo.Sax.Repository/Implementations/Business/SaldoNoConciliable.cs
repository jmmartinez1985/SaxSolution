using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces;
using System.Data.Entity;


namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class SaldoNoConciliable : RepositoryBase<SAX_SALDO_NOCONCILIABLE>, ISaldoNoConciliable
    {

        public SaldoNoConciliable()
            : this(new SaxRepositoryContext())
        {
        }
        public SaldoNoConciliable(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_SALDO_NOCONCILIABLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_SALDO_NOCONCILIABLE, bool>> SearchFilters(SAX_SALDO_NOCONCILIABLE obj)
        {
            throw new NotImplementedException();
        }

        
    }
}
