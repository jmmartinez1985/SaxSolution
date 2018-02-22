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
    public class PaisController : ApiController
    {
        private readonly IPaisService paisService;

        public PaisController(IPaisService pais)
        {
            paisService = pais;
        }
        
        public IHttpActionResult Create(PaisModel model) {
            paisService.Insert(model);
            return Ok();
        }

        public IHttpActionResult Get()
        {
           var paises = paisService.GetAll();
           return Ok(paises);
        }
    }
}
