using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Newtonsoft.Json;
using System.Web;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/CuentaContable")]
    public class CuentaContableController : ApiController
    {
        private readonly ICuentaContableService service;

        //public CuentaContableController()
        //{
        //    service = service ?? new CuentaContableService();
        //}

        public CuentaContableController(ICuentaContableService svc)
        {
            service = svc;
        }

        public IHttpActionResult Get()
        {
            List<CuentaContableModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CO_ID_CUENTA_CONTABLE == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CuentaContableModel model)
        {
            model.CO_USUARIO_CREACION = User.Identity.GetUserId();
            model.CO_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateCuenta"), HttpPost]
        public IHttpActionResult Put([FromBody] CuentaContableModel model)
        {
            model.CO_USUARIO_MOD = User.Identity.GetUserId();
            model.CO_FECHA_MOD = DateTime.Now;
            model.CO_ESTATUS = Convert.ToInt16(BusinessEnumations.Estatus.ACTIVO);
            service.Update(model);
            return Ok();
        }



        [Route("GetCuentaContablePag"),HttpPost]
        public IHttpActionResult GetPagination(PagingParameterModel pagingparametermodel)
        {
            var source = service.GetAll().OrderBy(c => c.CO_ID_CUENTA_CONTABLE);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage,
                data= items
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(paginationMetadata);

        }
    }
}
