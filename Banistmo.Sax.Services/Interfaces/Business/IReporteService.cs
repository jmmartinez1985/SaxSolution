using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IReporteService: IService<ReporteUsuarioModel, SAX_REPORTE_USUARIOS_Result, IReporte>
    {
        List<ReporteUsuarioModel> GetReporte();
    }
}
