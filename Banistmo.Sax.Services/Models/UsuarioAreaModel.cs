using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class UsuarioAreaModel
    {
        public string US_COD_USUARIO { get; set; }
        public int CA_COD_AREA { get; set; }
        public int UA_ESTATUS { get; set; }
        public System.DateTime UA_FECHA_CREACION { get; set; }
        public string UA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> UA_FECHA_MOD { get; set; }
        public string UA_USUARIO_MOD { get; set; }

        public  AspNetUserModel AspNetUsers { get; set; }
        public  AspNetUserModel AspNetUsers1 { get; set; }
        public  AreaOperativaModel SAX_AREA_OPERATIVA { get; set; }
    }
}
