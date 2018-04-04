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
    [RoutePrefix("api/UserRoles")]
    public class UserRolesController : ApiController
    {
        private readonly IAspNetUserRolesService objInj;

        public UserRolesController(IAspNetUserRolesService ue)
        {
            objInj = ue;
        }

        public IHttpActionResult Get()
        {
            List<AspNetUserRolesModel> ue = objInj.GetAll();
            if (ue == null)
            {
                return NotFound();
            }
            return Ok(ue);
        }

        public IHttpActionResult GetRoles(String id)
        {
            var usuariosRoles = objInj.GetAll(c => c.UserId == id, null, includes:  c => c.AspNetRoles );

            if (usuariosRoles != null)
            {
                return Ok(usuariosRoles);
            }

            return NotFound();
        }

        [Route("GetRolesByUser")]
        public IHttpActionResult GetRolesByUser(string id)
        {
            var rolesUsuarios = objInj.GetAll(c => c.UserId == id, null, includes: c => c.AspNetRoles);
            if (rolesUsuarios == null)
            {
                return null;
            }
            List<AspNetRolesModel> listRoles = new List<AspNetRolesModel>();

            foreach (var rol in rolesUsuarios.ToList())
            {
                listRoles.Add(rol.AspNetRoles);
            }

            return Ok(listRoles.Where(c => c.Estatus != 2).Select(c =>  new
            {
                SAX_MODULO_ROL = c.SAX_MODULO_ROL,
                Id = c.Id,
                Name = c.Name,
                Estatus = c.Estatus
            }));

        }
        

        [Route("CreateRolesForUser")]
        public IHttpActionResult CreateEmpresaForUser([FromBody] RolesToUser model)
        {
            var currentAreas = objInj.GetAll(c => c.UserId == model.id, null, includes: c => c.AspNetRoles);
            var denoms = new List<int>(currentAreas.Select(c => c.IDAspNetUserRol));
            objInj.CreateAndRemove(model.EnrolledRoles, denoms);
            return Ok();
        }
    }

    //RolesToUser
}
