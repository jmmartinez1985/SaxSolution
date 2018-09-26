using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Interfaces;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Implementations;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ReporteSaldoContableService : ServiceBase<ReporteSaldoContableModel, SAX_SALDO_CONTABLE, ReporteSaldoContable>, ISaldoContableService
    {

        public ReporteSaldoContableService()
            : this(new ReporteSaldoContable())
        {

        }
        public ReporteSaldoContableService(ReporteSaldoContable ao)
            : base(ao)
        { }

        
      
    }
    [Injectable]
    public class reporteSaldoServ : ServiceBase<reporteSaldos, SAX_REPORTE_SALDOS_Result, reporteSaldoContable>, ISaldoContableServ
    {

        public reporteSaldoContable reporteSaldoService()
        {
            throw new NotImplementedException();
        }
            //this(new reporteSaldoServ(),RepositoryBase <SAX_REPORTE_SALDOS_Result> obj); 
         

        public reporteSaldoServ(RepositoryBase<SAX_REPORTE_SALDOS_Result> obj) : base(obj)
        {
        }

        public reporteSaldoServ() : this(new reporteSaldoContable())
        {
            
        }

        public List<reporteSaldos> reportesaldoServ(DateTime fechaCorte)
        {
            var parameters = new List<object>();
            parameters.Add(new SqlParameter("fechaCorte", System.Data.SqlDbType.DateTime,20,fechaCorte.ToString() ));
            
            return this.ExecuteProcedure("SAX_REPORTE_SALDOS", parameters);
        }

        IQueryable<SAX_REPORTE_SALDOS_Result> ISaldoContableServ.reporteSaldoServ(DateTime? fechaCorte)
        {
            throw new NotImplementedException();
        }

        IQueryable<SAX_REPORTE_SALDOS_Result> ISaldoContableServ.reporteSaldoServ(DateTime fechaCorte)
        {
            throw new NotImplementedException();
        }

        public object reportesaldoServ(DateTime? fechaCorte)
        {
            throw new NotImplementedException();
        }

        public List<reporteSaldos> reportesaldoServLista(DateTime fechaCorte)
        {
            //throw new NotImplementedException();
            //var parameters = new List<object>();

            object[] passParameters = new object[1];

            passParameters[0] = fechaCorte.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            string fecha=fechaCorte.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            SqlParameter[]  sqlParams = new SqlParameter[]
            {
                new SqlParameter { ParameterName = "@0",  Value =fecha },

            };

            try
            {
                return this.ExecuteProcedure("SAX_REPORTE_SALDOS @fechaCorte = @0", sqlParams);
            }
            catch (Exception e) { throw e; }
        }
    }
}
