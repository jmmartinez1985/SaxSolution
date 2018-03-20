using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ReporteUsuarioModel
    {
        public string cod_usuario { get; set; }
        public string nombre_usuario { get; set; }
        public string status { get; set; }
        
        public string area_operativa { get; set; }
        public string empresa { get; set; }
    }
}
