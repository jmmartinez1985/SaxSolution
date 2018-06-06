using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;
using Banistmo.Sax.Common;
using Banistmo.Sax.WebApi.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web;
using Newtonsoft.Json;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Comprobante")]
    public class ComprobanteController : ApiController
    {
        private readonly IComprobanteService service;
        private readonly IPartidasService servicePartida;
        private ApplicationUserManager _userManager;
        private IUsuarioAreaService usuarioAreaService;
        private IUsuarioEmpresaService usuarioEmpService;

        //public ComprobanteController()
        //{
        //    service = service ?? new ComprobanteService();
        //}

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ComprobanteController(IComprobanteService svc, IPartidasService svcPart, IUsuarioEmpresaService usEmpServ)
        {
            service = svc;
            servicePartida = svcPart;
            usuarioAreaService = new UsuarioAreaService();
            usuarioEmpService = usEmpServ;
        }

        public IHttpActionResult Get()
        {
            List<ComprobanteModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] ComprobanteModel model)
        {
            model.TC_USUARIO_CREACION = User.Identity.GetUserId();
            model.TC_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateComprobante"), HttpPost]
        public IHttpActionResult Put([FromBody] ComprobanteModel model)
        {
            model.TC_FECHA_MOD = DateTime.Now;
            model.TC_USUARIO_MOD = User.Identity.GetUserId();
            service.Update(model);
            return Ok();
        }

        [Route("AnularComprobante/{id:int}"), HttpPost]
        public IHttpActionResult AnularComprobante( int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (control != null)
            {
                var userName = User.Identity.GetUserId();
                service.AnularComprobante(id, userName);
                return Ok();
            }
            else
                return BadRequest("No se puede anular un comprobante que no existe.");
        }

        [Route("SolicitarAnulacion/{id:int}"), HttpPost]
        public IHttpActionResult SolicitarAnulacion(int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (control != null)
            {
                var userName = User.Identity.GetUserId();
                service.SolitarAnulacion(control, userName);
                return Ok();
            }
            else
                return BadRequest("No se puede solicitar una anulacion de un comprobante que no existe.");
        }


        [Route("RechazarAnulacion/{id:int}"), HttpPost]
        public IHttpActionResult RechazarAnulacion(int id)
        {
            var control = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (control != null)
            {
                var userName = User.Identity.GetUserId();
                service.RechazarAnulacion(control, userName);
                return Ok();
            }
            else
                return BadRequest("No se puede rechazar una anulacion de un comprobante que no existe.");
        }

        [Route("ConciliacionManual"), HttpPost]
        public IHttpActionResult ConciliacionManual([FromBody] ConciliacionModel details)
        {
            var control = details.PartidasConciliar.Count;
            if (control > 0)
            {
                var userName = User.Identity.GetUserId();
                service.ConciliacionManual(details.PartidasConciliar, userName);
                return Ok();
            }
            else
                return BadRequest("Debe seleccionar partidas a conciliar.");
        }

        [Route("ListarComprobantesParaAnular"), HttpGet]
        public IHttpActionResult consultaRegAnular([FromUri] ComprobanteModels1 parameter)
        {
            try
            {
                var model = service.ConsultaComprobanteConciliadaServ(parameter == null ? null : parameter.FechaCreacion,
                                                                        parameter == null ? null : parameter.empresaCod,
                                                                        parameter == null ? null : parameter.comprobanteId,
                                                                        parameter == null ? null : parameter.cuentaContableId,
                                                                        parameter == null ? null : parameter.importe,
                                                                        parameter == null ? null : parameter.referencia);
                if (model.Count > 0)
                {
                    int count = model.Count();                   
                    int CurrentPage = parameter.pageNumber;
                    int PageSize = parameter.pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                    var modellist = items.Select((item, index) => new
                    {
                        idComprobante = item.TC_COD_COMPROBANTE,
                        codComprobante = item.TC_COD_OPERACION,
                        fechaProceso = item.TC_FECHA_PROCESO,
                        totalRegistro = item.TC_TOTAL_REGISTRO,
                        totalDebito = item.TC_TOTAL_DEBITO,
                        totalCredito = item.TC_TOTAL_CREDITO,
                        total = item.TC_TOTAL,
                        estatus = item.TC_ESTATUS,
                        fechaCreacion = item.TC_FECHA_CREACION,
                        usuarioCreacion = item.TC_USUARIO_CREACION,
                        nombreUsuarioCreacion = item.AspNetUsers.FirstName,
                        fechaAprobacion = item.TC_FECHA_APROBACION,
                        usuarioAprobador = item.TC_USUARIO_APROBADOR,
                        nombreUsuarioAprobador = item.AspNetUsers1.FirstName,
                        fechaMod = item.TC_FECHA_MOD,
                        usuarioMod = item.TC_USUARIO_MOD,
                        nombreUsuarioMod = item.AspNetUsers2.FirstName,
                        usuarioRechazo = item.TC_USUARIO_RECHAZO,
                        nombreUsuarioRechazo = item.AspNetUsers3.FirstName,
                        usuarioRechazoFecha = item.TC_FECHAN_RECHAZO                        
                    });

                    var modelPaginationMetadata = new
                    {
                        totalCount = TotalCount,
                        pageSize = PageSize,
                        currentPage = CurrentPage,
                        totalPages = TotalPages,
                        previousPage,
                        nextPage,
                        data=modellist
                    };

                    return Ok(modelPaginationMetadata);
                }
                else
                {
                    return Ok("Consulta no produjo resultados");
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class ComprobanteModels1
        {
            public DateTime? FechaCreacion { get; set; }
            public string empresaCod { get; set; }
            public int? comprobanteId { get; set; }
            public int? cuentaContableId { get; set; }
            public decimal? importe { get; set; }
            public string referencia { get; set; }

            const int maxPageSize = 20;

            public int pageNumber { get; set; } = 1;

            internal int _pageSize { get; set; } = 10;

            public int pageSize
            {

                get { return _pageSize; }
                set
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }

        [Route("ListarComprobante"), HttpGet]
        public async Task<IHttpActionResult> listarComprobante()
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);
                //Cuando se agregue el campo de area en la tabal de comprobante se cambiará el campo TC_ID_COMPROBANTE
                //por el nuevo campos de área en el comprobante.
                var model = service.GetAll(c=>c.SAX_AREA_OPERATIVA.CA_ID_AREA == userArea.SAX_AREA_OPERATIVA.CA_ID_AREA);

                if (model.Count > 0)
                {
                    var result = model.Select(c => new
                    {
                        idComprobante = c.TC_ID_COMPROBANTE,
                        codComprobante = c.TC_COD_OPERACION
                    });
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("ListarEmpresaComprobante"), HttpGet]
        public async Task<IHttpActionResult> listarEmpresaComprobante()
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var useremp = usuarioEmpService.GetAll(d => d.US_ID_USUARIO == user.Id);
                
                if (useremp.Count > 0)
                {
                    var result = useremp.Select(c => new
                    {
                        idEmpresa = c.SAX_EMPRESA.CE_ID_EMPRESA,
                        codEmpresa = c.SAX_EMPRESA.CE_COD_EMPRESA,
                        nombreEmpresa = c.SAX_EMPRESA.CE_NOMBRE
                    });
                    return Ok(useremp);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        
        [Route("ListarCuentasContables"), HttpGet]
        public async Task<IHttpActionResult> CuentaContablePartidasPorArea()
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var model = service.ListarCuentasContables(user.Id);              

                if (model != null)
                {                   
                    return Ok(model);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

     
    }
}
