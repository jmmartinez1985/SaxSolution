using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ModuloRolModel
    {
        public int MR_ID_MODULO_ROL { get; set; }
        public int MO_ID_MODULO { get; set; }
        public int RL_ID_ROL { get; set; }
        public int MR_ESTATUS { get; set; }
        public System.DateTime MR_FECHA_CREACION { get; set; }
        public string MR_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> MR_FECHA_MOD { get; set; }
        public string MR_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> MR_ULTIMO_ACCESO { get; set; }
    }
}
