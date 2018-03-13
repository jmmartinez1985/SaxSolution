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
    [RoutePrefix("api/Catalogo")]
    public class CatalogoController : ApiController
    {
        private readonly ICatalogoService service;

        public CatalogoController(ICatalogoService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<CatalogoModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CA_ID_CATALOGO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CatalogoModel model)
        {
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] CatalogoModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
