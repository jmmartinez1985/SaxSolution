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

        [Route("GetEmpresasByUser")]
        public IHttpActionResult GetEmpresasByUser(string id)
        {
            var usuarioEmpresa = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == id, null, includes: c => c.SAX_EMPRESA);
            if (usuarioEmpresa == null)
            {
                return null;
            }
            List<EmpresaModel> listEmp = new List<EmpresaModel>();

            foreach (var emp in usuarioEmpresa.ToList())
            {
                listEmp.Add(emp.SAX_EMPRESA);
            }

            return Ok(listEmp.Select(c => new
            {
                CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                CE_COD_EMPRESA = c.CE_COD_EMPRESA,
                CE_NOMBRE = c.CE_NOMBRE,
                CE_ESTATUS = c.CE_ESTATUS
            }));

        }

        [Route("ManageUsersInEmpresa")]
        public IHttpActionResult Post([FromBody] UsuariosInEmpresas model)
        {
            var denoms = new List<int>(model.RemovedUsers.Select(c => c.CE_ID_EMPRESA));
            usuarioEmpresaService.CreateAndRemove(model.EnrolledUsers, denoms);
            return Ok();
        }

        [Route("CreateEmpresaForUser")]
        public IHttpActionResult CreateEmpresaForUser([FromBody] EmpresasToUser model)
        {
            var currentAreas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == model.id, null, includes: c => c.SAX_EMPRESA);
            var denoms = new List<int>(currentAreas.Select(c => c.UE_ID_USUARIO_EMPRESA));
            usuarioEmpresaService.CreateAndRemove(model.EnrolledEmpresas, denoms);
            return Ok();
        }
    }
}
