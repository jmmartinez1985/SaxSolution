using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class SaldoContableNoConciliableViewModel
    {
    
            public int SC_ID_SALDO_CONTABLE { get; set; }
            public int SA_INSTITUCION { get; set; }
            public string Empresa { get; set; }
            public int CO_ID_CUENTA_CONTABLE { get; set; }
            public string CuentaContable { get; set; }
            public int CC_ID_MONEDA { get; set; }

            public string Moneda { get; set; }
            public System.DateTime SC_FECHA_CORTE { get; set; }
            public decimal SC_SALDOS { get; set; }
            public int SC_ESTATUS { get; set; }
            public System.DateTime SC_FECHA_CREACION { get; set; }
            public string SC_USUARIO_CREACION { get; set; }
            public Nullable<System.DateTime> SC_FECHA_MOD { get; set; }
            public string SC_USUARIO_MOD { get; set; }

        
    }
}
