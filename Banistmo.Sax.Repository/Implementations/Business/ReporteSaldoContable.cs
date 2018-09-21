using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces;
using System.Linq.Expressions;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class ReporteSaldoContable : RepositoryBase<SAX_SALDO_CONTABLE>, IReporteSaldoContable
    {
        public ReporteSaldoContable()
          : this(new SaxRepositoryContext())
        {
        }
        public ReporteSaldoContable(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public override Expression<Func<SAX_SALDO_CONTABLE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_SALDO_CONTABLE, bool>> SearchFilters(SAX_SALDO_CONTABLE obj)
        {
            throw new NotImplementedException();
        }
    }

    public class reporteSaldoContable : RepositoryBase<SAX_REPORTE_SALDOS_Result>, IreporteSaldoContable
    {
        public override Expression<Func<SAX_REPORTE_SALDOS_Result, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public IQueryable<SAX_REPORTE_SALDOS_Result> reportesaldo(DateTime? fechaCorte, int? IdEmpresa, int? IdCuenta, int? IdArea)
        {
            throw new NotImplementedException();
        }

        public List<SAX_REPORTE_SALDOS_Result> reportesaldoServLista(DateTime fechaCorte, int? idEmpresa, int idCuentaContable, int? idAreaOperativa)
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REPORTE_SALDOS_Result, bool>> SearchFilters(SAX_REPORTE_SALDOS_Result obj)
        {
            throw new NotImplementedException();
        }

        IQueryable<SAX_REPORTE_SALDOS_Result> IreporteSaldoContable.reportesaldo(DateTime fechaCorte, int IdEmpresa, int IdCuenta, int IdArea)
        {
            throw new NotImplementedException();
        }
    }
}
