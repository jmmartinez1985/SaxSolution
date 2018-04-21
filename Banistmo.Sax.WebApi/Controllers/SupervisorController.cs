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

        //Constructores
        public SupervisorController()
        {
            supervisorService = supervisorService ?? new SupervisorService();
            supervisorTempService = supervisorTempService ?? new SupervisorTempService();
        }
        public SupervisorController(ISupervisorService objSupervisorService, ISupervisorTempService objSupervisorTempService)
        {
            supervisorService = objSupervisorService;
            supervisorTempService = objSupervisorTempService;
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
        
        //Metodos
        public  IHttpActionResult Get()
        {
            IList<SupervisorModel> objSupervisorService = supervisorService.GetAll(null ,null, includes: c => c.SAX_AREA_OPERATIVA );
            if (objSupervisorService == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(objSupervisorService.Select( c => new
            {
                SV_ID_SUPERVISOR = c.SV_ID_SUPERVISOR,
                CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                CE_NOMBRE_EMPRESA = c.SAX_EMPRESA.CE_NOMBRE,
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
                SV_NOMBRE_AREA = c.SAX_AREA_OPERATIVA.CA_NOMBRE
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
                    CE_NOMBRE_EMPRESA = supervisor.SAX_EMPRESA.CE_NOMBRE,
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
                    SV_NOMBRE_AREA = supervisor.SAX_AREA_OPERATIVA.CA_NOMBRE
                });
            }
            return BadRequest("No se encontraron registros para la consulta realizada.");
        }
        public async Task<IHttpActionResult> Post([FromBody] SupervisorModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            model.SV_USUARIO_CREACION = user.Id;
            model.SV_FECHA_CREACION = DateTime.Now;
            var supervisor = supervisorService.InsertSupervisor(model);
            return Ok(supervisor);
        }
        [Route("UpdateSupervisor"), HttpPost]
        public async Task<IHttpActionResult> Put([FromBody] SupervisorModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            // Se obtiene el supervisor y se actualiza la fecha de modificación y el estatus
            var supervisor = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == model.SV_ID_SUPERVISOR);
            supervisor.SV_FECHA_MOD = DateTime.Now;
            supervisor.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Pendiente);
            supervisor.SV_FECHA_MOD = DateTime.Now;
            supervisor.SV_ID_AREA = model.SV_ID_AREA;
            supervisor.CE_ID_EMPRESA = model.CE_ID_EMPRESA;
            supervisor.SV_ID_SUPERVISOR = model.SV_ID_SUPERVISOR;
            supervisor.SV_LIMITE_MINIMO = model.SV_LIMITE_MINIMO;
            supervisor.SV_LIMITE_SUPERIOR = model.SV_LIMITE_SUPERIOR;

            supervisorService.Update(supervisor);
            // Se obtiene el supervisor temporal para luego actualizarlo con el supervisor 
            var supervisorTemp = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == model.SV_ID_SUPERVISOR);
            supervisorTemp = MappingTempFromSupervisor(supervisorTemp, model);
            supervisorTemp.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.PorAprobar);
            supervisorTempService.Update(supervisorTemp);
            return Ok();
        }
        public IHttpActionResult Delete(int id)
        {
            var supervisor = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == id);

            if (supervisor == null)
            {
                return NotFound();
            }

            supervisor.SV_FECHA_MOD = DateTime.Now;
            supervisor.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Eliminado);
            supervisorService.Update(supervisor);
            return Ok();

        }
        [Route("AprobarSupervisor"), HttpPost]
        public async Task<IHttpActionResult> PutAprobarParametro(int id)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var tempModel = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == id);
            if (tempModel != null)
            {
                tempModel.SV_FECHA_APROBACION = DateTime.Now;
                tempModel.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                tempModel.SV_USUARIO_APROBADOR = user.Id;
                supervisorTempService.Update(tempModel);
                SupervisorModel supervisor = new SupervisorModel();
                supervisor = MappingSupervisorFromTemp(supervisor, tempModel);

                supervisorService.Update(supervisor);
                return Ok();
            }
            return NotFound();
        }
        [Route("RechazarSupervisor"), HttpPost]
        public async Task<IHttpActionResult> PutRechazarSupervisor(int id)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var supervisorModel = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == id);
            if (supervisorModel != null)
            {
                supervisorModel.SV_USUARIO_MOD = user.Id;
                supervisorModel.SV_FECHA_MOD = DateTime.Now;
                supervisorModel.SV_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                supervisorService.Update(supervisorModel);
                var supervisorTempModel = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == id);
                supervisorTempModel = MappingTempFromSupervisor(supervisorTempModel, supervisorModel);

                supervisorTempService.Update(supervisorTempModel);
                return Ok();
            }
            return NotFound();
        }
        [Route("GetTemp")]
        public IHttpActionResult GetTemp()
        {
            List<SupervisorTempModel> objSupervisorTempService = supervisorTempService.GetAll();
            if (objSupervisorTempService == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(objSupervisorTempService.Select(c => new
            {
                SV_ID_SUPERVISOR = c.SV_ID_SUPERVISOR,
                CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                CE_NOMBRE_EMPRESA = c.SAX_EMPRESA.CE_NOMBRE,
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
                SV_NOMBRE_AREA = c.SAX_AREA_OPERATIVA.CA_NOMBRE
            }));
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
        [Route("ReporteSupervisor"), HttpPost]
        public IHttpActionResult GetReporte([FromBody] ReporteSupervisorModel model)
        {
            int estado = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            IList<SupervisorModel> objSupervisorService 
                = supervisorService.GetAll(f => f.SV_ESTATUS == estado 
                && f.SV_LIMITE_MINIMO == (model.LimiteInferior == null ? f.SV_LIMITE_MINIMO : model.LimiteInferior)
                && f.SV_LIMITE_SUPERIOR == (model.LimiteSuperior == null ? f.SV_LIMITE_SUPERIOR : model.LimiteSuperior)
                && f.SV_USUARIO_APROBADOR == (model.UsuarioAprobador == null ? f.SV_USUARIO_APROBADOR : model.UsuarioAprobador)
                && f.SV_COD_SUPERVISOR == (model.UsuarioSupervisor == null ? f.SV_COD_SUPERVISOR : model.UsuarioSupervisor), 
                null, includes: c => c.SAX_AREA_OPERATIVA);
            if (objSupervisorService == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(objSupervisorService.Select(c => new
            {
                SV_ID_SUPERVISOR = c.SV_ID_SUPERVISOR,
                CE_ID_EMPRESA = c.CE_ID_EMPRESA,
                CE_NOMBRE_EMPRESA = c.SAX_EMPRESA.CE_NOMBRE,
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
                SV_NOMBRE_AREA = c.SAX_AREA_OPERATIVA.CA_NOMBRE
            }));
        }
        //Mapping
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
    }
}
