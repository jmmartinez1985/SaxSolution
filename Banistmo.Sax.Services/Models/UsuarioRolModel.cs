using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class UsuarioRolModel
    {
        public int UR_ID_USUARIO_ROL { get; set; }
        public string US_ID_USUARIO { get; set; }
        public int RL_ID_ROL { get; set; }
        public int UR_ESTATUS { get; set; }
        public System.DateTime UR_FECHA_CREACION { get; set; }
        public string UR_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> UR_FECHA_MOD { get; set; }
        public string UR_USUARIO_MOD { get; set; }
        public RolesModel SAX_ROLES { get; set; }
    }
}
