using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Banistmo.Sax.Common;
using System.Globalization;


namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/ReporteRegistroControl")]
    public class ReporteRegistroControlController : ApiController
    {
        private readonly IReporteRegistroControlService reportService;
        private readonly IReporterService reportExcelService;
        private ICatalogoService catalagoService;
        private readonly IComprobanteService serviceComprobante;
        private IPartidasAprobadasService partidaService;
        private IUsuarioAreaService usuarioAreaService;
        private ApplicationUserManager _userManager;
        private IAreaOperativaService areaOperativaService;
        private IUserService usuarioService;
        private readonly ICatalogoService catalogoService;

        public ReporteRegistroControlController()
        {
            reportService = reportService ?? new ReporteRegistroControlService();
            reportExcelService = reportExcelService ?? new ReporterService();
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

        public ReporteRegistroControlController(IReporteRegistroControlService rep, IReporterService repexcel, ICatalogoService cat,
            ICatalogoService serv, IComprobanteService comprob, IUsuarioAreaService userArea, IAreaOperativaService area, IUserService usuario)
        {
            reportService = rep;
            reportExcelService = repexcel;
            catalagoService = serv;
            serviceComprobante = comprob;
            usuarioAreaService = userArea;
            areaOperativaService = area;
            usuarioService = usuario;
            catalogoService = cat;
        }

        [Route("GetRegistroControl"), HttpGet]
        public async Task<IHttpActionResult> GetRegistroControl([FromUri]ParametersRegistroControl parms)
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

                //var rgcont = GetRegistroControlFiltro(parms);


                List<ReporteRegistroControlPartialModel> RegistroControl = GetRegistroControlFiltro(parms);

                var retorno = new List<ReporteRegistroControlPartialModel>();

                foreach (var a in RegistroControl)
                {
                    foreach (var b in userAreacod)
                        if (a.AreaId == b.CA_ID_AREA)
                        {
                            retorno.Add(a);
                        }
                }

                var ListRetorno = retorno.Select(c => new
                {
                    Supervisor = c.Supervisor,
                    NombreOperacion = c.NombreOperacion,
                    Lote = c.Lote,
                    Capturador = c.Capturador,
                    TotalRegistro = c.TotalRegistro,
                    TotalDebito = c.TotalDebito.ToString("N2"),
                    TotalCredito = c.TotalCredito.ToString("N2"),
                    Total = c.Total.ToString("N2"),
                    Status = c.Status,
                    FechaCreacion = c.FechaCreacion,
                    HoraCreacion = c.HoraCreacion

                });

                if (ListRetorno != null)
                {
                    return Ok(ListRetorno);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetReporteExcelRegistroControl"), HttpGet]
        public HttpResponseMessage GetReporteExcelRegistroControl([FromUri]ParametersRegistroControl parms)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            List<ReporteRegistroControlPartialModel> Lista = GetRegistroControlFiltro(parms);

            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            header.Add(new string[] { "A" });
            header.Add(new string[] { "B" });
            header.Add(new string[] { "C" });
            header.Add(new string[] { "D" });
            header.Add(new string[] { "E" });
            byte[] fileExcell = reportExcelService.CreateReportBinary<ReporteRegistroControlPartialModel>(header, Lista, "Excel1");
            var contentLength = fileExcell.Length;

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


        private string GetNameTipoOperacion(string id, ref CatalogoModel model)
        {
            string name = string.Empty;
            if (model != null)
            {
                CatalogoDetalleModel cataloDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS.ToString() == id).FirstOrDefault();
                if (cataloDetalle != null)
                    name = cataloDetalle.CD_VALOR;
            }
            return name;
        }

        private string GetStatusRegistroControl(string idStatus, CatalogoModel model)
        {
            int status = Convert.ToInt16(idStatus);
            string result = string.Empty;
            if (model != null)
            {
                var modelCatalogoDetalle = model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS == status).FirstOrDefault();
                if (modelCatalogoDetalle != null)
                    result = modelCatalogoDetalle.CD_VALOR;
            }
            return result;
        }

        private string GetNameSupervisor(string id )
        {
           
            string name = string.Empty;
            var ltsUsuarios = usuarioService.GetAll(c=>c.Id == id).FirstOrDefault();
            if (ltsUsuarios != null)
            {
                
                name = ltsUsuarios.LastName.ToString();
            }


            return name;
        }

        public List<ReporteRegistroControlPartialModel> GetRegistroControlFiltro(ParametersRegistroControl parms)
        {

            List<ReporteRegistroControlModel> Registrocontrol;
            DateTime ParfechaAc = DateTime.Now.Date.Date;
            // ultima version
            Registrocontrol = reportService.GetAll(c => c.RC_FECHA_CREACION >= ParfechaAc, null, includes: c => c.AspNetUsers).ToList();
   
            List<ComprobanteModel> Comprobante = serviceComprobante.GetAll(c => c.TC_FECHA_CREACION >= ParfechaAc, null, includes: c => c.AspNetUsers).ToList();
           
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            
            var ltsTipoOperacion = catalagoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var ListaOperacion = ltsTipoOperacion.SAX_CATALOGO_DETALLE.Where(s => s.CD_VALOR != "CONCILIACION" ).ToList();

            //var retorno = new List<ReporteRegistroControlPartialModel>();

            int EstausConc = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
            string Conc_Aut = Convert.ToString(BusinessEnumerations.TipoConciliacion.AUTOMATICO);
            string Conc_Man = Convert.ToString(BusinessEnumerations.TipoConciliacion.MANUAL);
            int EstatusAnul = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);
            Comprobante = Comprobante.Where(t => (  t.TC_ESTATUS != EstausConc.ToString()) ).ToList();

            var comprobante = new List<ComprobanteModel>();
           var registrocontrol = new List< ReporteRegistroControlModel>()  ;
            foreach (var g in ListaOperacion)
                {
                    foreach (var r in Registrocontrol)  
                {
                    if(r.RC_COD_OPERACION == g.CD_ESTATUS)
                    registrocontrol.Add(r);

                }
                    foreach(var t in Comprobante)
                {
                    if (t.TC_COD_OPERACION == g.CD_ESTATUS.ToString())
                    {
                        comprobante.Add(t);
                    }
                }

            }
                   if (parms != null)
            {
                int aprobacion = Convert.ToInt16(parms.TipoAprobacion);
                
                
                if (parms.TipoAprobacion != null && parms.TipoAprobacion != string.Empty)
                {
                    if (aprobacion == 25) //Colocar anulaciones
                    {

                        comprobante = comprobante.Where(x => ( x.TC_ESTATUS == EstatusAnul.ToString())).ToList();
                        registrocontrol = registrocontrol.Where(x => x.RC_COD_OPERACION == aprobacion).ToList();
                    }
                    else
                    {
                        registrocontrol = registrocontrol.Where(x => x.RC_COD_OPERACION.Equals(aprobacion)).ToList();

                        comprobante = comprobante.Where(x => x.TC_COD_OPERACION == aprobacion.ToString()).ToList();
                    }
                }
                if (parms.Lote != null && parms.Lote != string.Empty)
                {
                    registrocontrol = registrocontrol.Where(x => x.RC_COD_PARTIDA.Equals(parms.Lote)).ToList();
                    comprobante = comprobante.Where(x => x.TC_COD_COMPROBANTE.Equals(parms.Lote)).ToList();
                }
                if (parms.UsuarioCapturador != null && parms.UsuarioCapturador != string.Empty)
                {
                    registrocontrol = registrocontrol.Where(x => x.RC_USUARIO_CREACION.Equals(parms.UsuarioCapturador)).ToList();
                    comprobante = comprobante.Where(x => x.TC_USUARIO_CREACION.Equals(parms.UsuarioCapturador)).ToList();
                }
            }


            foreach (var reg in comprobante)
            {
                if (reg.TC_USUARIO_APROBADOR == null)
                    reg.TC_USUARIO_APROBADOR = reg.TC_USUARIO_MOD;

                if ( reg.TC_ESTATUS == EstatusAnul.ToString()) //COMPROBANTES DE CONCILIACION
                {
                    reg.TC_USUARIO_CREACION = reg.TC_USUARIO_MOD;
                }
             }

       

            List<ReporteRegistroControlPartialModel> Lista = (from c in registrocontrol
                                                              select new ReporteRegistroControlPartialModel
                                                              {
                                                                  Supervisor = (c.AspNetUsers != null ? c.AspNetUsers.LastName : "" ),
                                                                  //Supervisor = GetNameSupervisor(c.RC_USUARIO_APROBADOR.ToString()),
                                                                  NombreOperacion = GetNameTipoOperacion(c.RC_COD_OPERACION.ToString(), ref ltsTipoOperacion),
                                                                  Lote = c.RC_COD_PARTIDA,
                                                                  Capturador = c.AspNetUsers1 != null ? c.AspNetUsers1.LastName : "",
                                                                  TotalRegistro = c.RC_TOTAL_REGISTRO,
                                                                  TotalDebito = c.RC_TOTAL_DEBITO,
                                                                  TotalCredito = c.RC_TOTAL_CREDITO,
                                                                  Total = c.RC_TOTAL,
                                                                  Status = GetStatusRegistroControl(c.RC_ESTATUS_LOTE.ToString(), estatusList),
                                                                  FechaCreacion = c.RC_FECHA_CREACION.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                                                  HoraCreacion = c.RC_FECHA_CREACION.ToString("HH:mm:ss"),
                                                                  AreaId = Convert.ToInt16(c.CA_ID_AREA)
                                                              }
                                                              ).ToList();

            List<ReporteRegistroControlPartialModel> Lista2 = (from c in comprobante
                                                               select new ReporteRegistroControlPartialModel
                                                               {
                                                                   Supervisor = c.AspNetUsers1 != null ? c.AspNetUsers1.LastName : "",
                                                                   NombreOperacion = GetNameTipoOperacion(c.TC_COD_OPERACION, ref ltsTipoOperacion),
                                                                   Lote = c.TC_COD_COMPROBANTE,
                                                                   //Capturador = c.AspNetUsers != null ? c.AspNetUsers.LastName : "",
                                                                   Capturador = GetNameSupervisor(c.TC_USUARIO_CREACION.ToString()),
                                                                   TotalRegistro = c.TC_TOTAL_REGISTRO,
                                                                   TotalDebito = c.TC_TOTAL_DEBITO,
                                                                   TotalCredito = c.TC_TOTAL_CREDITO,
                                                                   Total = c.TC_TOTAL,
                                                                   Status = GetStatusRegistroControl(c.TC_ESTATUS, estatusList),
                                                                   FechaCreacion = c.TC_FECHA_CREACION.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                                                                   HoraCreacion = c.TC_FECHA_CREACION.ToString("HH:mm:ss"),
                                                                   AreaId = Convert.ToInt16(c.CA_ID_AREA)
                                                               }).ToList();

            List<ReporteRegistroControlPartialModel> Lista3 = Lista.Union(Lista2).ToList();
            return Lista3.OrderBy(j=>j.FechaCreacion).ToList();
        }

        [Route("GetTipoOperacion"), HttpGet]
        public IHttpActionResult GetTipoOperacion()
        {
            
            var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE);

            List<CatalogoDetalleModel> estatusListDetalle = new List<CatalogoDetalleModel>();
            foreach (var details in estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE)
            {
                if (details.CD_VALOR != "CONCILIACION"  && details.CD_VALOR != "CONCILIACION AUTOMATICA" )
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

        private List<CatalogoDetalleModel> GetOperaciones()
        {
            
            var estatusList = catalogoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE);

            List<CatalogoDetalleModel> estatusListDetalle = new List<CatalogoDetalleModel>();
            foreach (var details in estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE)
            {
                if (details.CD_VALOR != "CONCILIACION" && details.CD_VALOR != "CONCILIACION AUTOMATICA")
                {
                    estatusListDetalle.Add(details);
                }
            }

            if (estatusListDetalle != null)
            {

            
                return estatusListDetalle;
            }
            // este es la ultima versionlllkk
            return estatusListDetalle;
        }

        
    }
}
