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
using Banistmo.Sax.Repository.Model;
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
        private IComprobanteService comprobanteService;
        private readonly IComprobanteDetalleService comprobanteServiceDetalle;
        private IUsuarioEmpresaService usuarioEmpService;
        private readonly IAreaOperativaService areaOperativa;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;
        private IComprobanteDetalleService comprobanteDetalleServ;
        private IComprobanteService comprobanteServ;
        private IEventosService eventoServ;
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
            comprobanteServiceDetalle = comprobanteServiceDetalle ?? new ComprobanteDetalleService();
            comprobanteService = comprobanteService ?? new ComprobanteService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            comprobanteDetalleServ = comprobanteDetalleServ ?? new ComprobanteDetalleService();
            comprobanteServ = comprobanteServ ?? new ComprobanteService();
            eventoServ = eventoServ ?? new EventosService();
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
            IAreaOperativaService area, IRegistroControlService registro, IUserService usuario, IUsuarioEmpresaService objUsuarioAreaService,
            IPartidasAprobadasService partAprob, ICatalogoDetalleService catDet,
            IUsuarioAreaService userArea, IComprobanteService comprobante, IComprobanteDetalleService comprobanteServDetalle
            ,IEventosService eventos)
        {
            partidasService = part;
            empresaService = em;
            reportExcelService = rep;
            catalogoService = cat;
            areaOperativaService = area;
            registroService = registro;
            usuarioSerive = usuario;
            usuarioEmpresaService = objUsuarioAreaService;
            partidasAprobadas = partAprob;
            catalagoDetalleService = catDet;
            usuarioAreaService = userArea;
            comprobanteServ = comprobante;
            comprobanteService = comprobante;
            comprobanteServiceDetalle = comprobanteServDetalle;
            eventoServ = eventos;
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
        [Route("GetCatalogDetails"), HttpGet]
        public IHttpActionResult GetCatalogoDetalle()
        {
            try
            {

                var items = (from ca in catalogoService.GetAll()
                             join cd in catalagoDetalleService.GetAll() on ca.CA_ID_CATALOGO equals cd.CA_ID_CATALOGO
                             where (ca.CA_TABLA == "sax_tipo_conciliacion" && cd.CD_VALOR == "PARCIAL")
                                    || ca.CA_TABLA == "sax_concilia"
                                    || (ca.CA_TABLA == "sax_estatus_carga" && cd.CD_VALOR == "ANULADO")
                             select new
                             {
                                 status = cd.CD_ESTATUS,
                                 description = (cd.CD_VALOR == "SI" ? "CONCILIADO" :
                                                cd.CD_VALOR == "NO" ? "CONCILIAR" :
                                                cd.CD_VALOR == "PARCIAL" ? "CONCILIADO PARCIAL" :
                                                cd.CD_VALOR)

                             }
                             ).ToList();
                if (items.Count() > 0)
                {
                    return Ok(items);
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
                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                int anulado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);
                //int concilia = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == user.Id).Select(y=>y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea  = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a=>a.CA_COD_AREA).ToList();

                var modelPartidaPorAprobar = partidasAprobadas.ConsultaPartidaPorAprobar(pagingparametermodel.codEnterprise,
                    pagingparametermodel.reference,
                    pagingparametermodel.importe,
                    pagingparametermodel.trxDateIni,
                    pagingparametermodel.trxDateFin,
                    pagingparametermodel.ctaAccount,
                    0,
                    pagingparametermodel.importeDesde,
                    pagingparametermodel.importeHasta);
                if(modelPartidaPorAprobar !=null && modelPartidaPorAprobar.Count() > 0)
                    modelPartidaPorAprobar = modelPartidaPorAprobar.Where(x => listAreaUsuario.Contains(x.RC_COD_AREA.HasValue ? x.RC_COD_AREA.Value : 0));

                if (modelPartidaPorAprobar.Count() > 0)
                {
                    int count = modelPartidaPorAprobar.Count();
                    int CurrentPage = pagingparametermodel.pageNumber;
                    int PageSize = pagingparametermodel.pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = modelPartidaPorAprobar.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

                    var itemList = new List<PartidasAprobadasModel>();
                    items.ForEach(c =>
                    {
                        itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasAprobadas, PartidasAprobadasModel>(c));
                    });
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
                        data = itemList

                    };
                    //HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                    return Ok(paginationMetadata);
                }
                else
                {
                    return BadRequest("No se encontraron partidas.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [Route("GetPartidaPorOperacion"), HttpGet]
        public async Task<IHttpActionResult> GetPartidaPorOperacion([FromUri] ParameterReportePartidaModel pagingparametermodel)
        {
            try
            {
                int capManual = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL);
                int capInicial = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL);
                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                int anulado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);
                //int concilia = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);
                var userAreacod = areaOperativaService.GetSingle(d => d.CA_ID_AREA == userArea.CA_ID_AREA);

                var model = partidasAprobadas.Query(

                                            p => (p.PA_STATUS_PARTIDA == aprobado
                                            || p.PA_STATUS_PARTIDA == anulado)
                                             && p.PA_ESTADO_CONCILIA == 0
                                             && p.PA_REFERENCIA != ""
                                             && p.RC_COD_AREA == userAreacod.CA_COD_AREA
                                             && p.RC_COD_AREA == (pagingparametermodel.areaOperativa == null ? p.RC_COD_AREA : pagingparametermodel.areaOperativa)
                                             && p.RC_COD_OPERACION == (pagingparametermodel.tipoReporte == null ? p.RC_COD_OPERACION : pagingparametermodel.tipoReporte)
                                             && p.PA_FECHA_CARGA == (pagingparametermodel.fechaCargaIni == null ? p.PA_FECHA_CARGA : pagingparametermodel.fechaCargaIni)
                                             && p.PA_FECHA_CARGA == (pagingparametermodel.fechaCargaFin == null ? p.PA_FECHA_CARGA : pagingparametermodel.fechaCargaFin)
                                             && p.PA_CTA_CONTABLE == (pagingparametermodel.cuentaContable == null ? p.PA_CTA_CONTABLE : pagingparametermodel.cuentaContable)
                                             && p.PA_REFERENCIA == (pagingparametermodel.referencia == null ? p.PA_REFERENCIA : pagingparametermodel.referencia)
                                             && p.PA_FECHA_TRX >= (pagingparametermodel.fechaTransaccionIni == null ? p.PA_FECHA_TRX : pagingparametermodel.fechaTransaccionIni)
                                             && p.PA_FECHA_TRX <= (pagingparametermodel.fechaTransaccionFin == null ? p.PA_FECHA_TRX : pagingparametermodel.fechaTransaccionFin)

                                             );
                if (model.Count() > 0)
                {
                    int count = model.Count();
                    int CurrentPage = pagingparametermodel.pageNumber;
                    int PageSize = pagingparametermodel.pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var items = model.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
                        data = items

                    };
                    //HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
                    return Ok(paginationMetadata);
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
            var source = partidasService.Query(c => c.PA_USUARIO_CREACION == id).OrderBy(c => c.RC_REGISTRO_CONTROL).OrderBy(c => c.PA_CONTADOR);
            var listEmpresas = empresaService.GetAll();
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
                nextPage,
                data = items
            };
            //HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(paginationMetadata);

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
            var model = partidasService.Query(
                c => c.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL
                && c.PA_CTA_CONTABLE == (string.IsNullOrEmpty(parms.PA_CTA_CONTABLE) ? c.PA_CTA_CONTABLE : parms.PA_CTA_CONTABLE)
                && c.PA_IMPORTE == (parms.PA_IMPORTE == null ? c.PA_IMPORTE : parms.PA_IMPORTE)
                && c.PA_REFERENCIA == (string.IsNullOrEmpty(parms.PA_REFERENCIA) ? c.PA_REFERENCIA : parms.PA_REFERENCIA)
                && c.PA_COD_EMPRESA == (string.IsNullOrEmpty(codEmpresa) ? c.PA_COD_EMPRESA : codEmpresa)
                ).OrderBy(c => c.PA_CONTADOR);

            var registroControl = registroService.Query(x => x.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL).Select(c => new { RC_COD_PARTIDA = c.RC_COD_PARTIDA, RC_COD_USUARIO = c.RC_COD_USUARIO });
            var itemRegistro = registroControl.FirstOrDefault();

            var usuario = usuarioSerive.GetSingle(x => x.Id == itemRegistro.RC_COD_USUARIO);
            int count = model.Count();
            int CurrentPage = parms.pageNumber;
            int PageSize = parms.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderBy(c => c.PA_CONTADOR).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            var partidasModel = new List<PartidasModel>();
            foreach (var row in items)
            {
                var row1 = new PartidasModel();
                row1 = Extension.CustomMapIgnoreICollection<Repository.Model.SAX_PARTIDAS, PartidasModel>(row);
                row1.RC_USUARIO_NOMBRE = usuario.FirstName;
                row1.RC_COD_PARTIDA = itemRegistro.RC_COD_PARTIDA;
                row1.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
                partidasModel.Add(row1);
            }
            var paginacion = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data = partidasModel
            };

            if (paginacion != null)
            {
                return Ok(paginacion);
            }

            return NotFound();
        }

        [Route("FindPartidaComprobante"), HttpPost]
        //public IHttpActionResult FindPartida(PartidasModel parms int idRegistro, string idEmpresa,string idCuentaContable, decimal importe,string referencia)
        public IHttpActionResult FindPartidaComprobante(ComprobanteModelParams parms)
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
            var comprobanteObj = comprobanteServ.GetSingle(x => x.TC_ID_COMPROBANTE == parms.TC_ID_COMPROBANTE);
            var detalleComprobante = comprobanteServiceDetalle.GetAll(x => x.TC_ID_COMPROBANTE == parms.TC_ID_COMPROBANTE).Select(x => x.PA_REGISTRO);
            List<PartidasModel> model = partidasService.GetAll(
                c => detalleComprobante.Contains(c.PA_REGISTRO)
                && c.PA_CTA_CONTABLE == (string.IsNullOrEmpty(parms.PA_CTA_CONTABLE) ? c.PA_CTA_CONTABLE : parms.PA_CTA_CONTABLE)
                && c.PA_IMPORTE == (parms.PA_IMPORTE == null ? c.PA_IMPORTE : parms.PA_IMPORTE)
                && c.PA_REFERENCIA == (string.IsNullOrEmpty(parms.PA_REFERENCIA) ? c.PA_REFERENCIA : parms.PA_REFERENCIA)
                && c.PA_COD_EMPRESA == (string.IsNullOrEmpty(codEmpresa) ? c.PA_COD_EMPRESA : codEmpresa)
                ).OrderBy(c => c.PA_CONTADOR).ToList();


            var usuario = usuarioSerive.GetSingle(x => x.Id == comprobanteObj.AspNetUsers.Id);
            int count = model.Count();
            int CurrentPage = parms.pageNumber;
            int PageSize = parms.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderBy(c => c.PA_CONTADOR).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            var partidasModel = new List<PartidasModel>();
            foreach (var row in items)
            {
                var row1 = new PartidasModel();
                row1 = (row);
                row1.RC_USUARIO_NOMBRE = getUsuario(row.PA_USUARIO_CREACION);
                row1.TC_COD_COMPROBANTE = comprobanteObj.TC_COD_COMPROBANTE;
                row1.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
                partidasModel.Add(row1);
            }
            var paginacion = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data = partidasModel
            };

            if (paginacion != null)
            {
                return Ok(paginacion);
            }

            return NotFound();
        }


        [Route("GetReporteComprobanteExcel"), HttpGet]
        public HttpResponseMessage GetReporteComprobanteExcel([FromUri]ComprobanteModelParams parms)
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
            var comprobanteObj = comprobanteServ.GetSingle(x => x.TC_ID_COMPROBANTE == parms.TC_ID_COMPROBANTE);
            var detalleComprobante = comprobanteServiceDetalle.GetAll(x => x.TC_ID_COMPROBANTE == parms.TC_ID_COMPROBANTE).Select(x => x.PA_REGISTRO);
            List<PartidasModel> model = partidasService.GetAll(
                c => detalleComprobante.Contains(c.PA_REGISTRO)
                && c.PA_CTA_CONTABLE == (string.IsNullOrEmpty(parms.PA_CTA_CONTABLE) ? c.PA_CTA_CONTABLE : parms.PA_CTA_CONTABLE)
                && c.PA_IMPORTE == (parms.PA_IMPORTE == null ? c.PA_IMPORTE : parms.PA_IMPORTE)
                && c.PA_REFERENCIA == (string.IsNullOrEmpty(parms.PA_REFERENCIA) ? c.PA_REFERENCIA : parms.PA_REFERENCIA)
                && c.PA_COD_EMPRESA == (string.IsNullOrEmpty(codEmpresa) ? c.PA_COD_EMPRESA : codEmpresa)
                ).OrderBy(c => c.PA_CONTADOR).ToList();


            var usuario = usuarioSerive.GetSingle(x => x.Id == comprobanteObj.AspNetUsers.Id);
            int count = model.Count();
            int CurrentPage = parms.pageNumber;
            int PageSize = parms.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = model.OrderBy(c => c.PA_CONTADOR).ToList();

            var partidasModel = new List<PartidasModel>();
            foreach (var row in items)
            {
                row.RC_USUARIO_NOMBRE = getUsuario(row.PA_USUARIO_CREACION);
                row.TC_COD_COMPROBANTE = comprobanteObj.TC_COD_COMPROBANTE;
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();

            }
            var formatModel = items.Select(x => new
            {
                PA_COD_EMPRESA = x.PA_COD_EMPRESA,
                PA_FECHA_CARGA = x.PA_FECHA_CARGA.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                PA_FECHA_TRX = x.PA_FECHA_TRX.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                PA_CTA_CONTABLE = x.PA_CTA_CONTABLE.Trim(),
                PA_CENTRO_COSTO = x.PA_CENTRO_COSTO,
                PA_COD_MONEDA = x.PA_COD_MONEDA,
                PA_IMPORTE = x.PA_IMPORTE,
                PA_REFERENCIA = x.PA_REFERENCIA,
                PA_EXPLICACION = x.PA_EXPLICACION,
                PA_PLAN_ACCION = x.PA_PLAN_ACCION,
                PA_CONCEPTO_COSTO = x.PA_CONCEPTO_COSTO,
                EVENTO_NOMBRE = x.EVENTO_NOMBRE,
                PA_CAMPO_1 = x.PA_CAMPO_1,
                PA_CAMPO_2 = x.PA_CAMPO_2,
                PA_CAMPO_3 = x.PA_CAMPO_3,
                PA_CAMPO_4 = x.PA_CAMPO_4,
                PA_CAMPO_5 = x.PA_CAMPO_5,
                PA_CAMPO_6 = x.PA_CAMPO_6,
                PA_CAMPO_7 = x.PA_CAMPO_7,
                PA_CAMPO_8 = x.PA_CAMPO_8,
                PA_CAMPO_9 = x.PA_CAMPO_9,
                PA_CAMPO_10 = x.PA_CAMPO_10,
                PA_CAMPO_11 = x.PA_CAMPO_11,
                PA_CAMPO_12 = x.PA_CAMPO_12,
                PA_CAMPO_13 = x.PA_CAMPO_13,
                PA_CAMPO_14 = x.PA_CAMPO_14,
                PA_CAMPO_15 = x.PA_CAMPO_15,
                PA_CAMPO_16 = x.PA_CAMPO_16,
                PA_CAMPO_17 = x.PA_CAMPO_17,
                PA_CAMPO_18 = x.PA_CAMPO_18,
                PA_CAMPO_19 = x.PA_CAMPO_19,
                PA_CAMPO_20 = x.PA_CAMPO_20,
                PA_CAMPO_21 = x.PA_CAMPO_21,
                PA_CAMPO_22 = x.PA_CAMPO_22,
                PA_CAMPO_23 = x.PA_CAMPO_23,
                PA_CAMPO_24 = x.PA_CAMPO_24,
                PA_CAMPO_25 = x.PA_CAMPO_25,
                PA_CAMPO_26 = x.PA_CAMPO_26,
                PA_CAMPO_27 = x.PA_CAMPO_27,
                PA_CAMPO_28 = x.PA_CAMPO_28,
                PA_CAMPO_29 = x.PA_CAMPO_29,
                PA_CAMPO_30 = x.PA_CAMPO_30,
                PA_CAMPO_31 = x.PA_CAMPO_31,
                PA_CAMPO_32 = x.PA_CAMPO_32,
                PA_CAMPO_33 = x.PA_CAMPO_33,
                PA_CAMPO_34 = x.PA_CAMPO_34,


                PA_CAMPO_35 = x.PA_CAMPO_35,


                PA_CAMPO_36 = x.PA_CAMPO_36,


                PA_CAMPO_37 = x.PA_CAMPO_37,


                PA_CAMPO_38 = x.PA_CAMPO_38,


                PA_CAMPO_39 = x.PA_CAMPO_39,


                PA_CAMPO_40 = x.PA_CAMPO_40,


                PA_CAMPO_41 = x.PA_CAMPO_41,


                PA_CAMPO_42 = x.PA_CAMPO_42,


                PA_CAMPO_43 = x.PA_CAMPO_43,


                PA_CAMPO_44 = x.PA_CAMPO_44,


                PA_CAMPO_45 = x.PA_CAMPO_45,


                PA_CAMPO_46 = x.PA_CAMPO_46,


                PA_CAMPO_47 = x.PA_CAMPO_47,


                PA_CAMPO_48 = x.PA_CAMPO_48,


                PA_CAMPO_49 = x.PA_CAMPO_49,


                Campo_50 = x.PA_CAMPO_50,


                Fecha_Creacion = x.PA_FECHA_CREACION.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),


                Usuario_Creacion = x.RC_USUARIO_NOMBRE,

            }).ToList();

            var dt = formatModel.ToList().AnonymousToDataTable();
            if (dt != null && dt.Columns.Count > 0)
            {
                dt.Columns[0].Caption = "Empresa";
                dt.Columns[1].Caption = "Fecha de carga";
                dt.Columns[2].Caption = "Fecha de Transacción";
                dt.Columns[3].Caption = "Cuenta contable";
                dt.Columns[4].Caption = "Centro de costo";
                dt.Columns[5].Caption = "Moneda";
                dt.Columns[6].Caption = "Importe";
                dt.Columns[7].Caption = "Referencia";
                dt.Columns[8].Caption = "Explicación";
                dt.Columns[9].Caption = "Plan de acción";
                dt.Columns[10].Caption = "Concepto de costo";
                dt.Columns[11].Caption = "Evento";
                dt.Columns[12].Caption = "Campo 1";
                dt.Columns[13].Caption = "Campo 2";
                dt.Columns[14].Caption = "Campo 3";
                dt.Columns[15].Caption = "Campo 4";
                dt.Columns[16].Caption = "Campo 5";
                dt.Columns[17].Caption = "Campo 6";
                dt.Columns[18].Caption = "Campo 7";
                dt.Columns[19].Caption = "Campo 8";
                dt.Columns[20].Caption = "Campo 9";
                dt.Columns[21].Caption = "Campo 10";
                dt.Columns[22].Caption = "Campo 11";
                dt.Columns[23].Caption = "Campo 12";
                dt.Columns[24].Caption = "Campo 13";
                dt.Columns[25].Caption = "Campo 14";
                dt.Columns[26].Caption = "Campo 15";
                dt.Columns[27].Caption = "Campo 16";
                dt.Columns[28].Caption = "Campo 17";
                dt.Columns[29].Caption = "Campo 18";
                dt.Columns[30].Caption = "Campo 19";
                dt.Columns[31].Caption = "Campo 20";
                dt.Columns[32].Caption = "Campo 21";
                dt.Columns[33].Caption = "Campo 22";
                dt.Columns[34].Caption = "Campo 23";
                dt.Columns[35].Caption = "Campo 24";
                dt.Columns[36].Caption = "Campo 25";
                dt.Columns[37].Caption = "Campo 26";
                dt.Columns[38].Caption = "Campo 27";
                dt.Columns[39].Caption = "Campo 28";
                dt.Columns[40].Caption = "Campo 29";
                dt.Columns[41].Caption = "Campo 30";
                dt.Columns[42].Caption = "Campo 31";
                dt.Columns[43].Caption = "Campo 32";
                dt.Columns[44].Caption = "Campo 33";
                dt.Columns[45].Caption = "Campo 34";
                dt.Columns[46].Caption = "Campo 35";
                dt.Columns[47].Caption = "Campo 36";
                dt.Columns[48].Caption = "Campo 37";
                dt.Columns[49].Caption = "Campo 38";
                dt.Columns[50].Caption = "Campo 39";
                dt.Columns[51].Caption = "Campo 40";
                dt.Columns[52].Caption = "Campo 41";
                dt.Columns[53].Caption = "Campo 42";
                dt.Columns[54].Caption = "Campo 43";
                dt.Columns[55].Caption = "Campo 44";
                dt.Columns[56].Caption = "Campo 45";
                dt.Columns[57].Caption = "Campo 46";
                dt.Columns[58].Caption = "Campo 47";
                dt.Columns[59].Caption = "Campo 48";
                dt.Columns[60].Caption = "Campo 49";
                dt.Columns[61].Caption = "Campo 50";
                dt.Columns[62].Caption = "Fecha creación";
                dt.Columns[63].Caption = "Usuario creación";
            }
            byte[] fileExcell = reportExcelService.CreateReportBinary(dt, "Partidas");
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
            var source = partidasService.Query(c => c.RC_REGISTRO_CONTROL > 0).OrderBy(c => c.RC_REGISTRO_CONTROL);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
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
                data = items
            };
            //HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(paginationMetadata);

        }

        [Route("GetPartidaPag")]
        public IHttpActionResult GetPagination(int partida, [FromUri]PagingParameterModel pagingparametermodel)
        {
            EventosModel evento= new EventosModel();
            string nombreEvento = string.Empty;
            var source = partidasService.Query(c => c.RC_REGISTRO_CONTROL == partida).OrderBy(c => c.PA_CONTADOR);
            var registroControl = registroService.Query(x => x.RC_REGISTRO_CONTROL == partida).Select(c => new { RC_COD_PARTIDA = c.RC_COD_PARTIDA, RC_COD_USUARIO = c.RC_COD_USUARIO, EV_COD_EVENTO=c.EV_COD_EVENTO });
            var itemRegistro = registroControl.FirstOrDefault();
            if(itemRegistro!=null)
             evento = eventoServ.GetSingle(e => e.EV_COD_EVENTO == itemRegistro.EV_COD_EVENTO);
            if(evento != null)
                nombreEvento = evento.EV_COD_EVENTO + "-" + evento.EV_DESCRIPCION_EVENTO;
            var usuario = usuarioSerive.GetSingle(x => x.Id == itemRegistro.RC_COD_USUARIO);
            var listEmpresas = empresaService.Query(c => c.CE_COD_EMPRESA != null).
                Select(c => new { CE_COD_EMPRESA = c.CE_COD_EMPRESA, CE_NOMBRE = c.CE_NOMBRE }).ToList();
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.OrderBy(c => c.PA_CONTADOR).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            var partidasModel = new List<PartidasModel>();
            foreach (var row in items)
            {
                var row1 = new PartidasModel();
                row1 = Extension.CustomMapIgnoreICollection<Repository.Model.SAX_PARTIDAS, PartidasModel>(row);
                row1.RC_USUARIO_NOMBRE = usuario.FirstName;
                row1.RC_COD_PARTIDA = itemRegistro.RC_COD_PARTIDA;
                row1.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
                row1.EVENTO_NOMBRE = nombreEvento;
                partidasModel.Add(row1);
            }

            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                data = partidasModel
            };
            return Ok(paginationMetadata);

        }

        [Route("GetReporteExcel"), HttpGet]
        public HttpResponseMessage GetReporteExcel([FromUri]PartidaModel parms)
        {
            EventosModel evento = new EventosModel();
            string nombreEvento = string.Empty;
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
                ).OrderBy(c => c.PA_CONTADOR).OrderBy(x => x.PA_CONTADOR).ToList();
            var registroControl = registroService.GetSingle(x => x.RC_REGISTRO_CONTROL == parms.RC_REGISTRO_CONTROL);
            var usuario = usuarioSerive.GetSingle(x => x.Id == registroControl.RC_COD_USUARIO);
            int count = model.Count();
            if (registroControl != null)
                evento = eventoServ.GetSingle(e => e.EV_COD_EVENTO == registroControl.EV_COD_EVENTO);
            if (evento != null)
                nombreEvento = evento.EV_COD_EVENTO + "-" + evento.EV_DESCRIPCION_EVENTO;

            foreach (var row in model)
            {
                row.RC_USUARIO_NOMBRE = usuario.FirstName;
                row.RC_COD_PARTIDA = registroControl.RC_COD_PARTIDA;
                row.PA_COD_EMPRESA = row.PA_COD_EMPRESA + "-" + listEmpresas.Where(e => e.CE_COD_EMPRESA.Trim() == row.PA_COD_EMPRESA).Select(e => e.CE_NOMBRE).FirstOrDefault();
                row.EVENTO_NOMBRE = nombreEvento;
            }
            var formatModel = model.Select(x => new
            {
                PA_COD_EMPRESA= x.PA_COD_EMPRESA,
                PA_FECHA_CARGA = x.PA_FECHA_CARGA.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                PA_FECHA_TRX = x.PA_FECHA_TRX.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                PA_CTA_CONTABLE = x.PA_CTA_CONTABLE.Trim(),
                PA_CENTRO_COSTO = x.PA_CENTRO_COSTO,
                PA_COD_MONEDA = x.PA_COD_MONEDA,
                PA_IMPORTE = x.PA_IMPORTE,
                PA_REFERENCIA = x.PA_REFERENCIA,
                PA_EXPLICACION = x.PA_EXPLICACION,
                PA_PLAN_ACCION = x.PA_PLAN_ACCION,
                PA_CONCEPTO_COSTO = x.PA_CONCEPTO_COSTO,
                EVENTO_NOMBRE= x.EVENTO_NOMBRE,
                PA_CAMPO_1 = x.PA_CAMPO_1,
                PA_CAMPO_2 = x.PA_CAMPO_2,
                PA_CAMPO_3 = x.PA_CAMPO_3,
                PA_CAMPO_4 = x.PA_CAMPO_4,
                PA_CAMPO_5 = x.PA_CAMPO_5,
                PA_CAMPO_6 = x.PA_CAMPO_6,
                PA_CAMPO_7 = x.PA_CAMPO_7,
                PA_CAMPO_8 = x.PA_CAMPO_8,
                PA_CAMPO_9 = x.PA_CAMPO_9,
                PA_CAMPO_10 = x.PA_CAMPO_10,
                PA_CAMPO_11 = x.PA_CAMPO_11,
                PA_CAMPO_12 = x.PA_CAMPO_12,
                PA_CAMPO_13 = x.PA_CAMPO_13,
                PA_CAMPO_14 = x.PA_CAMPO_14,
                PA_CAMPO_15 = x.PA_CAMPO_15,
                PA_CAMPO_16 = x.PA_CAMPO_16,
                PA_CAMPO_17 = x.PA_CAMPO_17,
                PA_CAMPO_18 = x.PA_CAMPO_18,
                PA_CAMPO_19 = x.PA_CAMPO_19,
                PA_CAMPO_20 = x.PA_CAMPO_20,
                PA_CAMPO_21 = x.PA_CAMPO_21,
                PA_CAMPO_22 = x.PA_CAMPO_22,
                PA_CAMPO_23 = x.PA_CAMPO_23,
                PA_CAMPO_24 = x.PA_CAMPO_24,
                PA_CAMPO_25 = x.PA_CAMPO_25,
                PA_CAMPO_26 = x.PA_CAMPO_26,
                PA_CAMPO_27 = x.PA_CAMPO_27,
                PA_CAMPO_28 = x.PA_CAMPO_28,
                PA_CAMPO_29 = x.PA_CAMPO_29,
                PA_CAMPO_30 = x.PA_CAMPO_30,
                PA_CAMPO_31 = x.PA_CAMPO_31,
                PA_CAMPO_32 = x.PA_CAMPO_32,
                PA_CAMPO_33 = x.PA_CAMPO_33,
                PA_CAMPO_34 = x.PA_CAMPO_34,


                PA_CAMPO_35 = x.PA_CAMPO_35,


                PA_CAMPO_36 = x.PA_CAMPO_36,


                PA_CAMPO_37 = x.PA_CAMPO_37,


                PA_CAMPO_38 = x.PA_CAMPO_38,


                PA_CAMPO_39 = x.PA_CAMPO_39,


                PA_CAMPO_40 = x.PA_CAMPO_40,


                PA_CAMPO_41 = x.PA_CAMPO_41,


                PA_CAMPO_42 = x.PA_CAMPO_42,


                PA_CAMPO_43 = x.PA_CAMPO_43,


                PA_CAMPO_44 = x.PA_CAMPO_44,


                PA_CAMPO_45 = x.PA_CAMPO_45,


                PA_CAMPO_46 = x.PA_CAMPO_46,


                PA_CAMPO_47 = x.PA_CAMPO_47,


                PA_CAMPO_48 = x.PA_CAMPO_48,


                PA_CAMPO_49 = x.PA_CAMPO_49,


                Campo_50 = x.PA_CAMPO_50,


                Fecha_Creacion = x.PA_FECHA_CREACION.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),


                Usuario_Creacion = x.RC_USUARIO_NOMBRE,

            }).ToList();
            var dt = formatModel.ToList().AnonymousToDataTable();
            if (dt != null && dt.Columns.Count > 0) {
                dt.Columns[0].Caption = "Empresa";
                dt.Columns[1].Caption = "Fecha de carga";
                dt.Columns[2].Caption = "Fecha de Transacción";
                dt.Columns[3].Caption = "Cuenta contable";
                dt.Columns[4].Caption = "Centro de costo";
                dt.Columns[5].Caption = "Moneda";
                dt.Columns[6].Caption = "Importe";
                dt.Columns[7].Caption = "Referencia";
                dt.Columns[8].Caption = "Explicación";
                dt.Columns[9].Caption = "Plan de acción";
                dt.Columns[10].Caption = "Concepto de costo";
                dt.Columns[11].Caption = "Evento";
                dt.Columns[12].Caption = "Campo 1";
                dt.Columns[13].Caption = "Campo 2";
                dt.Columns[14].Caption = "Campo 3";
                dt.Columns[15].Caption = "Campo 4";
                dt.Columns[16].Caption = "Campo 5";
                dt.Columns[17].Caption = "Campo 6";
                dt.Columns[18].Caption = "Campo 7";
                dt.Columns[19].Caption = "Campo 8";
                dt.Columns[20].Caption = "Campo 9";
                dt.Columns[21].Caption = "Campo 10";
                dt.Columns[22].Caption = "Campo 11";
                dt.Columns[23].Caption = "Campo 12";
                dt.Columns[24].Caption = "Campo 13";
                dt.Columns[25].Caption = "Campo 14";
                dt.Columns[26].Caption = "Campo 15";
                dt.Columns[27].Caption = "Campo 16";
                dt.Columns[28].Caption = "Campo 17";
                dt.Columns[29].Caption = "Campo 18";
                dt.Columns[30].Caption = "Campo 19";
                dt.Columns[31].Caption = "Campo 20";
                dt.Columns[32].Caption = "Campo 21";
                dt.Columns[33].Caption = "Campo 22";
                dt.Columns[34].Caption = "Campo 23";
                dt.Columns[35].Caption = "Campo 24";
                dt.Columns[36].Caption = "Campo 25";
                dt.Columns[37].Caption = "Campo 26";
                dt.Columns[38].Caption = "Campo 27";
                dt.Columns[39].Caption = "Campo 28";
                dt.Columns[40].Caption = "Campo 29";
                dt.Columns[41].Caption = "Campo 30";
                dt.Columns[42].Caption = "Campo 31";
                dt.Columns[43].Caption = "Campo 32";
                dt.Columns[44].Caption = "Campo 33";
                dt.Columns[45].Caption = "Campo 34";
                dt.Columns[46].Caption = "Campo 35";
                dt.Columns[47].Caption = "Campo 36";
                dt.Columns[48].Caption = "Campo 37";
                dt.Columns[49].Caption = "Campo 38";
                dt.Columns[50].Caption = "Campo 39";
                dt.Columns[51].Caption = "Campo 40";
                dt.Columns[52].Caption = "Campo 41";
                dt.Columns[53].Caption = "Campo 42";
                dt.Columns[54].Caption = "Campo 43";
                dt.Columns[55].Caption = "Campo 44";
                dt.Columns[56].Caption = "Campo 45";
                dt.Columns[57].Caption = "Campo 46";
                dt.Columns[58].Caption = "Campo 47";
                dt.Columns[59].Caption = "Campo 48";
                dt.Columns[60].Caption = "Campo 49";
                dt.Columns[61].Caption = "Campo 50";
                dt.Columns[62].Caption = "Fecha creación";
                dt.Columns[63].Caption = "Usuario creación";
            }
            
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
                return Ok("Plan de acción actualizado exitosamente");
            }
            return BadRequest("Debe seleccionar partidas validas para cambiar plan de accion.");
        }

        [Route("GetTipoCarga"), HttpGet]
        public IHttpActionResult GetTipoCarga()
        {
            int conciliacion = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);
            int anulacion = Convert.ToInt16(BusinessEnumerations.TipoOperacion.ANULACION);
            var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE);

            List<CatalogoDetalleModel> estatusListDetalle = new List<CatalogoDetalleModel>();
            foreach (var details in estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE)
            {
                if (details.CD_VALOR != "CONCILIACION" && details.CD_VALOR != "ANULACION" && details.CD_VALOR != "CONCILIACION AUTOMATICA" && details.CD_VALOR != "CONCILIACION MANUAL")
                {
                    estatusListDetalle.Add(details);
                }
            }

            if (estatusListDetalle != null)
            {

                return Ok(estatusListDetalle.Select(c => new
                {
                    idTipoCarga = c.CD_ESTATUS,
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
        public async Task<IHttpActionResult> GetAreaByEmpresa()
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            List<UsuarioAreaModel> listUsuarioArea = new List<UsuarioAreaModel>();
            var listAreas = usuarioAreaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, includes: c => c.SAX_AREA_OPERATIVA);
            string fmt = "000";
            if (listAreas.Count > 0)
            {
                foreach (var area in listAreas)
                {
                    listUsuarioArea.Add(area);
                }
            }
            if (listUsuarioArea != null && listUsuarioArea.Count() > 0)
            {
                return Ok(listUsuarioArea.Select(c => new
                {
                    CA_COD_AREA = c.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString(fmt),
                    CA_NOMBRE = c.SAX_AREA_OPERATIVA.CA_NOMBRE,
                    CA_ID = c.SAX_AREA_OPERATIVA.CA_ID_AREA
                }).OrderBy(j => j.CA_COD_AREA));
            }
            return null;
        }
        [Route("GetEmpresa"), HttpGet]
        public async Task<IHttpActionResult> GetEmpresa()
        {
            try
            {
                List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var listEmpresas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, c => c.SAX_EMPRESA);
                if (listEmpresas.Count > 0)
                {
                    foreach (var emp in listEmpresas)
                    {
                        listUsuarioEmpresas.Add(emp);
                    }
                }
                if (listUsuarioEmpresas != null && listUsuarioEmpresas.Count > 0)
                {
                    return Ok(listUsuarioEmpresas.Select(c => new
                    {
                        CE_ID_EMPRESA = c.SAX_EMPRESA.CE_ID_EMPRESA,
                        CE_COD_EMPRESA = c.SAX_EMPRESA.CE_COD_EMPRESA,
                        CE_NOMBRE = c.SAX_EMPRESA.CE_NOMBRE
                    }));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            return null;
        }

        [Route("GetConsultaPartidasAprobadas2"), HttpGet]
        public async Task<IHttpActionResult> GetConsultaPartidasAprobadas([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {

            try
            {


                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

                int estado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                int tipoComp = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);

                List<ComprobanteModel> model = comprobanteService.GetAll(c => c.TC_COD_OPERACION == tipoComp, null, includes: c => c.AspNetUsers).ToList();
                List<ComprobanteDetalleModel> detalleComp = comprobanteServiceDetalle.GetAll();

                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
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

                var source = partidasAprobadas.Query(

                    c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                    && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                    && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                    && c.PA_STATUS_PARTIDA == (aprobado)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)//userAreacod.CA_COD_AREA
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL);


                //var items = source.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }
                    //items = viPaApro;
                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                }
                int count = viPaApro.Count();
                int CurrentPage = partidasParameters.pageNumber;
                int PageSize = partidasParameters.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                var itemList = new List<PartidasAprobadasModel>();
                viPaApro.ForEach(c =>
                {
                    itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasAprobadas, PartidasAprobadasModel>(c));
                });

                foreach (var c in itemList)
                {

                    foreach (var a in detalleComp)
                    {
                        foreach (var b in model)
                        {
                            if (b.TC_ID_COMPROBANTE == a.TC_ID_COMPROBANTE)
                                if (a.PA_REGISTRO == c.PA_REGISTRO)
                                {
                                    c.comprobanteConciliacion = b.TC_COD_COMPROBANTE;
                                }
                        }
                    }
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
                return Ok(itemList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("GetConsultaPlan"), HttpGet]
        //modifica Linette Arcia
        public async Task<IHttpActionResult> GetConsultaPlan([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {
            try
            {
                partidasParameters.estatusConciliacion = Convert.ToInt16(ConciliaState.No);
                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);
                var userAreacod = areaOperativaService.GetSingle(d => d.CA_ID_AREA == userArea.CA_ID_AREA);
                //var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE);
                //var OperacionConc = from h in estatusList from j in h.SAX_CATALOGO_DETALLE where j.CD_VALOR == "CONCILIACION" select j.CD_ESTATUS;

                //Int16 Id = Convert.ToInt16(OperacionConc.FirstOrDefault());

                //var estatusConc = catalogoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE);
                //var Conciliacionxx = from h in estatusConc from j in h.SAX_CATALOGO_DETALLE where j.CD_VALOR == "POR CONCILIAR" select j.CD_ESTATUS;

                //Int16  IdEstatusPorConciliar = Convert.ToInt16(Conciliacionxx.FirstOrDefault());

                //var ComprobantespoConciliar = comprobanteService.GetAll(t => t.TC_ESTATUS == IdEstatusPorConciliar && t.TC_COD_OPERACION == Id);


                var source = partidasAprobadas.Query(

                c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                && c.PA_COD_EMPRESA == (partidasParameters.codEmpresa == null ? c.PA_COD_EMPRESA : partidasParameters.codEmpresa)
                && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                && c.PA_ESTADO_CONCILIA == partidasParameters.estatusConciliacion
                && c.PA_STATUS_PARTIDA == (aprobado)
                && c.RC_COD_AREA == userAreacod.CA_COD_AREA

                ).OrderBy(c => c.RC_REGISTRO_CONTROL);


                //var sourcefin = from m in source from j in ComprobantespoConciliar from h in j.SAX_COMPROBANTE_DETALLE where h.PA_REGISTRO == m.PA_REGISTRO select m ;
                int count = source.Count();
                int CurrentPage = partidasParameters.pageNumber;
                int PageSize = partidasParameters.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
                var itemList = new List<PartidasAprobadasModel>();
                items.ForEach(c =>
                {
                    itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasAprobadas, PartidasAprobadasModel>(c));
                });
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
                return Ok(itemList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("GetConsultaPartidas"), HttpGet]
        public async Task<IHttpActionResult> GetConsultaPartidas([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {
            try
            {

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

                int estado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                int tipoComp = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);

                List<ComprobanteModel> model = comprobanteService.GetAll(c => c.TC_COD_OPERACION == tipoComp, null, includes: c => c.AspNetUsers).ToList();
                var detalleComp = model.Select(x => x.SAX_COMPROBANTE_DETALLE).ToList();

                int Xaprobar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
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
                    partidasParameters.tipoCarga = null;
                }

                // var source = partidasAprobadas.GetAll(
                var source = partidasAprobadas.Query(

                c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                && c.PA_FECHA_CARGA == (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                && c.PA_COD_EMPRESA == (partidasParameters.codEmpresa == null ? c.PA_COD_EMPRESA : partidasParameters.codEmpresa)
                && c.PA_STATUS_PARTIDA != (Xaprobar)
                && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                //&& c.RC_COD_AREA == userAreacod.CA_COD_AREA
                ).OrderBy(c =>  c.RC_REGISTRO_CONTROL).ThenBy(n=>n.PA_CONTADOR);
                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }
                    //items = viPaApro;
                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                }

                var itemList = new List<PartidasAprobadasModel>();
                viPaApro.ForEach(c =>
                {
                    itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasAprobadas, PartidasAprobadasModel>(c));
                });

                foreach (var c in itemList)
                {

                    foreach(var reg in model)
                    {
                        var detalle = model.Select(r => r.SAX_COMPROBANTE_DETALLE.Where(j => j.PA_REGISTRO == c.PA_REGISTRO));
                        if (detalle.Count() > 0)
                        {
                            c.comprobanteConciliacion = reg.TC_COD_COMPROBANTE;
                        }
                    }
                }
                var retorno = itemList.Select(c => new
                {

                    Usuario = c.UsuarioC_Nombre,
                    Lote = c.RC_COD_PARTIDA,
                    Numero = c.PA_CONTADOR,
                    Empresa = c.EmpresaDesc,
                    FechaCarga = string.IsNullOrEmpty(c.PA_FECHA_CARGA.ToString()) ? "" : c.PA_FECHA_CARGA.Value.ToShortDateString().ToString(),
                    HoraCarga = getHora(c.PA_HORA_CREACION),
                    FechaTransaccion = c.PA_FECHA_TRX.Value.ToShortDateString().ToString(),
                    CtaContable = c.PA_CTA_CONTABLE,
                   // NombreCtaContable = c.PA_CTA_CONTABLE, // Falta
                    CentroCosto = c.CentroCostoDesc,
                    Moneda = c.PA_COD_MONEDA,
                    Importe = c.PA_IMPORTE.ToString("N2"),
                    Referencia = c.PA_REFERENCIA,
                    Explicacion = c.PA_EXPLICACION,
                    PlanAccion = c.PA_PLAN_ACCION,
                    ConceptoCosto = c.PA_CONCEPTO_COSTO,
                    UsuarioAprobador = c.UsuarioAprob_Nombre,
                    TipoConciliacion = c.TipoConciliaDesc,
                    EstadoConciliacion = c.EstadoConciliaDesc,
                    FechaConciliacion = string.IsNullOrEmpty(c.PA_FECHA_CONCILIA.ToString())?"":c.PA_FECHA_CONCILIA.Value.ToShortDateString().ToString(),
                    FechaAnulacion = string.IsNullOrEmpty(c.PA_FECHA_ANULACION.ToString())?"":c.PA_FECHA_ANULACION.Value.ToShortDateString().ToString(),
                    UsuarioAnulacion = string.IsNullOrEmpty(c.PA_USUARIO_ANULACION.ToString()) ? "" : c.PA_USUARIO_ANULACION.ToString(),
                    DiasAntiguedad = c.PA_DIAS_ANTIGUEDAD,
                    OrigenReferencia = c.OrigenRefDesc,
                    Area = c.AREAOPERATIVADESC,
                    Operacion = c.OperacionDesc,
                    TotalRegistro = c.RC_TOTAL_DEBITO.ToString("N2"),
                    TotalDebito = c.RC_TOTAL_DEBITO.ToString("N2"),
                    TotalCredito = c.RC_TOTAL_CREDITO.ToString("N2"),
                    Total = c.RC_TOTAL.ToString("N2"),
                    ComprobanteConciliacion = c.comprobanteConciliacion,
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
                    //IdPartida = c.PA_REGISTRO

                });
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("GetReportePartidasAprobadas"), HttpGet]
        public HttpResponseMessage GetReporteCuentaConcilia([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);

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
                && c.PA_STATUS_PARTIDA == (aprobado)
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

            var source = partidasAprobadas.Query(

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

            var itemList = new List<PartidasAprobadasModel>();
            items.ForEach(c =>
            {
                itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasAprobadas, PartidasAprobadasModel>(c));
            });

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
            return Ok(itemList);
        }

        public enum ConciliaState
        {
            No = 0,
            Si = 1,

        }

        private class tipopartida
        {
            public int id { get; set; }
            public string partida { get; set; }
        }
        private string getUsuario(string id)
        {
            string result = string.Empty; ;
            var usuario = usuarioSerive.GetSingle(u => u.Id == id);
            if (usuario != null)
            {
                result = usuario.FirstName;
            }
            return result;
        }
        [Route("GetTipoPartida")]
        public IHttpActionResult GetTipoPartida()
        {
            try
            {
                tipopartida par = new tipopartida();
                List<tipopartida> partidas = new List<tipopartida>();
                int indice = 0;
                string[] partida = new string[5] { "Aprobadas", "Conciliadas", "Conciliadas Parcialmente", "Pendientes Conciliar", "Anuladas" };

                foreach (var j in partida)
                {

                    partidas.Add(new tipopartida { id = indice, partida = j.ToString() });


                    indice++;
                }
                return Ok(partidas);


            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        //private List<PartidasAprobadasModel> PartidasAp(IdentityUser user, ParametrosPartidasAprobadas partidasParameters)
        private List<vi_PartidasAprobadas> PartidasAp(IdentityUser user, ParametrosPartidasAprobadas partidasParameters)
        {
            try
            {

                partidasParameters.tipoCarga = null;
                //IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

                int Xaprobar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
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

                var source = partidasAprobadas.Query(

                    c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                    && c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                    && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                    && c.PA_STATUS_PARTIDA != (Xaprobar)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)//userAreacod.CA_COD_AREA
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && c.PA_REFERENCIA.ToString().Length != 0
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                //var items = source.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }

                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();
                }
                return viPaApro;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<vi_PartidasAprobadas> PartidasConciliadas(IdentityUser user, ParametrosPartidasAprobadas partidasParameters)
        {
            try
            {

                partidasParameters.tipoCarga = null;
                partidasParameters.estatusConciliacion = 1;
                //IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

                partidasParameters.estatusConciliacion = Convert.ToInt16(BusinessEnumerations.Concilia.SI);

                //int TipoConcilia = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.AUTOMATICO);
                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                if (partidasParameters == null)
                {
                    partidasParameters = new ParametrosPartidasAprobadas();
                    partidasParameters.codArea = null;
                    partidasParameters.codEmpresa = null;
                    partidasParameters.cuentaContable = null;
                    partidasParameters.estatusConciliacion = Convert.ToInt16(BusinessEnumerations.Concilia.SI);
                    partidasParameters.fechaCarga = null;
                    partidasParameters.fechaConciliacion = null;
                    partidasParameters.fechaTransaccion = null;
                    partidasParameters.importe = null;
                    partidasParameters.referencia = null;
                    partidasParameters.tipoCarga = null;
                }

                var source = partidasAprobadas.Query(

                    c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                    && c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                    && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                    && c.PA_STATUS_PARTIDA == (aprobado) //&& c.PA_TIPO_CONCILIA == TipoConcilia
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)//userAreacod.CA_COD_AREA
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && c.PA_REFERENCIA.ToString().Length != 0
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                //var items = source.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }
                    //items = viPaApro;
                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                }
               
                return viPaApro;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<vi_PartidasAprobadas> PartidasNoConciliadas(IdentityUser user, ParametrosPartidasAprobadas partidasParameters)
        {
            try
            {

                partidasParameters.tipoCarga = null;
                
                //partidasParameters.fechaConciliacion = null;
                //IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

                int pendconciliar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);

                partidasParameters.estatusConciliacion = Convert.ToInt16(BusinessEnumerations.Concilia.NO);

                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                if (partidasParameters == null)
                {
                    partidasParameters = new ParametrosPartidasAprobadas();
                    partidasParameters.codArea = null;
                    partidasParameters.codEmpresa = null;
                    partidasParameters.cuentaContable = null;
                    partidasParameters.estatusConciliacion = Convert.ToInt16(BusinessEnumerations.Concilia.NO);
                    partidasParameters.fechaCarga = null;
                    partidasParameters.fechaConciliacion = null;
                    partidasParameters.fechaTransaccion = null;
                    partidasParameters.importe = null;
                    partidasParameters.referencia = null;
                    partidasParameters.tipoCarga = null;
                }

                var source = partidasAprobadas.Query(

                    c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                    && c.PA_FECHA_CARGA <= ( partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                    //&& c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                    && ((c.PA_STATUS_PARTIDA == pendconciliar && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)) 
                    || (c.PA_STATUS_PARTIDA == aprobado && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)))
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)//userAreacod.CA_COD_AREA
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && c.PA_REFERENCIA.ToString().Length != 0
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                //var items = source.OrderBy(c => c.PA_REGISTRO).Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }
                    //items = viPaApro;
                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                }


                  var Referencias = from h in viPaApro group h by h.PA_REFERENCIA into y
                                  select new { referencia = y.Key,
                                               importe = y.Sum(r=>r.PA_IMPORTE)};

                foreach(var reg in Referencias)
                {
                   
                    if (reg.importe==0)
                    {
                        viPaApro.RemoveAll(j => j.PA_REFERENCIA == reg.referencia);
                    }
                    
                }


            
                return viPaApro;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<vi_PartidasAprobadas> PartidasParcialmenteConciliadas(IdentityUser user, ParametrosPartidasAprobadas partidasParameters)
        {
            try
            {

                partidasParameters.tipoCarga = null;
                partidasParameters.estatusConciliacion = 0;


                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

                int TipoConcilia = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.PARCIAL);


                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
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

                var source = partidasAprobadas.Query(

                    c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                    && c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.PA_FECHA_CONCILIA != (null)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                    && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                    && c.PA_STATUS_PARTIDA == (aprobado)
                    && c.PA_TIPO_CONCILIA == (TipoConcilia)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)//userAreacod.CA_COD_AREA
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && c.PA_REFERENCIA.ToString().Length != 0
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }
                    //items = viPaApro;
                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                }
            
                return viPaApro;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<vi_PartidasAprobadas> PartidasAnuladas(IdentityUser user, ParametrosPartidasAprobadas partidasParameters )
        {
            try
            {

                partidasParameters.tipoCarga = null;

                //busco Comprobantes de conciliacion
                int tipoComp = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);
                int EstatusConc = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                List<ComprobanteModel> comp = comprobanteService.GetAll(c => c.TC_COD_OPERACION == tipoComp , null, includes: c => c.AspNetUsers).ToList();

                //Busco Area del usuario
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }

            

                int StatusPorConciliar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);
                int StatusAprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);

                
                int TipoConciliaManual = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.MANUAL);

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

                var source = partidasAprobadas.Query(

                    c => c.RC_COD_OPERACION == (partidasParameters.tipoCarga == null ? c.RC_COD_OPERACION : partidasParameters.tipoCarga)
                    && c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_IMPORTE == (partidasParameters.importe == null ? c.PA_IMPORTE : partidasParameters.importe)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.PA_FECHA_CONCILIA == (partidasParameters.fechaConciliacion == null ? c.PA_FECHA_CONCILIA : partidasParameters.fechaConciliacion)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)
                   // && c.PA_ESTADO_CONCILIA == (partidasParameters.estatusConciliacion == null ? c.PA_ESTADO_CONCILIA : partidasParameters.estatusConciliacion)
                    && ( (c.PA_STATUS_PARTIDA == StatusAprobado && c.PA_TIPO_CONCILIA == TipoConciliaManual) || c.PA_STATUS_PARTIDA == StatusPorConciliar)
                    && c.RC_COD_AREA == (partidasParameters.codArea == null ? c.RC_COD_AREA : partidasParameters.codArea)//userAreacod.CA_COD_AREA
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && c.PA_FECHA_ANULACION != null
                    && c.PA_REFERENCIA.ToString().Length != 0
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);

                //Validamos que Si tiene mas de un registro en comprobante detalle

                   
                var viPaApro = new List<vi_PartidasAprobadas>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.RC_COD_AREA == areaItem.CA_COD_AREA)
                            {
                                viPaApro.Add(item);
                            }
                        }
                    }
                    //items = viPaApro;
                }
                else if (partidasParameters.codArea != null)
                {
                    viPaApro = source.ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                }

                //int cuentaComprobante = 0;
              

               

                return viPaApro;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("GetConsultaPartidasAprobadas"), HttpGet]
        public async Task<IHttpActionResult> GetConsultaPartidasAprobadas2([FromUri]ParametrosPartidasAprobadas partidasParameters)
        {

            try
            {
 
                var viPaApro = new List<vi_PartidasAprobadas>();

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                int tipoComp = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);
                int EstatusConc = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                List<ComprobanteModel> model = comprobanteService.GetAll(c => c.TC_COD_OPERACION == tipoComp && c.TC_ESTATUS == EstatusConc, null, includes: c => c.AspNetUsers).ToList();

                List<EventosModel> eventos = eventoServ.GetAll(c=>c.EV_ESTATUS == 1,null,includes: c => c.AspNetUsers).ToList(); 

                if (partidasParameters.tipoCarga == 0) //aprobadas
                {
                    viPaApro = PartidasAp(user, partidasParameters);
                }
                else
                if (partidasParameters.tipoCarga == 1) // conciliadas
                {
                    viPaApro = PartidasConciliadas(user, partidasParameters);
                }
                else
                if (partidasParameters.tipoCarga == 2) // ParcialmenteConciliadas
                {
                    viPaApro = PartidasParcialmenteConciliadas(user, partidasParameters);
                }
                else

                if (partidasParameters.tipoCarga == 3) // No Conciliadas
                {
                    viPaApro = PartidasNoConciliadas(user, partidasParameters);
                }
                else
                if (partidasParameters.tipoCarga == 4) // Anuladas
                {
                    viPaApro = PartidasAnuladas(user, partidasParameters );

                }
                else

                {
                    viPaApro = null;
                }
                int count = viPaApro.Count();
                int CurrentPage = partidasParameters.pageNumber;
                int PageSize = partidasParameters.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                var itemList = new List<PartidasAprobadasModel>();
                viPaApro.ForEach(c =>
                {
                    itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasAprobadas, PartidasAprobadasModel>(c));
                });

                foreach (var c in itemList)
                {

                    
                    foreach (var j in model)
                    {
                       var compdetalle =  j.SAX_COMPROBANTE_DETALLE.Where(v => v.PA_REGISTRO == c.PA_REGISTRO).ToList();
                        if (compdetalle.Count >0)
                        {
                            c.comprobanteConciliacion = j.TC_COD_COMPROBANTE;
                        }
                    }

                    foreach (var ev in eventos)
                    {
                        var ListaEventos = eventos.Where(h => h.EV_COD_EVENTO == c.EV_COD_EVENTO).ToList();
                        if (ListaEventos.Count > 0)
                        {
                            c.EventoDescripcion = ev.EV_DESCRIPCION_EVENTO;
                        }
                    }

                   
                }


                var returnlist = itemList.Select(x => new
                {
                    Empresa = x.EmpresaDesc,
                    FechaCarga = x.PA_FECHA_CARGA.Value.ToShortDateString().ToString(),
                    HoraCarga = getHora(x.PA_HORA_CREACION),// arreglar 
                    FechaTrx = x.PA_FECHA_TRX.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    //x.PA_FECHA_CARGA.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    CuentaContable = x.PA_CTA_CONTABLE,
                    CentroCosto = x.PA_CENTRO_COSTO,
                    Moneda = x.PA_COD_MONEDA,
                    Importe = x.PA_IMPORTE.ToString("N2"),
                    Referencia = x.PA_REFERENCIA,
                    Explicacion = x.PA_EXPLICACION,
                    PlanAccion = x.PA_PLAN_ACCION,
                    ConceptoCosto = x.PA_CONCEPTO_COSTO,
                    UsuarioCarga = x.UsuarioC_Nombre,
                    UsuarioAprobador = x.UsuarioAprob_Nombre,
                    AplicacionOrigen = x.PA_APLIC_ORIGEN,
                    TipoConciliacion = x.TipoConciliaDesc,
                    EstatusConciliacion = x.EstadoConciliaDesc,
                    ImportePendiente = x.PA_IMPORTE_PENDIENTE.ToString("N2"),
                    DocumentodeCompensacion = x.comprobanteConciliacion,
                    FechaConciliacion = string.IsNullOrEmpty(x.PA_FECHA_CONCILIA.ToString()) ? "" : x.PA_FECHA_CONCILIA.Value.ToShortDateString().ToString(),
                    FechaAnulacion = string.IsNullOrEmpty(x.PA_FECHA_ANULACION.ToString()) ? "" : x.PA_FECHA_ANULACION.Value.ToShortDateString().ToString(),
                    UsuarioAnulacion = string.IsNullOrEmpty(x.PA_USUARIO_ANULACION.ToString())?"" :x.PA_USUARIO_ANULACION.ToString(),
                    DiasAntigüedad = x.PA_DIAS_ANTIGUEDAD,
                    OrigendeAsignacionReferencia = x.OrigenRefDesc,
                    OrigenCarga = x.OperacionDesc,
                    Evento = string.IsNullOrEmpty(x.EV_COD_EVENTO.ToString())?"": (x.EV_COD_EVENTO.ToString() + "-" + x.EventoDescripcion).ToString(),
                    Campo1 = x.PA_CAMPO_1,
                    Campo2 = x.PA_CAMPO_2,
                    Campo3 = x.PA_CAMPO_3,
                    Campo4 = x.PA_CAMPO_4,
                    Campo5 = x.PA_CAMPO_5,
                    Campo6 = x.PA_CAMPO_6,
                    Campo7 = x.PA_CAMPO_7,
                    Campo8 = x.PA_CAMPO_8,
                    Campo9 = x.PA_CAMPO_9,
                    Campo10 = x.PA_CAMPO_10,
                    Campo11 = x.PA_CAMPO_11,
                    Campo12 = x.PA_CAMPO_12,
                    Campo13 = x.PA_CAMPO_13,
                    Campo14 = x.PA_CAMPO_14,
                    Campo15 = x.PA_CAMPO_15,
                    Campo16 = x.PA_CAMPO_16,
                    Campo17 = x.PA_CAMPO_17,
                    Campo18 = x.PA_CAMPO_18,
                    Campo19 = x.PA_CAMPO_19,
                    Campo20 = x.PA_CAMPO_20,
                    Campo21 = x.PA_CAMPO_21,
                    Campo22 = x.PA_CAMPO_22,
                    Campo23 = x.PA_CAMPO_23,
                    Campo24 = x.PA_CAMPO_24,
                    Campo25 = x.PA_CAMPO_25,
                    Campo26 = x.PA_CAMPO_26,
                    Campo27 = x.PA_CAMPO_27,
                    Campo28 = x.PA_CAMPO_28,
                    Campo29 = x.PA_CAMPO_29,
                    Campo30 = x.PA_CAMPO_30,
                    Campo31 = x.PA_CAMPO_31,
                    Campo32 = x.PA_CAMPO_32,
                    Campo33 = x.PA_CAMPO_33,
                    Campo34 = x.PA_CAMPO_34,
                    Campo35 = x.PA_CAMPO_35,
                    Campo36 = x.PA_CAMPO_36,
                    Campo37 = x.PA_CAMPO_37,
                    Campo38 = x.PA_CAMPO_38,
                    Campo39 = x.PA_CAMPO_39,
                    Campo40 = x.PA_CAMPO_40,
                    Campo41 = x.PA_CAMPO_41,
                    Campo42 = x.PA_CAMPO_42,
                    Campo43 = x.PA_CAMPO_43,
                    Campo44 = x.PA_CAMPO_44,
                    Campo45 = x.PA_CAMPO_45,
                    Campo46 = x.PA_CAMPO_46,
                    Campo47 = x.PA_CAMPO_47,
                    Campo48 = x.PA_CAMPO_48,
                    Campo49 = x.PA_CAMPO_49,
                    Campo50 = x.PA_CAMPO_50
            });
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
                return Ok(returnlist);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private string getHora(TimeSpan? hora) {
            TimeSpan ts = TimeSpan.FromTicks(DateTime.Now.ToUniversalTime().Ticks);
            string result= string.Empty;
            if (hora == null)
                return result;
            DateTime time = DateTime.Today.Add(hora.Value);
            result = time.ToString("hh:mm tt");
            return result;
        }

    }
}
