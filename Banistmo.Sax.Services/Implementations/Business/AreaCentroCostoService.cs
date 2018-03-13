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
    public class AreaCentroCostoService :  ServiceBase<AreaCentroCostoModel, SAX_AREA_CENCOSTO, AreaCentroCosto>, IAreaCentroCostoService
    {
        public AreaCentroCostoService()
            : this(new AreaCentroCosto())
        {

        }
        public AreaCentroCostoService(AreaCentroCosto ao)
            : base(ao)
        { }
    }
}
