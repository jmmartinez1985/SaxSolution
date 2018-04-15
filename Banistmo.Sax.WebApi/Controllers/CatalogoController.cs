using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System.Threading.Tasks;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Catalogo")]
    public class CatalogoController : ApiController
    {
        private readonly ICatalogoService service;

        //public CatalogoController()
        //{
        //    service = service ?? new CatalogoService();
        //}

        public CatalogoController(ICatalogoService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<CatalogoModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs.Select(c => new {
                Id = c.CA_ID_CATALOGO,
                Description = c.CA_DESCRIPCION,
                Table = c.CA_TABLA
            }));
        }


        [Route("GetByCatalogo")]
        public async Task<IHttpActionResult> GetByCatalogo(string cat)
        {
            var  catalogo = await service.GetAllAsync(c => c.CA_TABLA == cat, c => c.SAX_CATALOGO_DETALLE  );
            if (catalogo == null)
            {
                return NotFound();
            }
            var hasItem = catalogo.FirstOrDefault();
            if(hasItem == null)
                return BadRequest("No existe el catalogo.");
            var detalle = hasItem.SAX_CATALOGO_DETALLE;
            return Ok(detalle.Select(c=> new {
                Id = c.CD_ID_CATALOGO_DETALLE, Description = c.CD_VALOR
            }));
        }

        
        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CA_ID_CATALOGO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CatalogoModel model)
        {
            model.CA_FECHA_CREACION = DateTime.Now;
            model.CA_USUARIO_CREACION = User.Identity.GetUserId();
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateCatalogo"), HttpPost]
        public IHttpActionResult Put([FromBody] CatalogoModel model)
        {
            model.CA_USUARIO_MOD = User.Identity.GetUserId();
            model.CA_FECHA_MOD = DateTime.Now;
            service.Update(model);
            return Ok();
        }
    }
}
