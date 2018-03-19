using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class SupervisorTempModel
    {
        public int SV_ID_SUPERVISOR_TEMP { get; set; }
        public int SV_ID_SUPERVISOR { get; set; }
        public string SV_COD_AREA { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string SV_COD_SUPERVISOR { get; set; }
        public string SV_LIMITE_MINIMO { get; set; }
        public string SV_LIMITE_SUPERIOR { get; set; }
        public string SV_TIPO_ACCION { get; set; }
        public string SV_ESTATUS_ACCION { get; set; }
        public int SV_ESTATUS { get; set; }
        public System.DateTime SV_FECHA_CREACION { get; set; }
        public string SV_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> SV_FECHA_MOD { get; set; }
        public string SV_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> SV_FECHA_APROBACION { get; set; }
        public string SV_USUARIO_APROBADOR { get; set; }

        //public virtual SAX_SUPERVISOR SAX_SUPERVISOR { get; set; }
    }
}
