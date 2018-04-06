using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Registro")]
    public class RegistroControlController : ApiController
    {
        private readonly IRegistroControlService service;
        private readonly IOnlyRegistroControlService srvOnlyRegistroControl;

        public RegistroControlController(IRegistroControlService rc, IOnlyRegistroControlService rcOnlyRegistro)
        {
            service = rc;
            srvOnlyRegistroControl = rcOnlyRegistro;
        }

        [Route("GetAllRegistro")]
        public IHttpActionResult GetAll()
        {
            var mdl = service.GetAll(c=>c.RC_ESTATUS_LOTE == "1").Select( c=> new {
                Registro = c.RC_REGISTRO_CONTROL,
                Area = c.RC_COD_AREA
            });
            if (mdl == null)
            {
                return NotFound();
            }
            return Ok(mdl);
        }

        [Route("GetRegistroByUser")]
        public IHttpActionResult GetRegistroByUser()
        {
            var userId = User.Identity.GetUserId();
            var mdl = service.GetAll(c=> c.RC_COD_USUARIO == userId,null,null);
            if (mdl == null)
            {
                return NotFound();
            }

            return Ok(mdl);
        }

        [Route("GetRegistroControlByUser")]
        public IHttpActionResult GetRegistroControlByUser()
        {
            var userId = User.Identity.GetUserId();
            List<OnlyRegistroControlModel> mdl = srvOnlyRegistroControl.GetAll(c => c.RC_COD_USUARIO == "22ad2290-b174-4a8d-bdc9-5115749e2001");
            if (mdl == null)
            {
                return NotFound();
            }

            return Ok(mdl);
        }


        [Route("GetRegistroByUserPag")]
        public IHttpActionResult GetRegistroByUserPag([FromUri]PagingParameterModel pagingparametermodel)
        {
            var userId = User.Identity.GetUserId();
            var source = service.GetAll(c => c.RC_COD_USUARIO == userId);
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

        [Route("GetRegistroById")]
        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.RC_REGISTRO_CONTROL == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] RegistroControlModel model)
        {
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] RegistroControlModel model)
        {
            service.Update(model);
            return Ok();
        }


    }
}
