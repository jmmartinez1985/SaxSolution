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
    [RoutePrefix("api/UsuarioRol")]
    public class UsuarioRolController : ApiController
    {
        private readonly IUsuarioRolService usuarioRolService;

        public UsuarioRolController(IUsuarioRolService ur)
        {
            usuarioRolService = ur;
        }

        public IHttpActionResult Get()
        {
            List<UsuarioRolModel> ur = usuarioRolService.GetAll();
            if (ur == null)
            {
                return NotFound();
            }
            return Ok(ur);
        }

        public IHttpActionResult GetUsuario(int id)
        {
            var usuarioRol = usuarioRolService.GetAll(c => c.RL_ID_ROL == id);

            if (usuarioRol != null)
            {
                return Ok(usuarioRol);
            }

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] UsuarioRolModel model)
        {
            return Ok(usuarioRolService.Insert(model, true));
        }

        // PUT: api/User/5
        public IHttpActionResult Put([FromBody] UsuarioRolModel model)
        {
            usuarioRolService.Update(model);
            return Ok();
        }
    }
}

