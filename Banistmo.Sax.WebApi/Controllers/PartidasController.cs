using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Controllers
{
    //[Authorize]
    [RoutePrefix("api/Partidas")]
    public class PartidasController : ApiController
    {
        private readonly IPartidasService partidasService;
        private readonly IEmpresaService empresaService;
        private readonly IReporterService reportExcelService;

        //public PartidasController()
        //{
        //    empresaService = empresaService ?? new EmpresaService();
        //    reportExcelService = reportExcelService ?? new ReporterService();
        //    partidasService = partidasService ?? new PartidasService();
        //}
        public PartidasController(IPartidasService part, IEmpresaService em, IReporterService rep)
        {
            partidasService = part;
            empresaService = em;
            reportExcelService = rep;
        }

        public IHttpActionResult Get()
        {
            var mdl = partidasService.GetAll(null, null, c=> c.SAX_REGISTRO_CONTROL);
            if (mdl == null)
            {
                return NotFound();
            }
            return Ok(mdl);
        }

        [Route("GetPartidaById")]
        public IHttpActionResult Get(int id)
        {
            var model = partidasService.GetSingle(c => c.PA_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

       

        [Route("GetPartidasByUser")]
        public IHttpActionResult GetPartidasByUser(String id)
        {
            var model = partidasService.GetAll(c => c.PA_USUARIO_CREACION == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [Route("GetPartidasByUserPag"),HttpGet]
        public IHttpActionResult PartidasByUserPagination(String id, [FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAll(c => c.PA_USUARIO_CREACION == id).OrderBy(c => c.RC_REGISTRO_CONTROL);
            var listEmpresas = empresaService.GetAll();
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            foreach (var row in items) {
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA+"-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);

        }

        [Route("GetPartidaByRegistro")]
        public IHttpActionResult GetByRegistro(int id)
        {
            var model = partidasService.GetAll(c => c.RC_REGISTRO_CONTROL == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }


        [Route("FindPartida"),HttpPost]
        //public IHttpActionResult FindPartida(PartidasModel parms int idRegistro, string idEmpresa,string idCuentaContable, decimal importe,string referencia)
        public IHttpActionResult FindPartida(PartidaModel parms)
        {
            List<PartidasModel> model = partidasService.GetAll(c => c.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL);

            var listEmpresas = empresaService.GetAll();

            if (parms.PA_COD_EMPRESA != null && parms.PA_COD_EMPRESA != String.Empty) {
                model = model.Where(x => x.PA_COD_EMPRESA.Equals(parms.PA_COD_EMPRESA)).ToList();
            }

            if (parms.PA_CTA_CONTABLE != null && parms.PA_CTA_CONTABLE != String.Empty)
            {
                model = model.Where(x => x.PA_CTA_CONTABLE.Trim().Equals(parms.PA_CTA_CONTABLE)).ToList();
            }

            if (parms.PA_IMPORTE!= 0)
            {
                model = model.Where(x => x.PA_IMPORTE == parms.PA_IMPORTE).ToList();
            }

            if (parms.PA_REFERENCIA != null && parms.PA_REFERENCIA != "")
            {
                model = model.Where(x => x.PA_REFERENCIA.Trim().Equals(parms.PA_REFERENCIA)).ToList();
            }

            int count = model.Count();
            int CurrentPage = parms.pageNumber;
            int PageSize = parms.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            foreach (var row in items)
            {
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }
            var paginacion = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data = items
            };

            if (paginacion != null)
            {
                return Ok(paginacion);
            }

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] PartidasModel model)
        {
            return Ok(partidasService.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] PartidasModel model)
        {
            partidasService.Update(model);
            return Ok();
        }


        [Route("GetAllPartidaPag")]
        public IHttpActionResult GetAllPagination([FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAll().OrderBy(c => c.RC_REGISTRO_CONTROL);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);

        }

        [Route("GetPartidaPag")]
        public IHttpActionResult GetPagination(int partida, [FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAll(c => c.RC_REGISTRO_CONTROL == partida);
            var listEmpresas = empresaService.GetAll();
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            foreach (var row in items)
            {
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }
            
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data=items
            };
            return Ok(paginationMetadata);

        }

        [Route("GetReporteExcel"),HttpGet]
        public HttpResponseMessage GetReporteExcel(PartidaModel parms) {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            List<PartidasModel> model = partidasService.GetAll(c => c.RC_REGISTRO_CONTROL == 11);

            var listEmpresas = empresaService.GetAll();

            if (parms.PA_COD_EMPRESA != null && parms.PA_COD_EMPRESA != String.Empty)
            {
                model = model.Where(x => x.PA_COD_EMPRESA.Equals(parms.PA_COD_EMPRESA)).ToList();
            }

            if (parms.PA_CTA_CONTABLE != null && parms.PA_CTA_CONTABLE != String.Empty)
            {
                model = model.Where(x => x.PA_CTA_CONTABLE.Trim().Equals(parms.PA_CTA_CONTABLE)).ToList();
            }

            if (parms.PA_IMPORTE != 0)
            {
                model = model.Where(x => x.PA_IMPORTE == parms.PA_IMPORTE).ToList();
            }

            if (parms.PA_REFERENCIA != null && parms.PA_REFERENCIA != "")
            {
                model = model.Where(x => x.PA_REFERENCIA.Trim().Equals(parms.PA_REFERENCIA)).ToList();
            }
            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            header.Add(new string[]{ "A","B","C","D","E","F","G","H","I","J"});
            byte[] fileExcell=reportExcelService.CreateReportBinary<PartidasModel>(header, model, "Excel1");
            var contentLength = fileExcell.Length;
            //200
            //successful
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

        [Route("Generate"), HttpGet]
        public HttpResponseMessage Generate()
        {
            var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "CertificationCard.pdf"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

    }
}
