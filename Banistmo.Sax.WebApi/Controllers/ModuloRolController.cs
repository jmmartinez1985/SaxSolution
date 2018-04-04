using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Microsoft.AspNet.Identity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/ModuloRol")]
    public class ModuloRolController : ApiController
    {
        private readonly IModuloRolService moduloRolService;

        public ModuloRolController(IModuloRolService mr)
        {
            moduloRolService = mr;
        }

        public IHttpActionResult GetModulos(int id)
        {
            var modulosRol = moduloRolService.GetSingle(c => c.MO_ID_MODULO == id);

            if (modulosRol != null)
            {
                return Ok(modulosRol);
            }

            return NotFound();
        }

        [Route("GetModulosByRoles")]
        public IHttpActionResult GetModulosByRoles(string id)
        {
            var modulosRol = moduloRolService.GetAll(c => c.RL_ID_ROL == id, null, c=> c.SAX_MODULO);
            List<ModuloModel> listModulos = new List<ModuloModel>();
            if (modulosRol != null)
            {
                foreach(var modulo in modulosRol.ToList())
                {
                    listModulos.Add(modulo.SAX_MODULO);
                }

                return Ok(listModulos.Select(c => new {
                    MO_ID_MODULO =  c.MO_ID_MODULO,
                    MO_MODULO =  c.MO_MODULO,
                    MO_PATH =  c.MO_PATH,
                    MO_DESCRIPCION =  c.MO_DESCRIPCION,
                    MO_ESTATUS =  c.MO_ESTATUS,
                    MO_FECHA_CREACION =  c.MO_FECHA_CREACION,
                    MO_USUARIO_CREACION =  c.MO_USUARIO_CREACION
                }));
            }

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] ModuloRolModel model)
        {
            model.MR_ESTATUS = 1;
            return Ok(moduloRolService.Insert(model, true));
        }

        // PUT: api/User/5
        public IHttpActionResult Put([FromBody] ModuloRolModel model)
        {
            moduloRolService.Update(model);
            return Ok();
        }

        [Route("ManageModeloInRol"),HttpPost]
        public IHttpActionResult Post([FromBody] ModuloInRole model)
        {
            var userId = User.Identity.GetUserId();
            model.AddModuloRolModel.All(c => { c.MR_USUARIO_CREACION = userId; c.MR_FECHA_CREACION = DateTime.Now; c.MR_FECHA_MOD = DateTime.Now; return true; });
            moduloRolService.CreateAndRemove(model.AddModuloRolModel);
            return Ok();
        }
    }
}
