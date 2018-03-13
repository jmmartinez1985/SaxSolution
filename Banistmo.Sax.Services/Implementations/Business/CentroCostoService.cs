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
    public class CentroCostoService : ServiceBase<CentroCostoModel, SAX_CENTRO_COSTO, CentroCosto>, ICentroCosto
    {
        public CentroCostoService()
            : this(new CentroCosto())
        {

        }
        public CentroCostoService(CentroCosto ao)
            : base(ao)
        { }
    }
}
