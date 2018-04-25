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
using System.ComponentModel.DataAnnotations;

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

        [Route("ListarEventos"), HttpGet]
        public IHttpActionResult ListarEventos()
        {
            var evnt = eventoService.GetAll();
                       
            if (evnt == null)
            {
                return BadRequest("No se puedo listar los eventos.");
            }
            else
            {
                var eve = evnt.Select(ev => new
                {
                     EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,EV_ID_AREA = ev.EV_ID_AREA
                    ,EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,EV_REFERENCIA = ev.EV_REFERENCIA
                    ,EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,EV_ESTATUS = ev.EV_ESTATUS
                    ,EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,NOMBRE_USER_MOD = ev.AspNetUsers1.FirstName
                    ,EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION==null? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR== null?"":ev.EV_USUARIO_APROBADOR)
                    ,NOMBRE_USER_APROB = (ev.AspNetUsers2==null?"": ev.AspNetUsers2.FirstName)
                });
                return Ok(eve);
            }                        
        }

        [Route("ListarEventosPorId"), HttpGet]
        public IHttpActionResult ListarEventosPorId(int eventoId)
        {
            var evnt = eventoService.GetAll(ev => ev.EV_COD_EVENTO == eventoId);

            if (evnt == null)
            {
                return BadRequest("No se puedo listar los eventos.");
            }
            else
            {
                var eve = evnt.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA = ev.EV_REFERENCIA
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1.FirstName
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });
                return Ok(eve);
            }
        }


        [Route("FiltrarEventos"), HttpGet]
        public IHttpActionResult ListarEventosPorId( [FromUri] ParameterFilter data)
        {
            var evnt = eventoService.GetAll(ev => ev.EV_COD_EVENTO == (data.EventoId == null ? ev.EV_COD_EVENTO: data.EventoId)
                                            && ev.EV_CUENTA_CREDITO == (data.IdCuentaDb == null ? ev.EV_CUENTA_CREDITO: data.IdCuentaDb)
                                            && ev.EV_CUENTA_DEBITO == (data.IdCuentaCr == null ? ev.EV_CUENTA_DEBITO: data.IdCuentaCr)
                                            && ev.CE_ID_EMPRESA == (data.EmpId == null ? ev.CE_ID_EMPRESA: data.EmpId)
                                            && ev.EV_ID_AREA == (data.IdAreaOpe == null? ev.EV_ID_AREA: data.IdAreaOpe));

            if (evnt.Count == 0)
            {
                return BadRequest("No se puedo listar los eventos filtrados.");
            }
            else
            {
                var eve = evnt.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA = ev.EV_REFERENCIA
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1.FirstName
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });
                return Ok(eve);
            }
        }

        public class ParameterFilter
        {
            
            public int? EventoId { get; set;}
            public int? IdCuentaDb { get; set; }
            public int? IdCuentaCr { get; set; }
            public int? EmpId { get; set; }
            public int? IdAreaOpe { get; set; }

        }
        
        [Route("NuevoEvento"), HttpPost]
        public async Task<IHttpActionResult> NuevoEvento([FromBody] ParameterEventoModel evemodel)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                evemodel.EV_USUARIO_CREACION = user.Id;
                evemodel.EV_FECHA_CREACION = DateTime.Now;
                evemodel.EV_USUARIO_MOD = user.Id;
                evemodel.EV_FECHA_MOD = DateTime.Now;
                evemodel.EV_ESTATUS = Convert.ToInt32(RegistryState.PorAprobar);
                var evento = eventoService.Insert_Eventos_EventosTempOperador(mapeoParametro_EventoModel(evemodel));

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo insertar el evento. " + ex.Message);
            }
        }

        private EventosModel mapeoParametro_EventoModel(ParameterEventoModel evt)
        {
            var evtReturn = new EventosModel();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO ;            
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = evt.EV_ESTATUS;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;         
            evtReturn.EV_REFERENCIA = evt.EV_REFERENCIA;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            return evtReturn;
        }

        [Route("ActualizarEvento"), HttpPost]
        public async Task<IHttpActionResult> Put([FromBody] ParameterEventoModel modelevtmp)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                modelevtmp.EV_USUARIO_CREACION = user.Id;
                modelevtmp.EV_USUARIO_MOD = user.Id;
                eventoService.Update_EventoTempOperador(mapeoParametro_EventosTempModel(modelevtmp));
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest("No se pudo actualizar el evento. " + ex.Message);
            }                
        }

        private EventosTempModel mapeoParametro_EventosTempModel(ParameterEventoModel evt)
        {
            var evtReturn = new EventosTempModel();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = evt.EV_ESTATUS;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            evtReturn.EV_REFERENCIA = evt.EV_REFERENCIA;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            return evtReturn;
        }
        
        [Route("CancelarEvento"), HttpPost]
        public IHttpActionResult CancelarEvento([FromBody] int eventoid)
        {
            try
            {
                var evnt = eventoService.GetSingle(ev => ev.EV_COD_EVENTO == eventoid);

                if (evnt == null)
                {
                    return BadRequest("No se puedo obtener el evento a cancelar.");
                }
                else
                {
                    evnt.EV_ESTATUS = 1;
                    bool Deshacer = eventoService.Update_EventoTempOperador(mapeoEventoModel_EventosTempModel(evnt));
                    if (Deshacer == false)
                    {
                        return BadRequest("No se puede cancelar el cambio del evento. ");
                    }
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest("No se pudo cancelar el cambio del evento. " + ex.Message);
            }            
        }

        private EventosTempModel mapeoEventoModel_EventosTempModel(EventosModel evt)
        {
            var evtReturn = new EventosTempModel();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO;
            //evtReturn.EV_COD_EVENTO_TEMP = evt.EV_COD_EVENTO;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = evt.EV_ESTATUS;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            evtReturn.EV_REFERENCIA = evt.EV_REFERENCIA;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            return evtReturn;
        }

        [Route("BuscarEventoPorAprobar"), HttpGet]
        public IHttpActionResult BuscarEventoPorAprobar(string fechaCaptura, string userCapturador)
        {
            try
            {
                DateTime fechaCap = Convert.ToDateTime(fechaCaptura);
                var evento = eventoService.GetAll(c => c.EV_FECHA_CREACION == (fechaCap == null ? c.EV_FECHA_CREACION : fechaCap)
                                                    && c.EV_USUARIO_CREACION == (userCapturador == null ? c.EV_USUARIO_CREACION : userCapturador));
                if (evento.Count == 0)
                {
                    return BadRequest("El filtro no trajo eventos. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA = ev.EV_REFERENCIA
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1.FirstName
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });

                return Ok(even);
            } catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos buscados. " + ex.Message);
            }
        }

        [Route("ListarEventosAprobados"), HttpGet]
        public IHttpActionResult ListarEventosAprobados()
        {
            try
            {                
                var evento = eventoService.GetAll(c => c.EV_ESTATUS == 1 );
                if (evento.Count == 0)
                {
                    return BadRequest("No existen eventos aprobados. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA = ev.EV_REFERENCIA
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1.FirstName
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });
                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos aprobador. " + ex.Message);
            }
        }

        [Route("FiltroRptEventosAprobados"), HttpGet]
        public IHttpActionResult FiltroRptEventosAprobados()
        {
            try
            {
                var evento = eventoService.GetAll(c => c.EV_ESTATUS == 1);
                if (evento.Count == 0)
                {
                    return BadRequest("No existen eventos aprobados. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA = ev.EV_REFERENCIA
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1.FirstName
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });
                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos aprobador. " + ex.Message);
            }
        }


        [Route("AprobarEvento"), HttpPost]
        public async Task<IHttpActionResult> ApruebaEvento([FromBody] Int32 eventoidAprobado)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                bool aprobado = eventoService.SupervidorAprueba_Evento(eventoidAprobado, user.Id);
                if (aprobado == false)
                {
                    return BadRequest("Error Aprobación Evento, No se aprobó el Evento");
                }
                return Ok(aprobado);
            }
            catch(Exception ex)
            {
                return BadRequest("Error en Aprobación de Evento. " + ex.Message);
            }
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
        
        public enum RegistryState
        {
            Pendiente = 0,
            Aprobado = 1,
            PorAprobar = 2,
            Eliminado = 3
        }
    }
}