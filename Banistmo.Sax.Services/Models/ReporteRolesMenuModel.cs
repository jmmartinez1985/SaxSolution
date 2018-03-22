using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ReporteRolesMenuModel
    {    
        public string Rol { get; set; }
        public string DescripcionRol { get; set; }
        public Nullable<int> MO_ID_MODULO { get; set; }
        public string MO_DESCRIPCION { get; set; }
        public int Estatus { get; set; }
    }
}
