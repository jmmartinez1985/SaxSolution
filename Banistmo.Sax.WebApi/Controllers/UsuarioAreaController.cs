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

        [Route("{id:int}", Name = "GetUsuarioAreaById")]
        public IHttpActionResult GetUsuario(int id)
        {
            var usuarioArea = usuarioAreaService.GetAll(c => c.CA_COD_AREA == id);

            if (usuarioArea != null)
            {
                return Ok(usuarioArea);
            }

            return NotFound();
        }

    }
}
