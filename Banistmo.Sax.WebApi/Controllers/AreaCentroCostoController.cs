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
            service                 = service ?? new AreaCentroCostoService();
            empresaCentroService    = empresaCentroService ?? new EmpresaCentroService();
            empresaService          = empresaService ?? new EmpresaService();
            centroCostoService      = centroCostoService ?? new CentroCostoService();
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
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var areaCentroCosto = service.GetAll(a => a.CA_ID_AREA == id && a.AD_ESTATUS== activo);
            var empresaCentro = empresaCentroService.GetAll().Where(e => areaCentroCosto.Any(a => a.EC_ID_REGISTRO == e.EC_ID_REGISTRO));
            if (empresaCentro != null)
            {
                return Ok(empresaCentro.Select(ec => new {
                    EC_ID_REGISTRO      = ec.EC_ID_REGISTRO,
                    CE_ID_EMPRESA       =ec.CE_ID_EMPRESA,
                    NAME_EMPRESA        =empresaService.GetSingle(e=>e.CE_ID_EMPRESA==ec.CE_ID_EMPRESA).CE_NOMBRE,
                    NAME_CENTRO_COSTO   =centroCostoService.GetSingle(cc=>cc.CC_ID_CENTRO_COSTO==ec.CC_ID_CENTRO_COSTO).CC_NOMBRE,
                    CC_ID_CENTRO_COSTO  =ec.CC_ID_CENTRO_COSTO,
                    EC_ESTATUS          =ec.EC_ESTATUS,
                    EC_FECHA_CREACION   =ec.EC_FECHA_CREACION,
                    EC_USUARIO_CREACION =ec.EC_USUARIO_CREACION,
                    EC_FECHA_MOD        =ec.EC_FECHA_MOD,
                    EC_USUARIO_MOD      =ec.EC_USUARIO_MOD
                })                    
                );
                //return Ok(model.Select( a=> new {
                //    AD_ID_REGISTRO  = a.AD_ID_REGISTRO,
                //    CA_ID_AREA      = a.CA_ID_AREA,
                //    EC_ID_REGISTRO  = a.EC_ID_REGISTRO,
                //    LISTA_EMPRESA_CENTRO = empresaCentroService.GetAll(e=>e.EC_ID_REGISTRO== a.EC_ID_REGISTRO).Select( x=> new {
                //                    EC_ID_REGISTRO          = x.EC_ID_REGISTRO,
                //                    CE_ID_EMPRESA           = x.CE_ID_EMPRESA,
                //                    NAME_EMPRESA            = empresaService.GetSingle(em=>em.CE_ID_EMPRESA==x.CE_ID_EMPRESA).CE_NOMBRE,
                //                    NAME_CENTRO_COSTO       = centroCostoService.GetSingle(cc=>cc.CC_ID_CENTRO_COSTO==x.CC_ID_CENTRO_COSTO).CC_NOMBRE,
                //                    CC_ID_CENTRO_COSTO      = x.CC_ID_CENTRO_COSTO,
                //                    EC_ESTATUS              = x.EC_ESTATUS
                //    }).ToList()

                //}));
            }
            return NotFound();
        }

        [Route("GetEmpresa"), HttpGet]
        public IHttpActionResult GetEmpresa(int id) {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var areaCentroCosto = service.GetAll(a => a.CA_ID_AREA == id && a.AD_ESTATUS== activo);
            var empresaCentro = empresaCentroService.GetAll().Where(e => areaCentroCosto.Any(ac => ac.EC_ID_REGISTRO == e.EC_ID_REGISTRO));
            var empresas = empresaService.GetAll().Where(e => !empresaCentro.Any(ec => ec.CE_ID_EMPRESA == e.CE_ID_EMPRESA) ).ToList();
            return Ok(empresas.Select( e=> new {
                CE_ID_EMPRESA=e.CE_ID_EMPRESA,
                CE_NOMBRE= e.CE_COD_EMPRESA+"-"+e.CE_NOMBRE
            }));
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

        [Route("insertAreaCento"), HttpPost]
        public IHttpActionResult Insert([FromBody] AreaCentroCostoInsertModel model)
        {
            var id_registro =empresaCentroService.GetSingle(ec => ec.CE_ID_EMPRESA == model.CE_ID_EMPRESA && ec.CC_ID_CENTRO_COSTO == model.CC_ID_CENTRO_COSTO).EC_ID_REGISTRO;
            AreaCentroCostoModel areaCentroInsert = new AreaCentroCostoModel();
            areaCentroInsert.EC_ID_REGISTRO = id_registro;
            areaCentroInsert.AD_ESTATUS = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            areaCentroInsert.CA_ID_AREA = model.CA_ID_AREA;
            areaCentroInsert.AD_FECHA_CREACION = DateTime.Now;
            areaCentroInsert.AD_USUARIO_CREACION = User.Identity.GetUserId();
            return Ok(service.Insert(areaCentroInsert, true));
        }

        [Route("UpdateAreaCenCosto"), HttpPost]
        public IHttpActionResult Put([FromBody] AreaCentroCostoModel model)
        {
            model.AD_FECHA_MOD = System.DateTime.Now;
            model.AD_USUARIO_MOD = User.Identity.GetUserId();
            service.Update(model);
            return Ok();
        }

        [Route("deleteAreaCentroCosto"), HttpPost]
        public IHttpActionResult delete([FromBody] AreaCentroCostoModel model)
        {
            var objDelete = service.GetSingle(x => x.EC_ID_REGISTRO == model.EC_ID_REGISTRO);
            if (objDelete != null)
            {
                objDelete.AD_ESTATUS= Convert.ToInt16(BusinessEnumerations.Estatus.ELIMINADO);
                objDelete.AD_FECHA_MOD = DateTime.Now;
                objDelete.AD_USUARIO_MOD = User.Identity.GetUserId();
                service.Update(objDelete);
                return Ok();
            }
            else {
                return BadRequest("No se pudo eliminar el registro.");
            }
        }
    }
}
