using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Registro")]
    public class RegistroControlController : ApiController
    {
        private readonly IRegistroControlService service;

        public RegistroControlController(IRegistroControlService rc)
        {
            service = rc;
        }

        public IHttpActionResult Get()
        {
            List<RegistroControlModel> mdl = service.GetAll();
            if (mdl == null)
            {
                return NotFound();
            }
            return Ok(mdl);
        }

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
