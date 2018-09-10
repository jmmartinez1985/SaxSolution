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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Banistmo.Sax.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Banistmo.Sax.Repository.Implementations.Business;
using System.Web.Configuration;
using System.Threading.Tasks;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {

        private readonly IUserService userService;
        private readonly IReporteService reporteSrv;
        private readonly IReporteRolesMenuService rrmService;
        private readonly IUsuariosPorRoleService usrRolService;
        private ApplicationUserManager _userManager;
        private readonly IUsuarioAreaService usuarioAreaService;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;
        //private readonly IModuloRolService moduloRolService;
        private readonly ICatalogoService catalagoService;
        private readonly ApplicationRoleManager _appRoleManager;
        private readonly ILDAP directorioactivo;
        private readonly IAspNetUserRolesService AspNetUserRolesService;

        private readonly ISPExecutor executorService;
        private IRolService rolService;

        private IAreaOperativaService areaOperativaService;

        public UserController(IUserService usr, IReporteService reporte, IReporteRolesMenuService rrmSrv, IUsuarioAreaService usrAreaSrv, IUsuarioEmpresaService usrEmpSrv,
            ICatalogoService catSrv, ILDAP dau, IUsuariosPorRoleService usrRol, IAspNetUserRolesService aspNetUserRolesServ, IAreaOperativaService area)
        {
            userService = usr;
            reporteSrv = reporte;
            rrmService = rrmSrv;
            usuarioAreaService = usrAreaSrv;
            usuarioEmpresaService = usrEmpSrv;
            catalagoService = catSrv;
            directorioactivo = dau;
            usrRolService = usrRol;
            AspNetUserRolesService = aspNetUserRolesServ;
            rolService = rolService ??  new RolService();
            areaOperativaService = area;

        }
        /// <summary>
        /// Este contructor se implemento, con un intento para resolver el problema del IIS
        /// en el servidor de banistmo 10.71.27.116
        /// </summary>
        public UserController()
        {
            userService = new UserService();
            reporteSrv = new ReporteService();
            rrmService = new ReporteRolesMenuService();
            usuarioAreaService = new UsuarioAreaService();
            usuarioEmpresaService = new UsuarioEmpresaService();
            catalagoService = new CatalogoService();
            directorioactivo = new LDAP();
            usrRolService = new UsuariosPorRolService();
            AspNetUserRolesService = new AspNetUserRolesService();
            rolService = rolService ?? new RolService();

            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
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

            List<AspNetUserModel> user = userService.GetAll(u => u.Estatus != 2);
            if (user == null)
            {
                return NotFound();
            }

            List<ApplicationUser> userList = new List<ApplicationUser>();

            foreach (var usr in user)
            {
                ApplicationUser usersToShow = new ApplicationUser();
                usersToShow.Email = ((AspNetUserModel)usr).Email;
                usersToShow.FirstName = ((AspNetUserModel)usr).FirstName;
                usersToShow.LastName = ((AspNetUserModel)usr).LastName;
                usersToShow.Id = ((AspNetUserModel)usr).Id;
                usersToShow.JoinDate = ((AspNetUserModel)usr).JoinDate;
                usersToShow.UserName = ((AspNetUserModel)usr).UserName;
                usersToShow.Estatus = ((AspNetUserModel)usr).Estatus;
                // usersToShow.EstatusDescrip = 
                userList.Add(usersToShow);
            }
            return Ok(userList.Select(c => new
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Id = c.Id,
                Email = c.Email,
                JoinDate = c.JoinDate,
                UserName = c.UserName,
                Estatus = c.Estatus,
                EstatusDesc = c.Estatus == 1 ? "Activo" : "Inactivo"
            }));
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
        [Route("GetUsuarioCapturadorParametro"), HttpGet]
        public IHttpActionResult GetUsuarioCapturador()
        {
            List<AspNetUserModel> listCapturador = new List<AspNetUserModel>();
            string  rolCapturador = WebConfigurationManager.AppSettings["capturador_parametros"];
            var rolCapturadorParametros = rolService.GetSingle(x => x.Name ==rolCapturador);
            if(rolCapturadorParametros==null)
                return BadRequest("No se encuentra el rol CAPTURADOR PARAMETRO");
            var objUsrRole = AspNetUserRolesService.GetAll(r=>r.RoleId== rolCapturadorParametros.Id, null, includes: c => c.AspNetUsers).Select(s=>s.UserId).ToList();
            if(objUsrRole!=null)
           listCapturador = userService.GetAll(x => objUsrRole.Contains(x.Id)).ToList();
            var lisUsr = userService.GetAll(c => c.Estatus == 1);
            
            if (listCapturador != null)
            {
                return Ok(listCapturador.Select(c => new
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserName = c.UserName,
                    Email = c.Email
                }));
            }
            return BadRequest("No se encontró ningún usuario con el rol de Capturador.");
        }

        [Route("GetUsuarioCapturador"), HttpGet]
        public IHttpActionResult GetUsuarioCapturadorParametro()
        {
            string vCapturador = "Capturador anulador";
            var rolUsuario = AspNetUserRolesService.Query(x => x.RoleId == "").Select(y => y.UserId);
            var objUsrRole = AspNetUserRolesService.GetAll(c => c.AspNetRoles.Name.ToLower()==vCapturador.ToLower(), null, includes: c => c.AspNetUsers);
            var lisUsr = userService.GetAll(c => c.Estatus == 1);
            List<AspNetUserModel> listCapturador = new List<AspNetUserModel>();
            foreach (var usrRole in objUsrRole)
            {
                foreach (AspNetUserModel usr in lisUsr)
                {
                    if (usrRole.UserId == usr.Id)
                    {
                        listCapturador.Add(usr);
                    }
                }
            }
            if (listCapturador != null)
            {
                return Ok(listCapturador.Select(c => new
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserName = c.UserName,
                    Email = c.Email
                }));
            }
            return BadRequest("No se encontró ningún usuario con el rol de Capturador.");
        }

        [Route("UserInformation"), HttpGet]
        public IHttpActionResult GetUsuario()
        {
            var id = User.Identity.GetUserId();
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

            //var res= executorService.GetUsuarioPorRol();

            /*
            List<ExistingRole> existingRoles = new List<ExistingRole>();
            var roles = RoleManager.Roles;
            foreach (var role in roles)
            {
                var casting = role as ApplicationRole;
                if (casting.Estatus != 2)
                    existingRoles.Add(new ExistingRole { Id = casting.Id, Name = casting.Name, Description = casting.Description, Estatus = casting.Estatus });
            }
            */

            List<ExistingRole> existingRoles = new List<ExistingRole>();
            var rolesPorUsuario = AspNetUserRolesService.GetAll(c => c.UserId == id, null,
                c => c.AspNetRoles,
                c => c.AspNetUsers);
            foreach (var rol in rolesPorUsuario)
            {
                existingRoles.Add(new ExistingRole { Id = rol.AspNetRoles.Id, Name = rol.AspNetRoles.Name, Description = rol.AspNetRoles.Description, Estatus = rol.AspNetRoles.Estatus });
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
                    Id = c.SAX_AREA_OPERATIVA.CA_ID_AREA,
                    Name = c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                    CA_COD_AREA = c.SAX_AREA_OPERATIVA != null ? c.SAX_AREA_OPERATIVA.CA_COD_AREA : 0
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_VALOR
                }),
                Empresas = listUsuarioEmpresas.Select(c => new
                {
                    Id = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    Name = c.SAX_EMPRESA.CE_NOMBRE,
                    CE_COD_EMPRESA = c.SAX_EMPRESA != null ? c.SAX_EMPRESA.CE_COD_EMPRESA : string.Empty
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
                    CA_COD_AREA = c.SAX_AREA_OPERATIVA != null ? c.SAX_AREA_OPERATIVA.CA_COD_AREA : 0
                    //IdEstatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_ESTATUS,
                    //Estatus = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.UA_ESTATUS).CD_VALOR
                }),
                Empresas = listUsuarioEmpresas.Select(c => new
                {
                    Id = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    Name = c.SAX_EMPRESA.CE_NOMBRE,
                    CE_COD_EMPRESA = c.SAX_EMPRESA != null ? c.SAX_EMPRESA.CE_COD_EMPRESA : string.Empty
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


        [Route("UpdateUser"), HttpPost]
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

        // [Route("UserValidation"), HttpPut]
        [Route("UserToValidate"), HttpGet]
        public IHttpActionResult validationUser(string UserToValidate)
        {
            try
            {
                //CODIGO PARA OBTENER EL USUARIO Y CONTRASEÑA DEL DIRECTORIO ACTIVO DESDE EL WEBCONFIG
                var a = directorioactivo.validaUsuarioLDAP(Properties.Settings.Default.userServiceDA, Properties.Settings.Default.passwordServiceDA, Properties.Settings.Default.loginIntranet, Properties.Settings.Default.dominioDa, UserToValidate);
                //var a = new { userNumber = "afmosqu",  nombreCompleto = "Anthony Mosquera", existe = true, error = "", mail = "anthony.mosquera@banistmo.com" };                    
                return Ok(a);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class userparameter
        {
            //public string userGSI { get; set; }
            //public string passwordGSI { get; set; }
            public string UserToValidate { get; set; }

        }

        [Route("UsuariosPorRol"), HttpGet]
        public IHttpActionResult UsuariosPorRol(String id)
        {
            var obj = usrRolService.GetReporte();
            return Ok(obj.Where(c => c.ROLEID == id).Select(c => new
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                UserName = c.UserName,
                Email = c.Email,
                JoinDate = c.JoinDate,
                ROLEID = c.ROLEID
            }));
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

        [Route("GetUsuarioCapturadorByArea"), HttpGet]
        public async Task<IHttpActionResult> GetUsuarioCapturadorOD()
        {

            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
            //var userAreacod = new List<AreaOperativaModel>();
            //foreach (var item in userArea)
            //{
            //    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
               

            //}

            string vCapturador = "Capturador anulador";
            string vConciliador = "Conciliador";
            var rolUsuario = AspNetUserRolesService.Query(x => x.RoleId == "").Select(y => y.UserId);
            var objUsrRole = AspNetUserRolesService.GetAll(c => c.AspNetRoles.Name.ToLower() == vCapturador.ToLower() || c.AspNetRoles.Name.ToLower() == vConciliador.ToLower(), null, includes: c => c.AspNetUsers);
            var lisUsr = userService.GetAll(x => x.Estatus == 1, null, includes: a => a.SAX_AREA_OPERATIVA).ToList();
                     

            List < AspNetUserModel > listCapturador = new List<AspNetUserModel>();
            foreach (var usrRole in objUsrRole)
            {
                foreach (AspNetUserModel usr in lisUsr)
                {
                    if (usrRole.UserId == usr.Id)
                    {
                        listCapturador.Add(usr);
                    }

                }
            }

            List<AspNetUserModel> listCapturadorbyArea = new List<AspNetUserModel>();
           
                foreach(var f in listCapturador)
                {
                foreach (var g in userArea)
                {
                    var userAreacod = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == f.Id && d.UA_ESTATUS == 1);

                    foreach (var v in userAreacod)
                    {
                        if (v.CA_ID_AREA == g.CA_ID_AREA)
                        {
                            listCapturadorbyArea.Add(f);
                        }
                    }

                }
            }
            if (listCapturadorbyArea != null)
            {
                return Ok(listCapturadorbyArea.Select(c => new
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserName = c.UserName,
                    Email = c.Email
                }));
            }
            return BadRequest("No se encontró ningún usuario con el rol de Capturador.");
        }
    }
}
