using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.WebApi.Controllers
{
   
    [RoutePrefix("api/Empresa")]
    public class EmpresaController : ApiController
    {
        private readonly IEmpresaService service;

        //public EmpresaController()
        //{
        //    service = service ?? new EmpresaService();
        //}

        public EmpresaController(IEmpresaService em)
        {
            service = em;
        }

        public IHttpActionResult Get()
        {
            /*
            List<EmpresaModel> em = service.GetAll();
            if (em == null)
            {
                return NotFound();
            }
            return Ok(em);*/

            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<EmpresaModel> em = service.GetAll(x => x.CE_ESTATUS == activo.ToString());
            if (em == null)
            {
                return BadRequest("No se encontraron empresas activas.");
            }
            return Ok(em);
        }
        [Route("EmpresaActiva")]
        public IHttpActionResult GetActivas()
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<EmpresaModel> em = service.GetAll( x=> x.CE_ESTATUS== activo.ToString());
            if (em == null)
            {
                return BadRequest("No se encontraron empresas activas.");
            }
            return Ok(em);
        }
    }
}
