using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class PartidaModel
    {
        public int RC_REGISTRO_CONTROL { get; set; }
      
        public int PA_CONTADOR { get; set; }
       
        public string PA_COD_EMPRESA { get; set; }
     
        public string PA_CTA_CONTABLE { get; set; }
      
        public string PA_CENTRO_COSTO { get; set; }
     
        public string PA_COD_MONEDA { get; set; }
       
        public decimal PA_IMPORTE { get; set; }
       
        public string PA_REFERENCIA { get; set; }
        
        public string PA_EXPLICACION { get; set; }
       
        public string PA_PLAN_ACCION { get; set; }
        
        public string PA_CONCEPTO_COSTO { get; set; }

        public int pageNumber { get; set; }

        public int pageSize { get; set; }

    }

    public class PlanAccionModel
    {
        public int PA_REGISTRO { get; set; }
        public string PA_PLAN_ACCION { get; set; }
    }
}