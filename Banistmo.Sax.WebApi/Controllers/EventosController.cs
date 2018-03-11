using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;

namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/Eventos")]
    public class EventosController : ApiController
    {
        private readonly IEventosService eventoService;

        public EventosController()
        {
        }

        public EventosController(IEventosService ev)
        {
            eventoService = ev;
        }

        public IHttpActionResult Get()
        {
            List<EventosModel> eve = eventoService.GetAll();
            if (eve == null)
            {
                return NotFound();
            }
            return Ok(eve);
        }

        [Route("InsertEvento_EventoTemp")]
        public IHttpActionResult Post([FromBody] Eventos_EventosTemp model)
        {
            eventoService.Insert_Eventos_EventosTemp(model.evemodel, model.evetempemodel);
            
            return Ok();
        }
    }
}