using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Repository.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface ISaldoContableService : IService<ReporteSaldoContableModel, SAX_SALDO_CONTABLE, IReporteSaldoContable>
    {
    }

    public interface ISaldoContableServ : IService<reporteSaldos, SAX_REPORTE_SALDOS_Result, IreporteSaldoContable>
    {
        IQueryable<SAX_REPORTE_SALDOS_Result> reporteSaldoServ(DateTime fechaCorte);
        List<reporteSaldos> reportesaldoServLista(DateTime fechaCorte);
        IQueryable<SAX_REPORTE_SALDOS_Result> reporteSaldoServ(DateTime? fechaCorte);
        object reportesaldoServ(DateTime? fechaCorte);
    }
}
