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
    [RoutePrefix("api/CentroCosto")]
    public class CentroCostoController : ApiController
    {
        private readonly ICentroCostoService service;

        //public CentroCostoController()
        //{
        //    service = service ?? new CentroCostoService();
        //}

        public CentroCostoController(ICentroCostoService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<CentroCostoModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CC_ID_CENTRO_COSTO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CentroCostoModel model)
        {
            model.CC_ESTATUS =Convert.ToInt16(BusinessEnumations.Estatus.ACTIVO);
            model.CC_USUARIO_CREACION = User.Identity.GetUserId();
            model.CC_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateCentroCosto"), HttpPost]
        public IHttpActionResult Put([FromBody] CentroCostoModel model)
        {
            model.CC_FECHA_MOD = DateTime.Now;
            model.CC_USUARIO_MOD = User.Identity.GetUserId();
            service.Update(model);
            return Ok();
        }
    }
}
