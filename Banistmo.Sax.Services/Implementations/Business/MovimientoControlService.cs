using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Business
{
    public class MovimientoControlService: ServiceBase<MovimientoControlModel, SAX_MOVIMIENTO_CONTROL, MovimientoControl>, IMovimientoControlService
    {
        private IMovimientoControl service;
    public MovimientoControlService()
            : this(new MovimientoControl())
        {

    }
    public MovimientoControlService(MovimientoControl em)
            : base(em)
        { }
    
    }
}
