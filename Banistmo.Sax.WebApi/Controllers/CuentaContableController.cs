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
    [RoutePrefix("api/CuentaContable")]
    public class CuentaContableController : ApiController
    {
        private readonly ICuentaContableService service;

        public CuentaContableController(ICuentaContableService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<CuentaContableModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CO_ID_CUENTA_CONTABLE == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CuentaContableModel model)
        {
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] CuentaContableModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
