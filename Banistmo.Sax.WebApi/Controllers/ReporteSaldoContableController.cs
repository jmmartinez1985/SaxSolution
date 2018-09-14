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
using Banistmo.Sax.Common;

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
        private IUsuarioEmpresaService usuarioEmpService;
        private readonly IUsuarioEmpresaService usuarioEmpresaService;

        public ReporteSaldoContableController()
        {
            reportService = reportService ?? new ReporteSaldoContableService();
            reportExcelService = reportExcelService ?? new ReporterService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            usuarioSerive = usuarioSerive ?? new UserService();
            usuarioEmpService = usuarioEmpService ?? new UsuarioEmpresaService();
        }

        public ReporteSaldoContableController(ISaldoContableService rep, IReporterService repexcel, IUsuarioAreaService userArea, IAreaOperativaService area, IUserService usuario,
            IUsuarioEmpresaService objUsuarioAreaService)
        {
            reportService = rep;
            reportExcelService = repexcel;
            usuarioAreaService = userArea;

            usuarioSerive = usuario;
            areaOperativaService = area;

            usuarioEmpresaService = objUsuarioAreaService;
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


            List<ReporteSaldoContablePartialModel> SaldoContable = GetSaldoContableFiltro(parms, user);


            if (SaldoContable != null)
            {
                var retornaSaldo = SaldoContable.Select(g =>
                     new
                     {
                         nombreempresa = g.nombreempresa,
                         codcuentacontable = g.codcuentacontable,
                         nombrecuentacontable = g.nombrecuentacontable,
                         nombreareaoperativa = g.nombreareaoperativa,
                         codmoneda = g.codmoneda,
                         saldo = g.saldo.ToString("N2")
                     });

                return Ok(retornaSaldo);
            }
            return NotFound();
        }

        [Route("GetReporteExcelSaldoContable"), HttpGet]
        //public HttpResponseMessage GetReporteExcelSaldoContable([FromUri]ParametersSaldoContable parms)
        public async Task<HttpResponseMessage> GetReporteExcelSaldoContable([FromUri]ParametersSaldoContable parms)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
            var userAreacod = new List<AreaOperativaModel>();

            foreach (var item in userArea)
            {
                userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
            }

            List<ReporteSaldoContablePartialModel> Lista = GetSaldoContableFiltro(parms, user);
  
            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            header.Add(new string[] { "A" });
            header.Add(new string[] { "B" });
            header.Add(new string[] { "C" });
            header.Add(new string[] { "D" });
            header.Add(new string[] { "E" });
            byte[] fileExcell = reportExcelService.CreateReportBinary<ReporteSaldoContablePartialModel>(header, Lista, "Excel1");
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

        public List<ReporteSaldoContablePartialModel> GetSaldoContableFiltro(ParametersSaldoContable parms, IdentityUser user)
        {
            try
            {
                int CodConcilia = Convert.ToInt16(BusinessEnumerations.Concilia.SI);

                // obtengo todos los saldos de las cuentas conciliables
                var model = reportService.GetAll(null, null, includes: c => c.AspNetUsers).Where(r => r.SAX_CUENTA_CONTABLE.CO_COD_CONCILIA == CodConcilia.ToString()
                  && r.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_ID_EMPRESA == (parms.IdEmpresa == null ? r.SAX_CUENTA_CONTABLE.CE_ID_EMPRESA : parms.IdEmpresa)
                  && r.SA_FECHA_CORTE.Date <= (Convert.ToDateTime(parms.FechaCorte).Date == null ? r.SA_FECHA_CORTE.Date : Convert.ToDateTime(parms.FechaCorte).Date)
                  && r.SAX_CUENTA_CONTABLE.CO_ID_CUENTA_CONTABLE == (parms.IdCuentaContable == null ? r.SAX_CUENTA_CONTABLE.CO_ID_CUENTA_CONTABLE : parms.IdCuentaContable)
                  && r.SAX_CUENTA_CONTABLE.ca_id_area == (parms.IdAreaOperativa == null ? r.SAX_CUENTA_CONTABLE.ca_id_area : parms.IdAreaOperativa)
                );

                // Inicio filtro de Empresas

                List<UsuarioEmpresaModel> listUsuarioEmpresas = new List<UsuarioEmpresaModel>();
                //IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var listEmpresas = usuarioEmpresaService.GetAll(c => c.US_ID_USUARIO == user.Id, null, c => c.SAX_EMPRESA);
                if (listEmpresas.Count > 0)
                {
                    foreach (var emp in listEmpresas)
                    {
                        listUsuarioEmpresas.Add(emp);
                    }
                }

                // Inicio filtro por area del usuario
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id && d.UA_ESTATUS == 1, null, includes: c => c.AspNetUsers).ToList();
                var userAreacod = new List<AreaOperativaModel>();
                
               
                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA ));

                }
                userAreacod.Add(areaOperativaService.GetSingle(h => h.CA_NOMBRE.Contains("Generica")));

                var SaldoContable_Emp = new List<ReporteSaldoContableModel>();
                var SaldoContable = new List<ReporteSaldoContableModel>();

                if (model != null)
                {

                    if (parms.IdEmpresa == null)
                    {
                        foreach (var a in listUsuarioEmpresas)
                        {
                            foreach (var b in model)
                            {
                                if (b.SAX_CUENTA_CONTABLE.CE_ID_EMPRESA == a.CE_ID_EMPRESA)
                                {
                                    SaldoContable_Emp.Add(b);
                                }
                            }
                        }

                    }
                    else
                    {
                        SaldoContable_Emp = model.ToList();

                    }
                    if (parms.IdAreaOperativa == null)
                    {
                        foreach (var a in userAreacod)
                        {
                            foreach (var b in SaldoContable_Emp)
                            {
                                if (b.SAX_CUENTA_CONTABLE.ca_id_area == a.CA_ID_AREA )
                                {
                                    SaldoContable.Add(b);
                                }
                               
                            }
                        }
                    }
                    else
                    {
                        SaldoContable = model.ToList();
                    }
                }


                var ListSaldos = (from c in SaldoContable
                                  select new ReporteSaldoContablePartialModel
                                  {
                                      nombreempresa = c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_COD_EMPRESA.Trim() + " " + c.SAX_CUENTA_CONTABLE.SAX_EMPRESA.CE_NOMBRE.Trim(),
                                      codcuentacontable = c.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE.Trim() + c.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR.Trim() + c.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR.Trim(),
                                      nombrecuentacontable = c.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA.Trim(),
                                      nombreareaoperativa = c.SAX_CUENTA_CONTABLE.SAX_AREA_OPERATIVA.CA_COD_AREA + " " + c.SAX_CUENTA_CONTABLE.SAX_AREA_OPERATIVA.CA_NOMBRE.Trim(),
                                      // fechaforte = c.SA_FECHA_CORTE.ToString("yyyy/MM/dd"),
                                      codmoneda = c.SAX_MONEDA.CC_NUM_MONEDA,
                                      saldo = c.SA_SALDOS

                                  }).ToList();

                var res =
                    from p in ListSaldos.ToList()
                    group p by new
                    {
                        p.nombreempresa,
                        p.codmoneda,
                        p.nombreareaoperativa,
                        p.nombrecuentacontable,
                        p.codcuentacontable
                    } into g

                    select new
                    {
                        nombreempresa = g.Key.nombreempresa,
                        codcuentacontable = g.Key.codcuentacontable,
                        nombrecuentacontable = g.Key.nombrecuentacontable,
                        nombreareaoperativa = g.Key.nombreareaoperativa,
                        codmoneda = g.Key.codmoneda,
                        saldo = g.Sum(y => y.saldo)
                    };

                List<ReporteSaldoContablePartialModel> ListSaldosRetur = (from c in res
                                                                          select new ReporteSaldoContablePartialModel
                                                                          {
                                                                              nombreempresa = c.nombreempresa,
                                                                              codcuentacontable = c.codcuentacontable,
                                                                              nombrecuentacontable = c.nombrecuentacontable,
                                                                              nombreareaoperativa = c.nombreareaoperativa,
                                                                              codmoneda = c.codmoneda,
                                                                              saldo = c.saldo

                                                                          }).ToList();

                ListSaldosRetur = ListSaldosRetur.OrderBy(c => c.nombreempresa).ToList();
                return ListSaldosRetur;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
