using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class LoadModel
    {
        public int area { get; set; }
        public int tipoOperacion { get; set; }
        public int? cod_event { get; set; }
    }
}