using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class PartidaManualModel: PartidasModel
    {
        public string PA_EVENTO { get; set; }
        public string PA_CREDITO { get; set; }
        public string PA_DEBITO { get; set; } 
        public string PA_NOMBRE_C { get; set; }
        public string PA_NOMBRE_D { get; set; }

        public string CUENTA_D_ID { get; set; }

        public string CUENTA_C_ID { get; set; }

        public int RC_COD_AREA { get; set; }

        public int EV_CUENTA_DEBITO { get; set; }

        public int EV_CUENTA_CREDITO { get; set; }
        public int CA_ID_AREA { get;  set; }
    }
}
