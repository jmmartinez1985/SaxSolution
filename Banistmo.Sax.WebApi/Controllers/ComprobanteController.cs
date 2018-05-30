﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;
using Banistmo.Sax.Common;
using Banistmo.Sax.WebApi.Models;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Comprobante")]
    public class ComprobanteController : ApiController
    {
        private readonly IComprobanteService service;
        private readonly IPartidasService servicePartida;

        //public ComprobanteController()
        //{
        //    service = service ?? new ComprobanteService();
        //}

        public ComprobanteController(IComprobanteService svc, IPartidasService svcPart)
        {
            service = svc;
            servicePartida = svcPart;
        }

        public IHttpActionResult Get()
        {
            List<ComprobanteModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] ComprobanteModel model)
        {
            model.TC_USUARIO_CREACION = User.Identity.GetUserId();
            model.TC_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateComprobante"), HttpPost]
        public IHttpActionResult Put([FromBody] ComprobanteModel model)
        {
            model.TC_FECHA_MOD = DateTime.Now;
            model.TC_USUARIO_MOD = User.Identity.GetUserId();
            service.Update(model);
            return Ok();
        }

        [Route("AnularComprobante/{id:int}"), HttpPost]
        public IHttpActionResult AnularComprobante( int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (control != null)
            {
                var userName = User.Identity.GetUserId();
                service.AnularComprobante(id, userName);
                return Ok();
            }
            else
                return BadRequest("No se puede anular un comprobante que no existe.");
        }

        [Route("SolicitarAnulacion/{id:int}"), HttpPost]
        public IHttpActionResult SolicitarAnulacion(int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (control != null)
            {
                var userName = User.Identity.GetUserId();
                service.SolitarAnulacion(control, userName);
                return Ok();
            }
            else
                return BadRequest("No se puede solicitar una anulacion de un comprobante que no existe.");
        }


        [Route("RechazarAnulacion/{id:int}"), HttpPost]
        public IHttpActionResult RechazarAnulacion(int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (control != null)
            {
                var userName = User.Identity.GetUserId();
                service.RechazarAnulacion(control, userName);
                return Ok();
            }
            else
                return BadRequest("No se puede rechazar una anulacion de un comprobante que no existe.");
        }

        [Route("ConciliacionManual"), HttpPost]
        public IHttpActionResult ConciliacionManual([FromBody] ConciliacionModel details)
        {
            var control = details.PartidasConciliar.Count;
            if (control > 0)
            {
                var userName = User.Identity.GetUserId();
                service.ConciliacionManual(details.PartidasConciliar, userName);
                return Ok();
            }
            else
                return BadRequest("Debe seleccionar partidas a conciliar.");
        }

        [Route("ConsultaRegAnular"), HttpGet]
        public IHttpActionResult consultaRegAnular()
        {
            try
            {
                var model = service.ConsultaComprobanteConciliadaServ();
                if (model.Count > 0)
                {
                    return Ok(model);
                }
                else
                {
                    return Ok("Consulta no produjo resultados");
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
