using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class CuentasContableMin
    {
        public int CO_ID_CUENTA_CONTABLE { get; set; }
        public int CO_INSTITUCION { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string CO_CUENTA_CONTABLE { get; set; }
    }
}