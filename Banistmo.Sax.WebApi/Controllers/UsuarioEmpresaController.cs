using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.WebApi.Providers;
using Banistmo.Sax.WebApi.Results;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System.Threading;
using System.Linq;
using System.Configuration;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/UsuarioEmpresa")]
    public class UsuarioEmpresaController : ApiController
    {
        private readonly IUsuarioEmpresaService usuarioEmpresaService;

        private ApplicationUserManager _userManager;

        public UsuarioEmpresaController(IUsuarioEmpresaService ue)
        {
            usuarioEmpresaService = ue;
        }

        public UsuarioEmpresaController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        public async Task<IHttpActionResult> CreateEmpresaForUser([FromBody] EmpresasToUser model)
        {

            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var currentAreas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == model.id, null, includes: c => c.SAX_EMPRESA);
            var denoms = new List<int>(currentAreas.Select(c => c.UE_ID_USUARIO_EMPRESA));
            //var listEmpresas =
            List<UsuarioEmpresaModel> objInsert = new List<UsuarioEmpresaModel>();
            foreach(var obj in model.EnrolledEmpresas)
            {
                obj.UE_ESTATUS = 1;
                obj.UE_FECHA_CREACION = DateTime.Now;
                obj.UE_USUARIO_CREACION = user.Id;
                objInsert.Add(obj);
            }
            usuarioEmpresaService.CreateAndRemove(objInsert, denoms);
            return Ok();
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}
