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
    [RoutePrefix("api/DiasFeriados")]
    public class DiasFeriadosController : ApiController
    {
        private readonly IDiasFeriadosService diasFeriadosService;

        public DiasFeriadosController (IDiasFeriadosService dfs)
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

        public string Get(int id)
        {
            return "value";
        }

        public IHttpActionResult Post([FromBody] DiasFeriadosModel model)
        {
            return Ok(diasFeriadosService.Insert(model, true));
        }

        // PUT: api/User/5
        public IHttpActionResult Put([FromBody] DiasFeriadosModel model)
        {
            diasFeriadosService.Update(model);
            return Ok(); 
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }

    }
}