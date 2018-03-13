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
    public class ComprobanteDetalle : RepositoryBase<SAX_COMPROBANTE_DETALLE>, IComprobanteDetalle
    {
        public ComprobanteDetalle()
            : this(new SaxRepositoryContext())
        {
        }
        public ComprobanteDetalle(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_COMPROBANTE_DETALLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_COMPROBANTE_DETALLE, bool>> SearchFilters(SAX_COMPROBANTE_DETALLE obj)
        {
            return x => x.TD_ID_DETALLE == obj.TD_ID_DETALLE;
        }
    }
}
