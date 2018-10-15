using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/EmpresaCentro")]
    public class EmpresaCentroController : ApiController
    {
        private IEmpresaCentroService service;
        private ICentroCostoService centroCostoService;

        public EmpresaCentroController()
        {
            service = service ?? new EmpresaCentroService();
            centroCostoService = centroCostoService ?? new CentroCostoService();
        }

        //public EmpresaCentroController(IEmpresaCentroService svc)
        //{
        //    service = svc;
        //}

        public IHttpActionResult Get()
        {
            List<EmpresaCentroModel> dfs = service.GetAll();
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs);
        }

        [Route("GetCentroCostoByIdEmpresaStandar"), HttpGet]
        public IHttpActionResult GetCentroCostoByIdEmpresaStandar(int id)
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<EmpresaCentroModel> dfs = service.GetAllFlatten<EmpresaCentroModel>(e => e.CE_ID_EMPRESA == id && e.EC_ESTATUS == activo);
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs.Select(d => new {
                disabled = false,
                CC_ID_CENTRO_COSTO = d.CC_ID_CENTRO_COSTO,
                text = NameCentroCosto(d.CC_ID_CENTRO_COSTO)
            }));
        }

        [Route("GetCentroCostoByIdEmpresa"), HttpGet]
        public IHttpActionResult GetCentroCostoByIdEmpresa( int id )
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<EmpresaCentroModel> dfs = service.GetAllFlatten<EmpresaCentroModel>( e=> e.CE_ID_EMPRESA==id && e.EC_ESTATUS== activo);
            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs.Select(d=> new {
                disabled = false,
                id = CodeCentroCosto(d.CC_ID_CENTRO_COSTO),
                text = NameCentroCosto(d.CC_ID_CENTRO_COSTO)
            }));
        }

        [Route("GetCentroCostoByIdEmpresaForSelect2"), HttpGet]
        public IHttpActionResult GetCentroCostoByIdEmpresaForSelect2(int id)
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<CentroCostoModel> listaCentroCosto = centroCostoService.Query(x => x.CC_ID_CENTRO_COSTO == x.CC_ID_CENTRO_COSTO).Select(y => new CentroCostoModel
            {
                CC_ID_CENTRO_COSTO = y.CC_ID_CENTRO_COSTO,
                CC_NOMBRE = y.CC_NOMBRE,
                CC_CENTRO_COSTO = y.CC_CENTRO_COSTO
            }).ToList();

            var dfs = service.Query(e => e.CE_ID_EMPRESA == id && e.EC_ESTATUS == activo).Select( x=> new  {
                CE_ID_EMPRESA = x.CE_ID_EMPRESA,
                CC_ID_CENTRO_COSTO = x.CC_ID_CENTRO_COSTO,
            }).ToList();

            if (dfs == null)
            {
                return NotFound();
            }
            return Ok(dfs.Select(d => new {
                disabled = false,
                id = CodeCentroCosto(d.CC_ID_CENTRO_COSTO,ref listaCentroCosto),
                text = NameCentroCosto(d.CC_ID_CENTRO_COSTO,ref listaCentroCosto)
            }));
        }
        private string NameCentroCosto(int centroCosto, ref List<CentroCostoModel> listCentroCosto)
        {
            string name = string.Empty;
            CentroCostoModel centroCostoTMP = listCentroCosto.FirstOrDefault(cc => cc.CC_ID_CENTRO_COSTO == centroCosto);
            if (centroCostoTMP != null)
                name = $"{centroCostoTMP.CC_CENTRO_COSTO}-{centroCostoTMP.CC_NOMBRE}";
            return name;
        }

        private string NameCentroCosto(int centroCosto) {
            string name = string.Empty;
            var centroCostoTMP = centroCostoService.GetSingle(cc => cc.CC_ID_CENTRO_COSTO == centroCosto);
            if (centroCostoTMP != null)
                name = $"{centroCostoTMP.CC_CENTRO_COSTO}-{centroCostoTMP.CC_NOMBRE}";
            return name;
        }

        private string CodeCentroCosto(int centroCosto)
        {
            string name = string.Empty;
            var centroCostoTMP = centroCostoService.GetSingle(cc => cc.CC_ID_CENTRO_COSTO == centroCosto);
            if (centroCostoTMP != null)
                name = $"{centroCostoTMP.CC_CENTRO_COSTO}";
            return name;
        }

        private string CodeCentroCosto(int centroCosto, ref List<CentroCostoModel> listCentroCosto)
        {
            string name = string.Empty;
            var centroCostoTMP = listCentroCosto.FirstOrDefault(cc => cc.CC_ID_CENTRO_COSTO == centroCosto);
            if (centroCostoTMP != null)
                name = $"{centroCostoTMP.CC_CENTRO_COSTO}";
            return name;
        }

        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.EC_ID_REGISTRO == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] EmpresaCentroModel model)
        {
            model.EC_ESTATUS = 1;
            return Ok(service.Insert(model, true));
        }


        [Route("UpdateEmpresaCentro"), HttpPost]
        public IHttpActionResult Put([FromBody] EmpresaCentroModel model)
        {
            service.Update(model);
            return Ok();
        }
    }
}
