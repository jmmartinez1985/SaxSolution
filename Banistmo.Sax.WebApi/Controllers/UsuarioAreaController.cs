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
            var denoms = new List<int>(model.RemovedUsers.Select(c=>c.CA_COD_AREA));
            var remover = new List<UsuarioAreaModel>();
            foreach (var item in denoms)
            {
                var index = usuarioAreaService.GetSingle(c=> c.CA_ID_AREA == item);
                if(null != index)
                    remover.Add(index);
            }
            usuarioAreaService.CreateAndRemove(model.EnrolledUsers, denoms);
            return Ok();
        }

    }
}
