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
   
    [RoutePrefix("api/Empresa")]
    public class EmpresaController : ApiController
    {
        private readonly IEmpresaService empresaService;

        public EmpresaController(IEmpresaService em)
        {
            empresaService = em;
        }

        public IHttpActionResult Get()
        {
            List<EmpresaModel> em = empresaService.GetAll();
            if (em == null)
            {
                return NotFound();
            }
            return Ok(em);
        }
    }
}
