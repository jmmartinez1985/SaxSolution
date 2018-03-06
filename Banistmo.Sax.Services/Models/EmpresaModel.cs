using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class EmpresaModel
    {
        public int CE_ID_EMPRESA { get; set; }
        public string CE_COD_EMPRESA { get; set; }
        public string CE_NOMBRE { get; set; }
        public string CE_ESTATUS { get; set; }
        public System.DateTime CE_FECHA_CREACION { get; set; }
        public string CE_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> CE_FECHA_MOD { get; set; }
        public string CE_USUARIO_MOD { get; set; }

        public  AspNetUserModel AspNetUsers { get; set; }
        public AspNetUserModel AspNetUsers1 { get; set; }
    }
}
