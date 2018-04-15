using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/AreaCentroCosto")]
    public class AreaCentroCostoController : ApiController
    {
        private readonly IAreaCentroCostoService service;

        //public AreaCentroCostoController()
        //{
        //    service = service ?? new AreaCentroCostoService();
        //}

        public AreaCentroCostoController(IAreaCentroCostoService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<AreaCentroCostoModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.AD_ID_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] AreaCentroCostoModel model)
        {
            model.AD_ESTATUS = 1;
            model.AD_USUARIO_CREACION = User.Identity.GetUserId();
            model.AD_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateAreaCenCosto"), HttpPost]
        public IHttpActionResult Put([FromBody] AreaCentroCostoModel model)
        {
            model.AD_FECHA_MOD = System.DateTime.Now;
            model.AD_USUARIO_MOD = User.Identity.GetUserId();
            service.Update(model);
            return Ok();
        }
    }
}
