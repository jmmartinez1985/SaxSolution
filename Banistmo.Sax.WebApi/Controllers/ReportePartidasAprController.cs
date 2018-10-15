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
        private IReportePartidasAprConciliableService partApConcSrv;
        private IUsuarioAreaService usuarioAreaService;
        private ApplicationUserManager _userManager;
        private IAreaOperativaService areaOperativaService;
        private IUserService usuarioService;
        private readonly ICatalogoService catalogoService;
        private IComprobanteService comprobanteService;
        private IEventosService eventoServ;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;
        private IParametroService parService;
        private ICuentaContableService Ctaservice;

        public ReportePartidasAprController()
        {
            partAprSrv = partAprSrv ?? new ReportePartidasAprService();
          //  reportExcelService = reportExcelService ?? new ReporterService();
            catalagoService = new CatalogoService();
            serviceComprobante = new ComprobanteService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            usuarioService = usuarioService ?? new UserService();
            partApConcSrv = partApConcSrv ?? new ReportePartidasAprConciliablesService();
            parService = parService ?? new ParametroService();
            Ctaservice = Ctaservice ?? new CuentaContableService();

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
            ICatalogoService serv, IComprobanteService comprob, IUsuarioAreaService userArea, IAreaOperativaService area, IUserService usuario,
            IReportePartidasAprConciliableService PartApConcSrv, IUsuarioEmpresaService objUsuarioAreaService)
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
            partApConcSrv = PartApConcSrv;
            usuarioEmpresaService = objUsuarioAreaService;
            parService = parService ?? new ParametroService();

        }

        [Route("GetReportePartidas"), HttpGet]
        public async Task<IHttpActionResult> GetPartidasAprobadas([FromUri]ParametrosPartidasApr parms)
        {
            try
            {
                var viPaApro = new List<vi_PartidasApr>();

                var PaConc = new List<vi_PartidasApr_Conciliadas>();

                var itemList = new List<ReportePartidasAprModel>();

                var PartidasNat = new List<vi_PartidasApr_Conciliadas>();

                List<ComprobanteModel> model = new List<ComprobanteModel>();
                int tipoComp = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION);
                int TipoConcMan = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CONCILIACION_MANUAL);
                int EstatusConc = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                int ParSConciliado = Convert.ToInt16(BusinessEnumerations.Concilia.SI);
                int ParTipoConPar = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.PARCIAL);
                //int EstatusAn = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);

                switch (parms.TipoReporte)
                {
                    case 0:
                        viPaApro = PartidasAp(parms);
                        viPaApro.ForEach(c =>
                        {
                            itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasApr, ReportePartidasAprModel>(c));
                        });
                        break;
                    case 1:
                        PaConc = PartidasConciliadas(parms);
                        PaConc.ToList().ForEach(c =>
                        {
                            itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasApr_Conciliadas, ReportePartidasAprModel>(c));
                        });
                        
                        break;
                    case 2: // ParcialmenteConciliadas
                        PaConc = PartidasParcConciliadas(parms);
                        PaConc.ToList().ForEach(c =>
                        {
                            itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasApr_Conciliadas, ReportePartidasAprModel>(c));
                        });
                        break;
                    case 3: // No Conciliadas
                        PaConc = PartidasPendConciliar(parms);
                        PaConc.ToList().ForEach(c =>
                        {
                            itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasApr_Conciliadas, ReportePartidasAprModel>(c));
                        });
                        break;
                    case 4: // Anuladas
                        PaConc = PartidasAnuladas(parms);
                        PaConc.ToList().ForEach(c =>
                        {
                            itemList.Add(Extension.CustomMapIgnoreICollection<vi_PartidasApr_Conciliadas, ReportePartidasAprModel>(c));
                        });
                        break;

                }



                int count = itemList.Count();
                //viPaApro.Count();

                if (count > 0)
                {
                    IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());


                    model = comprobanteService.GetAll(c => (c.TC_COD_OPERACION == tipoComp || c.TC_COD_OPERACION == TipoConcMan) && c.TC_ESTATUS == EstatusConc, null, includes: c => c.AspNetUsers).ToList();

                    List<EventosModel> eventos = eventoServ.GetAll(c => c.EV_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();

                    int CurrentPage = parms.pageNumber;
                    int PageSize = parms.pageSize;
                    int TotalCount = count;
                    int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                    var previousPage = CurrentPage > 1 ? "Yes" : "No";
                    var nextPage = CurrentPage < TotalPages ? "Yes" : "No";


                    var itemListxArea = new List<ReportePartidasAprModel>();
                    var itemListxEmpresa = new List<ReportePartidasAprModel>();
                    var ListaSele = new List<ReportePartidasAprModel>();
                    //var returnlist = new List<ReportePartidasAprModel>();

                    //var Par = parService.GetSingle();

                    //var FechaP = Par.PA_FECHA_PROCESO;

                    int OrigenAuto = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                    
                    var FechaCorte = parms.fechaCarga;

                    if (parms.IdEmpresa != null)
                    {
                        itemListxEmpresa = itemList.Where(x => x.PA_COD_EMPRESA == parms.IdEmpresa.ToString()).ToList();
                    }
                    else
                    {
                        List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
                        var listEmpresas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, c => c.SAX_EMPRESA);
                        if (listEmpresas.Count > 0)
                        {
                            foreach (var emp in listEmpresas)
                            {
                                listUsuarioEmpresas.Add(emp);
                            }
                        }

                        foreach (var j in itemList)
                        {
                            if (listUsuarioEmpresas.Count(x => x.CE_ID_EMPRESA.ToString() == j.PA_COD_EMPRESA) > 0)
                            {
                                itemListxEmpresa.Add(j);
                            }
                        }
                    }


                    if (parms.codArea == null)
                    {
                        var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                        var userAreacod = new List<AreaOperativaModel>();
                        foreach (var item in userArea)
                        {
                            userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                        }
                        userAreacod.Add(areaOperativaService.GetSingle(h => h.CA_NOMBRE.Contains("Generica")));

                        foreach (var b in itemListxEmpresa)
                        {
                            if (userAreacod.Count(v => v.CA_ID_AREA == b.CA_ID_AREA) > 0)
                            {
                                itemListxArea.Add(b);
                            }
                        }
                    }
                    else
                    {
                        itemListxArea = itemListxEmpresa.ToList();
                    }


                    foreach (var c in itemListxArea)
                    {


                        foreach (var j in model)
                        {
                            var compdetalle = j.SAX_COMPROBANTE_DETALLE.Where(v => v.PA_REGISTRO == c.PA_REGISTRO).ToList();
                            if (compdetalle.Count > 0)
                            {
                                c.ComprobanteConciliacion = j.TC_COD_COMPROBANTE;
                                c.Usuario_Conciliador = j.TC_USUARIO_CREACION;
                                c.Aprobador_Conciliacion = j.TC_USUARIO_APROBADOR;
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

                        if (c.PA_ESTADO_CONCILIA != ParSConciliado)
                        {
                            

                            c.PA_DIAS_ANTIGUEDAD = Convert.ToInt32(parms.fechaCarga.Date.Subtract(c.PA_FECHA_TRX.Value.Date).TotalDays.ToString());

                            if (c.PA_ORIGEN_REFERENCIA == OrigenAuto)
                            {
                                if (c.PA_FECHA_ANULACION == null)
                                {
                                    var partidasConc = partApConcSrv.Query(s => s.PA_REFERENCIA.Length > 0 
                                                                                && (s.PA_ESTADO_CONCILIA != ParSConciliado
                                                                                || (s.PA_ESTADO_CONCILIA == ParSConciliado
                                                                                && s.PA_TIPO_CONCILIA == ParTipoConPar))
                                                                                && s.PA_FECHA_ANULACION == null
                                                                                && s.PA_REFERENCIA == c.PA_REFERENCIA
                                                                                && s.PA_FECHA_CARGA <= parms.fechaCarga).ToList();
                                    var Referencias = from h in partidasConc
                                                      group h by h.PA_REFERENCIA into y
                                                      select new
                                                      {
                                                          referencia = y.Key,
                                                          importe = y.Sum(r => r.PA_IMPORTE)
                                                      };
                                    if (Referencias.Count() > 0)
                                        c.PA_IMPORTE_PENDIENTE = Referencias.SingleOrDefault(k => k.referencia == c.PA_REFERENCIA).importe;
                                    //else
                                    //    c.PA_IMPORTE_PENDIENTE
                                }
                                else
                                    //calcular importe de acuerdo a la naturaleza excuyendo las que estan en el mismo comprobante
                                {
                                    //var PartidasNat = new List<vi_PartidasApr_Conciliadas>();
                                    if (c.PA_IMPORTE > 0)
                                        
                                    {

                                        var PartidasxNat = partApConcSrv.Query(q => q.PA_CTA_CONTABLE == c.PA_CTA_CONTABLE && q.PA_COD_EMPRESA == c.PA_COD_EMPRESA
                                                                               && q.PA_COD_MONEDA == c.PA_COD_MONEDA && q.PA_CENTRO_COSTO == c.PA_CENTRO_COSTO
                                                                               && q.PA_FECHA_ANULACION == null
                                                                               && ((q.PA_ESTADO_CONCILIA == ParSConciliado && q.PA_TIPO_CONCILIA == ParTipoConPar)
                                                                               || q.PA_ESTADO_CONCILIA != ParSConciliado && q.PA_FECHA_CONCILIA == null)
                                                                               && q.PA_IMPORTE > 0
                                                                               && q.PA_FECHA_CARGA <= parms.fechaCarga
                                                                               ).ToList();
                                        var Referencias = from h in PartidasxNat
                                                          group h by h.PA_REFERENCIA into y
                                                          select new
                                                          {
                                                              referencia = y.Key,
                                                              importe = y.Sum(r => r.PA_IMPORTE)
                                                          };
                                        if (Referencias.Where(k => k.referencia == c.PA_REFERENCIA).Count() > 0)
                                        {
                                            c.PA_IMPORTE_PENDIENTE = c.PA_IMPORTE + Referencias.SingleOrDefault(k => k.referencia == c.PA_REFERENCIA).importe;
                                        }
                                        else
                                            c.PA_IMPORTE_PENDIENTE = c.PA_IMPORTE;
                                    }
                                    else
                                    {
                                        var PartidasxNat = partApConcSrv.Query(q => q.PA_CTA_CONTABLE == c.PA_CTA_CONTABLE && q.PA_COD_EMPRESA == c.PA_COD_EMPRESA
                                                                              && q.PA_COD_MONEDA == c.PA_COD_MONEDA && q.PA_CENTRO_COSTO == c.PA_CENTRO_COSTO
                                                                              && q.PA_FECHA_ANULACION == null
                                                                              && ((q.PA_ESTADO_CONCILIA == ParSConciliado && q.PA_TIPO_CONCILIA == ParTipoConPar)
                                                                              || q.PA_ESTADO_CONCILIA != ParSConciliado && q.PA_FECHA_CONCILIA == null)
                                                                              && q.PA_IMPORTE < 0
                                                                              && q.PA_FECHA_CARGA <= parms.fechaCarga
                                                                              ).ToList();
                                        var Referencias = from h in PartidasxNat
                                                          group h by h.PA_REFERENCIA into y
                                                          select new
                                                          {
                                                              referencia = y.Key,
                                                              importe = y.Sum(r => r.PA_IMPORTE)
                                                          };
                                        if (Referencias.Where(k => k.referencia == c.PA_REFERENCIA).Count() > 0) {
                                           

                                            c.PA_IMPORTE_PENDIENTE = c.PA_IMPORTE + Referencias.FirstOrDefault(k => k.referencia == c.PA_REFERENCIA).importe;
                                        }
                                           
                                        else
                                            c.PA_IMPORTE_PENDIENTE = c.PA_IMPORTE;
                                    }
                                    
                                    //var Referencias = from h in PartidasxNat
                                    //                  group h by h.PA_REFERENCIA into y
                                    //                  select new
                                    //                  {
                                    //                      referencia = y.Key,
                                    //                      importe = y.Sum(r => r.PA_IMPORTE)
                                    //                  };
                                    //c.PA_IMPORTE_PENDIENTE = c.PA_IMPORTE + Referencias.SingleOrDefault(k => k.referencia == c.PA_REFERENCIA).importe;
                                }

                            }
                               
                           
                        }
                      


                    }

                    ListaSele = itemListxArea.ToList();

                    if (parms.TipoReporte == 1)
                    {
                        ListaSele = ListaSele.Where(k => k.ComprobanteConciliacion != null).ToList();
                    }

                    if (parms.TipoReporte == 2)
                    {
                        ListaSele = ListaSele.Where(k => k.ComprobanteConciliacion == null).ToList();
                    }
                    //if (parms.TipoReporte == 4)
                    //{
                    //    modelanul = comprobanteService.GetAll(c => c.TC_COD_OPERACION == tipoComp &&  c.TC_ESTATUS == EstatusAn, null, includes: c => c.AspNetUsers).ToList();

                    //}

                    var  returnlist = ListaSele.Select(x => new
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
                        UsuarioCarga = x.PA_USUARIO_CREACION != null ? GetNameUser(x.PA_USUARIO_CREACION) : "",
                        UsuarioAprobador = x.PA_USUARIO_APROB != null ? GetNameUser(x.PA_USUARIO_APROB) : "", //x.UsuarioAprob_Nombre,
                        AplicacionOrigen = x.PA_APLIC_ORIGEN,
                        TipoConciliacion = GetNameCodigo(x.PA_TIPO_CONCILIA.ToString(), "sax_tipo_conciliacion"), //
                        EstatusConciliacion = GetNameCodigo(x.PA_ESTADO_CONCILIA.ToString(), "sax_concilia"), //
                        ImportePendiente = x.PA_IMPORTE_PENDIENTE.ToString("N2"),
                        DocumentodeCompensacion = string.IsNullOrEmpty(x.ComprobanteConciliacion) ? "": x.ComprobanteConciliacion,
                        UsuarioConciliador = string.IsNullOrEmpty(x.Usuario_Conciliador) ? "": GetNameUser(x.Usuario_Conciliador) ,
                        AprobadorConciliacion =  string.IsNullOrEmpty(x.Aprobador_Conciliacion) ?  "": GetNameUser(x.Aprobador_Conciliacion) ,
                        FechaConciliacion = string.IsNullOrEmpty(x.PA_FECHA_CONCILIA.ToString()) ? "" : x.PA_FECHA_CONCILIA.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(),
                        FechaAnulacion = string.IsNullOrEmpty(x.PA_FECHA_ANULACION.ToString()) ? "" : x.PA_FECHA_ANULACION.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture).ToString(),
                        UsuarioAnulacion = string.IsNullOrEmpty(x.PA_USUARIO_ANULACION) ?"": GetNameUser(x.PA_USUARIO_ANULACION) ,
                        AprobadorAnulacion = string.IsNullOrEmpty(x.PA_USUARIO_APROBADOR_ANULACION) ? "": GetNameUser(x.PA_USUARIO_APROBADOR_ANULACION) ,
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
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private List<vi_PartidasApr> PartidasAp( ParametrosPartidasApr partidasParameters)
        {
            try
            {                
                
                var source = partAprSrv.Query(

                    c => c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    //&& c.PA_COD_EMPRESA == (partidasParameters.IdEmpresa.ToString() == null ? c.PA_COD_EMPRESA : partidasParameters.IdEmpresa.ToString())
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)                    
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    //&& c.CA_COD_AREA == (partidasParameters.codArea == null ? c.CA_COD_AREA : partidasParameters.codArea)
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);



               
                return source.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<vi_PartidasApr_Conciliadas> PartidasConciliadas( ParametrosPartidasApr partidasParameters)
        {
            //var Listretur = new List<vi_PartidasApr_Conciliadas>();
            try
            {

                int EstadoConcilia = Convert.ToInt16(BusinessEnumerations.Concilia.SI);
               


                var source = partApConcSrv.Query(
                    //arreglar filtro
                    c => c.PA_FECHA_CONCILIA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.CA_COD_AREA == (partidasParameters.codArea == null ? c.CA_COD_AREA : partidasParameters.codArea)
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && (
                    c.PA_ESTADO_CONCILIA == EstadoConcilia)
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);

                return source.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        private List<vi_PartidasApr_Conciliadas> PartidasAnuladas(ParametrosPartidasApr partidasParameters)
        {
            try
            {

                
                int StatusPorConciliar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);
                //int StatusAprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                int TipoConciliaManual = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.MANUAL);
                //var itemList = new List<ReportePartidasAprModel>();
                
                var source = partApConcSrv.Query(
                    //arreglar filtro
                    c => c.PA_FECHA_ANULACION <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_ANULACION : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.CA_COD_AREA == (partidasParameters.codArea == null ? c.CA_COD_AREA : partidasParameters.codArea)
                    && c.PA_USUARIO_ANULACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_ANULACION : partidasParameters.usuarioCarga)
                    && (c.PA_TIPO_CONCILIA == TipoConciliaManual || c.PA_STATUS_PARTIDA == StatusPorConciliar )
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                return source.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<vi_PartidasApr_Conciliadas> PartidasParcConciliadas(ParametrosPartidasApr partidasParameters)
        {
            try
            {

                int TipoConcilia = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.PARCIAL);
                //int EstadoConcilia = Convert.ToInt16(BusinessEnumerations.Concilia.NO);
                //var itemList = new List<ReportePartidasAprModel>();
                var source = partApConcSrv.Query(
                    //arreglar filtro
                    c =>
                    c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    &&
                    c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.CA_COD_AREA == (partidasParameters.codArea == null ? c.CA_COD_AREA : partidasParameters.codArea)
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    
                    && c.PA_TIPO_CONCILIA == TipoConcilia
                    
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                return source.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<vi_PartidasApr_Conciliadas> PartidasPendConciliar(ParametrosPartidasApr partidasParameters)
        {
            try
            {

                
                int EstadoConcilia = Convert.ToInt16(BusinessEnumerations.Concilia.NO) ;
                int pendconciliar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);
                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                var itemList = new List<ReportePartidasAprModel>();
                var source = partApConcSrv.Query(
                    //arreglar filtro
                    c => c.PA_FECHA_CARGA <= (partidasParameters.fechaCarga == null ? c.PA_FECHA_CARGA : partidasParameters.fechaCarga)
                    && c.PA_FECHA_TRX == (partidasParameters.fechaTransaccion == null ? c.PA_FECHA_TRX : partidasParameters.fechaTransaccion)
                    && c.PA_CTA_CONTABLE == (partidasParameters.cuentaContable == null ? c.PA_CTA_CONTABLE : partidasParameters.cuentaContable)
                    && c.PA_REFERENCIA == (partidasParameters.referencia == null ? c.PA_REFERENCIA : partidasParameters.referencia)
                    && c.CA_COD_AREA == (partidasParameters.codArea == null ? c.CA_COD_AREA : partidasParameters.codArea)
                    && c.PA_USUARIO_CREACION == (partidasParameters.usuarioCarga == null ? c.PA_USUARIO_CREACION : partidasParameters.usuarioCarga)
                    && c.PA_ESTADO_CONCILIA == EstadoConcilia
                    && ((c.PA_STATUS_PARTIDA == pendconciliar && c.PA_ESTADO_CONCILIA == EstadoConcilia)
                    || ( c.PA_ESTADO_CONCILIA == EstadoConcilia))
                    ).OrderBy(c => c.RC_REGISTRO_CONTROL).ThenBy(n => n.PA_CONTADOR);


                return source.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private int CalculaImportePendiente(int paRegistro, string cuenta, string moneda, string centroCosto, int empresa, int area, decimal importe)
        {
            //Determinar si es Inicial
            //int concilia = Convert.ToInt16(BusinessEnumerations.Concilia.SI);
            //int areagenerica = areaOperativaService.GetSingle(r => r.CA_NOMBRE.Contains("generica")).CA_ID_AREA;
            //var ListaCtas = Ctaservice.GetAll(s => s.CE_ID_EMPRESA == empresa
            //                && s.CO_CUENTA_CONTABLE.TrimEnd() + s.CO_COD_AUXILIAR.TrimEnd() + s.CO_NUM_AUXILIAR.TrimEnd() == cuenta
            //                && s.CO_COD_CONCILIA == concilia.ToString()
            //                && (s.ca_id_area == area || s.ca_id_area == areagenerica)).ToList();
            // if el importe es + es un debito si no es un credito
            //Calcular Importe de acuerdo a la Naturaleza



            return 0;
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
            if (string.IsNullOrEmpty(id) == true)
            {
                name = "";
            }
            else
            {

                var ltsUsuarios = usuarioService.GetAll(c => c.Id == id).FirstOrDefault();
                if (ltsUsuarios != null)
                {

                    name = ltsUsuarios.FirstName.ToString();
                }
                else name = "";
            }

            return name;
        }
    }
}