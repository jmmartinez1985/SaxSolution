using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository;
using System.Globalization;
using Banistmo.Sax.Repository.Model;
using Newtonsoft.Json;

namespace Banistmo.Sax.WebApi.Controllers

{
    [Authorize]
    [RoutePrefix("api/ReportePartidasApr")]
    public class ReportePartidasAprController : ApiController
    {
        private readonly IReportePartidasAprService partAprSrv;
       // private readonly IReporterService reportExcelService;
        private ICatalogoService catalagoService;
        private readonly IComprobanteService serviceComprobante;
        private IReportePartidasAprService partidaService;
        private IUsuarioAreaService usuarioAreaService;
        private ApplicationUserManager _userManager;
        private IAreaOperativaService areaOperativaService;
        private IUserService usuarioService;
        private readonly ICatalogoService catalogoService;
        private IComprobanteService comprobanteService;
        private IEventosService eventoServ;

        public ReportePartidasAprController()
        {
            partAprSrv = partAprSrv ?? new ReportePartidasAprService();
          //  reportExcelService = reportExcelService ?? new ReporterService();
            catalagoService = new CatalogoService();
            serviceComprobante = new ComprobanteService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            usuarioService = usuarioService ?? new UserService();

        }
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

        public ReportePartidasAprController(IReportePartidasAprService PartService,  ICatalogoService cat,
            ICatalogoService serv, IComprobanteService comprob, IUsuarioAreaService userArea, IAreaOperativaService area, IUserService usuario)
        {
           // reportService = rep;
           // reportExcelService = repexcel;
            catalagoService = serv;
            serviceComprobante = comprob;
            usuarioAreaService = userArea;
            areaOperativaService = area;
            usuarioService = usuario;
            catalogoService = cat;
            partAprSrv = PartService;
            comprobanteService = comprobanteService ?? new ComprobanteService();
            eventoServ = eventoServ ?? new EventosService();
        }

