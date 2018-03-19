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
    [RoutePrefix("api/Parametro")]
    public class ParametroController : ApiController
    {
        private readonly IParametroService paramService;
        private readonly IParametroTempService paramTempService;
        public ParametroController(IParametroService objParamService, IParametroTempService objParamTempService)
        {
            paramService = objParamService;
            paramTempService = objParamTempService;
        }
        public IHttpActionResult Get()
        {
            List<ParametroModel> objParamService = paramService.GetAll();
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
        public IHttpActionResult Post([FromBody] ParametroModel model)
        {
            var parametro = paramService.InsertParametro(model);
            return Ok(parametro);
        }
        public IHttpActionResult Put([FromBody] ParametroModel model)
        {
            // Se obtiene el parametro y se actualiza la fecha de modificación y el estatus
            var param = paramService.GetSingle(c => c.PA_ID_PARAMETRO == model.PA_ID_PARAMETRO);
            param.PA_FECHA_MOD = DateTime.Now;
            param.PA_ESTATUS = Convert.ToInt16(RegistryState.Pendiente);
            paramService.Update(param);
            // Se obtiene el parametro temporal para luego actualizarlo con el parametro
            var paramTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == model.PA_ID_PARAMETRO);
            paramTemp = MappingTempFromParam(paramTemp, model);
            paramTemp.PA_ESTATUS = Convert.ToInt16(RegistryState.PorAprobar);
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
            diaFeriado.PA_ESTATUS = Convert.ToInt16(RegistryState.Eliminado);
            paramService.Update(diaFeriado);
            return Ok();

        }
        [Route("AprobarParametro")]
        public IHttpActionResult PutAprobarParametro(int id)
        {
            var tempModel = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == id);
            if (tempModel != null)
            {
                tempModel.PA_FECHA_APROBACION = DateTime.Now;
                tempModel.PA_ESTATUS = Convert.ToInt16(RegistryState.Aprobado);
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
                paramModel.PA_FECHA_APROBACION = DateTime.Now;
                paramModel.PA_ESTATUS = Convert.ToInt16(RegistryState.Aprobado);
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

            param.PA_COD_PARAMETRO = paramTemp.PA_COD_PARAMETRO;
            param.PA_DESCRIPCION = paramTemp.PA_DESCRIPCION;
            param.PA_ESTATUS = 1;
            param.PA_ESTATUS_ACCION = paramTemp.PA_ESTATUS_ACCION;
            param.PA_FECHA_APROBACION = DateTime.Today;
            param.PA_FECHA_CREACION = paramTemp.PA_FECHA_CREACION;
            param.PA_FECHA_MOD = DateTime.Today;
            param.PA_FECHA_PROCESO = paramTemp.PA_FECHA_PROCESO;
            param.PA_FILE_CONTABLE = paramTemp.PA_FILE_CONTABLE;
            param.PA_FRECUENCIA = paramTemp.PA_FRECUENCIA;
            param.PA_FRECUENCIA_LIMPIEZA = paramTemp.PA_FRECUENCIA_LIMPIEZA;
            param.PA_HORA_EJECUCION = paramTemp.PA_HORA_EJECUCION;
            param.PA_ID_PARAMETRO = paramTemp.PA_ID_PARAMETRO;
            param.PA_RUTA_CONTABLE = paramTemp.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = paramTemp.PA_RUTA_TEMPORAL;
            param.PA_TIPO_ACCION = paramTemp.PA_TIPO_ACCION;
            param.PA_USUARIO_APROBADOR = paramTemp.PA_USUARIO_APROBADOR;
            param.PA_USUARIO_CREACION = paramTemp.PA_USUARIO_CREACION;
            param.PA_USUARIO_MOD = paramTemp.PA_USUARIO_MOD;

            return param;
        }
        private ParametroModel MappingParamFromTemp(ParametroModel param, ParametroTempModel paramTemp)
        {
            param.PA_COD_PARAMETRO = paramTemp.PA_COD_PARAMETRO;
            param.PA_DESCRIPCION = paramTemp.PA_DESCRIPCION;
            param.PA_ESTATUS = 1;
            param.PA_ESTATUS_ACCION = paramTemp.PA_ESTATUS_ACCION;
            param.PA_FECHA_APROBACION = DateTime.Today;
            param.PA_FECHA_CREACION = paramTemp.PA_FECHA_CREACION;
            param.PA_FECHA_MOD = DateTime.Today;
            param.PA_FECHA_PROCESO = paramTemp.PA_FECHA_PROCESO;
            param.PA_FILE_CONTABLE = paramTemp.PA_FILE_CONTABLE;
            param.PA_FRECUENCIA = paramTemp.PA_FRECUENCIA;
            param.PA_FRECUENCIA_LIMPIEZA = paramTemp.PA_FRECUENCIA_LIMPIEZA;
            param.PA_HORA_EJECUCION = paramTemp.PA_HORA_EJECUCION;
            param.PA_ID_PARAMETRO = paramTemp.PA_ID_PARAMETRO;
            param.PA_RUTA_CONTABLE = paramTemp.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = paramTemp.PA_RUTA_TEMPORAL;
            param.PA_TIPO_ACCION = paramTemp.PA_TIPO_ACCION;
            param.PA_USUARIO_APROBADOR = paramTemp.PA_USUARIO_APROBADOR;
            param.PA_USUARIO_CREACION = paramTemp.PA_USUARIO_CREACION;
            param.PA_USUARIO_MOD = paramTemp.PA_USUARIO_MOD;

            return param;
        }
        private ParametroTempModel MappingTempFromParam(ParametroModel param)
        {
            var paramT = new ParametroTempModel();

            paramT.PA_COD_PARAMETRO = param.PA_COD_PARAMETRO;
            paramT.PA_DESCRIPCION = param.PA_DESCRIPCION;
            paramT.PA_ESTATUS = 1;
            paramT.PA_ESTATUS_ACCION = param.PA_ESTATUS_ACCION;
            paramT.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            paramT.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            paramT.PA_FECHA_MOD = DateTime.Today;
            paramT.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            paramT.PA_FILE_CONTABLE = param.PA_FILE_CONTABLE;
            paramT.PA_FRECUENCIA = param.PA_FRECUENCIA;
            paramT.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            paramT.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            paramT.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            paramT.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            paramT.PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL;
            paramT.PA_TIPO_ACCION = param.PA_TIPO_ACCION;
            paramT.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            paramT.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            paramT.PA_USUARIO_MOD = param.PA_USUARIO_MOD;

            return paramT;
        }
        private ParametroTempModel MappingTempFromParam(ParametroTempModel paramT, ParametroModel param)
        {
            paramT.PA_COD_PARAMETRO = param.PA_COD_PARAMETRO;
            paramT.PA_DESCRIPCION = param.PA_DESCRIPCION;
            paramT.PA_ESTATUS = 1;
            paramT.PA_ESTATUS_ACCION = param.PA_ESTATUS_ACCION;
            paramT.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            paramT.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            paramT.PA_FECHA_MOD = DateTime.Today;
            paramT.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            paramT.PA_FILE_CONTABLE = param.PA_FILE_CONTABLE;
            paramT.PA_FRECUENCIA = param.PA_FRECUENCIA;
            paramT.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            paramT.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            paramT.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            paramT.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            paramT.PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL;
            paramT.PA_TIPO_ACCION = param.PA_TIPO_ACCION;
            paramT.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            paramT.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            paramT.PA_USUARIO_MOD = param.PA_USUARIO_MOD;

            return paramT;
        }
    }
}