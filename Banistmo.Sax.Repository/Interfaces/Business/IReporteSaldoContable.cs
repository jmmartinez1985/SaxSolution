using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IReporteSaldoContable : IRepository<SAX_SALDO_CONTABLE>
    {
    }
    public interface IreporteSaldoContable : IRepository<SAX_REPORTE_SALDOS_Result>
    {

        IQueryable<SAX_REPORTE_SALDOS_Result> reportesaldo(DateTime? fechaCorte, int? IdEmpresa, int? IdCuenta, int? IdArea);
        List<SAX_REPORTE_SALDOS_Result> reportesaldoServLista(DateTime fechaCorte, int? idEmpresa, int idCuentaContable, int? idAreaOperativa);
        IQueryable<SAX_REPORTE_SALDOS_Result> reportesaldo(DateTime fechaCorte, int IdEmpresa, int IdCuenta, int IdArea);
    }
}
