using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Banistmo.Sax.WebApi.Models;

using Microsoft.AspNet.Identity.Owin;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {

        private readonly IUserService userService;
        private readonly IReporteService reporteSrv;
        private readonly IReporteRolesMenuService rrmService;

        private readonly IUsuarioAreaService usuarioAreaService;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;
        //private readonly IModuloRolService moduloRolService;
        private readonly ICatalogoService catalagoService;
        private readonly ApplicationRoleManager _appRoleManager;

        public UserController(IUserService usr, IReporteService reporte, IReporteRolesMenuService rrmSrv, IUsuarioAreaService usrAreaSrv, IUsuarioEmpresaService usrEmpSrv, ICatalogoService catSrv)
        {
            userService = usr;
            reporteSrv = reporte;
            rrmService = rrmSrv;
            usuarioAreaService = usrAreaSrv;
            usuarioEmpresaService = usrEmpSrv;
            catalagoService = catSrv;
        }

        public UserController(ApplicationRoleManager appRoleManager)
        {
            _appRoleManager = appRoleManager;
        }

        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _appRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        // GET: api/User
        public IHttpActionResult Get()
        {
            List<AspNetUserModel> user = userService.GetAll();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/User/5
        public IHttpActionResult GetUsuario(string id)
        {
            var usuario = userService.GetSingle(c => c.Id == id);

            if (usuario != null)
            {
                return Ok(usuario);
            }
            return NotFound();
        }

        /*
        
            "Result": {
        "Id": "0ed76381-437c-45d2-88bf-0d609d7e4edc",
        "FirstName": "Celia",
        "LastName": "Arias",
        "Level": 1,
        "JoinDate": "2018-03-12T16:39:51.743",
        "Email": "celia.x.arias@banistmo.com",
        "EmailConfirmed": true,
        "PasswordHash": "AKilhtDu3X+44Sgah/ArEvQY1BGfXKF1+yhi+GvsVcJX2VwsZZ9ncNU5xSELyZikjQ==",
        "SecurityStamp": "2ffb0ea2-4842-4b35-b0ea-86418ec29f18",
        "PhoneNumber": "256-6123",
        "PhoneNumberConfirmed": false,
        "TwoFactorEnabled": false,
        "LockoutEndDateUtc": "2018-03-12T16:39:51.743",
        "LockoutEnabled": false,
        "AccessFailedCount": 0,
        "UserName": "43577092"
    }

            */
        [Route("{id:guid}")]
        //[Route("GetUserInfoByID")]
        public IHttpActionResult GetUserInfoByID(String id)
        {
            List<ExistingRole> listExistingRoles = new List<ExistingRole>();
            List<UsuarioAreaModel> listUsuarioArea = new List<UsuarioAreaModel>();
            List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
            //List<ModuloRolModel> listModuloRol = new List<ModuloRolModel>();
            List<ApplicationRole> listRoles = new List<ApplicationRole>();
            UserAttributes attributes = new UserAttributes();
            var user = userService.GetSingle(c => c.Id == id);

            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE);

            if (user == null)
            {
                return null;
            }

            var listAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_AREA_OPERATIVA);
            if (listAreas.Count > 0)
            {
                foreach (var area in listAreas)
                {
                    listUsuarioArea.Add(area);
                }
            }
            var listEmpresas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, c => c.SAX_EMPRESA);
            if (listEmpresas.Count > 0)
            {
                foreach (var emp in listEmpresas)
                {
                    listUsuarioEmpresas.Add(emp);
                }
            }
            List<ExistingRole> existingRoles = new List<ExistingRole>();
            var roles = RoleManager.Roles;
            foreach (var role in roles)
            {
                var casting = role as ApplicationRole;
                if (casting.Estatus != 2)
                    existingRoles.Add(new ExistingRole { Id = casting.Id, Name = casting.Name, Description = casting.Description, Estatus = casting.Estatus });
            }
            return Ok(new
            {

                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = existingRoles.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_VALOR

                }),
                Areas = listUsuarioArea.Select(c => new
                {
                    Id = c.SAX_AREA_OPERATIVA.CA_COD_AREA,
                    Name = c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_VALOR
                }),
                Empresas = listUsuarioEmpresas.Select(c => new
                {
                    Id = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    Name = c.SAX_EMPRESA.CE_NOMBRE,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_VALOR
                })
                /*,
                Modulos = listModuloRol.Select(c => new
                {
                    Id = c.SAX_MODULO.MO_ID_MODULO,
                    Name = c.SAX_MODULO.MO_MODULO,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_VALOR
                })*/
            });
        }

        [Route("{id:int}")]
        //[Route("GetUserInfoByUserName")]
        public IHttpActionResult GetUserInfoByUserName(int id)
        {
            List<ExistingRole> listExistingRoles = new List<ExistingRole>();
            List<UsuarioAreaModel> listUsuarioArea = new List<UsuarioAreaModel>();
            List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
            //List<ModuloRolModel> listModuloRol = new List<ModuloRolModel>();
            List<ApplicationRole> listRoles = new List<ApplicationRole>();
            UserAttributes attributes = new UserAttributes();
            var user = userService.GetSingle(c => c.UserName == id.ToString());

            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE);

            if (user == null)
            {
                return null;
            }

            var listAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_AREA_OPERATIVA);
            if (listAreas.Count > 0)
            {
                foreach (var area in listAreas)
                {
                    listUsuarioArea.Add(area);
                }
            }
            var listEmpresas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, c => c.SAX_EMPRESA);
            if (listEmpresas.Count > 0)
            {
                foreach (var emp in listEmpresas)
                {
                    listUsuarioEmpresas.Add(emp);
                }
            }
            List<ExistingRole> existingRoles = new List<ExistingRole>();
            var roles = RoleManager.Roles;
            foreach (var role in roles)
            {
                var casting = role as ApplicationRole;
                if (casting.Estatus != 2)
                    existingRoles.Add(new ExistingRole { Id = casting.Id, Name = casting.Name, Description = casting.Description, Estatus = casting.Estatus });
            }
            return Ok(new
            {

                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = existingRoles.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.Estatus).CD_VALOR

                }),
                Areas = listUsuarioArea.Select(c => new
                {
                    Id = c.SAX_AREA_OPERATIVA.CA_COD_AREA,
                    Name = c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_VALOR
                }),
                Empresas = listUsuarioEmpresas.Select(c => new
                {
                    Id = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    Name = c.SAX_EMPRESA.CE_NOMBRE,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UE_ESTATUS).CD_VALOR
                })
                /*,
                Modulos = listModuloRol.Select(c => new
                {
                    Id = c.SAX_MODULO.MO_ID_MODULO,
                    Name = c.SAX_MODULO.MO_MODULO,
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.MR_ESTATUS).CD_VALOR
                })*/
            });
        }


        public IHttpActionResult Put([FromBody] AspNetUserModel model)
        {
            userService.Update(model);
            return Ok();
        }

        [Route("ReporterUser"), HttpGet]
        public IHttpActionResult GetDataReporterUser()
        {
            return Ok(reporteSrv.GetReporte());
        }

        [Route("ReporteRolesMenu"), HttpGet]
        public IHttpActionResult ReporteRolesMenu()
        {
            return Ok(rrmService.GetReporte());
        }

    }
}
