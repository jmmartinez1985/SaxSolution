using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Registro")]
    public class RegistroControlController : ApiController
    {
        private  IRegistroControlService service;
        private  IOnlyRegistroControlService srvOnlyRegistroControl;
        private  IUserService userService;
        private  ICatalogoService catalagoService;

        public RegistroControlController()
        {
            service = service ?? new RegistroControlService();
            srvOnlyRegistroControl = srvOnlyRegistroControl ?? new OnlyRegistroControlService();
            userService = new UserService();
            catalagoService = new CatalogoService();
        }

        //public RegistroControlController(IRegistroControlService rc, IOnlyRegistroControlService rcOnlyRegistro)
        //{
        //    service = rc;
        //    srvOnlyRegistroControl = rcOnlyRegistro;
        //    userService = new UserService();
        //}

        [Route("GetAllRegistro")]
        public IHttpActionResult GetAll()
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var mdl = service.GetAll(c=>c.RC_ESTATUS_LOTE == activo.ToString()).Select( c=> new {
                Registro = c.RC_REGISTRO_CONTROL,
                Area = c.RC_COD_AREA
            });
            if (mdl == null)
            {
                return NotFound();
            }
            return Ok(mdl);
        }

        [Route("GetRegistroByUser")]
        public IHttpActionResult GetRegistroByUser()
        {
            var userId = User.Identity.GetUserId();
            var mdl = service.GetAll(c=> c.RC_COD_USUARIO == userId,null,null);
            if (mdl == null)
            {
                return NotFound();
            }

            return Ok(mdl);
        }

        [Route("GetRegistroControlByUser")]
        public IHttpActionResult GetRegistroControlByUser()
        {
            var userId = User.Identity.GetUserId();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<OnlyRegistroControlModel> mdl = srvOnlyRegistroControl.GetAll(c => c.RC_COD_USUARIO == userId && c.RC_ESTATUS_LOTE== activo.ToString());
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE);
            if (mdl == null)
            {
                return NotFound();
            }

            return Ok(mdl.Select( x=> new  {
                RC_COD_OPERACION = x.RC_COD_OPERACION,
                RC_COD_PARTIDA = x.RC_COD_PARTIDA,
                RC_ARCHIVO = x.RC_ARCHIVO,
                RC_TOTAL_REGISTRO= x.RC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.RC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.RC_TOTAL_CREDITO,
                RC_TOTAL = x.RC_TOTAL,
                RC_ESTATUS_LOTE = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(e=>e.CD_TABLA.ToString()==x.RC_ESTATUS_LOTE),
                RC_FECHA_CREACION = x.RC_FECHA_CREACION!=null? x.RC_FECHA_CREACION.ToString("d/M/yyyy"): string.Empty,
                RC_HORA_CREACION = x.RC_FECHA_CREACION != null?  x.RC_FECHA_CREACION.ToString("hh:mm:tt"): string.Empty,
                RC_COD_USUARIO = UserName(x.RC_COD_USUARIO)
            }));
        }


        [Route("GetRegistroByUserPag")]
        public IHttpActionResult GetRegistroByUserPag([FromUri]PagingParameterModel pagingparametermodel)
        {
           var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            //estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE

            var userId = User.Identity.GetUserId();
            var source = service.GetAllFlatten<RegistroControlModel>(c => c.RC_COD_USUARIO == userId);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var listItem = items.Select(x => new
            {
                RC_COD_OPERACION = x.RC_COD_OPERACION=="I"?"Inicial":"Masiva",
                RC_COD_PARTIDA = x.RC_COD_PARTIDA,
                RC_ARCHIVO = x.RC_ARCHIVO,
                RC_TOTAL_REGISTRO = x.RC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.RC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.RC_TOTAL_CREDITO,
                RC_TOTAL = x.RC_TOTAL,
                RC_ESTATUS_LOTE = GetStatusRegistroControl(x.RC_ESTATUS_LOTE, estatusList) ,
                RC_FECHA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("d/M/yyyy") : string.Empty,
                RC_HORA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("hh:mm:tt") : string.Empty,
                RC_COD_USUARIO = UserName(x.RC_COD_USUARIO)
            });
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage,
                data= listItem
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(paginationMetadata);
        }

        [Route("GetRegistroById")]
        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.RC_REGISTRO_CONTROL == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] RegistroControlModel model)
        {
            model.RC_USUARIO_CREACION = User.Identity.GetUserId();
            model.RC_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateRegistro"), HttpPost]
        public IHttpActionResult Put([FromBody] RegistroControlModel model)
        {
            model.RC_USUARIO_MOD = User.Identity.GetUserId();
            model.RC_FECHA_MOD = DateTime.Now;
            service.Update(model);
            return Ok();
        }

        [Route("AprobarRegistro"), HttpGet]
        public IHttpActionResult AprobarRegistro(int id)
        {
            var control = service.GetSingle(c => c.RC_REGISTRO_CONTROL == id);
            if (control != null)
            {
                control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO).ToString();
                control.RC_USUARIO_APROBADOR = User.Identity.GetUserId();
                control.RC_FECHA_APROBACION = DateTime.Now;
                service.Update(control);
                return Ok();
            }
            else
                return NotFound();
        }

        private string UserName(string id) {
            string result = string.Empty;
            AspNetUserModel usuario = userService.GetSingle(u => u.Id == id);
            if(usuario!=null)
                result = usuario.FirstName;
            return result;

        }

        private string GetStatusRegistroControl(string idStatus, CatalogoModel model) {
            int status = Convert.ToInt16(idStatus);
            string result = string.Empty;
            if (model != null) {
                var modelCatalogoDetalle=model.SAX_CATALOGO_DETALLE.Where(x=>x.CD_ESTATUS== status).FirstOrDefault();
                if (modelCatalogoDetalle != null)
                    result= modelCatalogoDetalle.CD_VALOR;
            }
            return result;
        }

      
    }
}
