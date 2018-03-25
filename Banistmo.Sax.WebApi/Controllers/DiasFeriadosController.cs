using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.WebApi.Models;

namespace Banistmo.Sax.WebApi.Controllers
{
    
    [Authorize]
    [RoutePrefix("api/DiasFeriados")]
    public class DiasFeriadosController : ApiController
    {
        private readonly IDiasFeriadosService diasFeriadosService;

        public DiasFeriadosController(IDiasFeriadosService dfs)
        {
            diasFeriadosService = dfs;
        }

        public IHttpActionResult Get()
        {
            List<DiasFeriadosModel> dfs = diasFeriadosService.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var diaFeriado = diasFeriadosService.GetSingle(c => c.CD_ID_DIA_FERIADO == id);

            if (diaFeriado != null)
            {
                return Ok(diaFeriado);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] DiasFeriadosModel model)
        {
            model.CD_FECHA_CREACION = DateTime.Now;
            model.CD_ESTATUS = 1;
            return Ok(diasFeriadosService.Insert(model, true));
        }

        // PUT: api/DiasFeriados/5
        public IHttpActionResult Put([FromBody] DiasFeriadosModel model)
        {
            model.CD_FECHA_MOD = DateTime.Now;
            diasFeriadosService.Update(model);
            return Ok();
        }

        // DELETE: api/DiasFeriados/5
        public IHttpActionResult Delete(int id)
        {
            var diaFeriado = diasFeriadosService.GetSingle(c => c.CD_ID_DIA_FERIADO == id);

            if (diaFeriado == null)
            {
                return NotFound();
            }

            diaFeriado.CD_FECHA_MOD = DateTime.Now;
            diaFeriado.CD_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Eliminado);
            diasFeriadosService.Update(diaFeriado);
            return Ok();

        }
    }
}