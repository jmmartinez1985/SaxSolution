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

namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/Parametro")]
    public class ParametroController : ApiController
    {
        private readonly IParametroService paramService;
        private readonly IParametroTempService paramTempService;
        private ApplicationUserManager _userManager;
        public ParametroController(IParametroService objParamService, IParametroTempService objParamTempService)
        {
            paramService = objParamService;
            paramTempService = objParamTempService;
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
            var objParamService = paramService.GetAll(null, null,
                k => k.AspNetUsers
                );
            if (objParamService == null)
            {
                return NotFound();
            }
            return Ok(objParamService);
        }
        public IHttpActionResult Get(int id)
        {
            var param = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (param != null)
            {
                return Ok(param);
            }
            return NotFound();
        }
        public async Task<IHttpActionResult> Post([FromBody] ParametroModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            model.PA_USUARIO_CREACION = user.Id;
            model.PA_FECHA_CREACION = DateTime.Now;
            var parametro = paramService.InsertParametro(model);
            return Ok(parametro);
        }
        public async Task< IHttpActionResult> Put([FromBody] ParametroModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());        
            // Se obtiene el parametro y se actualiza la fecha de modificación y el estatus
            var param = paramService.GetSingle(c => c.PA_ID_PARAMETRO == model.PA_ID_PARAMETRO);
            param.PA_USUARIO_MOD = user.Id;
            param.PA_FECHA_MOD = DateTime.Now;
            param.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Pendiente);
            param.PA_FECHA_PROCESO = model.PA_FECHA_PROCESO;
            param.PA_FRECUENCIA = model.PA_FRECUENCIA;
            param.PA_HORA_EJECUCION = model.PA_HORA_EJECUCION;
            param.PA_RUTA_CONTABLE = model.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = model.PA_RUTA_TEMPORAL;
            param.PA_FRECUENCIA_LIMPIEZA = model.PA_FRECUENCIA_LIMPIEZA;

            paramService.Update(param);
            // Se obtiene el parametro temporal para luego actualizarlo con el parametro
            var paramTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == model.PA_ID_PARAMETRO);
            paramTemp = MappingTempFromParam(paramTemp, param);
            paramTemp.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.PorAprobar);
            paramTempService.Update(paramTemp);
            return Ok();
        }
        public IHttpActionResult Delete(int id)
        {
            var diaFeriado = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (diaFeriado == null)
            {
                return NotFound();
            }

            diaFeriado.PA_FECHA_MOD = DateTime.Now;
            diaFeriado.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Eliminado);
            paramService.Update(diaFeriado);
            return Ok();

        }
        [Route("AprobarParametro")]
        public async Task< IHttpActionResult> PutAprobarParametro(int id)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var tempModel = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == id);
            if (tempModel != null)
            {
                tempModel.PA_FECHA_APROBACION = DateTime.Now;
                tempModel.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                tempModel.PA_USUARIO_APROBADOR = user.Id;
                paramTempService.Update(tempModel);
                ParametroModel param = new ParametroModel();
                param = MappingParamFromTemp(param, tempModel);

                paramService.Update(param);
                return Ok();
            }
            return NotFound();
        }
        [Route("RechazarParametro")]
        public IHttpActionResult PutRechazarParametro(int id)
        {
            var paramModel = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);
            if (paramModel != null)
            {
                paramModel.PA_FECHA_MOD = DateTime.Now;
                paramModel.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                paramService.Update(paramModel);
                var paramTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == id);
                paramTemp = MappingTempFromParam(paramTemp, paramModel);

                paramTempService.Update(paramTemp);
                return Ok();
            }
            return NotFound();
        }
        [Route("GetTemp")]
        public IHttpActionResult GetTemp()
        {
            List<ParametroTempModel> objParametroTempService = paramTempService.GetAll();
            if (objParametroTempService == null)
            {
                return NotFound();
            }
            return Ok(objParametroTempService);
        }
        [Route("GetTempById")]
        public IHttpActionResult GetTemp(int id)
        {
            var parametroTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (parametroTemp != null)
            {
                return Ok(parametroTemp);
            }
            return NotFound();
        }
        private ParametroModel MappingParamFromTemp(ParametroTempModel paramTemp)
        {
            var param = new ParametroModel();

            param.PA_ESTATUS = 1;
            param.PA_FECHA_APROBACION = paramTemp.PA_FECHA_APROBACION;
            param.PA_FECHA_CREACION = paramTemp.PA_FECHA_CREACION;
            param.PA_FECHA_MOD = paramTemp.PA_FECHA_MOD;
            param.PA_FECHA_PROCESO = paramTemp.PA_FECHA_PROCESO;
            param.PA_HORA_EJECUCION = paramTemp.PA_HORA_EJECUCION;
            param.PA_ID_PARAMETRO = paramTemp.PA_ID_PARAMETRO;
            param.PA_RUTA_CONTABLE = paramTemp.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = paramTemp.PA_RUTA_TEMPORAL;
            param.PA_USUARIO_APROBADOR = paramTemp.PA_USUARIO_APROBADOR;
            param.PA_USUARIO_CREACION = paramTemp.PA_USUARIO_CREACION;
            param.PA_USUARIO_MOD = paramTemp.PA_USUARIO_MOD;

            param.PA_FRECUENCIA = paramTemp.PA_FRECUENCIA;
            param.PA_FRECUENCIA_LIMPIEZA = paramTemp.PA_FRECUENCIA_LIMPIEZA;
            return param;
        }
        private ParametroModel MappingParamFromTemp(ParametroModel param, ParametroTempModel paramTemp)
        {
            param.PA_ESTATUS = 1;
            param.PA_FECHA_APROBACION = paramTemp.PA_FECHA_APROBACION;
            param.PA_FECHA_CREACION = paramTemp.PA_FECHA_CREACION;
            param.PA_FECHA_MOD = paramTemp.PA_FECHA_MOD ;
            param.PA_FECHA_PROCESO = paramTemp.PA_FECHA_PROCESO;
            param.PA_HORA_EJECUCION = paramTemp.PA_HORA_EJECUCION;
            param.PA_ID_PARAMETRO = paramTemp.PA_ID_PARAMETRO;
            param.PA_RUTA_CONTABLE = paramTemp.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = paramTemp.PA_RUTA_TEMPORAL;
            param.PA_USUARIO_APROBADOR = paramTemp.PA_USUARIO_APROBADOR;
            param.PA_USUARIO_CREACION = paramTemp.PA_USUARIO_CREACION;
            param.PA_USUARIO_MOD = paramTemp.PA_USUARIO_MOD;

            param.PA_FRECUENCIA = paramTemp.PA_FRECUENCIA;
            param.PA_FRECUENCIA_LIMPIEZA = paramTemp.PA_FRECUENCIA_LIMPIEZA;

            return param;
        }
        private ParametroTempModel MappingTempFromParam(ParametroModel param)
        {
            var paramT = new ParametroTempModel();
            
            paramT.PA_ESTATUS = 1;
            paramT.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            paramT.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            paramT.PA_FECHA_MOD = DateTime.Today;
            paramT.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            paramT.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            paramT.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            paramT.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            paramT.PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL;
            paramT.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            paramT.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            paramT.PA_USUARIO_MOD = param.PA_USUARIO_MOD;

            paramT.PA_FRECUENCIA = param.PA_FRECUENCIA;
            paramT.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            return paramT;
        }
        private ParametroTempModel MappingTempFromParam(ParametroTempModel paramT, ParametroModel param)
        {
            paramT.PA_ESTATUS = 1;
            paramT.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            paramT.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            paramT.PA_FECHA_MOD = DateTime.Today;
            paramT.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            paramT.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            paramT.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            paramT.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            paramT.PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL;
            paramT.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            paramT.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            paramT.PA_USUARIO_MOD = param.PA_USUARIO_MOD;

            paramT.PA_FRECUENCIA = param.PA_FRECUENCIA;
            paramT.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            return paramT;
        }
    }
}