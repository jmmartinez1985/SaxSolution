using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Partidas")]
    public class PartidasController : ApiController
    {
        private readonly IPartidasService partidasService;
        public PartidasController(IPartidasService part)
        {
            partidasService = part;
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

        [Route("GetPartidasByUserPag")]
        public IHttpActionResult GetPartidasByUserPagination(String id, [FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAll(c => c.PA_USUARIO_CREACION == id).OrderBy(c => c.RC_REGISTRO_CONTROL);
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

            if (parms.PA_COD_EMPRESA != null && parms.PA_COD_EMPRESA != String.Empty) {
                model = model.Where(x => x.PA_COD_EMPRESA == parms.PA_COD_EMPRESA).ToList();
            }

            if (parms.PA_CTA_CONTABLE != null && parms.PA_CTA_CONTABLE != String.Empty)
            {
                model = model.Where(x => x.PA_CTA_CONTABLE == parms.PA_CTA_CONTABLE).ToList();
            }

            if (parms.PA_IMPORTE!= 0)
            {
                model = model.Where(x => x.PA_IMPORTE == parms.PA_IMPORTE).ToList();
            }

            if (parms.PA_REFERENCIA != null && parms.PA_REFERENCIA != "")
            {
                model = model.Where(x => x.PA_REFERENCIA == parms.PA_REFERENCIA).ToList();
            }

            if (model != null)
            {
                return Ok(model);
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
    }
}
