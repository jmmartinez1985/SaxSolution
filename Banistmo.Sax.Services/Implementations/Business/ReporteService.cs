using Banistmo.Sax.Common;
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
    [Injectable]
    public class ReporteService: ServiceBase<ReporteUsuarioModel, SAX_REPORTE_USUARIOS_Result, Reporte>,IReporteService
    {
        public ReporteService()
            :this(new Reporte() )
            {}
        public ReporteService(Reporte repor)
            : base(repor)
        { }

        public List<ReporteUsuarioModel> GetReporte()
        {
            return this.ExecuteProcedure("SAX_REPORTE_USUARIO", new object[0]);
        }
    }
}
