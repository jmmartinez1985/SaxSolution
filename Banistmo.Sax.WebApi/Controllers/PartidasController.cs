using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using log4net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Partidas")]
    public class PartidasController : ApiController
    {
        private readonly IPartidasService partidasService;
        private readonly IEmpresaService empresaService;
        private readonly IReporterService reportExcelService;
        private readonly ICatalogoService catalogoService;
        private IAreaOperativaService areaOperativaService;
        private readonly ICatalogoDetalleService catalagoDetalleService;
        private readonly IRegistroControlService registroService;
        private readonly IUserService usuarioSerive;
        private readonly IPartidasAprobadasService partidasAprobadas;
        private ApplicationUserManager _userManager;
        private IUsuarioAreaService usuarioAreaService;
        private readonly IComprobanteService comprobanteService;
        private IUsuarioEmpresaService usuarioEmpService;
        public PartidasController()
        {
            empresaService = empresaService ?? new EmpresaService();
            reportExcelService = reportExcelService ?? new ReporterService();
            partidasService = partidasService ?? new PartidasService();
            registroService = registroService ?? new RegistroControlService();
            usuarioSerive = usuarioSerive ?? new UserService();
            partidasAprobadas = partidasAprobadas ?? new PartidasAprobadasService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            usuarioEmpService = usuarioEmpService ?? new UsuarioEmpresaService();
        }
        //public PartidasController(IPartidasService part, IEmpresaService em, IReporterService rep)
        //{
        //    partidasService = part;
        //    empresaService = em;
        //    reportExcelService = rep;
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
        public PartidasController(IPartidasService part, IEmpresaService em, IReporterService rep, ICatalogoService cat, 
            IAreaOperativaService area, IRegistroControlService registro, IUserService usuario, 
            IPartidasAprobadasService partAprob, ICatalogoDetalleService catDet,
            IUsuarioAreaService userArea)
        {
            partidasService = part;
            empresaService = em;
            reportExcelService = rep;
            catalogoService = cat;
            areaOperativaService = area;
            registroService = registro;
            usuarioSerive = usuario;
            partidasAprobadas = partAprob;
            catalagoDetalleService = catDet;
            usuarioAreaService = userArea;
        }

        public IHttpActionResult Get()
        {
            var mdl = partidasService.GetAll(null, null, c => c.SAX_REGISTRO_CONTROL);
            if (mdl == null)
            {
                return NotFound();
            }
            return Ok(mdl);
        }

        [Route("GetPartidaById")]
        public IHttpActionResult Get(int id)
        {
            var model = partidasService.GetSingle(c => c.PA_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [Route("GetPartidaPorAprobar"), HttpGet]
        public async Task<IHttpActionResult> GetPartidaPorAprobar([FromUri] PagingParameterModel pagingparametermodel)
        {
            try
            {
                int capManual = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL);
                int capInicial = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL);
                int status = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
                int concilia = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);

                var model = partidasAprobadas.GetAll(p => p.RC_COD_OPERACION == capManual
                                             && p.RC_COD_OPERACION == capInicial
                                             && p.PA_FECHA_CREACION.Value.Year == DateTime.Now.Year
                                             && p.PA_FECHA_TRX.Value.Month == DateTime.Now.Month                                            
                                             && p.PA_STATUS_PARTIDA == status
                                             && p.PA_ESTADO_CONCILIA == concilia
                                             && p.RC_COD_AREA == userArea.CA_ID_AREA);

                int count = model.Count();
                int CurrentPage = pagingparametermodel.pageNumber;
                int PageSize = pagingparametermodel.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                var paginationMetadata = new
                {
                    totalCount = TotalCount,
                    pageSize = PageSize,
                    currentPage = CurrentPage,
                    totalPages = TotalPages,
                    previousPage,
                    nextPage
                };
                HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                return Ok(items);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("GetPartidasByUser")]
        public IHttpActionResult GetPartidasByUser(String id)
        {
            var model = partidasService.GetAll(c => c.PA_USUARIO_CREACION == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [Route("GetPartidasByUserPag"), HttpGet]
        public IHttpActionResult PartidasByUserPagination(String id, [FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAllFlatten<PartidasModel>(c => c.PA_USUARIO_CREACION == id).OrderBy(c => c.RC_REGISTRO_CONTROL).OrderBy(c => c.PA_CONTADOR);
            var listEmpresas = empresaService.GetAll();
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            foreach (var row in items)
            {
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);

        }


        [Route("FindPartida"), HttpPost]
        //public IHttpActionResult FindPartida(PartidasModel parms int idRegistro, string idEmpresa,string idCuentaContable, decimal importe,string referencia)
        public IHttpActionResult FindPartida(PartidaModel parms)
        {
            List<EmpresaModel> listEmpresas = empresaService.GetAllFlatten<EmpresaModel>();
            string codEmpresa = string.Empty;
            if (!String.IsNullOrEmpty(parms.PA_COD_EMPRESA))
            {
                int idEmpresa = Convert.ToInt16(parms.PA_COD_EMPRESA);
                var singleEmpresa = empresaService.GetSingle(x => x.CE_ID_EMPRESA == idEmpresa);
                if (singleEmpresa != null)
                    codEmpresa = singleEmpresa.CE_COD_EMPRESA;
            }
            List<PartidasModel> model = partidasService.GetAllFlatten<PartidasModel>(
                c => c.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL
                && c.PA_CTA_CONTABLE == (string.IsNullOrEmpty(parms.PA_CTA_CONTABLE) ? c.PA_CTA_CONTABLE : parms.PA_CTA_CONTABLE)
                && c.PA_IMPORTE == (parms.PA_IMPORTE == null ? c.PA_IMPORTE : parms.PA_IMPORTE)
                && c.PA_REFERENCIA == (string.IsNullOrEmpty(parms.PA_REFERENCIA) ? c.PA_REFERENCIA : parms.PA_REFERENCIA)
                && c.PA_COD_EMPRESA == (string.IsNullOrEmpty(codEmpresa) ? c.PA_COD_EMPRESA : codEmpresa)
                ).OrderBy(c => c.PA_CONTADOR).ToList();
            var registroControl = registroService.GetSingle(x => x.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL);
            var usuario = usuarioSerive.GetSingle(x => x.Id == registroControl.RC_COD_USUARIO);
            int count = model.Count();
            int CurrentPage = parms.pageNumber;
            int PageSize = parms.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            foreach (var row in items)
            {
                row.RC_USUARIO_NOMBRE = usuario.FirstName;
                row.RC_COD_PARTIDA = registroControl.RC_COD_PARTIDA;
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }
            var paginacion = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data = items
            };

            if (paginacion != null)
            {
                return Ok(paginacion);
            }

            return NotFound();
        }

        public IHttpActionResult Post([FromBody] PartidasModel model)
        {
            model.PA_USUARIO_CREACION = User.Identity.GetUserId();
            model.PA_FECHA_CREACION = DateTime.Now;
            return Ok(partidasService.Insert(model, true));
        }

        public IHttpActionResult Put([FromBody] PartidasModel model)
        {
            model.PA_USUARIO_MOD = User.Identity.GetUserId();
            model.PA_FECHA_MOD = DateTime.Now;
            partidasService.Update(model);
            return Ok();
        }

        [Route("GetAllPartidaPag")]
        public IHttpActionResult GetAllPagination([FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAll().OrderBy(c => c.RC_REGISTRO_CONTROL);
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
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);

        }

        [Route("GetPartidaPag")]
        public IHttpActionResult GetPagination(int partida, [FromUri]PagingParameterModel pagingparametermodel)
        {
            var source = partidasService.GetAllFlatten<PartidasModel>(c => c.RC_REGISTRO_CONTROL == partida).OrderBy(c => c.PA_CONTADOR);
            var registroControl = registroService.GetSingle(x => x.RC_REGISTRO_CONTROL == partida);
            var usuario = usuarioSerive.GetSingle(x => x.Id == registroControl.RC_COD_USUARIO);
            var listEmpresas = empresaService.GetAll();
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            foreach (var row in items)
            {
                row.RC_USUARIO_NOMBRE = usuario.FirstName;
                row.RC_COD_PARTIDA = registroControl.RC_COD_PARTIDA;
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }

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

        [Route("GetReporteExcel"), HttpGet]
        public HttpResponseMessage GetReporteExcel([FromUri]PartidaModel parms)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            List<EmpresaModel> listEmpresas = empresaService.GetAllFlatten<EmpresaModel>();
            string codEmpresa = string.Empty;
            if (!String.IsNullOrEmpty(parms.PA_COD_EMPRESA))
            {
                int idEmpresa = Convert.ToInt16(parms.PA_COD_EMPRESA);
                var singleEmpresa = empresaService.GetSingle(x => x.CE_ID_EMPRESA == idEmpresa);
                if (singleEmpresa != null)
                    codEmpresa = singleEmpresa.CE_COD_EMPRESA;
            }
            List<PartidasModel> model = partidasService.GetAllFlatten<PartidasModel>(
                c => c.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL
                && c.PA_CTA_CONTABLE == (string.IsNullOrEmpty(parms.PA_CTA_CONTABLE) ? c.PA_CTA_CONTABLE : parms.PA_CTA_CONTABLE)
                && c.PA_IMPORTE == (parms.PA_IMPORTE == null ? c.PA_IMPORTE : parms.PA_IMPORTE)
                && c.PA_REFERENCIA == (string.IsNullOrEmpty(parms.PA_REFERENCIA) ? c.PA_REFERENCIA : parms.PA_REFERENCIA)
                && c.PA_COD_EMPRESA == (string.IsNullOrEmpty(codEmpresa) ? c.PA_COD_EMPRESA : codEmpresa)
                ).OrderBy(c => c.PA_CONTADOR).ToList();
            var registroControl = registroService.GetSingle(x => x.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL);
            var usuario = usuarioSerive.GetSingle(x => x.Id == registroControl.RC_COD_USUARIO);
            int count = model.Count();
            foreach (var row in model)
            {
                row.RC_USUARIO_NOMBRE = usuario.FirstName;
                row.RC_COD_PARTIDA = registroControl.RC_COD_PARTIDA;
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
            }
            var dt = model.ToList().AnonymousToDataTable();
            byte[] fileExcell = reportExcelService.CreateReportBinary(dt, "Hoja1");
            var contentLength = fileExcell.Length;
            //200
            //successful
            var statuscode = HttpStatusCode.OK;
            response = Request.CreateResponse(statuscode);
            response.Content = new StreamContent(new MemoryStream(fileExcell));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = contentLength;
            ContentDispositionHeaderValue contentDisposition = null;
            if (ContentDispositionHeaderValue.TryParse("inline; filename=" + "document" + ".xlsx", out contentDisposition))
            {
                response.Content.Headers.ContentDisposition = contentDisposition;
            }
            return response;
        }

        [Route("Generate"), HttpGet]
        public HttpResponseMessage Generate()
        {
            var stream = new MemoryStream();
            // processing the stream.

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "CertificationCard.pdf"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

        [Route("EditarPlanAccion"), HttpPost]
        public IHttpActionResult EditarPlanAccion([FromBody] PlanAccionModel plan)
        {
            var partida = partidasService.GetSingle(c => c.PA_REGISTRO == plan.PA_REGISTRO);
            if (partida != null)
            {
                partida.PA_PLAN_ACCION = plan.PA_PLAN_ACCION;
                partida.PA_USUARIO_MOD = User.Identity.GetUserId();
                partida.PA_FECHA_MOD = DateTime.Now;
                partidasService.Update(partida);
                return Ok();
            }
            return BadRequest("Debe seleccionar partidas validas para cambiar plan de accion.");
        }

        [Route("GetTipoCarga"), HttpGet]
        public IHttpActionResult GetTipoCarga()
        {
            var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE);

            if (estatusList != null)
            {
                return Ok(estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.Select(c => new
                {
                    idTipoCarga = c.CD_ID_CATALOGO_DETALLE,
                    tipoCarga = c.CD_VALOR

                }));
            }
            return BadRequest("No se encontraron datos para la lista.");
        }

        [Route("GetTipoConciliacion"), HttpGet]
        public IHttpActionResult GetTipoConciliacion()
        {
            var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_tipo_conciliacion", null, c => c.SAX_CATALOGO_DETALLE);

            if (estatusList != null)
            {
                return Ok(estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.Select(c => new
                {
                    idTipoConciliacion = c.CD_ID_CATALOGO_DETALLE,
                    tipoConciliacion = c.CD_VALOR

                }));
            }
            return BadRequest("No se encontraron datos para la lista.");
        }

        [Route("GetAreaOperativa"), HttpGet]
        public IHttpActionResult GetAreaOperativa()
        {
            List<AreaOperativaModel> ar = areaOperativaService.GetAllFlatten<AreaOperativaModel>(a => a.CA_ESTATUS == 1);
            if (ar == null)
            {
                return BadRequest("No se encontraron datos para la lista.");
            }
            return Ok(ar.Select(c => new
            {
                idArea = c.CA_COD_AREA,
                nombreArea = c.CA_NOMBRE
                //CA_ID_AREA = c.CA_ID_AREA,
                //CA_COD_AREA = c.CA_COD_AREA,
                //CA_NOMBRE = c.CA_NOMBRE,
                //CA_ESTATUS = c.CA_ESTATUS,
                //ESTATUS_TXT = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.CA_ESTATUS).CD_VALOR,
                //CA_FECHA_CREACION = c.CA_FECHA_CREACION,
                //CA_USUARIO_CREACION = c.CA_USUARIO_CREACION,
                //CA_FECHA_MOD = c.CA_FECHA_MOD,
                //CA_USUARIO_MOD = c.CA_USUARIO_MOD
            }));
        }

        [Route("GetEmpresa"), HttpGet]
        public IHttpActionResult GetEmpresa()
        {
            List<EmpresaModel> empresa = empresaService.GetAllFlatten<EmpresaModel>(a => a.CE_ESTATUS == "1");
            if (empresa == null)
            {
                return BadRequest("No se encontraron datos para la lista.");
            }
            return Ok(empresa.Select(c => new
            {
                idEmpresa = c.CE_COD_EMPRESA,
                nombreEmpresa = c.CE_NOMBRE
            }));
        }

        [Route("GetConsultaPartidasAprobadas"), HttpGet]
        public IHttpActionResult GetConsultaPartidasAprobadas([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {
            if (partidasParameters == null)
            {
                partidasParameters = new ParametrosPartidasAprobadas();
                partidasParameters.codArea = null;
                partidasParameters.codEmpresa = null;
                partidasParameters.cuentaContable = null;
                partidasParameters.estatusConciliacion = null;
                partidasParameters.fechaCarga = null;
                partidasParameters.fechaConciliacion = null;
                partidasParameters.fechaTransaccion = null;
                partidasParameters.importe = null;
                partidasParameters.referencia = null;
                partidasParameters.tipoCarga = null;
            }

            var source = partidasAprobadas.GetAll(

                c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)

                ).OrderBy(c => c.RC_REGISTRO_CONTROL);

            int count = source.Count();
            int CurrentPage = partidasParameters.pageNumber;
            int PageSize = partidasParameters.pageSize;
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
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);
        }

        [Route("GetConsultaPlan"), HttpGet]
        public IHttpActionResult GetConsultaPlan([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {

            var source = partidasAprobadas.GetAll(

                c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                && c.PA_COD_EMPRESA == (partidasParameters.codEmpresa == null ? c.PA_COD_EMPRESA : partidasParameters.codEmpresa)
                && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)

                ).OrderBy(c => c.RC_REGISTRO_CONTROL);

            int count = source.Count();
            int CurrentPage = partidasParameters.pageNumber;
            int PageSize = partidasParameters.pageSize;
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
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);
        }

        [Route("GetReportePartidasAprobadas"), HttpGet]
        public HttpResponseMessage GetReporteCuentaConcilia([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();

            if (partidasParameters == null)
            {
                partidasParameters = new ParametrosPartidasAprobadas();
                partidasParameters.codArea = null;
                partidasParameters.codEmpresa = null;
                partidasParameters.cuentaContable = null;
                partidasParameters.estatusConciliacion = null;
                partidasParameters.fechaCarga = null;
                partidasParameters.fechaConciliacion = null;
                partidasParameters.fechaTransaccion = null;
                partidasParameters.importe = null;
                partidasParameters.referencia = null;
                partidasParameters.tipoCarga = null;
            }

            var partidas = partidasAprobadas.GetAll(

                c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                ).OrderBy(c => c.RC_REGISTRO_CONTROL);


            var source = partidas.Select(c => new
            {
                Usuario = c.UsuarioC_Nombre,
                Lote = c.RC_COD_PARTIDA,
                Numero = c.PA_CONTADOR,
                Empresa = c.EmpresaDesc,
                FechaCarga = c.PA_FECHA_CARGA,
                FechaTransaccion = c.PA_FECHA_TRX,
                CtaContable = c.PA_CTA_CONTABLE,
                NombreCtaContable = c.PA_CTA_CONTABLE, // Falta
                CentroCosto = c.CentroCostoDesc,
                Moneda = c.MonedaDesc,
                Importe = c.PA_IMPORTE,
                Referencia = c.PA_REFERENCIA,
                Explicacion = c.PA_EXPLICACION,
                PlanAccion = c.PA_PLAN_ACCION,
                ConceptoCosto = c.ConceptoCostoDesc,
                Campo1 = c.PA_CAMPO_1,
                Campo2 = c.PA_CAMPO_2,
                Campo3 = c.PA_CAMPO_3,
                Campo4 = c.PA_CAMPO_4,
                Campo5 = c.PA_CAMPO_5,
                Campo6 = c.PA_CAMPO_6,
                Campo7 = c.PA_CAMPO_7,
                Campo8 = c.PA_CAMPO_8,
                Campo9 = c.PA_CAMPO_9,
                Campo10 = c.PA_CAMPO_10,
                Campo11 = c.PA_CAMPO_11,
                Campo12 = c.PA_CAMPO_12,
                Campo13 = c.PA_CAMPO_13,
                Campo14 = c.PA_CAMPO_14,
                Campo15 = c.PA_CAMPO_15,
                Campo16 = c.PA_CAMPO_16,
                Campo17 = c.PA_CAMPO_17,
                Campo18 = c.PA_CAMPO_18,
                Campo19 = c.PA_CAMPO_19,
                Campo20 = c.PA_CAMPO_20,
                Campo21 = c.PA_CAMPO_21,
                Campo22 = c.PA_CAMPO_22,
                Campo23 = c.PA_CAMPO_23,
                Campo24 = c.PA_CAMPO_24,
                Campo25 = c.PA_CAMPO_25,
                Campo26 = c.PA_CAMPO_26,
                Campo27 = c.PA_CAMPO_27,
                Campo28 = c.PA_CAMPO_28,
                Campo29 = c.PA_CAMPO_29,
                Campo30 = c.PA_CAMPO_30,
                Campo31 = c.PA_CAMPO_31,
                Campo32 = c.PA_CAMPO_32,
                Campo33 = c.PA_CAMPO_33,
                Campo34 = c.PA_CAMPO_34,
                Campo35 = c.PA_CAMPO_35,
                Campo36 = c.PA_CAMPO_36,
                Campo37 = c.PA_CAMPO_37,
                Campo38 = c.PA_CAMPO_38,
                Campo39 = c.PA_CAMPO_39,
                Campo40 = c.PA_CAMPO_40,
                Campo41 = c.PA_CAMPO_41,
                Campo42 = c.PA_CAMPO_42,
                Campo43 = c.PA_CAMPO_43,
                Campo44 = c.PA_CAMPO_44,
                Campo45 = c.PA_CAMPO_45,
                Campo46 = c.PA_CAMPO_46,
                Campo47 = c.PA_CAMPO_47,
                Campo48 = c.PA_CAMPO_48,
                Campo49 = c.PA_CAMPO_49,
                Campo50 = c.PA_CAMPO_50,


            });
            var dt = source.ToList().AnonymousToDataTable();

            byte[] fileExcell = reportExcelService.CreateReportBinary(dt, "Excel1");
            var contentLength = fileExcell.Length;
            //200
            //successful
            var statuscode = HttpStatusCode.OK;
            response = Request.CreateResponse(statuscode);
            response.Content = new StreamContent(new MemoryStream(fileExcell));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = contentLength;
            ContentDispositionHeaderValue contentDisposition = null;
            if (ContentDispositionHeaderValue.TryParse("inline; filename=" + "document" + ".xlsx", out contentDisposition))
            {
                response.Content.Headers.ContentDisposition = contentDisposition;
            }
            return response;
        }

        //[Route("ConsultarPartidaManualPorAprobar"), HttpGet]
        //public IHttpActionResult ConsultarPartidaManualPorAprobar([FromUri] ComprobanteModelsPar parameter)
        //{
        //    try
        //    {
        //        var model = partidasService.ConsultaConciliacioneManualPorAprobar(parameter == null ? null : parameter.fechaTrx,
        //                                                                parameter == null ? null : parameter.empresaCod,
        //                                                                parameter == null ? null : parameter.comprobanteId,
        //                                                                parameter == null ? null : parameter.cuentaContableId,
        //                                                                parameter == null ? null : parameter.importe);
        //        if (model.Count > 0)
        //            return Ok(model);
        //        else
        //            return Ok("Consulta no produjo resultados");
                
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

            //public class ComprobanteModelsPar
            //{
            //    public DateTime? fechaTrx { get; set; }
            //    public string empresaCod { get; set; }
            //    public int? comprobanteId { get; set; }
            //    public int? cuentaContableId { get; set; }
            //    public decimal? importe { get; set; }
            //}
        [Route("ListarComprobante"), HttpGet]
        public async Task<IHttpActionResult> listarComprobante()
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);
               
                var model = comprobanteService.GetAll(c => c.SAX_AREA_OPERATIVA.CA_ID_AREA == userArea.SAX_AREA_OPERATIVA.CA_ID_AREA, null
                    , includes: c => c.AspNetUsers);

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
            catch (Exception ex)
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
                //var model = servicePartida.GetAll(c=> c.PA_COD_EMPRESA == useremp.SAX_EMPRESA.CE_COD_EMPRESA,null,includes: a => a.AspNetUsers);

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

        [Route("GetConsultaPartidasMasivaManual"), HttpGet]
        public IHttpActionResult GetConsultaPartidasMasivaManual([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {
            var estatusList = catalogoService.GetSingle(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE);
            var detailsEstatusList = catalagoDetalleService.GetAll(c => c.CA_ID_CATALOGO == estatusList.CA_ID_CATALOGO && (c.CD_VALOR.Contains("MANUAL") || c.CD_VALOR.Contains("MASIVA")));//
            //estatusList.SAX_CATALOGO_DETALLE.Select(c => c.CD_VALOR.Contains("MANUAL") || c.CD_VALOR.Contains("MASIVA"));

            int[] listTipoCarga = new int[detailsEstatusList.Count()];
            for (int i = 0; i < detailsEstatusList.Count(); i++)
            {
                listTipoCarga[i] = detailsEstatusList[i].CD_TABLA;
            }

            if (partidasParameters == null)
            {
                partidasParameters = new ParametrosPartidasAprobadas();
                partidasParameters.codArea = null;
                partidasParameters.codEmpresa = null;
                partidasParameters.cuentaContable = null;
                partidasParameters.estatusConciliacion = null;
                partidasParameters.fechaCarga = null;
                partidasParameters.fechaConciliacion = null;
                partidasParameters.fechaTransaccion = null;
                partidasParameters.importe = null;
                partidasParameters.referencia = null;
                partidasParameters.tipoCarga = null;
            }

            var source = partidasAprobadas.GetAll(

                c => listTipoCarga.Contains(c.RC_COD_OPERACION)
                //c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)

                ).OrderBy(c => c.RC_REGISTRO_CONTROL);

            int count = source.Count();
            int CurrentPage = partidasParameters.pageNumber;
            int PageSize = partidasParameters.pageSize;
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
                nextPage
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(items);
        }



    }
}
