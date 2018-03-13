using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ConceptoCostoModel
    {
        public int CC_ID_CONCEPTO_COSTO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public string CC_NUM_CONCEPTO { get; set; }
        public string CC_CUENTA_MAYOR { get; set; }
        public string CC_DESCRIPCION { get; set; }
        public string CC_ESTATUS { get; set; }
        public System.DateTime CC_FECHA_CREACION { get; set; }
        public string CC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CC_FECHA_MOD { get; set; }
        public string CC_USUARIO_MOD { get; set; }
    }
}
