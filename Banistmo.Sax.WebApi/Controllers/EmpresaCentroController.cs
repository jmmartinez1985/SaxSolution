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
    [RoutePrefix("api/EmpresaCentro")]
    public class EmpresaCentroController : ApiController
    {
        private readonly IEmpresaCentroService service;

        public EmpresaCentroController(IEmpresaCentroService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<EmpresaCentroModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.EC_ID_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] EmpresaCentroModel model)
        {
            return Ok(service.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] EmpresaCentroModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
