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
            List<PartidasModel> mdl = partidasService.GetAll();
            if (mdl == null)
            {
                return NotFound();
            }
            throw new Exception("Error Jose.");
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


        [Route("GetPartidaByRegistro")]
        public IHttpActionResult GetByRegistro(int id)
        {
            var model = partidasService.GetSingle(c => c.RC_REGISTRO_CONTROL == id);

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

        [Route("GetPartidaPag")]
        public IHttpActionResult GetPagination([FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAll().OrderBy(c => c.PA_REGISTRO);
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
