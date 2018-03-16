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
    [RoutePrefix("api/AreaCentroCosto")]
    public class AreaCentroCostoController : ApiController
    {
        private readonly IAreaCentroCostoService service;

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
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] AreaCentroCostoModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
