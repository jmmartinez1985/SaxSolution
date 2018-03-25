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
    [RoutePrefix("api/AreaOperativa")]
    public class AreaOperativaController : ApiController
    {
        private readonly IAreaOperativaService areaOperativaService;

        public AreaOperativaController(IAreaOperativaService ao)
        {
            areaOperativaService = ao;
        }

        public IHttpActionResult Get()
        {
            List<AreaOperativaModel> ar = areaOperativaService.GetAll();
            if (ar == null)
            {
                return NotFound();
            }
            return Ok(ar);
        }

        public IHttpActionResult Post([FromBody] AreaOperativaModel model)
        {
            model.CA_FECHA_CREACION = DateTime.Now;
            model.CA_ESTATUS = 1;
            return Ok(areaOperativaService.Insert(model, true));
        }

        // PUT: api/DiasFeriados/5
        public IHttpActionResult Put([FromBody] AreaOperativaModel model)
        {
            model.CA_FECHA_MOD = DateTime.Now;
            areaOperativaService.Update(model);
            return Ok();
        }

    }
}
