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
    [RoutePrefix("api/ModuloRol")]
    public class ModuloRolController : ApiController
    {
        private readonly IModuloRolService moduloRolService;

        public ModuloRolController(IModuloRolService mr)
        {
            moduloRolService = mr;
        }

        [Route("{id:id}", Name = "GetBy")]
        public IHttpActionResult GetModulos(int id)
        {
            var modulosRol = moduloRolService.GetAll(c => c.MR_ID_MODULO_ROL == id);

            if (modulosRol != null)
            {
                return Ok(modulosRol);
            }

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] ModuloRolModel model)
        {
            return Ok(moduloRolService.Insert(model, true));
        }

        // PUT: api/User/5
        public IHttpActionResult Put([FromBody] ModuloRolModel model)
        {
            moduloRolService.Update(model);
            return Ok();
        }
    }
}
