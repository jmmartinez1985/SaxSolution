using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;

namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/CentroCosto")]
    public class CentroCostoController : ApiController
    {
        private readonly ICentroCosto service;

        public CentroCostoController(ICentroCosto svc)
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
            model.CC_ESTATUS = 1;
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] CentroCostoModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
