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
    [RoutePrefix("api/UsuarioEmpresa")]
    public class UsuarioEmpresaController : ApiController
    {
        private readonly IUsuarioEmpresaService usuarioEmpresaService;

        public UsuarioEmpresaController(IUsuarioEmpresaService ue)
        {
            usuarioEmpresaService = ue;
        }

        public IHttpActionResult Get()
        {
            List<UsuarioEmpresaModel> ue = usuarioEmpresaService.GetAll();
            if (ue == null)
            {
                return NotFound();
            }
            return Ok(ue);
        }

        public IHttpActionResult GetUsuarios(int id)
        {
            var usuarioEmpresa = usuarioEmpresaService.GetAll(c => c.CE_ID_EMPRESA == id, null, includes: c => c.SAX_EMPRESA);

            if (usuarioEmpresa != null)
            {
                return Ok(usuarioEmpresa);
            }

            return NotFound();
        }

        [Route("ManageUsersInEmpresa")]
        public IHttpActionResult Post([FromBody] UsuariosInEmpresas model)
        {
            var denoms = new List<int>(model.RemovedUsers.Select(c => c.CE_ID_EMPRESA));
            usuarioEmpresaService.CreateAndRemove(model.EnrolledUsers, denoms);
            return Ok();
        }
    }
}
