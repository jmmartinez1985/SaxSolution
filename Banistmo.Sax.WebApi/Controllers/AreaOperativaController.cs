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
    [RoutePrefix("api/AreaOperativa")]
    public class AreaOperativaController : ApiController
    {
        private  IAreaOperativaService areaOperativaService;
        private ICatalogoService catalagoService;
        private IUsuarioAreaService usuarioAreaService;

        public AreaOperativaController()
        {
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            catalagoService = new CatalogoService();
            usuarioAreaService = new UsuarioAreaService();
        }

        //public AreaOperativaController(IAreaOperativaService ao)
        //{
        //    areaOperativaService = ao;
        //}

        public IHttpActionResult Get()
        {
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();
            List<AreaOperativaModel> ar = areaOperativaService.GetAll( a=>a.CA_ESTATUS!=2);
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
            var existAreaOperativa = areaOperativaService.GetSingle(a => a.CA_COD_AREA == model.CA_COD_AREA);
            if (existAreaOperativa == null)
            {
                model.CA_FECHA_CREACION = DateTime.Now;
                model.CA_USUARIO_CREACION = User.Identity.GetUserId();
                model.CA_ESTATUS = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                return Ok(areaOperativaService.Insert(model, true));
            }
            else {
                return BadRequest("El código ingresado ya está siendo utilizado por otra área operativa.");
            }
           
           
        }

        // PUT: api/DiasFeriados/5
        [Route("UpdateAreaOperativa"), HttpPost]
        public IHttpActionResult Put([FromBody] AreaOperativaModel model)
        {

            if (model.CA_ESTATUS == 2)
            {
                List<UsuarioAreaModel> listUsuarioArea = usuarioAreaService.GetAll(u => u.CA_ID_AREA == model.CA_ID_AREA && u.UA_ESTATUS == 1);
                if(listUsuarioArea!=null && listUsuarioArea.Count>0)
                    return BadRequest("No se puede eliminar un area operativa con supervisores asociados");

                model.CA_FECHA_MOD = DateTime.Now;
                model.CA_USUARIO_MOD = User.Identity.GetUserId();
                areaOperativaService.Update(model);
                return Ok();
            }
            else {
                AreaOperativaModel existAreaOperativa = null;
                var isChangeCodeArea = areaOperativaService.GetSingle(a => a.CA_COD_AREA == model.CA_COD_AREA && a.CA_ID_AREA == model.CA_ID_AREA);
                if (isChangeCodeArea == null)
                    existAreaOperativa = areaOperativaService.GetSingle(a => a.CA_COD_AREA == model.CA_COD_AREA);
                if (existAreaOperativa == null)
                {
                    model.CA_FECHA_MOD = DateTime.Now;
                    model.CA_USUARIO_MOD = User.Identity.GetUserId();
                    areaOperativaService.Update(model);
                    return Ok();
                }
                else
                {
                    return BadRequest("El código ingresado ya está siendo utilizado por otra área operativa.");
                }
            }
           
        }

    }
}
