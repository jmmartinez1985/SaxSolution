﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/EmpresaCentro")]
    public class EmpresaCentroController : ApiController
    {
        private readonly IEmpresaCentroService service;

        //public EmpresaCentroController()
        //{
        //    service = service ?? new EmpresaCentroService();
        //}

        public EmpresaCentroController(IEmpresaCentroService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<EmpresaCentroModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.EC_ID_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] EmpresaCentroModel model)
        {
            model.EC_ESTATUS = 1;
            return Ok(service.Insert(model, true));
        }


        [Route("UpdateEmpresaCentro"), HttpPost]
        public IHttpActionResult Put([FromBody] EmpresaCentroModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
