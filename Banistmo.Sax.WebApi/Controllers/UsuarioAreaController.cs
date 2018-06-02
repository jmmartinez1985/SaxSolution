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
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/UsuarioArea")]
    public class UsuarioAreaController : ApiController
    {
        private readonly IUsuarioAreaService usuarioAreaService;
        private IAreaOperativaService areaOperativaService;
        private ApplicationUserManager _userManager;

        //public UsuarioAreaController()
        //{
        //    usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
        //}

        public UsuarioAreaController(IUsuarioAreaService ua)
        {
            usuarioAreaService = ua;
            areaOperativaService =  new AreaOperativaService();
        }

        public UsuarioAreaController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        [Route("AreaOperativaPorLogin"), HttpGet]
        public IHttpActionResult GetAreaOperativaActiva()
        {
            string idUsuario=User.Identity.GetUserId();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<UsuarioAreaModel> listUsuarioArea = usuarioAreaService.GetAllFlatten<UsuarioAreaModel>(x=>x.US_ID_USUARIO == idUsuario && x.UA_ESTATUS== activo);
            List<AreaOperativaModel> ar = areaOperativaService.GetAllFlatten<AreaOperativaModel>(a => a.CA_ESTATUS == activo);
            if (listUsuarioArea == null)
            {
                return NotFound();
            }
            return Ok(listUsuarioArea.Select(c => new
            {
                CA_COD_AREA = c.CA_ID_AREA,
                CA_NOMBRE = GetNombreAreaOperativa(c.CA_ID_AREA, ref ar)
            }));
        }


        [Route("ManageUsersInArea")]
        public IHttpActionResult Post([FromBody] UsuariosInAreas model)
        {
            var denoms = new List<int>(model.RemovedUsers.Select(c=>c.CA_ID_AREA));
            usuarioAreaService.CreateAndRemove(model.EnrolledUsers, denoms);
            return Ok();
        }


        [Route("CreateAreaForUser")]
        public async Task<IHttpActionResult> CreateAreaForUser([FromBody] AreasToUser model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var currentAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == model.id, null, includes: c => c.SAX_AREA_OPERATIVA);
            var denoms = new List<int>(currentAreas.Select(c => c.UA_ID_USUARIO_AREA));
            List<UsuarioAreaModel> objInsert = new List<UsuarioAreaModel>();
            foreach (var obj in model.EnrolledAreas)
            {
                obj.UA_ESTATUS = 1;
                obj.UA_FECHA_CREACION = DateTime.Now;
                obj.UA_USUARIO_CREACION = user.Id;
                objInsert.Add(obj);
            }
            usuarioAreaService.CreateAndRemove(model.EnrolledAreas, denoms);
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

        private string GetNombreAreaOperativa(int idAreaOperativa, ref List<AreaOperativaModel> listAreaOperativa) {
            string result = string.Empty;
            if (listAreaOperativa != null && listAreaOperativa.Count > 0) {
                AreaOperativaModel objTmp = listAreaOperativa.Where(x => x.CA_ID_AREA == idAreaOperativa).FirstOrDefault();
                if (objTmp != null)
                    result = $"{objTmp.CA_COD_AREA}-{objTmp.CA_NOMBRE}";
            }
                return result;
        }

    }
}
