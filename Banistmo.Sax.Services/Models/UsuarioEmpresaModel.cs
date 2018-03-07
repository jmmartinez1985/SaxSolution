using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class UsuarioEmpresaModel
    {
        public int UE_ID_USUARIO_EMPRESA { get; set; }
        public string US_ID_USUARIO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int UE_ESTATUS { get; set; }
        public System.DateTime UE_FECHA_CREACION { get; set; }
        public string UE_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> UE_FECHA_MOD { get; set; }
        public string UE_USUARIO_MOD { get; set; }

        //public AspNetUserModel AspNetUsers { get; set; }
        //public AspNetUserModel AspNetUsers1 { get; set; }
        //public AspNetUserModel AspNetUsers2 { get; set; }
        public EmpresaModel SAX_EMPRESA { get; set; }
    }
}
