using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ComprobanteModel
    {
        public int TC_ID_COMPROBANTE { get; set; }
        public string TC_COD_OPERACION { get; set; }
        public System.DateTime TC_FECHA_PROCESO { get; set; }
        public int TC_TOTAL_REGISTRO { get; set; }
        public decimal TC_TOTAL_DEBITO { get; set; }
        public decimal TC_TOTAL_CREDITO { get; set; }
        public decimal TC_TOTAL { get; set; }
        public string TC_ESTATUS { get; set; }
        public System.DateTime TC_FECHA_CREACION { get; set; }
        public string TC_USUARIO_CREACION { get; set; }
        public System.DateTime TC_FECHA_APROBACION { get; set; }
        public string TC_USUARIO_APROBADOR { get; set; }
        public System.DateTime? TC_FECHA_MOD { get; set; }
        public string TC_USUARIO_MOD { get; set; }
        public string TC_COD_COMPROBANTE { get; set; }
        public string TC_USUARIO_RECHAZO { get; set; }
        public DateTime? TC_FECHAN_RECHAZO { get; set; }
        public Int16? CA_ID_AREA { get; set; }
        public AspNetUsers AspNetUsers { get; set; }
        public AspNetUsers AspNetUsers1 { get; set; }
        public AspNetUsers AspNetUsers2 { get; set; }
        public AspNetUsers AspNetUsers3 { get; set; }
        public  List<ComprobanteDetalleModel> SAX_COMPROBANTE_DETALLE { get; set; }

        //public int CA_ID_AREA { get; set; }
    }
}
