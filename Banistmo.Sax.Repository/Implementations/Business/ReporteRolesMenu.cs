using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class ReporteRolesMenu : RepositoryBase<SAX_REPORTE_ROLES_MENU_Result>, IReporteRolesMenu
    {

        public ReporteRolesMenu()
            : this(new SaxRepositoryContext())
        {
        }
        public ReporteRolesMenu(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_REPORTE_ROLES_MENU_Result, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REPORTE_ROLES_MENU_Result, bool>> SearchFilters(SAX_REPORTE_ROLES_MENU_Result obj)
        {
            throw new NotImplementedException();
        }
    }
}
