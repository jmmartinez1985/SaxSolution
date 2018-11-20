using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
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
    [Authorize]
    [RoutePrefix("api/SaldoNoConciliable")]
    public class SaldoNoConciliableController : ApiController
    {
        private readonly ISaldoNoConciliableService service;
        private readonly IReporterService reportExcelService;

        public SaldoNoConciliableController(ISaldoNoConciliableService sa, IReporterService rp)
        {
            service = sa;
            reportExcelService = rp;
        }

        public SaldoNoConciliableController()
        {
            service = service ?? new SaldoNoConciliableService();
            reportExcelService=reportExcelService ?? new ReporterService();
        }

        [HttpGet, Route("GetSaldo")]
        public IHttpActionResult Get(PagingParameterModel parametro)
        {

            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var tmp = service.Query(x => x.SC_ESTATUS == activo);
            var reslt = tmp.Select(x => new
            {
                CO_ID_CUENTA_CONTABLE = x.CO_ID_CUENTA_CONTABLE,
                CE_ID_EMPRESA = x.CE_ID_EMPRESA,
                CC_ID_MONEDA = x.CC_ID_MONEDA,
                EMPRESA_NOMBRE = x.SAX_EMPRESA != null ? (x.SAX_EMPRESA.CE_COD_EMPRESA + "-" + x.SAX_EMPRESA.CE_NOMBRE) : string.Empty,
                MONEDA_NOMBRE= x.SAX_MONEDA !=null ? (x.SAX_MONEDA.CC_NUM_MONEDA+"-"+x.SAX_MONEDA.CC_DESC_MONEDA):string.Empty,
                CUENTA_CONTABLE_NOMBRE = x.SAX_CUENTA_CONTABLE != null ? (x.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE+x.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR+x.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR) : string.Empty,
                SC_FECHA_CORTE = x.SC_FECHA_CORTE,
                SC_SALDOS = x.SC_SALDOS,
                SC_FECHA_CREACION = x.SC_FECHA_CREACION
            }).OrderBy(y=> y.SC_FECHA_CORTE).ToList();
            if (reslt == null)
            {
                return BadRequest("No se encontraron registros.");
            }
            return Ok(reslt);
        }

        [Route("GetReporteExcelSaldoNoConciliable"), HttpGet]
        public HttpResponseMessage GetReporteExcel([FromUri]PartidaModel parms)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var tmp = service.Query(x => x.SC_ESTATUS == activo);
            var result = tmp.Select(x => new
            {
                EMPRESA_NOMBRE = x.SAX_EMPRESA != null ? (x.SAX_EMPRESA.CE_COD_EMPRESA.Trim() + "-" + x.SAX_EMPRESA.CE_NOMBRE.Trim()) : string.Empty,
                MONEDA_NOMBRE = x.SAX_MONEDA != null ? (x.SAX_MONEDA.CC_NUM_MONEDA.Trim() + "-" + x.SAX_MONEDA.CC_DESC_MONEDA.Trim()) : string.Empty,
                CUENTA_CONTABLE_NOMBRE = x.SAX_CUENTA_CONTABLE != null ? (x.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE.Trim() + x.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR.Trim() + x.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR.Trim()) : string.Empty,
                SC_SALDOS = x.SC_SALDOS
            }).OrderBy(y => y.EMPRESA_NOMBRE ).ThenBy(z=> z.MONEDA_NOMBRE).ThenBy(a=>a.CUENTA_CONTABLE_NOMBRE);
            if (result !=null && result.Count() == 0) {
                var codeHttp = HttpStatusCode.BadRequest;
                response = Request.CreateResponse(codeHttp);
                response.Content = new StringContent("No hay cuentas contables para ser limpiadas.");
                return response;
            }
            var dt = result.ToList().AnonymousToDataTable();
            if (dt != null && dt.Columns.Count > 0)
            {
                dt.Columns[0].Caption = "Empresa";
                dt.Columns[1].Caption = "Moneda";
                dt.Columns[2].Caption = "Cuenta Contable";
                dt.Columns[3].Caption = "Saldo";
            }

            byte[] fileExcell = reportExcelService.CreateReportBinary(dt, "Hoja1");
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
    }
}
