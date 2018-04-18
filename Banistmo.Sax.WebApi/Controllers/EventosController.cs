using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.Services.Implementations.Business;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Eventos")]
    public class EventosController : ApiController
    {
        private readonly IEventosService eventoService;

        //public EventosController()
        //{
        //    eventoService = eventoService ?? new EventosService();
        //}

        public EventosController(IEventosService ev)
        {
            eventoService = ev;
        }

        public EventosController()
        {
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

        [Route("NuevoEvento"), HttpPost]
        public IHttpActionResult NuevoEvento(EventosModelsapi modelev)
        {
            
            eventoService.Insert_Eventos_EventosTempOperador(modelev.evemodel);

            return Ok();
        }

        [Route("Update_EventoTempOperador"),HttpPost]
        public IHttpActionResult Put([FromBody] EventosModelsapi modelevtmp)
        {
            //eventoService.Update_EventoTempOperador(modelevtmp.evetempemodel);


            return Ok();
        }

        [Route("Consulta_EventoTempOperador")]
        public IHttpActionResult Get(int eventoid)
        {
            var evento = eventoService.GetAll(c => c.EV_COD_EVENTO == eventoid);
            if (evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        [Route("DeshacerEventOperador"), HttpPost]
        public IHttpActionResult Put(int eventoid)
        {
            bool Deshacer = eventoService.Deshacer_EventoTempOperador(eventoid);
            if (Deshacer == false)
            {
                return BadRequest("No se puedo deshacer el cambio");
            }
            return Ok();
        }

        [Route("ApruebaEvento"), HttpPost]
        public IHttpActionResult ApruebaEvento(int eventoidAprobado)
        {
            bool aprobado = eventoService.SupervidorAprueba_Evento(eventoidAprobado);
            if (aprobado == false)
            {
                return BadRequest("Error Aprobación Evento, No se aprobó el Evento");
            }
            return Ok();
        }

        [Route("RechazaEvento"), HttpPost]
        public IHttpActionResult RechazaEvento(int eventoidRechazado)
        {
            bool rechazado = eventoService.SupervidorRechaza_Evento(eventoidRechazado);
            if (rechazado == false)
            {
                return BadRequest("Error rechazando Evento, No se pudo declinar el Evento");
            }
            return Ok();
        }
    }
}