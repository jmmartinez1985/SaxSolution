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

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class ReporteRegistroControl : RepositoryBase<SAX_REGISTRO_CONTROL>, IReporteRegistroControl
    {
        public ReporteRegistroControl()
          : this(new SaxRepositoryContext())
        {
        }
        public ReporteRegistroControl(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> SearchFilters(SAX_REGISTRO_CONTROL obj)
        {
            throw new NotImplementedException();
        }
    }
}
