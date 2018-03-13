using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class CentroCostoModel
    {
        public int CC_ID_CENTRO_COSTO { get; set; }
        public string CC_CENTRO_COSTO { get; set; }
        public string CC_NOMBRE { get; set; }
        public int CC_ESTATUS { get; set; }
        public System.DateTime CC_FECHA_CREACION { get; set; }
        public string CC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CC_FECHA_MOD { get; set; }
        public string CC_USUARIO_MOD { get; set; }
    }
}
