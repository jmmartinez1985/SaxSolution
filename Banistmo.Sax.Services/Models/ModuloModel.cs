using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ModuloModel
    {
        public int MO_ID_MODULO { get; set; }
        public string MO_MODULO { get; set; }
        public string MO_PATH { get; set; }
        public string MO_DESCRIPCION { get; set; }
        public int MO_ESTATUS { get; set; }
        public System.DateTime MO_FECHA_CREACION { get; set; }
        public string MO_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> MO_FECHA_MOD { get; set; }
        public string MO_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> MO_ULTIMO_ACCESO { get; set; }

        public  AspNetUserModel AspNetUsers { get; set; }
        public  AspNetUserModel AspNetUsers1 { get; set; }

        public  ICollection<ModuloRolModel> SAX_MODULO_ROL { get; set; }
    }
}
