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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/ReporteSaldoContable")]
    public class ReporteSaldoContableController : ApiController
    {
        private readonly ISaldoContableService reportService;
        private readonly IReporterService reportExcelService;
        private ApplicationUserManager _userManager;
        private IUsuarioAreaService usuarioAreaService;
        private readonly IAreaOperativaService areaOperativa;
        private readonly IUserService usuarioSerive;
        private IAreaOperativaService areaOperativaService;

        public ReporteSaldoContableController()
        {
            reportService = reportService ?? new ReporteSaldoContableService();
            reportExcelService = reportExcelService ?? new ReporterService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            usuarioSerive = usuarioSerive ?? new UserService();
        }

        public ReporteSaldoContableController(ISaldoContableService rep, IReporterService repexcel, IUsuarioAreaService userArea, IAreaOperativaService area, IUserService usuario)
        {
            reportService = rep;
            reportExcelService = repexcel;
            usuarioAreaService = userArea;
            
            usuarioSerive = usuario;
            areaOperativaService = area;
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
        [Route("GetSaldoContable"), HttpGet]
        public async Task<IHttpActionResult> GetSaldoContable([FromUri]ParametersSaldoContable parms)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
            var userAreacod = new List<AreaOperativaModel>();
           
            foreach (var item in userArea)
            {
                userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
            }
           
            List<ReporteSaldoContablePartialModel> SaldoContable = GetSaldoContableFiltro(parms);
            var SaldoContableReturn = new List<ReporteSaldoContablePartialModel>();

            if (SaldoContable != null)
            {
                if (parms.IdAreaOperativa== null)
                {
                    foreach(var a in userAreacod)
                    {
                        foreach(var b in SaldoContable )
                        {
                            string nombrearea = a.CA_COD_AREA + " " + a.CA_NOMBRE;
                            if (b.nombreareaoperativa == nombrearea)
                            {
                                SaldoContableReturn.Add(b);
                            }
                        }
                    }
                }
                else
                {
                    SaldoContableReturn = SaldoContable.ToList();
                    }

                 return Ok(SaldoContableReturn);
            }
            return NotFound();
        }

        [Route("GetReporteExcelSaldoContable"), HttpGet]
        //public HttpResponseMessage GetReporteExcelSaldoContable([FromUri]ParametersSaldoContable parms)
        public  async Task<HttpResponseMessage> GetReporteExcelSaldoContable([FromUri]ParametersSaldoContable parms)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
            var userAreacod = new List<AreaOperativaModel>();

            foreach (var item in userArea)
            {
                userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
            }

            List<ReporteSaldoContablePartialModel> Lista = GetSaldoContableFiltro(parms);
            var SaldoContableReturn = new List<ReporteSaldoContablePartialModel>();

            if (Lista != null)
            {
                if (parms.IdAreaOperativa == null)
                {
                    foreach (var a in userAreacod)
                    {
                        foreach (var b in Lista)
                        {
                            string nombrearea = a.CA_COD_AREA + " " + a.CA_NOMBRE;
                            if (b.nombreareaoperativa == nombrearea)
                            {
                                SaldoContableReturn.Add(b);
                            }
                        }
                    }
                }
                else
                {
                    SaldoContableReturn = Lista.ToList();
                }
            }
                MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            header.Add(new string[] { "A" });
            header.Add(new string[] { "B" });
            header.Add(new string[] { "C" });
            header.Add(new string[] { "D" });
            header.Add(new string[] { "E" });
            byte[] fileExcell = reportExcelService.CreateReportBinary<ReporteSaldoContablePartialModel>(header, SaldoContableReturn, "Excel1");
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

        public List<ReporteSaldoContablePartialModel> GetSaldoContableFiltro(ParametersSaldoContable parms)
        {
            try {
                var model = reportService.GetAll(null,null,includes: c => c.AspNetUsers);
                if (parms != null)
                {
                    if (parms.IdEmpresa != null)
                        model = model.Where(x => x.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_ID_EMPRESA.Equals(parms.IdEmpresa)).ToList();

                    if (parms.FechaCorte != null)
                        model = model.Where(x => x.SA_FECHA_CORTE.Date == Convert.ToDateTime(parms.FechaCorte).Date).ToList();

                    if (parms.IdCuentaContable != null)
                        
                        model = model.Where(x => (x.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE + x.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR + x.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR).Equals(parms.IdCuentaContable)).ToList();

                    if (parms.IdAreaOperativa != null)
                        model = model.Where(x => x.SAX_CUENTA_CONTABLE.ca_id_area.Equals(parms.IdAreaOperativa)).ToList();
                }

                List<ReporteSaldoContablePartialModel> ListSaldos = (from c in model
                                                                     select new ReporteSaldoContablePartialModel
                                                                     {
                                                                         nombreempresa = c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_COD_EMPRESA.Trim() + " " + c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_NOMBRE.Trim(),
                                                                         codcuentacontable = c.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE.Trim() + c.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR.Trim() + c.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR.Trim(),
                                                                         nombrecuentacontable = c.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA.Trim(),
                                                                         nombreareaoperativa = c.SAX_CUENTA_CONTABLE.SAX_AREA_OPERATIVA.CA_COD_AREA + " " + c.SAX_CUENTA_CONTABLE.SAX_AREA_OPERATIVA.CA_NOMBRE.Trim(),
                                                                         fechaforte = c.SA_FECHA_CORTE.ToString("yyyy/MM/dd"),
                                                                         codmoneda = c.SAX_MONEDA.CC_NUM_MONEDA,
                                                                         saldo = c.SA_SALDOS

                                                                     }).ToList();

                return ListSaldos;
            }
            catch (Exception e){
                return null;
            }
            
        }
    }
}
