using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
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
    public class ReporteService: ServiceBase<ReporteUsuarioModel, SAX_REPORTE_USUARIO_Result, Reporte>,IReporteService
    {
        public ReporteService()
            :this(new Reporte() )
            {}
        public ReporteService(Reporte repor)
            : base(repor)
        { }

    }
}
