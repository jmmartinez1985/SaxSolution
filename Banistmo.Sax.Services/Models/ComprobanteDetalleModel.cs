using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ComprobanteDetalleModel
    {
        public int TD_ID_DETALLE { get; set; }
        public int TC_ID_COMPROBANTE { get; set; }
        public int PA_REGISTRO { get; set; }
        public System.DateTime TD_FECHA_CREACION { get; set; }
        public string TD_USUARIO_CREACION { get; set; }
        public System.DateTime TD_FECHA_MOD { get; set; }
        public string TD_USUARIO_MOD { get; set; }

        public ComprobanteModel SAX_COMPROBANTE { get; set; }
        public PartidasModel SAX_PARTIDAS { get; set; }

    }
}
