using Banistmo.Sax.Services.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using Banistmo.Sax.Common;
using System.Diagnostics;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ReporteGenerico<ReporteUsuarioModel, SAX_REPORTE_USUARIO_Result> : IReporte<ReporteUsuarioModel, SAX_REPORTE_USUARIO_Result>
    {
        public ReporteGenerico(){
            Debug.Print("sd");
            }

        public DbRawSqlQuery<SAX_REPORTE_USUARIO_Result> ExecuteProcedure(string spName, params object[] parameters)
        {
            throw new NotImplementedException();
        }
        //public DbRawSqlQuery<E> ExecuteProcedure(string spName, params object[] parameters)
        //{
        //    return this.ExecuteProcedure(spName, parameters);
        //}
    }
}
