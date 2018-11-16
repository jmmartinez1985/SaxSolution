using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
   public class SaldoContableNoConciliableModel
    {
        public int SC_ID_SALDO_CONTABLE { get; set; }
        public int SA_INSTITUCION { get; set; }
        public int CO_ID_CUENTA_CONTABLE { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int CC_ID_MONEDA { get; set; }
        public System.DateTime SC_FECHA_CORTE { get; set; }
        public decimal SC_SALDOS { get; set; }
        public int SC_ESTATUS { get; set; }
        public System.DateTime SC_FECHA_CREACION { get; set; }
        public string SC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> SC_FECHA_MOD { get; set; }
        public string SC_USUARIO_MOD { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual SAX_CUENTA_CONTABLE SAX_CUENTA_CONTABLE { get; set; }
        public virtual SAX_EMPRESA SAX_EMPRESA { get; set; }
        public virtual SAX_MONEDA SAX_MONEDA { get; set; }

    }
}
