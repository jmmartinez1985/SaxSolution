using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class EmpresaCentroModel
    {
        public int EC_ID_REGISTRO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CC_ID_CENTRO_COSTO { get; set; }
        public int EC_ESTATUS { get; set; }
        public System.DateTime EC_FECHA_CREACION { get; set; }
        public string EC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> EC_FECHA_MOD { get; set; }
        public string EC_USUARIO_MOD { get; set; }
    }
}
