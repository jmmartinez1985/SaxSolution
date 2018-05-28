using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Model;
using System.Globalization;


namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Supervisor")]
    public class SupervisorController : ApiController
    {
        //Variables
        private readonly ISupervisorService supervisorService;
        private readonly ISupervisorTempService supervisorTempService;
        private ApplicationUserManager _userManager;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;
        private readonly IEmpresaAreasCentroCostoService empresaAreaCentroCostoService;
        private readonly IUsuariosPorRoleService usuarioRol;
        private readonly IAspNetUserRolesService objInj;
        private readonly ApplicationRoleManager _appRoleManager;
        private readonly IUserService userService;
        private readonly IUsuarioAreaService usuarioAreaService;

        //Constructores
        public SupervisorController()
        {
            supervisorService = supervisorService ?? new SupervisorService();
            supervisorTempService = supervisorTempService ?? new SupervisorTempService();
        }
        public SupervisorController(ISupervisorService objSupervisorService, ISupervisorTempService objSupervisorTempService, IUsuarioEmpresaService objUsuarioAreaService, IEmpresaAreasCentroCostoService eacc, IUsuariosPorRoleService usrRol, IAspNetUserRolesService ue, IUserService userServ, IUsuarioAreaService uas)
        {
            objInj = ue;
            supervisorService = objSupervisorService;
            supervisorTempService = objSupervisorTempService;
            usuarioEmpresaService = objUsuarioAreaService;
            empresaAreaCentroCostoService = eacc;
            usuarioRol = usrRol;
            userService = userServ;
            usuarioAreaService = uas;
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
        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _appRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        //Metodos
        public async Task<IHttpActionResult> Get([FromUri] AprobacionParametrosModel model)
        {
            
            if (model == null)
            {
                model = new AprobacionParametrosModel();
                model.FechaCreacion = null;
                model.UsuarioCreacion = null;
            }
            /*int yyyy = 0;
            int mm = 0;
            int dd = 0;
            DateTime dt = DateTime.Today;
            if (model.FechaCreacion != null)
            {
                mm = Convert.ToInt32(model.FechaCreacion.ToString().Substring(0, 2));
                dd = Convert.ToInt32(model.FechaCreacion.ToString().Substring(3, 2));
                yyyy = Convert.ToInt32(model.FechaCreacion.ToString().Substring(6, 4));
                dt = new DateTime(yyyy, mm, dd);
                dt = dt.AddDays(1);
            }*/
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var objUsuarioArea = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_EMPRESA);
            string[] listEmpresa = new string[objUsuarioArea.Count()];
            for (int i = 0; i < objUsuarioArea.Count(); i++)
            {
                listEmpresa[i] = objUsuarioArea[i].CE_ID_EMPRESA.ToString();
            }

            IList<SupervisorModel> objSupervisorService = supervisorService.GetAll(
            //c => c.CE_ID_EMPRESA == objUsuarioArea.CE_ID_EMPRESAf
            c => listEmpresa.Contains(c.CE_ID_EMPRESA.ToString())
            && c.SV_ESTATUS != 3
            //&& c.SV_FECHA_CREACION >= (model.FechaCreacion == null ? c.SV_FECHA_CREACION : model.FechaCreacion)
            //&& c.SV_FECHA_CREACION <= (model.FechaCreacion == null ? c.SV_FECHA_CREACION : dt)
            && c.SV_FECHA_CREACION == (model.FechaCreacion == null ? c.SV_FECHA_CREACION : model.FechaCreacion)
            && c.AspNetUsers3.Estatus == 1
            && c.SV_USUARIO_CREACION == (model.UsuarioCreacion == null ? c.SV_USUARIO_CREACION : model.UsuarioCreacion), null, includes: c => c.SAX_AREA_OPERATIVA);
            if (objSupervisorService == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(objSupervisorService.Select(c => new
            {
                SV_ID_SUPERVISOR = c.SV_ID_SUPERVISOR,
                CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                CE_NOMBRE_EMPRESA = c.SAX_EMPRESA.CE_COD_EMPRESA + '-' + c.SAX_EMPRESA.CE_NOMBRE,
                SV_COD_SUPERVISOR = c.SV_COD_SUPERVISOR,
                SV_NOMBRE_SUPERVISOR = c.AspNetUsers3.FirstName,
                SV_LIMITE_MINIMO = c.SV_LIMITE_MINIMO,
                SV_LIMITE_SUPERIOR = c.SV_LIMITE_SUPERIOR,
                SV_ESTATUS = c.SV_ESTATUS,
                SV_FECHA_CREACION = c.SV_FECHA_CREACION,
                SV_USUARIO_CREACION = c.SV_USUARIO_CREACION,
                SV_USUARIO_CREACION_NOMBRE = c.AspNetUsers1.FirstName,
                SV_FECHA_MOD = c.SV_FECHA_MOD,
                SV_USUARIO_MOD = c.SV_USUARIO_MOD,
                SV_FECHA_APROBACION = c.SV_FECHA_APROBACION,
                SV_FECHA_APROBACION_NOMBRE = c.AspNetUsers != null ? c.AspNetUsers.FirstName : null,
                SV_USUARIO_APROBADOR = c.SV_USUARIO_APROBADOR,
                SV_USUARIO_APROBADOR_NOMBRE = c.AspNetUsers2 != null ? c.AspNetUsers2.FirstName : null,
                SV_ID_AREA = c.SV_ID_AREA,
                SV_NOMBRE_AREA = c.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + c.SAX_AREA_OPERATIVA.CA_NOMBRE
            }));
        }
        public IHttpActionResult Get(int id)
        {
            var supervisor = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == id);

            if (supervisor != null)
            {
                return Ok(new
                {
                    SV_ID_SUPERVISOR = supervisor.SV_ID_SUPERVISOR,
                    CE_ID_EMPRESA = supervisor.CE_ID_EMPRESA,
                    CE_NOMBRE_EMPRESA = supervisor.SAX_EMPRESA.CE_COD_EMPRESA + '-' + supervisor.SAX_EMPRESA.CE_NOMBRE,
                    SV_COD_SUPERVISOR = supervisor.SV_COD_SUPERVISOR,
                    SV_NOMBRE_SUPERVISOR = supervisor.AspNetUsers3.FirstName,
                    SV_LIMITE_MINIMO = supervisor.SV_LIMITE_MINIMO,
                    SV_LIMITE_SUPERIOR = supervisor.SV_LIMITE_SUPERIOR,
                    SV_ESTATUS = supervisor.SV_ESTATUS,
                    SV_FECHA_CREACION = supervisor.SV_FECHA_CREACION,
                    SV_USUARIO_CREACION = supervisor.SV_USUARIO_CREACION,
                    SV_USUARIO_CREACION_NOMBRE = supervisor.AspNetUsers1.FirstName,
                    SV_FECHA_MOD = supervisor.SV_FECHA_MOD,
                    SV_USUARIO_MOD = supervisor.SV_USUARIO_MOD,
                    SV_FECHA_APROBACION = supervisor.SV_FECHA_APROBACION,
                    SV_FECHA_APROBACION_NOMBRE = supervisor.AspNetUsers != null ? supervisor.AspNetUsers.FirstName : null,
                    SV_USUARIO_APROBADOR = supervisor.SV_USUARIO_APROBADOR,
                    SV_USUARIO_APROBADOR_NOMBRE = supervisor.AspNetUsers2 != null ? supervisor.AspNetUsers2.FirstName : null,
                    SV_ID_AREA = supervisor.SV_ID_AREA,
                    SV_NOMBRE_AREA = supervisor.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + supervisor.SAX_AREA_OPERATIVA.CA_NOMBRE
                });
            }
            return BadRequest("No se encontraron registros para la consulta realizada.");
        }
        public async Task<IHttpActionResult> Post([FromBody] SupervisorModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            model.SV_USUARIO_CREACION = user.Id;
            model.SV_FECHA_CREACION = DateTime.Now.Date;
            int estadoAprobado = Convert.ToInt32(RegistryStateModel.RegistryState.Aprobado);

            var listSupervisor = supervisorService.GetAll(c => c.CE_ID_EMPRESA == model.CE_ID_EMPRESA
                                && c.SV_ID_AREA == model.SV_ID_AREA
                                && c.SV_ESTATUS == estadoAprobado
                                && c.SV_COD_SUPERVISOR == model.SV_COD_SUPERVISOR, null, includes: c => c.SAX_EMPRESA);
            if (listSupervisor != null & listSupervisor.Count > 0)
            {
                return BadRequest("No se puede registrar un supervisor dos veces en la misma empresa y área.");
            }
            var supervisor = supervisorService.InsertSupervisor(model);
            return Ok(supervisor);
        }
        [Route("UpdateSupervisor"), HttpPost]
        public async Task<IHttpActionResult> Put([FromBody] SupervisorModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            // Se obtiene el supervisor y se actualiza la fecha de modificación y el estatus
            var supervisor = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == model.SV_ID_SUPERVISOR);
            if (supervisor != null)
            {
                supervisor.SV_FECHA_MOD = DateTime.Now.Date;
                supervisor.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Pendiente);
                supervisor.SV_FECHA_MOD = DateTime.Now.Date;
                supervisor.SV_ID_AREA = model.SV_ID_AREA;
                supervisor.CE_ID_EMPRESA = model.CE_ID_EMPRESA;
                supervisor.SV_ID_SUPERVISOR = model.SV_ID_SUPERVISOR;
                supervisor.SV_LIMITE_MINIMO = model.SV_LIMITE_MINIMO;
                supervisor.SV_LIMITE_SUPERIOR = model.SV_LIMITE_SUPERIOR;

                //supervisorService.Update(supervisor);
                // Se obtiene el supervisor temporal para luego actualizarlo con el supervisor 
                var supervisorTemp = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == model.SV_ID_SUPERVISOR);
                supervisorTemp = MappingTempFromSupervisor(supervisorTemp, supervisor);
                supervisorTemp.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.PorAprobar);
                supervisorTempService.Update(supervisorTemp);
                return Ok();
            }
            else
                return NotFound();
        }
        public IHttpActionResult Delete(int id)
        {
            var supervisor = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == id);

            if (supervisor == null)
            {
                return NotFound();
            }

            supervisor.SV_FECHA_MOD = DateTime.Now.Date;
            supervisor.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Eliminado);
            supervisorService.Update(supervisor);
            return Ok();

        }
        [Route("AprobarSupervisor"), HttpPost]
        public async Task<IHttpActionResult> PutAprobarParametro([FromBody] AprobacionModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var tempModel = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == model.id);
            if (tempModel != null)
            {
                tempModel.SV_FECHA_APROBACION = DateTime.Now.Date;
                tempModel.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                tempModel.SV_USUARIO_APROBADOR = user.Id;
                supervisorTempService.Update(tempModel);
                SupervisorModel supervisor = new SupervisorModel();
                supervisor = MappingSupervisorFromTemp(supervisor, tempModel);

                supervisorService.Update(supervisor);
                return Ok("El supervisor ha sido aprobado.");
            }
            return BadRequest("No se encontraron datos para actualizar.");
        }
        [Route("RechazarSupervisor"), HttpPost]
        public async Task<IHttpActionResult> PutRechazarSupervisor([FromBody] AprobacionModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var supervisorModel = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == model.id);
            if (supervisorModel != null)
            {
                supervisorModel.SV_USUARIO_MOD = user.Id;
                supervisorModel.SV_FECHA_MOD = DateTime.Now.Date;

                supervisorModel.SV_FECHA_APROBACION = DateTime.Now.Date;
                supervisorModel.SV_USUARIO_APROBADOR = user.Id;

                if (supervisorModel.SV_ESTATUS == Convert.ToInt16(RegistryStateModel.RegistryState.Pendiente))
                    supervisorModel.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Eliminado);
                else
                    supervisorModel.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);

                //supervisorModel.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                supervisorService.Update(supervisorModel);
                var supervisorTempModel = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == model.id);
                supervisorTempModel = MappingTempFromSupervisor(supervisorTempModel, supervisorModel);

                supervisorTempService.Update(supervisorTempModel);
                return Ok("El supervisor ha sido rechazado.");
            }
            return BadRequest("No se encontraron datos para actualizar.");
        }
        [Route("GetTemp"), HttpGet]
        public async Task<IHttpActionResult> GetTemp([FromUri] AprobacionParametrosModel model)
        {

            try
            {
                if (model == null)
                {
                    model = new AprobacionParametrosModel();
                    model.FechaCreacion = null;
                    model.UsuarioCreacion = null;
                }
                /*fechaModel = model.FechaCreacion == null ? null : model.FechaCreacion.ToString();
                
                int yyyy = 0;
                int mm = 0;
                int dd = 0;
                */
                /*DateTime dt = DateTime.Today;

                DateTime dtND = DateTime.Today;

                
                String[] dateArray;
                string separator = "-";
               

                dt = DateTime.Parse(fechaModel);

                if (model.FechaCreacion != null)
                {
                  
                    if (model.FechaCreacion.ToString().Contains("/"))
                    {
                        separator = "/";
                    }
                    dateArray = model.FechaCreacion.ToString().Split(Convert.ToChar(separator));
                    mm = Convert.ToInt32(dateArray[0]);
                    dd = Convert.ToInt32(dateArray[1]);
                    yyyy = Convert.ToInt32(dateArray[2]);
                    
                    mm = Convert.ToInt32(model.FechaCreacion.ToString().Substring(0, 2));
                    dd = Convert.ToInt32(model.FechaCreacion.ToString().Substring(3, 2));
                    yyyy = Convert.ToInt32(model.FechaCreacion.ToString().Substring(6, 4));
                   
                    dt = new DateTime(yyyy, mm, dd); 
                   

                    dtND = dt.AddDays(1); 
                }
                */
         

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var objUsuarioArea = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_EMPRESA);
                string[] listEmpresa = new string[objUsuarioArea.Count()];
                for (int i = 0; i < objUsuarioArea.Count(); i++)
                {
                    listEmpresa[i] = objUsuarioArea[i].CE_ID_EMPRESA.ToString();
                }

                var objSupervisorTempService = supervisorTempService.GetAll(c => c.SV_ESTATUS == 2
                //&& c.CE_ID_EMPRESA == objUsuarioArea[0].CE_ID_EMPRESA
                && listEmpresa.Contains(c.CE_ID_EMPRESA.ToString())
                //&& c.SV_FECHA_CREACION >= (model.FechaCreacion == null ? c.SV_FECHA_CREACION : dt)
                //&& c.SV_FECHA_CREACION <= (model.FechaCreacion == null ? c.SV_FECHA_CREACION : dtND)
                && c.SV_FECHA_CREACION == (model.FechaCreacion == null ? c.SV_FECHA_CREACION : model.FechaCreacion)
                && c.SV_USUARIO_CREACION == (model.UsuarioCreacion == null ? c.SV_USUARIO_CREACION : model.UsuarioCreacion), null, includes: c => c.AspNetUsers);

                if (objSupervisorTempService == null)
                {
                    return BadRequest("No se encontraron registros para la consulta realizada.");
                }

                return Ok(objSupervisorTempService.Select(c => new
                {
                    SV_ID_SUPERVISOR = c.SV_ID_SUPERVISOR,
                    CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                    CE_NOMBRE_EMPRESA = c.SAX_EMPRESA.CE_COD_EMPRESA + '-' + c.SAX_EMPRESA.CE_NOMBRE,
                    SV_COD_SUPERVISOR = c.SV_COD_SUPERVISOR,
                    SV_NOMBRE_SUPERVISOR = c.AspNetUsers3.FirstName,
                    SV_LIMITE_MINIMO = c.SV_LIMITE_MINIMO,
                    SV_LIMITE_SUPERIOR = c.SV_LIMITE_SUPERIOR,
                    SV_ESTATUS = c.SV_ESTATUS,
                    SV_FECHA_CREACION = c.SV_FECHA_CREACION,
                    SV_USUARIO_CREACION = c.SV_USUARIO_CREACION,
                    SV_USUARIO_CREACION_NOMBRE = c.AspNetUsers1.FirstName,
                    SV_FECHA_MOD = c.SV_FECHA_MOD,
                    SV_USUARIO_MOD = c.SV_USUARIO_MOD,
                    SV_FECHA_APROBACION = c.SV_FECHA_APROBACION,
                    SV_FECHA_APROBACION_NOMBRE = c.AspNetUsers != null ? c.AspNetUsers.FirstName : null,
                    SV_USUARIO_APROBADOR = c.SV_USUARIO_APROBADOR,
                    SV_USUARIO_APROBADOR_NOMBRE = c.AspNetUsers2 != null ? c.AspNetUsers2.FirstName : null,
                    SV_ID_AREA = c.SV_ID_AREA,
                    SV_NOMBRE_AREA = c.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + c.SAX_AREA_OPERATIVA.CA_NOMBRE
                }));

            }
            catch (Exception ex)
            {
                return BadRequest("Data: " + model.FechaCreacion.ToString() + "Error: " + ex.ToString());
            }

        }
        [Route("GetTempById")]
        public IHttpActionResult GetTemp(int id)
        {
            var supervisorTemp = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == id);

            if (supervisorTemp != null)
            {
                return Ok(new
                {
                    SV_ID_SUPERVISOR = supervisorTemp.SV_ID_SUPERVISOR,
                    CE_ID_EMPRESA = supervisorTemp.CE_ID_EMPRESA,
                    CE_NOMBRE_EMPRESA = supervisorTemp.SAX_EMPRESA.CE_NOMBRE,
                    SV_COD_SUPERVISOR = supervisorTemp.SV_COD_SUPERVISOR,
                    SV_NOMBRE_SUPERVISOR = supervisorTemp.AspNetUsers3.FirstName,
                    SV_LIMITE_MINIMO = supervisorTemp.SV_LIMITE_MINIMO,
                    SV_LIMITE_SUPERIOR = supervisorTemp.SV_LIMITE_SUPERIOR,
                    SV_ESTATUS = supervisorTemp.SV_ESTATUS,
                    SV_FECHA_CREACION = supervisorTemp.SV_FECHA_CREACION,
                    SV_USUARIO_CREACION = supervisorTemp.SV_USUARIO_CREACION,
                    SV_USUARIO_CREACION_NOMBRE = supervisorTemp.AspNetUsers1.FirstName,
                    SV_FECHA_MOD = supervisorTemp.SV_FECHA_MOD,
                    SV_USUARIO_MOD = supervisorTemp.SV_USUARIO_MOD,
                    SV_FECHA_APROBACION = supervisorTemp.SV_FECHA_APROBACION,
                    SV_FECHA_APROBACION_NOMBRE = supervisorTemp.AspNetUsers != null ? supervisorTemp.AspNetUsers.FirstName : null,
                    SV_USUARIO_APROBADOR = supervisorTemp.SV_USUARIO_APROBADOR,
                    SV_USUARIO_APROBADOR_NOMBRE = supervisorTemp.AspNetUsers2 != null ? supervisorTemp.AspNetUsers2.FirstName : null,
                    SV_ID_AREA = supervisorTemp.SV_ID_AREA,
                    SV_NOMBRE_AREA = supervisorTemp.SAX_AREA_OPERATIVA.CA_NOMBRE

                });
            }
            return BadRequest("No se encontraron registros para la consulta realizada.");
        }
        [Route("ReporteSupervisor"), HttpGet]
        public IHttpActionResult GetReporte([FromUri] ReporteSupervisorModel model)
        {
            int estado = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);

            if (model == null)
            {
                model = new ReporteSupervisorModel();
                model.SV_ID_AREA = null;
                model.CE_ID_EMPRESA = null;
                model.SV_LIMITE_MINIMO = null;
                model.SV_LIMITE_SUPERIOR = null;
                model.UsuarioAprobador = null;
                model.SV_COD_SUPERVISOR = null;
            }
            else
            {
                if (model.SV_COD_SUPERVISOR != null && model.SV_COD_SUPERVISOR.ToUpper() == "UNDEFINED")
                    model.SV_COD_SUPERVISOR = null;
                if (model.SV_LIMITE_MINIMO != null && model.SV_LIMITE_MINIMO.ToUpper() == "UNDEFINED")
                    model.SV_LIMITE_MINIMO = null;
                if (model.SV_LIMITE_SUPERIOR != null && model.SV_LIMITE_SUPERIOR.ToUpper() == "UNDEFINED")
                    model.SV_LIMITE_SUPERIOR = null;
                try
                {
                    if (model.SV_ID_AREA != null && model.SV_ID_AREA.ToUpper() == "UNDEFINED")
                        model.SV_ID_AREA = null;
                    else
                    {
                        if (model.SV_ID_AREA == null)
                            model.Area = null;
                        else
                            model.Area = Convert.ToInt32(model.SV_ID_AREA);
                    }

                }
                catch
                {
                    model.Area = null;
                }

                try
                {
                    if (model.CE_ID_EMPRESA != null && model.CE_ID_EMPRESA.ToUpper() == "UNDEFINED")
                        model.CE_ID_EMPRESA = null;
                    else
                    {
                        if (model.CE_ID_EMPRESA == null)
                            model.Empresa = null;
                        else
                            model.Empresa = Convert.ToInt32(model.CE_ID_EMPRESA);
                    }
                }
                catch
                {
                    model.Empresa = null;
                }

            }

            IList<SupervisorModel> objSupervisorService
                = supervisorService.GetAll(f => //f.SV_ESTATUS == estado &&
                 f.SV_LIMITE_MINIMO == (model.SV_LIMITE_MINIMO == null ? f.SV_LIMITE_MINIMO : model.SV_LIMITE_MINIMO)
                && f.SV_LIMITE_SUPERIOR == (model.SV_LIMITE_SUPERIOR == null ? f.SV_LIMITE_SUPERIOR : model.SV_LIMITE_SUPERIOR)
                //&& f.SV_USUARIO_APROBADOR == (model.UsuarioAprobador == null ? f.SV_USUARIO_APROBADOR : model.UsuarioAprobador)
                && f.AspNetUsers3.Estatus == estado
                && f.SV_COD_SUPERVISOR == (model.SV_COD_SUPERVISOR == null ? f.SV_COD_SUPERVISOR : model.SV_COD_SUPERVISOR)
                && f.SV_ID_AREA == (model.SV_ID_AREA == null ? f.SV_ID_AREA : model.Area)
                && f.CE_ID_EMPRESA == (model.CE_ID_EMPRESA == null ? f.CE_ID_EMPRESA : model.Empresa),
                null, includes: c => c.SAX_AREA_OPERATIVA);
            if (objSupervisorService == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(objSupervisorService.Select(c => new
            {
                SV_ID_SUPERVISOR = c.SV_ID_SUPERVISOR,
                CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                CE_NOMBRE_EMPRESA = c.SAX_EMPRESA.CE_COD_EMPRESA + '-' + c.SAX_EMPRESA.CE_NOMBRE,
                SV_COD_SUPERVISOR = c.SV_COD_SUPERVISOR,
                SV_NOMBRE_SUPERVISOR = c.AspNetUsers3.FirstName,
                SV_LIMITE_MINIMO = c.SV_LIMITE_MINIMO,
                SV_LIMITE_SUPERIOR = c.SV_LIMITE_SUPERIOR,
                SV_ESTATUS = c.SV_ESTATUS,
                SV_FECHA_CREACION = c.SV_FECHA_CREACION,
                SV_USUARIO_CREACION = c.SV_USUARIO_CREACION,
                SV_USUARIO_CREACION_NOMBRE = c.AspNetUsers1.FirstName,
                SV_FECHA_MOD = c.SV_FECHA_MOD,
                SV_USUARIO_MOD = c.SV_USUARIO_MOD,
                SV_FECHA_APROBACION = c.SV_FECHA_APROBACION,
                SV_USUARIO_APROBADOR = c.SV_USUARIO_APROBADOR,
                SV_USUARIO_APROBADOR_NOMBRE = c.AspNetUsers != null ? c.AspNetUsers.FirstName : null,
                SV_ID_AREA = c.SV_ID_AREA,
                SV_NOMBRE_AREA = c.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                SV_ROL_SUPERVISOR = MappingRol(c.AspNetUsers3)
            }));
        }
        [Route("GetSupervisorID"), HttpGet]
        public async Task<IHttpActionResult> GetSupervisorID([FromUri] ReporteSupervisorModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var objUsuarioArea = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_EMPRESA);

            string[] listEmpresa = new string[objUsuarioArea.Count()];
            for (int i = 0; i < objUsuarioArea.Count(); i++)
            {
                listEmpresa[i] = objUsuarioArea[i].CE_ID_EMPRESA.ToString();
            }

            IList<SupervisorModel> objSupervisorService = supervisorService.GetAll(c => listEmpresa.Contains(c.CE_ID_EMPRESA.ToString()),
                                                                        null, includes: c => c.SAX_AREA_OPERATIVA);
            if (objSupervisorService != null)
            {
                return Ok(objSupervisorService.Select(c => new
                {
                    SV_COD_SUPERVISOR = c.SV_COD_SUPERVISOR,
                    SV_COD_SUPERVISOR_DESC = c.AspNetUsers3.FirstName
                }).Distinct());
            }
            //SV_COD_SUPERVISOR
            //SV_COD_SUPERVISOR_DESC
            return null;
        }
        [Route("GetEmpresa"), HttpGet]
        public async Task<IHttpActionResult> GetEmpresa()
        {
            /*
            var obj = empresaAreaCentroCostoService.GetAll();
            if (obj != null)
            {
                return Ok(obj.Select(c => new
                {
                    IdEmpresa = c.IdEmpresa,
                    EmpresaDesc = c.EmpresaDesc
                }).Distinct());
            }*/

            List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var listEmpresas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, c => c.SAX_EMPRESA);
            if (listEmpresas.Count > 0)
            {
                foreach (var emp in listEmpresas)
                {
                    listUsuarioEmpresas.Add(emp);
                }
            }
            if (listUsuarioEmpresas != null && listUsuarioEmpresas.Count > 0)
            {
                return Ok(listUsuarioEmpresas.Select(c => new
                {
                    IdEmpresa = c.SAX_EMPRESA.CE_ID_EMPRESA,
                    EmpresaDesc = c.SAX_EMPRESA.CE_ID_EMPRESA + "-" + c.SAX_EMPRESA.CE_NOMBRE
                }));
            }

            return null;
        }
        [Route("GetAreaByEmpresa"), HttpGet]
        public async Task<IHttpActionResult> GetAreaByEmpresa(int IdEmpresa)
        {
            /*
            var obj = empresaAreaCentroCostoService.GetAll(c => c.IdEmpresa == IdEmpresa);
            if (obj != null)
            {
                return Ok(obj.Select(c => new
                {
                    IdArea = c.IdArea,
                    AreaDesc = c.AreaDesc
                }).Distinct());
            }
            */

            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            List<UsuarioAreaModel> listUsuarioArea = new List<UsuarioAreaModel>();
            var listAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_AREA_OPERATIVA);
            if (listAreas.Count > 0)
            {
                foreach (var area in listAreas)
                {
                    listUsuarioArea.Add(area);
                }
            }
            if (listUsuarioArea != null && listUsuarioArea.Count() > 0)
            {
                return Ok(listUsuarioArea.Select(c => new
                {
                    IdArea = c.SAX_AREA_OPERATIVA.CA_ID_AREA,
                    AreaDesc = c.SAX_AREA_OPERATIVA.CA_ID_AREA + "-" + c.SAX_AREA_OPERATIVA.CA_NOMBRE
                }));
            }
            return null;
        }
        [Route("GetCentroCostoByAreaEmpresa"), HttpGet]
        public IHttpActionResult GetCentroCostoByAreaEmpresa(int IdEmpresa, int IdArea)
        {

            var obj = empresaAreaCentroCostoService.GetAll(c => c.IdEmpresa == IdEmpresa && c.IdArea == IdArea);
            if (obj != null)
            {
                return Ok(obj.Select(c => new
                {
                    IdCentroCosto = c.IdCentroCosto,
                    CentroCostoDesc = c.CentroCostoDesc
                }).Distinct());
            }

            return null;
        }
        [Route("GetSupervisorList"), HttpGet]
        public async Task<IHttpActionResult> GetSupervisorList(int IdEmpresa, int IdArea)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            //Se obtiene el ID del ROL del tipo de ROL Aprobador
            List<ExistingRole> existingRoles = new List<ExistingRole>();
            var roles = RoleManager.Roles;
            string roleAprobadorID = string.Empty;
            foreach (var role in roles)
            {
                var casting = role as ApplicationRole;
                if (casting.Name.Equals("APROBADOR"))
                    roleAprobadorID = casting.Id;
            }

            // Se obtiene la lista de todos los usuarios que tengan el ROL APROBADOR
            var objUsrRole = objInj.GetAll(c => c.AspNetRoles.Id == roleAprobadorID, null, includes: c => c.AspNetUsers);
            var lisUsr = userService.GetAll(c => c.Estatus == 1);
            List<AspNetUserModel> listAprobador = new List<AspNetUserModel>();
            foreach (var usrRole in objUsrRole)
            {
                foreach (AspNetUserModel usr in lisUsr)
                {
                    if (usrRole.UserId == usr.Id)
                    {
                        listAprobador.Add(usr);
                    }
                }
            }
            string[] listIdAprobador = new string[listAprobador.Count()];
            for (int i = 0; i < listAprobador.Count(); i++)
            {
                listIdAprobador[i] = listAprobador[i].Id;
            }

            //Se obtienen todas las empresas del usuario capturador
            var objUsuarioEmpresa = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_EMPRESA);
            string[] listEmpresa = new string[objUsuarioEmpresa.Count()];
            for (int i = 0; i < objUsuarioEmpresa.Count(); i++)
            {
                listEmpresa[i] = objUsuarioEmpresa[i].CE_ID_EMPRESA.ToString();
            }

            //Se obtienen todas las areas del usuario capturador
            var objUsuarioArea = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_AREA_OPERATIVA);
            string[] listArea = new string[objUsuarioArea.Count()];
            for (int i = 0; i < objUsuarioEmpresa.Count(); i++)
            {
                listEmpresa[i] = objUsuarioEmpresa[i].CE_ID_EMPRESA.ToString();
            }

            //Se obtiene todas las empresas por los APROBADORES
            var objUsuarioEmpresaAprobador = usuarioEmpresaService.GetAll(c => listIdAprobador.Contains(c.US_ID_USUARIO) && c.CE_ID_EMPRESA == IdEmpresa, null, includes: c => c.SAX_EMPRESA);
            //Se obtiene todas las areas por los APROBADORES
            var objUsuarioAreaAprobador = usuarioAreaService.GetAll(c => listIdAprobador.Contains(c.US_ID_USUARIO) && c.CA_ID_AREA == IdArea, null, includes: c => c.SAX_AREA_OPERATIVA);

            //Se crea la lista de definitiva de usuarios supervisores
            List<AspNetUserModel> listSupervisor = new List<AspNetUserModel>();
            // Se recorren las empresas y areas de los usuarios aprobadores y se comparan con las areas y empresas del usuario capturador
            for (int e = 0; e < objUsuarioEmpresaAprobador.Count; e++)
            {
                for (int a = 0; a < objUsuarioAreaAprobador.Count; a++)
                {
                    if (objUsuarioEmpresaAprobador[e].US_ID_USUARIO == objUsuarioAreaAprobador[a].US_ID_USUARIO)
                    {
                        for (int ec = 0; ec < objUsuarioEmpresa.Count; ec++)
                        {
                            for (int ac = 0; ac < objUsuarioArea.Count; ac++)
                            {
                                if (objUsuarioEmpresa[ec].US_ID_USUARIO == objUsuarioArea[ac].US_ID_USUARIO)
                                {
                                    if (objUsuarioEmpresa[ec].CE_ID_EMPRESA == objUsuarioEmpresaAprobador[e].CE_ID_EMPRESA && objUsuarioArea[ac].CA_ID_AREA == objUsuarioAreaAprobador[a].CA_ID_AREA)
                                    {
                                        listSupervisor.Add(objUsuarioEmpresaAprobador[e].AspNetUsers);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            if (listSupervisor != null && listSupervisor.Count() > 0)
            {
                return Ok(listSupervisor.Select(c => new
                {
                    //SV_COD_SUPERVISOR = c.Id,
                    //SV_COD_SUPERVISOR_DESC = c.FirstName
                    Id = c.Id,
                    FirstName = c.FirstName
                }).Distinct());
            }
            //SV_COD_SUPERVISOR
            //SV_COD_SUPERVISOR_DESC
            return BadRequest("No se encontraron supervisores que coincidan con su empresa y área");
        }

        //Mapping && Parser
        private SupervisorModel MappingSupervisorFromTemp(SupervisorTempModel supervisorTemp)
        {
            var supervisor = new SupervisorModel();

            supervisor.SV_ID_SUPERVISOR = supervisorTemp.SV_ID_SUPERVISOR;
            supervisor.SV_ID_AREA = supervisorTemp.SV_ID_AREA;
            supervisor.CE_ID_EMPRESA = supervisorTemp.CE_ID_EMPRESA;
            supervisor.SV_COD_SUPERVISOR = supervisorTemp.SV_COD_SUPERVISOR;
            supervisor.SV_LIMITE_MINIMO = supervisorTemp.SV_LIMITE_MINIMO;
            supervisor.SV_LIMITE_SUPERIOR = supervisorTemp.SV_LIMITE_SUPERIOR;
            supervisor.SV_ESTATUS = supervisorTemp.SV_ESTATUS;
            supervisor.SV_FECHA_CREACION = supervisorTemp.SV_FECHA_CREACION;
            supervisor.SV_USUARIO_CREACION = supervisorTemp.SV_USUARIO_CREACION;
            supervisor.SV_FECHA_MOD = supervisorTemp.SV_FECHA_MOD;
            supervisor.SV_USUARIO_MOD = supervisorTemp.SV_USUARIO_MOD;
            supervisor.SV_FECHA_APROBACION = supervisorTemp.SV_FECHA_APROBACION;
            supervisor.SV_USUARIO_APROBADOR = supervisorTemp.SV_USUARIO_APROBADOR;

            return supervisor;
        }
        private SupervisorModel MappingSupervisorFromTemp(SupervisorModel supervisor, SupervisorTempModel supervisorTemp)
        {
            supervisor.SV_ID_SUPERVISOR = supervisorTemp.SV_ID_SUPERVISOR;
            supervisor.SV_ID_AREA = supervisorTemp.SV_ID_AREA;
            supervisor.CE_ID_EMPRESA = supervisorTemp.CE_ID_EMPRESA;
            supervisor.SV_COD_SUPERVISOR = supervisorTemp.SV_COD_SUPERVISOR;
            supervisor.SV_LIMITE_MINIMO = supervisorTemp.SV_LIMITE_MINIMO;
            supervisor.SV_LIMITE_SUPERIOR = supervisorTemp.SV_LIMITE_SUPERIOR;
            supervisor.SV_ESTATUS = supervisorTemp.SV_ESTATUS;
            supervisor.SV_FECHA_CREACION = supervisorTemp.SV_FECHA_CREACION;
            supervisor.SV_USUARIO_CREACION = supervisorTemp.SV_USUARIO_CREACION;
            supervisor.SV_FECHA_MOD = supervisorTemp.SV_FECHA_MOD;
            supervisor.SV_USUARIO_MOD = supervisorTemp.SV_USUARIO_MOD;
            supervisor.SV_FECHA_APROBACION = supervisorTemp.SV_FECHA_APROBACION;
            supervisor.SV_USUARIO_APROBADOR = supervisorTemp.SV_USUARIO_APROBADOR;
            return supervisor;
        }
        private SupervisorTempModel MappingTempFromSupervisor(SupervisorModel supervisor)
        {
            var supervisorTemp = new SupervisorTempModel();

            supervisorTemp.SV_ID_SUPERVISOR = supervisor.SV_ID_SUPERVISOR;
            supervisorTemp.SV_ID_AREA = supervisor.SV_ID_AREA;
            supervisorTemp.CE_ID_EMPRESA = supervisor.CE_ID_EMPRESA;
            supervisorTemp.SV_COD_SUPERVISOR = supervisor.SV_COD_SUPERVISOR;
            supervisorTemp.SV_LIMITE_MINIMO = supervisor.SV_LIMITE_MINIMO;
            supervisorTemp.SV_LIMITE_SUPERIOR = supervisor.SV_LIMITE_SUPERIOR;
            supervisorTemp.SV_ESTATUS = supervisor.SV_ESTATUS;
            supervisorTemp.SV_FECHA_CREACION = supervisor.SV_FECHA_CREACION;
            supervisorTemp.SV_USUARIO_CREACION = supervisor.SV_USUARIO_CREACION;
            supervisorTemp.SV_FECHA_MOD = supervisor.SV_FECHA_MOD;
            supervisorTemp.SV_USUARIO_MOD = supervisor.SV_USUARIO_MOD;
            supervisorTemp.SV_FECHA_APROBACION = supervisor.SV_FECHA_APROBACION;
            supervisorTemp.SV_USUARIO_APROBADOR = supervisor.SV_USUARIO_APROBADOR;

            return supervisorTemp;
        }
        private SupervisorTempModel MappingTempFromSupervisor(SupervisorTempModel supervisorTemp, SupervisorModel supervisor)
        {
            supervisorTemp.SV_ID_SUPERVISOR = supervisor.SV_ID_SUPERVISOR;
            supervisorTemp.SV_ID_AREA = supervisor.SV_ID_AREA;
            supervisorTemp.CE_ID_EMPRESA = supervisor.CE_ID_EMPRESA;
            supervisorTemp.SV_COD_SUPERVISOR = supervisor.SV_COD_SUPERVISOR;
            supervisorTemp.SV_LIMITE_MINIMO = supervisor.SV_LIMITE_MINIMO;
            supervisorTemp.SV_LIMITE_SUPERIOR = supervisor.SV_LIMITE_SUPERIOR;
            supervisorTemp.SV_ESTATUS = supervisor.SV_ESTATUS;
            supervisorTemp.SV_FECHA_CREACION = supervisor.SV_FECHA_CREACION;
            supervisorTemp.SV_USUARIO_CREACION = supervisor.SV_USUARIO_CREACION;
            supervisorTemp.SV_FECHA_MOD = supervisor.SV_FECHA_MOD;
            supervisorTemp.SV_USUARIO_MOD = supervisor.SV_USUARIO_MOD;
            supervisorTemp.SV_FECHA_APROBACION = supervisor.SV_FECHA_APROBACION;
            supervisorTemp.SV_USUARIO_APROBADOR = supervisor.SV_USUARIO_APROBADOR;
            return supervisorTemp;
        }
        private string MappingRol(AspNetUsers val)
        {
            if (val.AspNetUserRoles != null && val.AspNetUserRoles.Count > 0)
                return val.AspNetUserRoles.ToList()[0].AspNetRoles.Description.ToString();

            return "No ha sido asignado el rol al usuario supervisor.";
        }
    }
}
