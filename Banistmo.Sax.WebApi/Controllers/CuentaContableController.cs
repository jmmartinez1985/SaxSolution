using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Newtonsoft.Json;
using System.Web;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;
using static Banistmo.Sax.Services.Models.BusinessEnumerations;
using System.IO;
using System.Net.Http.Headers;
using Banistmo.Sax.Services.Interfaces;
using Banistmo.Sax.Services.Implementations;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/CuentaContable")]
    public class CuentaContableController : ApiController
    {
        private ICuentaContableService service;
        private IEmpresaService empresaService;
        private IAreaOperativaService areaOperativaService;
        private IReporterService reportExcelService;


        public CuentaContableController()
        {
            service = service ?? new CuentaContableService();
            empresaService = empresaService ?? new EmpresaService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            reportExcelService = reportExcelService ?? new ReporterService();
        }

        //public CuentaContableController(ICuentaContableService svc)
        //{
        //    service = svc;
        //}

        public IHttpActionResult Get()
        {
            List<CuentaContableModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.CO_ID_CUENTA_CONTABLE == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] CuentaContableModel model)
        {
            model.CO_USUARIO_CREACION = User.Identity.GetUserId();
            model.CO_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateCuenta"), HttpPost]
        public IHttpActionResult Put([FromBody] CuentaContableModel model)
        {
            model.CO_USUARIO_MOD = User.Identity.GetUserId();
            model.CO_FECHA_MOD = DateTime.Now;
            model.CO_ESTATUS = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            service.Update(model);
            return Ok();
        }

        [Route("GetCuentaContablePag"), HttpGet]
        public IHttpActionResult GetPagination([FromUri] ParametrosCuentaContableModel pagingparametermodel)
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var source = this.GetDataReporteCuentaContable(pagingparametermodel);
            int count = source.Count();
            //TipoConciliacion.NO.ToString
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
                nextPage,
                data = items.Select(c => new {
                    CE_ID_EMPRESA = NameEmpresa(c.CE_ID_EMPRESA),
                    CO_CUENTA_CONTABLE = c.CO_CUENTA_CONTABLE,
                    CUENTA_TEXT = $"{c.CO_CUENTA_CONTABLE}- {c.CO_COD_AUXILIAR}-{c.CO_NOM_AUXILIAR}",
                    CO_NOM_CUENTA = c.CO_NOM_CUENTA,
                    CO_COD_CONCILIA = GetConcilia(c.CO_COD_CONCILIA),
                    CO_COD_NATURALEZA = GetNaturaleza(c.CO_COD_NATURALEZA),
                    CO_COD_AREA = NameAreaOperativa(c.CO_COD_AREA),
                    CO_ID_CUENTA_CONTABLE = c.CO_ID_CUENTA_CONTABLE
                })
            };
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            return Ok(paginationMetadata);

        }

        [Route("GetCtaDbCr"), HttpGet]
        public IHttpActionResult Get([FromUri ]parametroData data )
        {
            try
            {
                List<CuentaContableModel> dfs = service.GetAll(cc => (cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR).Contains(data.cuenta == null? cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR : data.cuenta)
                                                                && cc.CE_ID_EMPRESA == data.empresaId
                                                                && cc.CO_COD_NATURALEZA == data.naturalezaCta);
                if (dfs.Count == 0)
                {
                    return BadRequest("No existen registros de cuentas.");
                }
                return Ok(dfs);
            }
            catch (Exception ex)
            {
                return BadRequest("No se puede obtener las cuentas. " + ex.Message);
            }
        }

        public class parametroData
        {
            public string naturalezaCta { get; set; }
            public int? empresaId { get; set; }
            public string cuenta { get; set; }
        }

        [Route("GetCuentaContableByEmpresa"), HttpGet]
        public IHttpActionResult GetCuentaContableByEmpresa([FromUri] ParametrosCuentaContableModel model)
        {
            try
            {
                List<CuentaContableModel> dfs = service.GetAll(cc => cc.CE_ID_EMPRESA == model.Empresa );
                var list = dfs.GroupBy(cc => cc.CO_CUENTA_CONTABLE);
                if (dfs.Count == 0)
                {
                    return BadRequest("No existen registros para la búsqueda solicitada.");
                }
                return Ok(list.ToList().Select(c => new
                {
                    CuentaContable = c.Key.Trim()
                }).OrderBy(cc => cc.CuentaContable));
            }
            catch (Exception ex)
            {
                return BadRequest("No existen registros para la búsqueda solicitada. " + ex.Message);
            }
        }
        [Route("GetCodigoAuxiliarByCuentaContable"), HttpGet]
        public IHttpActionResult GetCodigoAuxiliarByCuentaContable([FromUri] ParametrosCuentaContableModel model)
        {
            try
            {
                List<CuentaContableModel> dfs = service.GetAll(cc => cc.CE_ID_EMPRESA == model.Empresa && cc.CO_CUENTA_CONTABLE == model.CuentaContable);
                var list = dfs.GroupBy(cc => cc.CO_COD_AUXILIAR);
                if (dfs.Count == 0)
                {
                    return BadRequest("No existen registros para la búsqueda solicitada.");
                }
                return Ok(list.ToList().Select(c => new
                {
                    CodigoAuxiliar = c.Key.Trim()
                }).OrderBy(cc => cc.CodigoAuxiliar));
            }
            catch (Exception ex)
            {
                return BadRequest("No existen registros para la búsqueda solicitada. " + ex.Message);
            }
        }
        [Route("GetNumeroAuxiliarByCodigoAuxiliar"), HttpGet]
        public IHttpActionResult GetNumeroAuxiliarByCodigoAuxiliar([FromUri]  ParametrosCuentaContableModel model)
        {
            try
            {
                List<CuentaContableModel> dfs = service.GetAll(cc => cc.CE_ID_EMPRESA == model.Empresa && cc.CO_CUENTA_CONTABLE == model.CuentaContable && cc.CO_COD_AUXILIAR == model.CodigoAuxiliar);
                var list = dfs.GroupBy(cc => cc.CO_NUM_AUXILIAR);
                if (dfs.Count == 0)
                {
                    return BadRequest("No existen registros para la búsqueda solicitada.");
                }
                return Ok(list.ToList().Select(c => new
                {
                    NumeroAuxiliar = c.Key.Trim()
                }).OrderBy (cc => cc.NumeroAuxiliar ));
            }
            catch (Exception ex)
            {
                return BadRequest("No existen registros para la búsqueda solicitada. " + ex.Message);
            }
        }
        [Route("GetReporteCuentaContable"), HttpGet]
        public IHttpActionResult GetReporteCuentaContable([FromUri] ParametrosCuentaContableModel model)
        {
            try
            {
                List<CuentaContableModel> dfs = service.GetAll(cc => cc.CE_ID_EMPRESA == (model.Empresa == null ? cc.CE_ID_EMPRESA : model.Empresa)
                && cc.CO_CUENTA_CONTABLE == (model.CuentaContable == null ? cc.CO_CUENTA_CONTABLE : model.CuentaContable)
                && cc.CO_COD_AUXILIAR == (model.CodigoAuxiliar == null ? cc.CO_COD_AUXILIAR : model.CodigoAuxiliar)
                && cc.CO_COD_AREA == (model.AreaOperativa == null ? cc.CO_COD_AREA : model.AreaOperativa)
                && cc.CO_COD_NATURALEZA == (model.Naturaleza == null ? cc.CO_COD_NATURALEZA : model.Naturaleza)
                && cc.CO_NUM_AUXILIAR == (model.NumeroAuxiliar == null ? cc.CO_NUM_AUXILIAR : model.NumeroAuxiliar));
                
                if (dfs.Count == 0)
                {
                    return BadRequest("No existen registros para la búsqueda solicitada.");
                }
                return Ok(dfs.ToList().Select(c => new
                {
                    Empresa = empresaService.GetSingle(x=>x.CE_ID_EMPRESA ==c.CE_ID_EMPRESA).CE_NOMBRE,
                    CuentaContable = c.CO_CUENTA_CONTABLE,
                    NombreCuenta = c.CO_NOM_CUENTA,
                    NombreAuxiliar = c.CO_NOM_AUXILIAR,
                    Concilia = c.CO_COD_CONCILIA,
                    Naturaleza = c.CO_COD_NATURALEZA,
                    AreaOperativa = c.CO_COD_AREA
                }));
            }
            catch (Exception ex)
            {
                return BadRequest("No existen registros para la búsqueda solicitada. " + ex.Message);
            }
        }

        [Route("GetReporteCuentaConcilia"), HttpGet]
        public HttpResponseMessage GetReporteCuentaConcilia([FromUri] ParametrosCuentaContableModel model) {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            MemoryStream memoryStream = new MemoryStream();
            List<string[]> header = new List<string[]>();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<CuentaContableModel> listCuentaContable = this.GetDataReporteCuentaContable(model);
            var source = listCuentaContable.Select(c => new
            {
                Empresa = NameEmpresa(c.CE_ID_EMPRESA),
                CuentaContable = c.CO_CUENTA_CONTABLE,
                NombreCuenta = $"{c.CO_CUENTA_CONTABLE}- {c.CO_COD_AUXILIAR}-{c.CO_NOM_AUXILIAR}",
                Concilia = GetConcilia(c.CO_COD_CONCILIA),
                Naturaleza = GetNaturaleza(c.CO_COD_NATURALEZA),
                AreaOperativa = NameAreaOperativa(c.CO_COD_AREA)
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

        private List<CuentaContableModel> GetDataReporteCuentaContable(ParametrosCuentaContableModel model)
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<CuentaContableModel> dfs = service.GetAll(cc => cc.CE_ID_EMPRESA == (model.Empresa == null ? cc.CE_ID_EMPRESA : model.Empresa)
            && cc.CO_CUENTA_CONTABLE == (model.CuentaContable == null ? cc.CO_CUENTA_CONTABLE : model.CuentaContable)
            && cc.CO_COD_AUXILIAR == (model.CodigoAuxiliar == null ? cc.CO_COD_AUXILIAR : model.CodigoAuxiliar)
            && cc.CO_COD_AREA == (model.AreaOperativa == null ? cc.CO_COD_AREA : model.AreaOperativa)
            && cc.CO_COD_NATURALEZA == (model.Naturaleza == null ? cc.CO_COD_NATURALEZA : model.Naturaleza)
            && cc.CO_NUM_AUXILIAR == (model.NumeroAuxiliar == null ? cc.CO_NUM_AUXILIAR : model.NumeroAuxiliar)
            && cc.CO_ESTATUS==activo);
            return dfs.ToList();
        }

        private string NameEmpresa(int empresa)
        {
            string name = string.Empty;
            var result = empresaService.GetSingle(e => e.CE_ID_EMPRESA == empresa);
            if (result != null)
                name = $"{result.CE_COD_EMPRESA}-{result.CE_NOMBRE}";
            return name;
        }

        private string NameAreaOperativa(string areaOperativa)
        {
            string name = string.Empty;
            if (areaOperativa != null) {
                var result = areaOperativaService.GetSingle(cc => cc.CA_COD_AREA.ToString()==areaOperativa);
                if (result != null)
                    name = $"{result.CA_COD_AREA}-{result.CA_NOMBRE}";
            }
            
            return name;
        }

        private string GetNaturaleza(string parm) {
            string result = string.Empty;
            if (parm != null && parm != string.Empty) {
                result = parm.Equals("C") ? Naturaleza.CREDITO.ToString() : Naturaleza.DEBITO.ToString();
            }
            return result;
        }

        private string GetConcilia(string parm)
        {
            string result = string.Empty;
            if (parm != null && parm != string.Empty)
            {
                result = parm.Equals("Y") ? TipoConciliacion.SI.ToString() : TipoConciliacion.NO.ToString();
            }
            return result;
        }


    }

}
