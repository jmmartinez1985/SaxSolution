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
    [RoutePrefix("api/ModuloRol")]
    public class ModuloRolController : ApiController
    {
        private readonly IModuloRolService moduloRolService;
        private ApplicationUserManager _userManager;

        public ModuloRolController(IModuloRolService mr)
        {
            moduloRolService = mr;
        }

        public ModuloRolController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        /*

        [Route("ManageModeloInRol"),HttpPost]
        public IHttpActionResult Post([FromBody] ModuloInRole model)
        {
            var userId = User.Identity.GetUserId();
            model.CreateRomevModuloRolModel.All(c => { c.MR_USUARIO_CREACION = userId; c.MR_USUARIO_MOD = userId;  c.MR_FECHA_CREACION = DateTime.Now; c.MR_FECHA_MOD = DateTime.Now; return true; });
            moduloRolService.CreateAndRemove(model.CreateRomevModuloRolModel);
            return Ok();
        }
        */

        [Route("CreateModuloForRole")]
        public async Task<IHttpActionResult> CreateModuloForRole([FromBody] ModuloInRole model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var currentModuloRol = moduloRolService.GetAll(c => c.RL_ID_ROL == model.id, null, includes: c => c.SAX_MODULO);
            var denoms = new List<int>(currentModuloRol.Select(c => c.MR_ID_MODULO_ROL));
            List<ModuloRolModel> objInsert = new List<ModuloRolModel>();
            foreach (var obj in model.EnrolledModulos)
            {
                obj.MR_ESTATUS = 1;
                obj.MR_FECHA_CREACION = DateTime.Now;
                obj.MR_USUARIO_CREACION = user.Id;
                obj.RL_ID_ROL = model.id;
                objInsert.Add(obj);
            }
            moduloRolService.CreateAndRemove(model.EnrolledModulos, denoms);
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
