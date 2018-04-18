using Banistmo.Sax.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;


namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Role")]
    public class RoleController : ApiController
    {


        private readonly ApplicationRoleManager _appRoleManager;
        private ApplicationUserManager _userManager;
        private readonly IAspNetUserRolesService objInj;
        private readonly IUserService userService;

        //private readonly IRolesService _rolesService;


        public RoleController()
        {
        }
        public RoleController(ApplicationUserManager userManager, ApplicationRoleManager appRoleManager)
        {
            _appRoleManager = appRoleManager;
            _userManager = userManager;
            //_rolesService = roleService;
        }
        public RoleController(IAspNetUserRolesService serv, IUserService userv)
        {
            objInj = serv;
            userService = userv;
        }
        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _appRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
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
        public IHttpActionResult Get()
        {
            List<ExistingRole> existingRoles = new List<ExistingRole>();
            var roles = RoleManager.Roles;
            foreach (var role in roles)
            {
                var casting = role as ApplicationRole;
                if (casting.Estatus != 2)
                    existingRoles.Add(new ExistingRole { Id = casting.Id, Name = casting.Name, Description = casting.Description, Estatus = casting.Estatus });
            }
            return Ok(existingRoles);
        }
        [Route("GetRolesById")]
        public async Task<IHttpActionResult> GetRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Ok(role);
            }
            return NotFound();

        }
        [Route("GetModuloByRole")]
        public async Task<IHttpActionResult> GetModuloByRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Ok(role);
            }
            return NotFound();

        }
        [Route("Create")]
        public async Task<IHttpActionResult> Create(CreateRoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new ApplicationRole { Name = model.Name, Description = model.Description, Estatus = 1 };

            var result = await RoleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return BadRequest("Error al crear el Rol, verifique si ya exista en la base de datos");
            }
            //Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Ok(role);

        }
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string Id)
        {
            var role = await RoleManager.FindByIdAsync(Id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    return InternalServerError();
                }
                return Ok();
            }
            return NotFound();
        }
        [Route("DeleteRole")]
        public async Task<IHttpActionResult> SoftDeleteRole(string Id)
        {
            var role = await RoleManager.FindByIdAsync(Id);
            if (role != null)
            {
                ApplicationRole rol = ((ApplicationRole)role);
                rol.Estatus = 0;
                IdentityResult result = await RoleManager.UpdateAsync(rol);
                if (result.Succeeded)
                {
                    var currentRoles = objInj.GetAll(c => c.RoleId == Id, null, includes: c => c.AspNetRoles);
                    var denoms = new List<int>(currentRoles.Select(c => c.IDAspNetUserRol));
                    objInj.CreateAndRemove(null, denoms);
                }
                else
                {
                    return InternalServerError();
                }
                return Ok();
            }
            return NotFound();
        }
        [Route("ManageUsersInRole")]
        public async Task<IHttpActionResult> ManageUsersInRole(UsersInRoleModel model)
        {
            var role = await RoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            foreach (string user in model.EnrolledUsers)
            {
                var appUser = await UserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                    continue;
                }

                if (!UserManager.IsInRole(user, role.Name))
                {
                    IdentityResult result = await UserManager.AddToRoleAsync(user, role.Name);

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", String.Format("User: {0} could not be added to role", user));
                    }

                }
            }

            foreach (string user in model.RemovedUsers)
            {
                var appUser = await UserManager.FindByIdAsync(user);

                if (appUser == null)
                {
                    ModelState.AddModelError("", String.Format("User: {0} does not exists", user));
                    continue;
                }

                IdentityResult result = await UserManager.RemoveFromRoleAsync(user, role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", String.Format("User: {0} could not be removed from role", user));
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }
        [Route("UpdateRole"), HttpPost]
        public async Task<IHttpActionResult> Put([FromBody] EditRoleBindingModel model)
        {
            var role = await RoleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return BadRequest(ModelState);
            }

            if (model.Estatus == 2)
            {
                List<AspNetUserRolesModel> usrRoles = null;
                try
                {
                    // Se valida que ningun usuario tenga el rol que se va a eliminar
                    usrRoles = objInj.GetAll(c => c.RoleId == model.Id);
                }
                catch
                {
                    //Si falla el query es por que no hay coincidencia, pero se puede continuar
                    usrRoles = null;
                }

                var usrActived = userService.GetAll(c => c.Estatus != 2);
                if (usrRoles != null & usrActived != null)
                {
                    foreach (var usrInRole in usrRoles)
                    {
                        foreach (var usr in usrActived)
                        {
                            if (usr.Id == usrInRole.UserId)
                            {
                                return BadRequest("El rol que desea eliminar está asociado con otros usuarios. No se puede eliminar el rol.");
                            }
                        }
                    }
                }
            }

            var updateRol = (ApplicationRole)role;
            updateRol.Description = model.Description;
            updateRol.Estatus = model.Estatus;
            updateRol.Name = model.Name;

            IdentityResult result = await RoleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                return BadRequest("Error al actualizar el Rol, verifique que no exista en la base de datos.");
                //return BadRequest(((string[])result.Errors)[0]) ;
            }
            return Ok();
        }
    }
}
