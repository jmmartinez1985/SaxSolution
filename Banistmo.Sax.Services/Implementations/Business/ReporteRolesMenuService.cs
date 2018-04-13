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
    public class ReporteRolesMenuService: ServiceBase<ReporteRolesMenuModel, SAX_REPORTE_ROLES_MENU_Result, ReporteRolesMenu>,IReporteRolesMenuService
    {

        public ReporteRolesMenuService()
            : this(new ReporteRolesMenu())
        {

        }
        public ReporteRolesMenuService(ReporteRolesMenu rrmService)
            : base(rrmService)
        { }

        public List<ReporteRolesMenuModel> GetReporte() {
            return this.ExecuteProcedure("SAX_REPORTE_ROLES_MENU", new object[0]);
        }

    }
}
