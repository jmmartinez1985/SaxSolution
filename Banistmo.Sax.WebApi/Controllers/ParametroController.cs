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
            var par = new Sax.Repository.Model.SAX_PARAMETRO();
            par = paramService.InsertParametro(model);
            return Ok(par);

            /* 
            model.PA_FECHA_CREACION = DateTime.Now;
            model.PA_ESTATUS = Convert.ToInt16(RegistryState.Aprobado);
            return Ok(paramService.Insert(model, true));
            */

        }
        // PUT: api/DiasFeriados/5
        public IHttpActionResult Put([FromBody] ParametroModel model)
        {
            model.PA_FECHA_MOD = DateTime.Now;
            paramService.Update(model);
            return Ok();
        }
        // DELETE: api/DiasFeriados/5
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
            ParametroModel param = new ParametroModel();
            param = MappingParamFromTemp(tempModel);

            if (tempModel != null)
            {
                return Ok(paramService.Insert(param, true));
            }
            return NotFound();
        }
        [Route("RechazarParametro")]
        public IHttpActionResult PutRechazarParametro(int id)
        {
            var paramModel = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);
            ParametroTempModel paramTemp = new ParametroTempModel();
            paramTemp = MappingTempFromParam(paramModel);

            if (paramModel != null)
            {
                return Ok(paramTempService.Insert(paramTemp, true));
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
    }
}