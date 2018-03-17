using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/Supervisor")]
    public class SupervisorController : ApiController
    {
        private readonly ISupervisorService supervisorService;
        private readonly ISupervisorTempService supervisorTempService;

      
        public SupervisorController(ISupervisorService objSupervisorService, ISupervisorTempService objSupervisorTempService)
        {
            supervisorService = objSupervisorService;
            supervisorTempService = objSupervisorTempService;
        }

        public IHttpActionResult Get()
        {
            List<SupervisorModel> objSupervisorService = supervisorService.GetAll();
            if (objSupervisorService == null)
            {
                return NotFound();
            }
            return Ok(objSupervisorService);
        }
        public IHttpActionResult Get(int id)
        {
            var param = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == id);

            if (param != null)
            {
                return Ok(param);
            }
            return NotFound();
        }
        public IHttpActionResult Post([FromBody] SupervisorModel model)
        {
            var supervisor = supervisorService.InsertSupervisor(model);
            return Ok(supervisor);
        }
        public IHttpActionResult Put([FromBody] SupervisorModel model)
        {
            model.SV_FECHA_MOD = DateTime.Now;
            supervisorService.Update(model);
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
            supervisor.SV_ESTATUS = Convert.ToInt16(RegistryState.Eliminado);
            supervisorService.Update(supervisor);
            return Ok();

        }
        [Route("AprobarSupervisor")]
        public IHttpActionResult PutAprobarParametro(int id)
        {
            var tempModel = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == id);
            SupervisorModel param = new SupervisorModel();
            param = MappingSupervisorFromTemp(tempModel);

            if (tempModel != null)
            {
                return Ok(supervisorService.Insert(param, true));
            }
            return NotFound();
        }
        [Route("RechazarSupervisor")]
        public IHttpActionResult PutRechazarParametro(int id)
        {
            var paramModel = supervisorService.GetSingle(c => c.SV_ID_SUPERVISOR == id);
            SupervisorTempModel supervisorTemp = new SupervisorTempModel();
            supervisorTemp = MappingTempFromSupervisor(paramModel);

            if (paramModel != null)
            {
                return Ok(supervisorTempService.Insert(supervisorTemp, true));
            }
            return NotFound();
        }
        [Route("GetTemp")]
        public IHttpActionResult GetTemp()
        {
            List<SupervisorTempModel> objSupervisorTempService = supervisorTempService.GetAll();
            if (objSupervisorTempService == null)
            {
                return NotFound();
            }
            return Ok(objSupervisorTempService);
        }
        [Route("GetTempById")]
        public IHttpActionResult GetTemp(int id)
        {
            var supervisorTemp = supervisorTempService.GetSingle(c => c.SV_ID_SUPERVISOR == id);

            if (supervisorTemp != null)
            {
                return Ok(supervisorTemp);
            }
            return NotFound();
        }
        private SupervisorModel MappingSupervisorFromTemp(SupervisorTempModel supervisorTemp)
        {
            var supervisor = new SupervisorModel();

            supervisor.SV_ID_SUPERVISOR = supervisorTemp.SV_ID_SUPERVISOR;
            supervisor.SV_COD_AREA = supervisorTemp.SV_COD_AREA;
            supervisor.CE_ID_EMPRESA = supervisorTemp.CE_ID_EMPRESA;
            supervisor.SV_COD_SUPERVISOR = supervisorTemp.SV_COD_SUPERVISOR;
            supervisor.SV_LIMITE_MINIMO = supervisorTemp.SV_LIMITE_MINIMO;
            supervisor.SV_LIMITE_SUPERIOR = supervisorTemp.SV_LIMITE_SUPERIOR;
            supervisor.SV_TIPO_ACCION = supervisorTemp.SV_TIPO_ACCION;
            supervisor.SV_ESTATUS_ACCION = supervisorTemp.SV_ESTATUS_ACCION;
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
            supervisorTemp.SV_COD_AREA = supervisor.SV_COD_AREA;
            supervisorTemp.CE_ID_EMPRESA = supervisor.CE_ID_EMPRESA;
            supervisorTemp.SV_COD_SUPERVISOR = supervisor.SV_COD_SUPERVISOR;
            supervisorTemp.SV_LIMITE_MINIMO = supervisor.SV_LIMITE_MINIMO;
            supervisorTemp.SV_LIMITE_SUPERIOR = supervisor.SV_LIMITE_SUPERIOR;
            supervisorTemp.SV_TIPO_ACCION = supervisor.SV_TIPO_ACCION;
            supervisorTemp.SV_ESTATUS_ACCION = supervisor.SV_ESTATUS_ACCION;
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
