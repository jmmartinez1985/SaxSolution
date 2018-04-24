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
    [RoutePrefix("api/AreaCentroCosto")]
    public class AreaCentroCostoController : ApiController
    {
        private  IAreaCentroCostoService service;
        private IEmpresaCentroService empresaCentroService;
        private IEmpresaService empresaService;
        private ICentroCostoService centroCostoService;

        public AreaCentroCostoController()
        {
            service = service ?? new AreaCentroCostoService();
            empresaCentroService    = new EmpresaCentroService();
            empresaService          = new EmpresaService();
            centroCostoService      = new CentroCostoService();
        }

        public AreaCentroCostoController(IAreaCentroCostoService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<AreaCentroCostoModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.AD_ID_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [Route("GetByAreaOperativa"), HttpGet]
        public IHttpActionResult GetByAreaOperativa(int id)
        {
            var model = service.GetAll(c => c.CA_ID_AREA == id);

            if (model != null)
            {
                return Ok(model.Select( a=> new {
                    AD_ID_REGISTRO  = a.AD_ID_REGISTRO,
                    CA_ID_AREA      = a.CA_ID_AREA,
                    EC_ID_REGISTRO  = a.EC_ID_REGISTRO,
                    LISTA_EMPRESA_CENTRO = empresaCentroService.GetAll(e=>e.EC_ID_REGISTRO== a.EC_ID_REGISTRO).Select( x=> new {
                                    EC_ID_REGISTRO          = x.EC_ID_REGISTRO,
                                    CE_ID_EMPRESA           = x.CE_ID_EMPRESA,
                                    NAME_EMPRESA            = empresaService.GetSingle(em=>em.CE_ID_EMPRESA==x.CE_ID_EMPRESA).CE_NOMBRE,
                                    NAME_CENTRO_COSTO       = centroCostoService.GetSingle(cc=>cc.CC_ID_CENTRO_COSTO==x.CC_ID_CENTRO_COSTO).CC_NOMBRE,
                                    CC_ID_CENTRO_COSTO      = x.CC_ID_CENTRO_COSTO,
                                    EC_ESTATUS              = x.EC_ESTATUS
                    }).ToList()

                }));
            }
            return NotFound();
        }

        [Route("GetEmpresa"), HttpGet]
        public IHttpActionResult GetEmpresa(int id) {
            var areaCentroCosto = service.GetAll(a => a.CA_ID_AREA == id);
            var empresaCentro = empresaCentroService.GetAll().Where(e => areaCentroCosto.Any( ac=>ac.EC_ID_REGISTRO ==e.EC_ID_REGISTRO));
            return Ok();
        }

        [Route("GetCentroCosto"), HttpGet]
        public IHttpActionResult GetCentroCosto(int id) {
            return Ok(centroCostoService.GetAll());
        }
        public IHttpActionResult Post([FromBody] AreaCentroCostoModel model)
        {
            model.AD_ESTATUS = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            model.AD_USUARIO_CREACION = User.Identity.GetUserId();
            model.AD_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateAreaCenCosto"), HttpPost]
        public IHttpActionResult Put([FromBody] AreaCentroCostoModel model)
        {
            model.AD_FECHA_MOD = System.DateTime.Now;
            model.AD_USUARIO_MOD = User.Identity.GetUserId();
            service.Update(model);
            return Ok();
        }
    }
}
