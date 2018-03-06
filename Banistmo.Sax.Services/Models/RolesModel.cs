using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class RolesModel
    {
        public int RL_ID_ROL { get; set; }
        public string RL_NOMBRE_ROL { get; set; }
        public string RL_DESCRIPCION_ROL { get; set; }
        public int RL_ESTATUS_ROL { get; set; }
        public System.DateTime RL_FECHA_CREACION { get; set; }
        public string RL_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> RL_FECHA_MOD { get; set; }
        public string RL_USUARIO_MOD { get; set; }
    }
}
