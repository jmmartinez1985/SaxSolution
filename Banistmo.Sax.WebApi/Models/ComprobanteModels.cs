using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class ComprobanteModels
    {
        public DateTime? FechaCreacion { get; set; }
        public string empresaCod { get; set; }
        public int? comprobanteId { get; set; }
        public int? cuentaContableId { get; set; }
        public decimal? importe { get; set; }
        public string referencia { get; set; }
    }
}