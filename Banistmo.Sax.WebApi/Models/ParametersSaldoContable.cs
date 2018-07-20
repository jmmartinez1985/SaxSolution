using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class ParametersSaldoContable
    {
        public int? IdEmpresa { get; set; }
        public Nullable<System.DateTime> FechaCorte { get; set; }
        public string IdCuentaContable { get; set; }
        public int? IdAreaOperativa { get; set; }

    }
}