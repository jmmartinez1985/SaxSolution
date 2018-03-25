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
    [Authorize]
    [RoutePrefix("api/ConceptoCosto")]
    public class ConceptoCostoController : ApiController
    {
        private readonly IConceptoCostoService service;

        public ConceptoCostoController(IConceptoCostoService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<ConceptoCostoModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CC_ID_CONCEPTO_COSTO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] ConceptoCostoModel model)
        {
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] ConceptoCostoModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
