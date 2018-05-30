using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/ReporteSaldoContable")]
    public class ReporteSaldoContableController : ApiController
    {
        private readonly ISaldoContableService reportService;
        private readonly IReporterService reportExcelService;

        public ReporteSaldoContableController()
        {
            reportService = reportService ?? new ReporteSaldoContableService();
            reportExcelService = reportExcelService ?? new ReporterService();
        }

        public ReporteSaldoContableController(ISaldoContableService rep, IReporterService repexcel)
        {
            reportService = rep;
            reportExcelService = repexcel;
        }

        [Route("GetSaldoContable")]
        public IHttpActionResult GetSaldoContable([FromUri]ParametersSaldoContable parms)
        {
            List<ReporteSaldoContablePartialModel> SaldoContable = GetSaldoContableFiltro(parms);

            if (SaldoContable != null)
            {
                return Ok(SaldoContable);
            }
            return NotFound();
        }

        [Route("GetReporteExcelSaldoContable"), HttpGet]
        public HttpResponseMessage GetReporteExcelSaldoContable([FromUri]ParametersSaldoContable parms)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            List<ReporteSaldoContablePartialModel> Lista = GetSaldoContableFiltro(parms);
            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            header.Add(new string[] { "A", "B" });
            byte[] fileExcell = reportExcelService.CreateReportBinary<ReporteSaldoContablePartialModel>(header, Lista, "Excel1");
            var contentLength = fileExcell.Length;

            var statuscode = HttpStatusCode.OK;
            response = Request.CreateResponse(statuscode);
            response.Content = new StreamContent(new MemoryStream(fileExcell));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = contentLength;
            ContentDispositionHeaderValue contentDisposition = null;
            if (ContentDispositionHeaderValue.TryParse("inline; filename=" + "document" + ".xlsx", out contentDisposition))
            {
                response.Content.Headers.ContentDisposition = contentDisposition;
            }
            return response;
        }

        public List<ReporteSaldoContablePartialModel> GetSaldoContableFiltro(ParametersSaldoContable parms)
        {
            List<ReporteSaldoContableModel> model = reportService.GetAll();

            //model = (from c in model where                     
            //         c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_ID_EMPRESA == parms.IdEmpresa &&
            //         parms.FechaCorte != null ? c.SA_FECHA_CORTE.Date == Convert.ToDateTime(parms.FechaCorte).Date &&
            //         parms.IdCuentaContable != null ? c.SAX_CUENTA_CONTABLE.CO_ID_CUENTA_CONTABLE.Equals(parms.IdCuentaContable) &&
            //         parms.IdAreaOperativa != null ? c.SAX_CUENTA_CONTABLE.ca_id_area.Equals(parms.IdAreaOperativa)
            //         select c).ToList();

            if (parms != null)
            {
                if (parms.IdEmpresa != null)
                    model = model.Where(x => x.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_ID_EMPRESA.Equals(parms.IdEmpresa)).ToList();

                if (parms.FechaCorte != null)
                    model = model.Where(x => x.SA_FECHA_CORTE.Date == Convert.ToDateTime(parms.FechaCorte).Date).ToList();

                if (parms.IdCuentaContable != null)
                    model = model.Where(x => x.SAX_CUENTA_CONTABLE.CO_ID_CUENTA_CONTABLE.Equals(parms.IdCuentaContable)).ToList();

                if (parms.IdAreaOperativa != null )
                    model = model.Where(x => x.SAX_CUENTA_CONTABLE.ca_id_area.Equals(parms.IdAreaOperativa)).ToList();
            }

            List < ReporteSaldoContablePartialModel > ListSaldos = (from c in model
                                                                    select new ReporteSaldoContablePartialModel
                                                                    {
                                                                        nombreempresa = c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_COD_EMPRESA.Trim() + " " + c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_NOMBRE.Trim(),
                                                                        codcuentacontable = c.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE.Trim(),
                                                                        nombrecuentacontable = c.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA.Trim(),
                                                                        nombreareaoperativa = c.SAX_CUENTA_CONTABLE.SAX_AREA_OPERATIVA.CA_COD_AREA + " " + c.SAX_CUENTA_CONTABLE.SAX_AREA_OPERATIVA.CA_NOMBRE.Trim(),
                                                                        fechaforte = c.SA_FECHA_CORTE.Date,
                                                                        codmoneda = c.SAX_MONEDA.CC_NUM_MONEDA,
                                                                        saldo = c.SA_SALDOS

                                                                    }).ToList();

            return ListSaldos;
        }
    }
}
