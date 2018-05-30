using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces;
using System.Linq.Expressions;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class ReporteSaldoContable : RepositoryBase<SAX_SALDO_CONTABLE>, IReporteSaldoContable
    {
        public ReporteSaldoContable()
          : this(new SaxRepositoryContext())
        {
        }
        public ReporteSaldoContable(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_SALDO_CONTABLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_SALDO_CONTABLE, bool>> SearchFilters(SAX_SALDO_CONTABLE obj)
        {
            throw new NotImplementedException();
        }
    }
}
