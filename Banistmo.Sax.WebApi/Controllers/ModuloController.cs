using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Modulo")]
    public class ModuloController : ApiController
    {
        private readonly IModuloService moduloService;
        private readonly ICatalogoService catalogoService;

        public ModuloController(IModuloService mo, ICatalogoService catServ)
        {
            moduloService = mo;
            catalogoService = catServ;
        }

        public IHttpActionResult Get()
        {
            var mo = moduloService.GetAll(null, null, includes: c => c.SAX_MODULO_ROL);
            if (mo == null)
            {
                return NotFound();
            }

            //return Ok(mo);
            var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, includes: c => c.SAX_CATALOGO_DETALLE);
            
            var listaFiltrada = mo.Where(c => c.MO_ESTATUS != 2);
            return Ok(listaFiltrada.Select(c => new
            {
                MO_ID_MODULO = c.MO_ID_MODULO,
                MO_MODULO = c.MO_MODULO,
                MO_PATH = c.MO_PATH,
                MO_DESCRIPCION = c.MO_DESCRIPCION,
                MO_ESTATUS = c.MO_ESTATUS,
                MO_ESTATUS_DESCRIPCION = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MO_ESTATUS).CD_VALOR,
                MO_FECHA_CREACION = c.MO_FECHA_CREACION,
                MO_USUARIO_CREACION = c.MO_USUARIO_CREACION,
                MO_FECHA_MOD = c.MO_FECHA_MOD,
                MO_USUARIO_MOD = c.MO_USUARIO_MOD,
                MO_ULTIMO_ACCESO = c.MO_ULTIMO_ACCESO
            }));
        }

        public IHttpActionResult Post([FromBody] ModuloModel model)
        {
            model.MO_ESTATUS = 1;
            return Ok(moduloService.Insert(model, true));
        }
    }
}
