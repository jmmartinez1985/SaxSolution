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

        public ParametroController(IParametroService objParamService)
        {
            paramService = objParamService;
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
            var diaFeriado = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (diaFeriado != null)
            {
                return Ok(diaFeriado);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] ParametroModel model)
        {
            
            paramService.InsertParametro(model);
            return Ok();

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

    }
}