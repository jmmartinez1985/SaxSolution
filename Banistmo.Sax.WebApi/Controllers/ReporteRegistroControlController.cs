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

        public ReporteRegistroControlController()
        {
            reportService = reportService ?? new ReporteRegistroControlService();
            reportExcelService = reportExcelService ?? new ReporterService();
            catalagoService = new CatalogoService();
            serviceComprobante = new ComprobanteService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
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

        public ReporteRegistroControlController(IReporteRegistroControlService rep, IReporterService repexcel,
            ICatalogoService serv, IComprobanteService comprob, IUsuarioAreaService userArea, IAreaOperativaService area)
        {
            reportService = rep;
            reportExcelService = repexcel;
            catalagoService = serv;
            serviceComprobante = comprob;
            usuarioAreaService = userArea;
            areaOperativaService = area;
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

                var rgcont = GetRegistroControlFiltro(parms);


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


                if (retorno != null)
                {
                    return Ok(retorno);
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

        public List<ReporteRegistroControlPartialModel> GetRegistroControlFiltro(ParametersRegistroControl parms)
        {

            List<ReporteRegistroControlModel> registrocontrol;
            DateTime ParfechaAc = DateTime.Now.Date.Date;
            registrocontrol = reportService.GetAll(c => c.RC_FECHA_CREACION >= ParfechaAc, null, includes: c => c.AspNetUsers).ToList();
            // List<PartidasAprobadasModel> partidas = partidaService.GetAll(c => c.PA_FECHA_CREACION >= ParfechaAc);
            List<ComprobanteModel> comprobante = serviceComprobante.GetAll(c => c.TC_FECHA_CREACION >= ParfechaAc, null, includes: c => c.AspNetUsers).ToList();
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            int aprobacion = Convert.ToInt16(parms.TipoAprobacion);
            var ltsTipoOperacion = catalagoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();

            if (parms != null)
            {
                if (parms.TipoAprobacion != null && parms.TipoAprobacion != string.Empty)
                {
                    registrocontrol = registrocontrol.Where(x => x.RC_COD_OPERACION.Equals(aprobacion)).ToList();

                    comprobante = comprobante.Where(x => x.TC_COD_OPERACION.Equals(aprobacion)).ToList();
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

            List<ReporteRegistroControlPartialModel> Lista = (from c in registrocontrol
                                                              select new ReporteRegistroControlPartialModel
                                                              {
                                                                  Supervisor = c.AspNetUsers != null ? c.AspNetUsers.LastName : "",
                                                                  NombreOperacion = GetNameTipoOperacion(c.RC_COD_OPERACION.ToString(), ref ltsTipoOperacion),
                                                                  Lote = c.RC_COD_PARTIDA,
                                                                  Capturador = c.AspNetUsers1 != null ? c.AspNetUsers1.LastName : "",
                                                                  TotalRegistro = c.RC_TOTAL_REGISTRO,
                                                                  TotalDebito = c.RC_TOTAL_DEBITO,
                                                                  TotalCredito = c.RC_TOTAL_CREDITO,
                                                                  Total = c.RC_TOTAL,
                                                                  Status = GetStatusRegistroControl(c.RC_ESTATUS_LOTE.ToString(), estatusList),
                                                                  FechaCreacion = c.RC_FECHA_CREACION.ToString("yyyy/MM/dd"),
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
                                                                   Capturador = c.AspNetUsers != null ? c.AspNetUsers.LastName : "",
                                                                   TotalRegistro = c.TC_TOTAL_REGISTRO,
                                                                   TotalDebito = c.TC_TOTAL_DEBITO,
                                                                   TotalCredito = c.TC_TOTAL_CREDITO,
                                                                   Total = c.TC_TOTAL,
                                                                   Status = GetStatusRegistroControl(c.TC_ESTATUS, estatusList),
                                                                   FechaCreacion = c.TC_FECHA_CREACION.ToString("yyyy/MM/dd"),
                                                                   HoraCreacion = c.TC_FECHA_CREACION.ToString("HH:mm:ss"),
                                                                   AreaId = Convert.ToInt16(c.CA_ID_AREA)
                                                               }).ToList();

            List<ReporteRegistroControlPartialModel> Lista3 = Lista.Union(Lista2).ToList();
            return Lista3;
        }
    }
}
