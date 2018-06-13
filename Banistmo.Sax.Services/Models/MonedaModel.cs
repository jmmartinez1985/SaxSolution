using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class MonedaModel
    {

        public int CC_ID_MONEDA { get; set; }
        public string CC_NUM_MONEDA { get; set; }
        public string CC_DESC_MONEDA { get; set; }
        public string CC_COD_CURRENCY { get; set; }
        public string CC_ESTATUS { get; set; }
        public System.DateTime CC_FECHA_CREACION { get; set; }
        public string CC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CC_FECHA_MOD { get; set; }
        public string CC_USUARIO_MOD { get; set; }

    
    }
}
