using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/SaldoNoConciliable")]
    public class SaldoNoConciliableController : ApiController
    {
        private readonly ISaldoNoConciliableService service;

        public SaldoNoConciliableController(ISaldoNoConciliableService sa)
        {
            service = sa;
        }

        public SaldoNoConciliableController()
        {
            service = service ?? new SaldoNoConciliableService();
        }

        [HttpGet, Route("GetSaldo")]
        public IHttpActionResult Get(PagingParameterModel parametro)
        {

            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var tmp = service.Query(x => x.SC_ESTATUS == activo);
            var reslt = tmp.Select(x => new
            {
                CO_ID_CUENTA_CONTABLE = x.CO_ID_CUENTA_CONTABLE,
                CE_ID_EMPRESA = x.CE_ID_EMPRESA,
                CC_ID_MONEDA = x.CC_ID_MONEDA,
                EMPRESA_NOMBRE = x.SAX_EMPRESA != null ? (x.SAX_EMPRESA.CE_COD_EMPRESA + "-" + x.SAX_EMPRESA.CE_NOMBRE) : string.Empty,
                MONEDA_NOMBRE= x.SAX_MONEDA !=null ? (x.SAX_MONEDA.CC_COD_CURRENCY):string.Empty,
                CUENTA_CONTABLE_NOMBRE = x.SAX_CUENTA_CONTABLE != null ? (x.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE+x.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR+x.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR) : string.Empty,
                SC_FECHA_CORTE = x.SC_FECHA_CORTE,
                SC_SALDOS = x.SC_SALDOS,
                SC_FECHA_CREACION = x.SC_FECHA_CREACION
            }).OrderBy(y=> y.SC_FECHA_CORTE).ToList();
            if (reslt == null)
            {
                return BadRequest("No se encontraron registros.");
            }
            return Ok(reslt);
        }
    }
}
