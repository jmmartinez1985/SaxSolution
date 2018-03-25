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
        private readonly IUserService userService;

        public DiasFeriadosController(IDiasFeriadosService dfs, IUserService usr)
        {
            diasFeriadosService = dfs;
            userService = usr;
        }

        public IHttpActionResult Get()
        {

            var dfs = diasFeriadosService.GetAll(null, null, includes: c => c.AspNetUsers);
            var userList = userService.GetAll();

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
                CD_USUARIO_CREACION = getUserName(userList, c.CD_USUARIO_CREACION),
                CD_FECHA_MOD = c.CD_FECHA_MOD,
                CD_USUARIO_MOD = getUserName(userList, c.CD_USUARIO_MOD)
            }));
        }

        public IHttpActionResult Get(int id)
        {
            var diaFeriado = diasFeriadosService.GetSingle(c => c.CD_ID_DIA_FERIADO == id, includes: c => c.AspNetUsers);
            List<DiasFeriadosModel> listDiasFeriados = new List<DiasFeriadosModel>();
            var userList = userService.GetAll();

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
                    CD_USUARIO_CREACION = getUserName(userList , c.CD_USUARIO_CREACION),
                    CD_FECHA_MOD = c.CD_FECHA_MOD,
                    CD_USUARIO_MOD = getUserName(userList, c.CD_USUARIO_MOD)
                }));
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

        public String getUserName(List<AspNetUserModel> userList, string userID)
        {
            string userName = string.Empty;
            if(userID != null)
            {
                userName = userList.FirstOrDefault(k => k.Id == userID).FirstName + " " + userList.FirstOrDefault(k => k.Id == userID).LastName;
            }
            else
            {
                return null;
            }
            return userName;
        }
    }
}