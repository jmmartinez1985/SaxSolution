using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ParametroTempModel
    {
        public int PA_ID_PARAMETRO_TEMP { get; set; }
        public int PA_ID_PARAMETRO { get; set; }
        public string PA_COD_PARAMETRO { get; set; }
        public string PA_DESCRIPCION { get; set; }
        public System.DateTime PA_FECHA_PROCESO { get; set; }
        public string PA_FRECUENCIA { get; set; }
        public int PA_HORA_EJECUCION { get; set; }
        public string PA_FILE_CONTABLE { get; set; }
        public string PA_RUTA_CONTABLE { get; set; }
        public string PA_RUTA_TEMPORAL { get; set; }
        public string PA_FRECUENCIA_LIMPIEZA { get; set; }
        public string PA_TIPO_ACCION { get; set; }
        public string PA_ESTATUS_ACCION { get; set; }
        public int PA_ESTATUS { get; set; }
        public System.DateTime PA_FECHA_CREACION { get; set; }
        public string PA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> PA_FECHA_MOD { get; set; }
        public string PA_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> PA_FECHA_APROBACION { get; set; }
        public string PA_USUARIO_APROBADOR { get; set; }

    }
}
