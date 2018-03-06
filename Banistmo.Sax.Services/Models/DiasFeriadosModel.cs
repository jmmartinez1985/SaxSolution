using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class DiasFeriadosModel
    {
        public int CD_ID_DIA_FERIADO { get; set; }
        public int CD_ANNIO { get; set; }
        public string CD_MES { get; set; }
        public int CD_DIA { get; set; }
        public int CD_ESTATUS { get; set; }
        public System.DateTime CD_FECHA_CREACION { get; set; }
        public string CD_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CD_FECHA_MOD { get; set; }
        public string CD_USUARIO_MOD { get; set; }

    }
}
