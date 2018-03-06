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
    [RoutePrefix("api/Modulo")]
    public class ModuloController : ApiController
    {
        private readonly IModuloService moduloService;

        public ModuloController(IModuloService mo)
        {
            moduloService = mo;
        }

        public IHttpActionResult Get()
        {
            List<ModuloModel> mo = moduloService.GetAll();
            if (mo == null)
            {
                return NotFound();
            }
            return Ok(mo);
        }

        public IHttpActionResult Post([FromBody] ModuloModel model)
        {
            return Ok(moduloService.Insert(model, true));
        }
    }
}
