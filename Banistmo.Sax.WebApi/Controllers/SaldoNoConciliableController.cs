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
        private readonly IMovimientoControlService MovimientoControlService; 

        public SaldoNoConciliableController(ISaldoNoConciliableService sa, IReporterService rp, IMovimientoControlService mc)
        {
            service = sa;
            reportExcelService = rp;
            MovimientoControlService = mc;
        }

        public SaldoNoConciliableController()
        {
            service = service ?? new SaldoNoConciliableService();
            reportExcelService=reportExcelService ?? new ReporterService();
            MovimientoControlService = MovimientoControlService ?? new MovimientoControlService();
        }

        [HttpGet, Route("GetSaldo")]
        public IHttpActionResult Get([FromUri]PagingParameterModel parametro)
        {

            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var tmp = service.Query(x => x.SC_ESTATUS == activo);
            if (tmp == null | tmp.Count()==0)
            {
                return BadRequest("No se encontraron registros.");
            }

            int count = tmp.Count();
            int CurrentPage = parametro.pageNumber;
            int PageSize = parametro.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = tmp.OrderBy(c => c.SC_FECHA_CORTE).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var result = items.Select(x => new
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
            var paginacion = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data = result
            };
            return Ok(paginacion);
        }

        [HttpGet, Route("GetMovimientoControl")]
        public IHttpActionResult GetMovimientoControl(PagingParameterModel parametro)
        {

            var tmp = MovimientoControlService.Query(x=>x.MC_ID_MOVIMIENTO_CONTROL== x.MC_ID_MOVIMIENTO_CONTROL);
            var reslt = tmp.Select(x => new
            {
                ID_MOVIMIENTO_CONTROL = x.MC_ID_MOVIMIENTO_CONTROL,
                FECHA = x.MC_FECHA_PROCESO,
                EMPRESA = x.SAX_EMPRESA != null ? (x.SAX_EMPRESA.CE_COD_EMPRESA + "-" + x.SAX_EMPRESA.CE_NOMBRE) : string.Empty,
                MONEDA = x.SAX_MONEDA != null ? (x.SAX_MONEDA.CC_NUM_MONEDA + "-" + x.SAX_MONEDA.CC_DESC_MONEDA) : string.Empty,
                CUENTA_LIMPIEZA = x.MC_CUENTA_LIMPIEZA,
                TOTAL_CUENTAS = x.MC_TOTAL_REGISTRO,
                SALDO_TOTAL = x.MC_SALDO_TOTAL,
            }).OrderBy(y => y.FECHA).ToList();
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
