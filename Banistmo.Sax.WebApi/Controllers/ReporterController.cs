using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.WebApi.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System.Threading;
using System.IO;
using System.Web;
using System.Net.Http.Headers;
using Banistmo.Sax.WebApi.Results;

namespace Banistmo.Sax.WebApi.Controllers
{

    // [Authorize]
    [RoutePrefix("api/Reporter")]
    public class ReporterController : ApiController
    {
        private readonly IReporterService reporterService;
        private readonly IRegistroControlService registroService;
        private readonly ApplicationRoleManager _appRoleManager;

        const string dateFormat = "MMddyyyy";
        protected ApplicationRoleManager RoleManager
        {
            get
            {
                return _appRoleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        public ReporterController()
        {
        }

        public ReporterController(IReporterService reporterSvc, IRegistroControlService registroSvc)
        {
            reporterService = reporterSvc;
            registroService = registroSvc;
        }

        [Route("GenerateReportRoles")]
        public IHttpActionResult GenerateRoles()
        {
            var listRoles = new List<ApplicationRole>();
            var role = RoleManager.Roles;
            foreach (var item in role)
            {
                var newitem = (ApplicationRole)item;
                listRoles.Add(newitem);
            }
            List<string[]> headerRow = new List<string[]>()
            {
              new string[] { "Id", "Name", "Status" }
            };
            var result = listRoles.Select(c => new
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Estatus
            });
            List<dynamic> dynaList = new List<dynamic>();
            foreach (var item in result)
            {
                dynaList.Add(item.ToDynamic());
            }

            reporterService.CreateReport<ApplicationRole>(new List<string[]>(), listRoles, $"reportRoles{System.DateTime.Now.ToString(dateFormat)}");

            //reporterService.CreateReport<object>(headerRow, result);
            return Ok();
        }

        [Route("GenerateReportRegistro")]
        public IHttpActionResult GenerateRegistro()
        {

            var registro = registroService.GetAll();

            reporterService.CreateReport<RegistroControlModel>(new List<string[]>(), registro, $"reportRoles{System.DateTime.Now.ToString(dateFormat)}");

            return Ok();
        }


        [Route("GenerateReportRolesBinary"), HttpGet]
        public IHttpActionResult GenerateRolesBinary()
        {
            var listRoles = new List<ApplicationRole>();
            var role = RoleManager.Roles;
            foreach (var item in role)
            {
                var newitem = (ApplicationRole)item;
                listRoles.Add(newitem);
            }
            List<string[]> headerRow = new List<string[]>()
            {
              new string[] { "Id", "Name", "Status" }
            };
            var result = listRoles.Select(c => new
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Estatus
            });
            List<dynamic> dynaList = new List<dynamic>();
            foreach (var item in result)
            {
                dynaList.Add(item.ToDynamic());
            }

            var byteresult = reporterService.CreateReportBinary<ApplicationRole>(new List<string[]>(), listRoles, $"reportRoles{System.DateTime.Now.ToString(dateFormat)}");

            //reporterService.CreateReport<object>(headerRow, result);
            return new FileResult(byteresult);
        }
    }


}


