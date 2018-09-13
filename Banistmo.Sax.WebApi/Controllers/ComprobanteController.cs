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
        private IComprobanteDetalleService comprobanteDetalleServ;
        private readonly IPartidasService servicePartida;
        private ApplicationUserManager _userManager;
        private IUsuarioAreaService usuarioAreaService;
        private IUsuarioEmpresaService usuarioEmpService;
        private IAreaOperativaService areaOperativaService;
        private ICatalogoDetalleService catalagoService;

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
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            catalagoService = catalagoService ?? new CatalogoDetalleService();
            comprobanteDetalleServ = comprobanteDetalleServ ?? new ComprobanteDetalleService();

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

        [Route("ListarComprobantesParaConciliar"), HttpGet]
        public IHttpActionResult ListarComprobantesParaConciliar()
        {
            int  estado=Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            List<ComprobanteModel> model = service.GetAllFlatten <ComprobanteModel>(c => c.TC_ESTATUS== estado);
            if (model != null)
            {
                int count = model.Count();
                int CurrentPage = 1;
                int PageSize = 10;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var paginationMetadata = new
                {
                    totalCount = TotalCount,
                    pageSize = PageSize,
                    currentPage = CurrentPage,
                    totalPages = TotalPages,
                    data = items
                };
                return Ok(paginationMetadata);
                
            }
            else
                return BadRequest("No hay partidas por conciliar.");
        }

        [Route("AprobarComprobante"), HttpPost]
        public IHttpActionResult AprobarComprobante(int id)
        {
           
            var model = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (model != null)
            {

                bool result=service.AprobarComprobante(id, User.Identity.GetUserId());
                if (result)
                    return Ok();
                else
                    return BadRequest("No es posible aprobar el registro,  favor contactar al administrador");
            }
            else
                return BadRequest("No se puede aprobar un comprobante que no existe.");
        }

        [Route("RechazarComprobante"), HttpPost]
        public IHttpActionResult RechazarComprobante(int id)
        {
            //

            var model = service.GetSingle(c => c.TC_ID_COMPROBANTE == id);
            if (model != null)
            {

                bool result = service.RechazarComprobante(id, User.Identity.GetUserId());
                if (result)
                    return Ok();
                else
                    return BadRequest("No es posible aprobar el registro,  favor contactar al administrador");
            }
            else
                return BadRequest("No se puede aprobar un comprobante que no existe.");
            //if (model != null)
            //{
            //    model.TC_FECHA_MOD = DateTime.Now;
            //    model.TC_USUARIO_MOD = User.Identity.GetUserId();
            //    model.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.RECHAZADO).ToString();
            //    service.Update(model);
            //    return Ok();
            //}
            //else
            //    return BadRequest("No se puede anular un comprobante que no existe.");

        }

        [Route("AnularComprobante/"), HttpPost]
        public IHttpActionResult AnularComprobante(int id)
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

        [Route("RechazarAnulacion/"), HttpPost]
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

        //Este metodo hay que mejorarlo
        [Route("SolicitarAnulacionComprobante"), HttpPost]
        public IHttpActionResult SolicitarAnulacionComprobantes(AnularPartidas  partidasPorAnular)
        {
            bool result = false;
            var userName = User.Identity.GetUserId();
            result = service.SolicitarAnulaciones(partidasPorAnular.Partidas, userName);
            if (result)
                return Ok();
            else
                return BadRequest("No se puede solicitar una anulacion de un comprobante que no existe.");

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

        [Route("GetRegistroControlPorConciliar")]
        public IHttpActionResult GetRegistroControlPorConciliar([FromUri]PagingRegistroControlModel pagingparametermodel)
        {
            var catalagoServ = new CatalogoService();
            var estatusList = catalagoServ.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var ltsTipoOperacion = catalagoServ.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            int porConciliar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            int manual = Convert.ToInt16(BusinessEnumerations.EstatusCarga.MANUAL);
            var userId = User.Identity.GetUserId();
            List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
            List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
            List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();
            var source = service.Query(c => c.TC_ESTATUS == porConciliar 
                                       && (pagingparametermodel.lote == null ? c.TC_COD_COMPROBANTE == c.TC_COD_COMPROBANTE : c.TC_COD_COMPROBANTE == (pagingparametermodel.lote.Trim()))
                                       && (pagingparametermodel.idCapturador == null ? c.TC_USUARIO_CREACION == c.TC_USUARIO_CREACION : c.TC_USUARIO_CREACION == pagingparametermodel.idCapturador)).OrderBy(c => c.TC_ID_COMPROBANTE);
            if (source.Count() > 0)
                source = source.Where(c => listAreaUsuario.Contains(c.CA_ID_AREA)).OrderBy(c => c.TC_ID_COMPROBANTE);
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
                RC_REGISTRO_CONTROL = x.TC_ID_COMPROBANTE,
                RC_COD_OPERACION = GetNameTipoOperacion(x.TC_COD_OPERACION, ref ltsTipoOperacion),
                RC_COD_PARTIDA = x.TC_COD_COMPROBANTE,
                RC_ARCHIVO = string.Empty,
                RC_TOTAL_REGISTRO = x.TC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.TC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.TC_TOTAL_CREDITO,
                RC_TOTAL = x.TC_TOTAL,
                COD_ESTATUS_LOTE = x.TC_ESTATUS,
                RC_ESTATUS_LOTE = GetStatusRegistroControl(x.TC_ESTATUS, estatusList),
                RC_FECHA_CREACION = x.TC_FECHA_CREACION != null ? x.TC_FECHA_CREACION.ToString("d/M/yyyy") : string.Empty,
                RC_HORA_CREACION = x.TC_FECHA_CREACION != null ? x.TC_FECHA_CREACION.ToString("hh:mm:tt") : string.Empty,
                RC_COD_USUARIO = UserName(x.TC_USUARIO_CREACION),
                SELETED = false
            });
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage,
                data = listItem
            };
            return Ok(paginationMetadata);
        }

        [Route("ListarComprobantesParaAnular"), HttpGet]
        public async Task<IHttpActionResult> consultaRegAnular([FromUri] ComprobanteModels parameter)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();
                int estado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                int? idcompro = null;
                if (parameter != null)
                {
                    if (parameter.comprobanteId != null)
                    {
                        var comprobante = service.GetSingle(x => x.TC_COD_COMPROBANTE == parameter.comprobanteId);
                        if (comprobante == null) {
                            return BadRequest($"No  se puede encontrar el comprobante {parameter.comprobanteId}, favor verifica, e  intentar de nuevo");
                        }
                        idcompro = comprobante.TC_ID_COMPROBANTE;
                    }
                }
                var source = service.ConsultaComprobanteConciliadaServ(parameter == null ? null : parameter.FechaCreacion,
                                                                        parameter == null ? null : parameter.empresaCod,
                                                                        parameter == null ? null : idcompro,
                                                                        parameter == null ? null : parameter.cuentaContableId,
                                                                        parameter == null ? null : parameter.importe,
                                                                        parameter == null ? null : parameter.referencia,
                                                                        null,
                                                                        null,
                                                                        parameter == null ? null : parameter.idCapturador,
                                                                       estado
                                                                        );
                if (source.Count() > 0)
                    source = source.Where(c => listAreaUsuario.Contains(c.CA_ID_AREA));

                var comprobantes = new List<Repository.Model.SAX_COMPROBANTE>();
                int count = source.Count();                
                int CurrentPage = parameter .pageNumber;
                int PageSize = parameter.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = source.OrderBy(x=>x.TC_COD_COMPROBANTE).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
                    data = items.Select(c => new
                    {
                        SELECTED = false,
                        idComprobante = c.TC_ID_COMPROBANTE,
                        codComprobante = c.TC_COD_COMPROBANTE,
                        codOperacion = c.TC_COD_OPERACION,
                        nombreOperacion = (c.TC_COD_OPERACION == 24 ? "AUTOMATICO" : "MANUAL"),
                        fechaProceso = c.TC_FECHA_PROCESO,
                        totalRegistro = c.TC_TOTAL_REGISTRO,
                        totalDebito = c.TC_TOTAL_DEBITO,
                        totalCredito = c.TC_TOTAL_CREDITO,
                        total = c.TC_TOTAL,
                        estatus = c.TC_ESTATUS,
                        nombreEtatus = getEstado(c.TC_ESTATUS),
                        fechaCreacion = c.TC_FECHA_CREACION,
                        usuarioCreacion = c.TC_USUARIO_CREACION,
                        nombreUsuarioCreacion = c.AspNetUsers.FirstName,
                        nombreUsuarioAprobador = c.AspNetUsers1 == null ? null : c.AspNetUsers1.FirstName,
                        fechaMod = c.TC_FECHA_MOD,
                        usuarioMod = c.TC_USUARIO_MOD,
                        nombreUsuarioMod = c.AspNetUsers2 == null ? null : c.AspNetUsers2.FirstName,
                       
                       
                    })
                };

                //HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                return Ok(paginationMetadata);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("ListarComprobantesPorAnular"), HttpGet]
        public async Task<IHttpActionResult> consultaRegPorAnular([FromUri] ComprobanteModels parameter)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();

                //foreach (var item in userArea)
                //{
                //    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                //}
                int estado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_ANULAR);
                int? idcompro = null;
                if (parameter != null)
                {
                    if (parameter.comprobanteId != null)
                    {
                        idcompro = Convert.ToInt16(service.GetSingle(x => x.TC_COD_COMPROBANTE == parameter.comprobanteId).TC_COD_COMPROBANTE);
                    }
                }

                var source = service.ConsultaComprobanteConciliadaServ(parameter == null ? null : parameter.FechaCreacion,
                                                                        parameter == null ? null : parameter.empresaCod,
                                                                        parameter == null ? null : idcompro,
                                                                        parameter == null ? null : parameter.cuentaContableId,
                                                                        parameter == null ? null : parameter.importe,
                                                                        parameter == null ? null : parameter.referencia,
                                                                        null,
                                                                        parameter == null ? null : parameter.lote,
                                                                        parameter == null ? null : parameter.idCapturador,
                                                                       estado
                                                                        ).ToList();
                if (source.Count() > 0)
                    source = source.Where(c => listAreaUsuario.Contains(c.CA_ID_AREA)).ToList();
                //var comprobantes = new List<Repository.Model.SAX_COMPROBANTE>();
                //if (parameter.areaOpe == null)
                //{
                //    foreach (var areaItem in userAreacod)
                //    {
                //        foreach (var item in source)
                //        {
                //            if (item.CA_ID_AREA == areaItem.CA_ID_AREA)
                //            {
                //                comprobantes.Add(item);
                //            }
                //        }
                //    }
                //}
                //else if (parameter.areaOpe != null)
                //{
                //    comprobantes = source.ToList();
                //}
                int count = source.Count();
                int CurrentPage = parameter.pageNumber;
                int PageSize = parameter.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = source.OrderBy(x => x.TC_COD_COMPROBANTE).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
                    data = items.Select(c => new
                    {
                        SELECTED = false,
                        idComprobante = c.TC_ID_COMPROBANTE,
                        codComprobante = c.TC_COD_COMPROBANTE,
                        codOperacion = c.TC_COD_OPERACION,
                        nombreOperacion = (c.TC_COD_OPERACION == 24 ? "AUTOMATICO" : "MANUAL"),
                        fechaProceso = c.TC_FECHA_PROCESO,
                        totalRegistro = c.TC_TOTAL_REGISTRO,
                        totalDebito = c.TC_TOTAL_DEBITO,
                        totalCredito = c.TC_TOTAL_CREDITO,
                        total = c.TC_TOTAL,
                        estatus = c.TC_ESTATUS,
                        nombreEtatus = getEstado(c.TC_ESTATUS),
                        fechaCreacion = c.TC_FECHA_CREACION,
                        usuarioCreacion = c.TC_USUARIO_CREACION,
                        nombreUsuarioCreacion = c.AspNetUsers.FirstName,
                        fechaAprobacion = c.TC_FECHA_APROBACION,
                        usuarioAprobador = c.TC_USUARIO_APROBADOR,
                        nombreUsuarioAprobador = c.AspNetUsers1 == null ? null : c.AspNetUsers1.FirstName,
                        fechaMod = c.TC_FECHA_MOD,
                        usuarioMod = c.TC_USUARIO_MOD,
                        nombreUsuarioMod = c.AspNetUsers2 == null ? null : c.AspNetUsers2.FirstName,
                        usuarioRechazo = c.TC_USUARIO_RECHAZO,
                        nombreUsuarioRechazo = c.AspNetUsers3 == null ? null : c.AspNetUsers3.FirstName,
                        usuarioRechazoFecha = c.TC_FECHA_RECHAZO
                    })
                };

                //HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                return Ok(paginationMetadata);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

        [Route("GetPartidas"), HttpGet]
        public async Task<IHttpActionResult> getPartidasPorIdComprobantes( int idComprobante)
        {
            try
            {
                var comprobante = comprobanteDetalleServ.GetAll(x => x.TC_ID_COMPROBANTE == idComprobante).Select(y=>y.PA_REGISTRO);
                var partidas = servicePartida.GetAll(x => comprobante.Contains(x.PA_REGISTRO)).ToList();

                if (partidas != null)
                {
                    return Ok(partidas.OrderBy(x=>x.PA_REGISTRO));
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

        private string getEstado(int idEstado)
        {
            string result = string.Empty;
            var estado = catalagoService.GetSingle(d => d.CD_ESTATUS == idEstado && d.CA_ID_CATALOGO == 14);
            if (estado != null)
                result = estado.CD_VALOR;
            return result;
        }

        private string GetStatusRegistroControl(int idStatus, CatalogoModel model)
        {
            string result = string.Empty;
            if (model != null)
            {
                var modelCatalogoDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS == idStatus).FirstOrDefault();
                if (modelCatalogoDetalle != null)
                    result = modelCatalogoDetalle.CD_VALOR;
            }
            return result;
        }

        private string GetNameTipoOperacion(int id, ref CatalogoModel model)
        {
            string name = string.Empty;
            if (model != null)
            {
                CatalogoDetalleModel cataloDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS == id).FirstOrDefault();
                if (cataloDetalle != null)
                    name = cataloDetalle.CD_VALOR;
            }
            return name;
        }

        private string UserName(string id)
        {
           var userService = new UserService();
            string result = string.Empty;
            AspNetUserModel usuario = userService.GetSingle(u => u.Id == id);
            if (usuario != null)
                result = usuario.FirstName;
            return result;

        }

    }
}
