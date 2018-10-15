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
using Banistmo.Sax.Common;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/AreaOperativa")]
    public class AreaOperativaController : ApiController
    {
        private  IAreaOperativaService areaOperativaService;
        private ICatalogoService catalagoService;
        private IUsuarioAreaService usuarioAreaService;
        private IAreaCentroCostoService areaCentroCostoService;
        private IEmpresaService empresaService;
        private ICentroCostoService centroCostoService;
        private IEmpresaCentroService empresaCentroCostoService;

        public AreaOperativaController()
        {
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            catalagoService = new CatalogoService();
            usuarioAreaService = new UsuarioAreaService();
            areaCentroCostoService = new AreaCentroCostoService();
            empresaService = empresaService ?? new EmpresaService();
            centroCostoService = centroCostoService ?? new CentroCostoService();
            empresaCentroCostoService = empresaCentroCostoService ?? new EmpresaCentroService();
        }

        //public AreaOperativaController(IAreaOperativaService ao)
        //{
        //    areaOperativaService = ao;
        //}

        public IHttpActionResult Get()
        {
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<AreaOperativaModel> ar = areaOperativaService.GetAllFlatten<AreaOperativaModel>(a => a.CA_ESTATUS == activo);
            if (ar == null)
            {
                return NotFound();
            }
            return Ok(ar.Select(c => new {
                CA_ID_AREA = c.CA_ID_AREA,
                CA_COD_AREA = c.CA_COD_AREA,
                CA_NOMBRE = c.CA_NOMBRE,
                CA_ESTATUS = c.CA_ESTATUS,
                ESTATUS_TXT = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.CA_ESTATUS).CD_VALOR,
                CA_FECHA_CREACION = c.CA_FECHA_CREACION,
                CA_USUARIO_CREACION = c.CA_USUARIO_CREACION,
                CA_FECHA_MOD = c.CA_FECHA_MOD,
                CA_USUARIO_MOD = c.CA_USUARIO_MOD
            }));
            /*
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();
            List<AreaOperativaModel> ar = areaOperativaService.GetAllFlatten<AreaOperativaModel>( a=>a.CA_ESTATUS!=2);
            if (ar == null)
            {
                return NotFound();
            }
            return Ok(ar.Select( c=> new {
                        CA_ID_AREA          = c.CA_ID_AREA,
                        CA_COD_AREA         = c.CA_COD_AREA,
                        CA_NOMBRE           =c.CA_NOMBRE,
                        CA_ESTATUS          =c.CA_ESTATUS,
                        ESTATUS_TXT         = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.CA_ESTATUS).CD_VALOR,
                        CA_FECHA_CREACION   = c.CA_FECHA_CREACION,
                        CA_USUARIO_CREACION =c.CA_USUARIO_CREACION,
                        CA_FECHA_MOD        =c.CA_FECHA_MOD,
                        CA_USUARIO_MOD      =c.CA_USUARIO_MOD
            }));
            */
        }

        [Route("AreaOperativaActiva"), HttpGet]
        public IHttpActionResult GetAreaOperativaActiva()
        {
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<AreaOperativaModel> ar = areaOperativaService.GetAllFlatten<AreaOperativaModel>(a => a.CA_ESTATUS == activo);
            if (ar == null)
            {
                return NotFound();
            }
            string fmt = "000";
            return Ok(ar.Select(c => new {
                CA_ID_AREA = c.CA_ID_AREA,
                CA_COD_AREA = c.CA_COD_AREA.ToString(fmt),
                CA_NOMBRE = c.CA_NOMBRE,
                CA_ESTATUS = c.CA_ESTATUS,
                ESTATUS_TXT = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.CA_ESTATUS).CD_VALOR,
                CA_FECHA_CREACION = c.CA_FECHA_CREACION,
                CA_USUARIO_CREACION = c.CA_USUARIO_CREACION,
                CA_FECHA_MOD = c.CA_FECHA_MOD,
                CA_USUARIO_MOD = c.CA_USUARIO_MOD
            }).OrderBy(j=>j.CA_COD_AREA));
        }


        public IHttpActionResult Get( int id)
        {
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();
            AreaOperativaModel ar = areaOperativaService.GetSingle( a=>a.CA_ID_AREA==id);
            if (ar == null)
            {
                return NotFound();
            }
            return Ok(ar);
        }

        public IHttpActionResult Post([FromBody] AreaOperativaModel model)
        {
            int eliminado = Convert.ToInt16(BusinessEnumerations.Estatus.ELIMINADO);
            var existAreaOperativa = areaOperativaService.GetSingle(a => a.CA_COD_AREA == model.CA_COD_AREA);
            if (existAreaOperativa == null)
            {
                model.CA_FECHA_CREACION = DateTime.Now;
                model.CA_USUARIO_CREACION = User.Identity.GetUserId();
                model.CA_ESTATUS = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                return Ok(areaOperativaService.Insert(model, true));
            }
            else if  (existAreaOperativa !=null && existAreaOperativa.CA_ESTATUS == eliminado){
                existAreaOperativa.CA_FECHA_MOD = DateTime.Now;
                existAreaOperativa.CA_USUARIO_MOD = User.Identity.GetUserId();
                existAreaOperativa.CA_ESTATUS = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                existAreaOperativa.CA_NOMBRE = model.CA_NOMBRE;
                areaOperativaService.Update(existAreaOperativa);
                return Ok();
            }
            else {
                    return BadRequest("El código ingresado ya está siendo utilizado por otra área operativa.");

            }
           
           
        }

        // PUT: api/DiasFeriados/5
        [Route("UpdateAreaOperativa"), HttpPost]
        public IHttpActionResult Put([FromBody] AreaOperativaModel model)
        {
            try
            {
                if (model.CA_ESTATUS == 2)
                {
                    UsuarioAreaModel listUsuarioArea = usuarioAreaService.GetSingle(u => u.CA_ID_AREA == model.CA_ID_AREA && u.UA_ESTATUS == 1);
                    if (listUsuarioArea != null)
                        return BadRequest("No se puede eliminar un area operativa con supervisores asociados");

                    model.CA_FECHA_MOD = DateTime.Now;
                    model.CA_USUARIO_MOD = User.Identity.GetUserId();
                    areaOperativaService.Update(model);
                    return Ok();
                }
                else
                {
                    AreaOperativaModel existAreaOperativa = null;
                    var isChangeCodeArea = areaOperativaService.GetSingle(a => a.CA_COD_AREA == model.CA_COD_AREA && a.CA_ID_AREA == model.CA_ID_AREA);
                    if (isChangeCodeArea == null)
                        existAreaOperativa = areaOperativaService.GetSingle(a => a.CA_COD_AREA == model.CA_COD_AREA);
                    if (existAreaOperativa == null)
                    {
                       var areaOperativaUpdate = areaOperativaService.GetSingle(a => a.CA_ID_AREA == model.CA_ID_AREA);
                        areaOperativaUpdate.CA_FECHA_MOD = DateTime.Now;
                        areaOperativaUpdate.CA_USUARIO_MOD = User.Identity.GetUserId();
                        areaOperativaUpdate.CA_COD_AREA = model.CA_COD_AREA;
                        areaOperativaUpdate.CA_NOMBRE = model.CA_NOMBRE;
                        areaOperativaUpdate.CA_ESTATUS = model.CA_ESTATUS;
                        areaOperativaService.Update(areaOperativaUpdate);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("El código ingresado ya está siendo utilizado por otra área operativa.");
                    }
                }
                }catch (Exception ex) {
                    return BadRequest("Error al actualizar el registro " + ex.Message);

            }


        }

        [Route("ReporteAreaOperativa"), HttpPost]
        public IHttpActionResult ReporteAreaOperativa([FromBody]AreaOpeEmpresaCenCosto model)
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            int eliminado = Convert.ToInt16(BusinessEnumerations.Estatus.ELIMINADO);
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();
            List<AreaOperativaModel> ar = areaOperativaService.GetAllFlatten<AreaOperativaModel>(a => a.CA_ESTATUS != eliminado && (model.CA_ID_AREA == 0 ? true : a.CA_ID_AREA == model.CA_ID_AREA)).ToList();
            List<AreaCentroCostoModel> acc = areaCentroCostoService.GetAllFlatten<AreaCentroCostoModel>(ac => ac.AD_ESTATUS == activo).ToList();
            List<EmpresaCentroModel> ecc = empresaCentroCostoService.GetAllFlatten<EmpresaCentroModel>(empcen => empcen.EC_ESTATUS == activo).ToList();
            var result = from areaOperativa in ar
                    join areaCentroCosto in acc on areaOperativa.CA_ID_AREA equals areaCentroCosto.CA_ID_AREA into acGroup
                    from acJoin in acGroup.DefaultIfEmpty()
                    join empresaCentro in ecc on acJoin == null ? 0 : acJoin.EC_ID_REGISTRO equals empresaCentro.EC_ID_REGISTRO into eccGroup
                    from EccJoin in eccGroup.DefaultIfEmpty()
                    where (model.CE_ID_EMPRESA == 0 ? true : EccJoin.CE_ID_EMPRESA == model.CE_ID_EMPRESA) && (model.CC_ID_CENTRO_COSTO == 0 ? true : (EccJoin!=null? EccJoin.CC_ID_CENTRO_COSTO == model.CC_ID_CENTRO_COSTO: true))
                         select new
                    {
                        CA_COD_AREA = areaOperativa.CA_COD_AREA,
                        CA_NOMBRE   = areaOperativa.CA_NOMBRE,
                        EMPRESA     = NameEmpresa(EccJoin==null? 0:EccJoin.CE_ID_EMPRESA),
                        CENTROCOSTO = NameCentroCosto(EccJoin == null ? 0:EccJoin.CC_ID_CENTRO_COSTO),
                        ESTATUS_TXT = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == areaOperativa.CA_ESTATUS).CD_VALOR
                    };

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        private string NameEmpresa(int empresa)
        {
            string name = string.Empty;
            var result = empresaService.GetSingle(e => e.CE_ID_EMPRESA == empresa);
            if (result != null)
                name = $"{result.CE_COD_EMPRESA}-{result.CE_NOMBRE}";
            return name;
        }

        private string NameCentroCosto(int centroCosto)
        {
            string name = string.Empty;
            var result = centroCostoService.GetSingle(cc => cc.CC_ID_CENTRO_COSTO == centroCosto);
            if (result != null)
                name = $"{result.CC_CENTRO_COSTO}-{result.CC_NOMBRE}";
            return name;
        }

    }
}
