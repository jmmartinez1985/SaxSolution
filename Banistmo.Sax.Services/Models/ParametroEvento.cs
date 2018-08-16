using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ParametroEvento: EventosModel
    {
        public string cuentaDebito { get; set; }
        public string cuentaCredito { get; set; }
    }
}
