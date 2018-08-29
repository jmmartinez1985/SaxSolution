using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/Moneda")]
    public class MonedaController : ApiController
    {
        private readonly IMonedaService service;

        public MonedaController(IMonedaService mo)
        {
            service = mo;
        }

        public IHttpActionResult Get()
        {
           
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<MonedaModel> mo = service.GetAll(x => x.CC_ESTATUS == activo.ToString());
            if (mo == null)
            {
                return BadRequest("No se encontraron empresas activas.");
            }
            return Ok(mo);
        }
        [Route("GetForSelect2")]
        public IHttpActionResult GetForSelect2()
        {

            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<MonedaModel> mo = service.GetAll(x => x.CC_ESTATUS == activo.ToString());
            if (mo == null)
            {
                return BadRequest("No se encontraron empresas activas.");
            }
            return Ok(mo.Select(x => new {
                id = x.CC_NUM_MONEDA,
                disabled = false,
                text=x.CC_NUM_MONEDA+"-"+x.CC_DESC_MONEDA
            }));
        }
    }
}