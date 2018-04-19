using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.Services.Implementations.Business;

namespace Banistmo.Sax.WebApi.Controllers
{

    [Authorize]
    [RoutePrefix("api/DiasFeriados")]
    public class DiasFeriadosController : ApiController
    {
        private readonly IDiasFeriadosService diasFeriadosService;
        private readonly IUserService userService;
        private readonly IUserUtil userUtilService;

        //public DiasFeriadosController()
        //{
        //    diasFeriadosService = diasFeriadosService ?? new DiasFeriadosService();
        //    userService = userService ?? new UserService();
        //    userUtilService = userUtilService ?? new UserUtil();
        //}

        public DiasFeriadosController(IDiasFeriadosService dfs, IUserService usr, IUserUtil util)
        {
            diasFeriadosService = dfs;
            userService = usr;
            userUtilService = util;
        }

        public IHttpActionResult Get()
        {

            var dfs = diasFeriadosService.GetAll(null, null, includes: c => c.AspNetUsers);
         
            if (dfs == null)
            {
                return NotFound();
            }

            
            return Ok(dfs.Select(c => new
            {
                CD_ID_DIA_FERIADO = c.CD_ID_DIA_FERIADO,
                CD_ANNIO = c.CD_ANNIO,
                CD_MES = c.CD_MES,
                CD_DIA = c.CD_DIA,
                CD_ESTATUS = c.CD_ESTATUS,
                CD_FECHA_CREACION = c.CD_FECHA_CREACION,
                CD_USUARIO_CREACION = userUtilService.getUserFullName(dfs.FirstOrDefault().AspNetUsers, c.CD_USUARIO_CREACION),
                CD_FECHA_MOD = c.CD_FECHA_MOD,
                CD_USUARIO_MOD = userUtilService.getUserFullName(dfs.FirstOrDefault().AspNetUsers1, c.CD_USUARIO_MOD)
            }));
        }

        public IHttpActionResult Get(int id)
        {
            var diaFeriado = diasFeriadosService.GetSingle(c => c.CD_ID_DIA_FERIADO == id, includes: c => c.AspNetUsers);
            List<DiasFeriadosModel> listDiasFeriados = new List<DiasFeriadosModel>();
            

            if (diaFeriado != null)
            {
                listDiasFeriados.Add(diaFeriado);
                return Ok(listDiasFeriados.Select(c => new
                {
                    CD_ID_DIA_FERIADO = c.CD_ID_DIA_FERIADO,
                    CD_ANNIO = c.CD_ANNIO,
                    CD_MES = c.CD_MES,
                    CD_DIA = c.CD_DIA,
                    CD_ESTATUS = c.CD_ESTATUS,
                    CD_FECHA_CREACION = c.CD_FECHA_CREACION,
                    CD_USUARIO_CREACION = userUtilService.getUserFullName(c.AspNetUsers, c.CD_USUARIO_CREACION),
                    CD_FECHA_MOD = c.CD_FECHA_MOD,
                    CD_USUARIO_MOD = userUtilService.getUserFullName(c.AspNetUsers1, c.CD_USUARIO_MOD)
                }));
            }

            
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] DiasFeriadosModel model)
        {
            model.CD_FECHA_CREACION = DateTime.Now;
            model.CD_ESTATUS= Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            return Ok(diasFeriadosService.Insert(model, true));
        }

        // PUT: api/DiasFeriados/5
        [Route("UpdateDiaFeriado"), HttpPost]
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