        [Route("GetReportePartidasApr"), HttpGet]
        public async Task<IHttpActionResult> GetPartidasAprobadas([FromUri]ParametrosPartidasApr parms)
        {
            try
            {
                var viPaApro = new List<vi_PartidasApr>();

                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                int tipoComp = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);
                int EstatusConc = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                List<ComprobanteModel> model = comprobanteService.GetAll(c => c.TC_COD_OPERACION == tipoComp && c.TC_ESTATUS == EstatusConc, null, includes: c => c.AspNetUsers).ToList();

                List<EventosModel> eventos = eventoServ.GetAll(c => c.EV_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();

                viPaApro = PartidasAp(user, parms);

                int count = viPaApro.Count();
                int CurrentPage = parms.pageNumber;
                int PageSize = parms.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";

                var itemList = new List<ReportePartidasAprModel>();
                viPaApro.ForEach(c =>
                {
                    itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasApr, ReportePartidasAprModel>(c));
                });

                foreach (var c in itemList)
                {


                    foreach (var j in model)
                    {
                        var compdetalle = j.SAX_COMPROBANTE_DETALLE.Where(v => v.PA_REGISTRO == c.PA_REGISTRO).ToList();
                        if (compdetalle.Count > 0)
                        {
                            c.ComprobanteConciliacion = j.TC_COD_COMPROBANTE;
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
                    FechaCarga = x.PA_FECHA_CARGA.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    HoraCarga = getHora(x.PA_HORA_CREACION),
                    FechaTrx = x.PA_FECHA_TRX.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    CuentaContable = x.PA_CTA_CONTABLE,
                    CentroCosto = x.PA_CENTRO_COSTO,
                    Moneda = x.PA_COD_MONEDA,
                    Importe = x.PA_IMPORTE.ToString("N2"),
                    Referencia = x.PA_REFERENCIA,
                    Explicacion = x.PA_EXPLICACION,
                    PlanAccion = x.PA_PLAN_ACCION,
                    ConceptoCosto = x.PA_CONCEPTO_COSTO,
                    UsuarioCarga = x.PA_USUARIO_CREACION != null? GetNameUser(x.PA_USUARIO_CREACION):"",
                    UsuarioAprobador = x.PA_USUARIO_APROB!= null ? GetNameUser(x.PA_USUARIO_APROB) : "", //x.UsuarioAprob_Nombre,
                    AplicacionOrigen = x.PA_APLIC_ORIGEN,
                    TipoConciliacion = GetNameCodigo(x.PA_TIPO_CONCILIA.ToString(), "sax_tipo_conciliacion"), //
                    EstatusConciliacion = GetNameCodigo(x.PA_ESTADO_CONCILIA.ToString(), "sax_concilia"), //
                    ImportePendiente = x.PA_IMPORTE_PENDIENTE.ToString("N2"),
                    DocumentodeCompensacion = x.ComprobanteConciliacion,
                    FechaConciliacion = string.IsNullOrEmpty(x.PA_FECHA_CONCILIA.ToString()) ? "" : x.PA_FECHA_CONCILIA.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(),
                    FechaAnulacion = string.IsNullOrEmpty(x.PA_FECHA_ANULACION.ToString()) ? "" : x.PA_FECHA_ANULACION.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(),
                    UsuarioAnulacion = x.PA_USUARIO_ANULACION != null ? GetNameUser(x.PA_USUARIO_ANULACION):"",
                    //x.AspNetUsers2!=null ? x.AspNetUsers2.LastName : "",
                    //string.IsNullOrEmpty(x.PA_USUARIO_ANULACION.ToString()) ? "" : x.PA_USUARIO_ANULACION.ToString(),
                    DiasAntigüedad = x.PA_DIAS_ANTIGUEDAD,
                    OrigendeAsignacionReferencia = GetNameCodigo(x.PA_ORIGEN_REFERENCIA.ToString(), "sax_tipo_referencia"), //
                    OrigenCarga = GetNameCodigo(x.RC_COD_OPERACION.ToString(), "sax_tipo_operacion"), //
                    Evento = string.IsNullOrEmpty(x.EV_COD_EVENTO.ToString()) ? "" : (x.EV_COD_EVENTO.ToString() + "-" + x.EventoDescripcion).ToString(),
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private List<vi_PartidasApr> PartidasAp(IdentityUser user, ParametrosPartidasApr partidasParameters)
        {
            try
            {
                                
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }
                userAreacod.Add(areaOperativaService.GetSingle(h => h.CA_NOMBRE.Contains("Generica")));

                if (partidasParameters == null)
                {
                    partidasParameters = new ParametrosPartidasApr();
                    partidasParameters.codArea = null;
                    
                    partidasParameters.cuentaContable = null;
                    
                    partidasParameters.fechaCarga = null;
                    //partidasParameters.fechaConciliacion = null;
                    partidasParameters.fechaTransaccion = null;
                    
                    partidasParameters.referencia = null;
                    
                }

                var source = partAprSrv.Query(

                    c => c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)                    
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.CA_COD_AREA == (partidasParameters.codArea == null ? c.CA_COD_AREA : partidasParameters.codArea)
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);



                var viPaApro = new List<vi_PartidasApr>();

                if (partidasParameters.codArea == null)
                {
                    foreach (var areaItem in userAreacod)
                    {
                        foreach (var item in source)
                        {
                            if (item.CA_COD_AREA == areaItem.CA_COD_AREA)
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

        private string getHora(TimeSpan? hora)
        {
            TimeSpan ts = TimeSpan.FromTicks(DateTime.Now.ToUniversalTime().Ticks);
            string result = string.Empty;
            if (hora == null)
                return result;
            DateTime time = DateTime.Today.Add(hora.Value);
            result = time.ToString("hh:mm tt");
            return result;
        }

        private string GetNameCodigo(string id, string Tabla)
        {
            var model = catalagoService.GetAll(c => c.CA_TABLA ==Tabla, null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
       
            string name = string.Empty;
            
            if (model != null)
            {
                CatalogoDetalleModel cataloDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS.ToString() == id).FirstOrDefault();
                if (cataloDetalle != null)
                    name = cataloDetalle.CD_VALOR;
               
            }
            return name;
        }

        private string GetNameUser(string id)
        {

            string name = string.Empty;
            var ltsUsuarios = usuarioService.GetAll(c => c.Id == id).FirstOrDefault();
            if (ltsUsuarios != null)
            {

                name = ltsUsuarios.LastName.ToString();
            }


            return name;
        }
    }
}