using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
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
    [RoutePrefix("api/ReporteRegistroControl")]
    public class ReporteRegistroControlController : ApiController
    {
        private readonly IReporteRegistroControlService reportService;
        private readonly IReporterService reportExcelService;
        private ICatalogoService catalagoService;

        public ReporteRegistroControlController()
        {
            reportService = reportService ?? new ReporteRegistroControlService();
            reportExcelService = reportExcelService ?? new ReporterService();
            catalagoService = new CatalogoService();
        }

        public ReporteRegistroControlController(IReporteRegistroControlService rep, IReporterService repexcel, ICatalogoService serv)
        {
            reportService = rep;
            reportExcelService = repexcel;
            catalagoService = serv;
        }

        [Route("GetReporteExcelRegistros"), HttpGet]
        public HttpResponseMessage GetReporteExcelSaldos()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            List<ReporteRegistroControlModel> model = reportService.GetAll();
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var ltsTipoOperacion = catalagoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            //GetNameTipoOperacion(x.RC_COD_OPERACION, ref ltsTipoOperacion),
            List<ReporteRegistroControlPartialModel> Lista = (from c in model
                                                       select new ReporteRegistroControlPartialModel
                                                       {
                                                           //Supervisor = c.AspNetUsers.FirstName + " " + c.AspNetUsers.LastName,
                                                           NombreOperacion = GetNameTipoOperacion(c.RC_COD_OPERACION, ref ltsTipoOperacion),
                                                           Lote = c.RC_COD_PARTIDA,
                                                           //Capturador = c.AspNetUsers1.FirstName + " " + c.AspNetUsers1.LastName,
                                                           TotalRegistro = c.RC_TOTAL_REGISTRO,
                                                           TotalDebito = c.RC_TOTAL_DEBITO,
                                                           TotalCredito = c.RC_TOTAL_CREDITO,
                                                           Total = c.RC_TOTAL,
                                                           Status = GetStatusRegistroControl(c.RC_ESTATUS_LOTE, estatusList),
                                                           FechaCreacion = c.RC_FECHA_CREACION.Date,
                                                           HoraCreacion = c.RC_FECHA_CREACION.Hour
                                                       }).ToList();

            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            header.Add(new string[] { "A", "B" });
            byte[] fileExcell = reportExcelService.CreateReportBinary<ReporteRegistroControlPartialModel>(header, Lista, "Excel1");
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


        private string GetNameTipoOperacion(string id, ref CatalogoModel model)
        {
            string name = string.Empty;
            if (model != null)
            {
                CatalogoDetalleModel cataloDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS.ToString() == id).FirstOrDefault();
                if (cataloDetalle != null)
                    name = cataloDetalle.CD_VALOR;
            }
            return name;
        }

        private string GetStatusRegistroControl(string idStatus, CatalogoModel model)
        {
            int status = Convert.ToInt16(idStatus);
            string result = string.Empty;
            if (model != null)
            {
                var modelCatalogoDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS == status).FirstOrDefault();
                if (modelCatalogoDetalle != null)
                    result = modelCatalogoDetalle.CD_VALOR;
            }
            return result;
        }
    }
}
