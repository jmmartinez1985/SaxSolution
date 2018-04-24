using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Comprobante")]
    public class ComprobanteController : ApiController
    {
        private readonly IComprobanteService service;

        //public ComprobanteController()
        //{
        //    service = service ?? new ComprobanteService();
        //}

        public ComprobanteController(IComprobanteService svc)
        {
            service = svc;
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

        [Route("AnularComprobante"), HttpGet]
        public IHttpActionResult AnularComprobante(int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            control.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO).ToString();
            control.TC_USUARIO_MOD = User.Identity.GetUserId();
            control.TC_FECHA_MOD = DateTime.Now;
            service.Update(control);
            return Ok();
        }
    }
}
