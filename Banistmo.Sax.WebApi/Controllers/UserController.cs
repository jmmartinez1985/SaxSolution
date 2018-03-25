using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {

        private readonly IUserService userService;
        private readonly IReporteService reporteSrv;
        private readonly IReporteRolesMenuService rrmService;

        public UserController(IUserService usr, IReporteService reporte, IReporteRolesMenuService rrmSrv)
        {
            userService = usr;
            reporteSrv = reporte;
            rrmService = rrmSrv;
        }
        // GET: api/User
        public IHttpActionResult Get()
        {
            List<AspNetUserModel> user = userService.GetAll();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/User/5
        public IHttpActionResult GetUsuario(string id)
        {
            var usuario = userService.GetSingle(c => c.Id == id);

            if (usuario != null)
            {
                return Ok(usuario);
            }
            return NotFound();
        }

        // POST: api/User
        //public IHttpActionResult Post([FromBody] AspNetUserModel model)
        //{
        //    userService.Insert(model, true);
        //    return Ok();
        //}

        public IHttpActionResult Put([FromBody] AspNetUserModel model)
        {
            userService.Update(model);
            return Ok();
        }

        [Route("ReporterUser"), HttpGet]
        public IHttpActionResult GetDataReporterUser()
        {
            return Ok(reporteSrv.GetReporte());
        }

        [Route("ReporteRolesMenu"),HttpGet]
        public IHttpActionResult ReporteRolesMenu()
        {
            return Ok(rrmService.GetReporte());
        }

    }
}
