using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Services.Models
{
    public class ReporteSaldoContableModel
    {
        public int SA_ID_SALDO_CONTABLE { get; set; }
        public int SA_INSTITUCION { get; set; }
        public string CO_ID_CUENTA_CONTABLE { get; set; }
        public int CC_ID_MONEDA { get; set; }
        public System.DateTime SA_FECHA_CORTE { get; set; }
        public decimal SA_SALDOS { get; set; }
        public int SA_ESTATUS { get; set; }
        public System.DateTime SA_FECHA_CREACION { get; set; }
        public string SA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> SA_FECHA_MOD { get; set; }
        public string SA_USUARIO_MOD { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_CUENTA_CONTABLE SAX_CUENTA_CONTABLE { get; set; }
        public virtual SAX_MONEDA SAX_MONEDA { get; set; }


    }

    public class ReporteSaldoContablePartialModel
    {
        public string nombreempresa { get; set; }
        //public string fechaforte { get; set; }
        public string codcuentacontable { get; set; }
        public string nombrecuentacontable { get; set; }
        public string codmoneda { get; set; }
        public decimal saldo { get; set; }
        public string nombreareaoperativa { get; set; }
        
    }
}
