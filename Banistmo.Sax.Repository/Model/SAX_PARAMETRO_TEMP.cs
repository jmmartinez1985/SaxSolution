//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Banistmo.Sax.Repository.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SAX_PARAMETRO_TEMP
    {
        public int PA_ID_PARAMETRO_TEMP { get; set; }
        public int PA_ID_PARAMETRO { get; set; }
        public System.DateTime PA_FECHA_PROCESO { get; set; }
        public Nullable<int> PA_FRECUENCIA { get; set; }
        public int PA_HORA_EJECUCION { get; set; }
        public string PA_RUTA_CONTABLE { get; set; }
        public string PA_RUTA_TEMPORAL { get; set; }
        public Nullable<int> PA_FRECUENCIA_LIMPIEZA { get; set; }
        public int PA_ESTATUS { get; set; }
        public System.DateTime PA_FECHA_CREACION { get; set; }
        public string PA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> PA_FECHA_MOD { get; set; }
        public string PA_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> PA_FECHA_APROBACION { get; set; }
        public string PA_USUARIO_APROBADOR { get; set; }
    
        public virtual SAX_PARAMETRO SAX_PARAMETRO { get; set; }
    }
}
