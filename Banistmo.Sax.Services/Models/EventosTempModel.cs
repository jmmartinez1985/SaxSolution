using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Services.Models
{
    public class EventosTempModel
    {
        public int EV_COD_EVENTO_TEMP { get; set; }
        public int? EV_COD_EVENTO { get; set; }
        public int CE_ID_EMPRESA { get; set; }
        public int EV_ID_AREA { get; set; }
        public string EV_DESCRIPCION_EVENTO { get; set; }
        public Int32 EV_CUENTA_DEBITO { get; set; }
        public Int32 EV_CUENTA_CREDITO { get; set; }
        public string EV_REFERENCIA_DEBITO { get; set; }
        public string EV_REFERENCIA_CREDITO { get; set; }
        public string EV_ESTATUS_ACCION { get; set; }
        public int? EV_ESTATUS { get; set; }
        public DateTime? EV_FECHA_CREACION { get; set; }
        public string EV_USUARIO_CREACION { get; set; }
        public DateTime? EV_FECHA_MOD { get; set; }
        public string EV_USUARIO_MOD { get; set; }
        public DateTime? EV_FECHA_APROBACION { get; set; }
        public string EV_USUARIO_APROBADOR { get; set; }

        public virtual AspNetUserModel AspNetUsers { get; set; }
        public virtual AspNetUserModel AspNetUsers1 { get; set; }
        public virtual AspNetUserModel AspNetUsers2 { get; set; }
        public virtual SAX_AREA_OPERATIVA SAX_AREA_OPERATIVA { get; set; }
        public virtual CuentaContableModel SAX_CUENTA_CONTABLE { get; set; }
        public virtual CuentaContableModel SAX_CUENTA_CONTABLE1 { get; set; }
        public virtual EmpresaModel SAX_EMPRESA { get; set; }
        public virtual EventosTempModel EventoTemporal { get; set; }
    }
}
