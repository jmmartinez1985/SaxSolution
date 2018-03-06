using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class AreaOperativaModel
    {
        public int CA_COD_AREA { get; set; }
        public string CA_NOMBRE { get; set; }
        public int CA_ESTATUS { get; set; }
        public System.DateTime CA_FECHA_CREACION { get; set; }
        public string CA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CA_FECHA_MOD { get; set; }
        public string CA_USUARIO_MOD { get; set; }

        public virtual AspNetUserModel AspNetUsers { get; set; }
        public virtual AspNetUserModel AspNetUsers1 { get; set; }
    }
}
