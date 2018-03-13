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
    public class CatalogoDetalle : RepositoryBase<SAX_CATALOGO_DETALLE>, ICatalogoDetalle
    {
        public CatalogoDetalle()
            : this(new SaxRepositoryContext())
        {
        }
        public CatalogoDetalle(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_CATALOGO_DETALLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_CATALOGO_DETALLE, bool>> SearchFilters(SAX_CATALOGO_DETALLE obj)
        {
            return x => x.CD_ID_CATALOGO_DETALLE == obj.CD_ID_CATALOGO_DETALLE;
        }
    }
}