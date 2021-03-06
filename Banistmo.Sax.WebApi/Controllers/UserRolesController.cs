﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/UserRoles")]
    public class UserRolesController : ApiController
    {
        private  IAspNetUserRolesService objInj;

        public UserRolesController()
        {
            objInj = objInj ?? new AspNetUserRolesService();
        }

        public UserRolesController(IAspNetUserRolesService ue)
        {
            objInj = ue;
        }

        public IHttpActionResult Get()
        {
            var ue = objInj.GetAll(null, null, includes:  c => c.AspNetRoles );
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
        public IHttpActionResult GetRolesByUser(String id)
        {
            var rolesUsuarios = objInj.GetAll(c => c.UserId == id, null, includes: c => c.AspNetRoles);
            if (rolesUsuarios == null)
            {
                return null;
            }
            List<AspNetRolesModel> listRoles = new List<AspNetRolesModel>();

            foreach (var rol in rolesUsuarios.ToList())
            {
                //AspNetRolesModel listModel = Mapping(rol.AspNetRoles);
                //listRoles.Add(listModel);
            }

            return Ok(listRoles.Where(c => c.Estatus != 2).Select(c =>  new
            {
                Id = c.Id,
                Name = c.Name,
                Estatus = c.Estatus
            }));

        }
        

        [Route("CreateRolesForUser")]
        public IHttpActionResult CreateRolesForUser([FromBody] RolesToUser model)
        {
            objInj = objInj ?? new AspNetUserRolesService();
            var currentAreas = objInj.GetAll(c => c.UserId == model.id, null, includes: c => c.AspNetRoles);
            var denoms = new List<int>(currentAreas.Select(c => c.IDAspNetUserRol));
            objInj.CreateAndRemove(model.EnrolledRoles, denoms);
            return Ok();
        }

        private AspNetRolesModel Mapping(AspNetRoles rol)
        {
            AspNetRolesModel model = new AspNetRolesModel();
            model.Description = rol.Description;
            model.Estatus = rol.Estatus;
            model.Id = rol.Id;
            model.Name = rol.Name;

            return model;
        }
    }

    //RolesToUser
}
