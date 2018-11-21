using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.Repository.Implementations.Business
{ [Injectable]
    public class MovimientoControl : RepositoryBase<SAX_MOVIMIENTO_CONTROL>, IMovimientoControl
    {
        public MovimientoControl()
            : this(new SaxRepositoryContext())
        {
        }
        public MovimientoControl(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_MOVIMIENTO_CONTROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_MOVIMIENTO_CONTROL, bool>> SearchFilters(SAX_MOVIMIENTO_CONTROL obj)
        {
            throw new NotImplementedException();
        }
    }
}
