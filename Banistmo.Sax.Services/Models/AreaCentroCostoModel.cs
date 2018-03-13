using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class AreaCentroCostoModel
    {
        public int AD_ID_REGISTRO { get; set; }
        public int CA_ID_AREA { get; set; }
        public int EC_ID_REGISTRO { get; set; }
        public int AD_ESTATUS { get; set; }
        public System.DateTime AD_FECHA_CREACION { get; set; }
        public string AD_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> AD_FECHA_MOD { get; set; }
        public string AD_USUARIO_MOD { get; set; }


    }
}
