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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Eventos")]
    public class EventosController : ApiController
    {
        private readonly IEventosService eventoService;
        private ApplicationUserManager _userManager;
        public EventosController()
        {
            //eventoService = eventoService ?? new EventosService();
        }

        public EventosController(IEventosService ev)
        {
            eventoService = ev;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Route("GetAll"), HttpGet]
        public IHttpActionResult GetAll()
        {
            var evnt = eventoService.GetAll();
                       
            if (evnt == null)
            {
                return BadRequest("No se puedo listar los eventos.");
            }
            else
            {              
                return Ok(evnt);
            }                        
        }

        [Route("NuevoEvento"), HttpPost]
        public async Task<IHttpActionResult> NuevoEvento([FromBody] EventosModel evemodel)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                evemodel.EV_USUARIO_CREACION = user.Id;
                evemodel.EV_USUARIO_MOD = user.Id;
                var evento = eventoService.Insert_Eventos_EventosTempOperador(evemodel);

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Update_EventoTempOperador"), HttpPost]
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
        public async Task<IHttpActionResult> ApruebaEvento([FromBody] Int32 eventoidAprobado)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //evemodel.EV_USUARIO_CREACION = user.Id;
            bool aprobado = eventoService.SupervidorAprueba_Evento(eventoidAprobado, user.Id);
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

        [Route("FilterEvents"), HttpGet]
        public IHttpActionResult FilterEvents(Int32 IdEmp, Int32 IdAreaOpe, string IdCuentaDb, string IdCuentaCR)
        {
            try
            {
                List<EventosModel> filters = eventoService.SearchByFilter(IdEmp, IdAreaOpe, IdCuentaDb, IdCuentaCR);
                return Ok(filters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}