using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;



namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class CatalogoDetalleService : ServiceBase<CatalogoDetalleModel, SAX_CATALOGO_DETALLE, CatalogoDetalle>, ICatalogoDetalleService
    {
        public CatalogoDetalleService()
            : this(new CatalogoDetalle())
        {

        }
        public CatalogoDetalleService(CatalogoDetalle ao)
            : base(ao)
        { }
    }
}