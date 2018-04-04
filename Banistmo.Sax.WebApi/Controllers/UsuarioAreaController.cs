using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/UsuarioArea")]
    public class UsuarioAreaController : ApiController
    {
        private readonly IUsuarioAreaService usuarioAreaService;

        public UsuarioAreaController(IUsuarioAreaService ua)
        {
            usuarioAreaService = ua;
        }

        public IHttpActionResult Get()
        {
            List<UsuarioAreaModel> ua = usuarioAreaService.GetAll();
            if (ua == null)
            {
                return NotFound();
            }
            return Ok(ua);
        }
        public IHttpActionResult GetUsuario(int id)
        {
            var usuarioArea = usuarioAreaService.GetAll(c => c.CA_ID_AREA == id);

            if (usuarioArea != null)
            {
                return Ok(usuarioArea);
            }

            return NotFound();
        }


        [Route("ManageUsersInArea")]
        public IHttpActionResult Post([FromBody] UsuariosInAreas model)
        {
            var denoms = new List<int>(model.RemovedUsers.Select(c=>c.CA_ID_AREA));
            usuarioAreaService.CreateAndRemove(model.EnrolledUsers, denoms);
            return Ok();
        }


        [Route("CreateAreaForUser")]
        public IHttpActionResult CreateAreaForUser([FromBody] AreasToUser model)
        {
            var currentAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == model.id, null, includes: c => c.SAX_AREA_OPERATIVA);
            var denoms = new List<int>(currentAreas.Select(c => c.UA_ID_USUARIO_AREA));
            usuarioAreaService.CreateAndRemove(model.EnrolledAreas, denoms);
            return Ok();
        }

    }
}
