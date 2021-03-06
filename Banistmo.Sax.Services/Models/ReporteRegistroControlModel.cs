﻿using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class ReporteRegistroControlModel
    {
        public int RC_REGISTRO_CONTROL { get; set; }
        public string RC_COD_PARTIDA { get; set; }
        public string RC_COD_USUARIO { get; set; }
        public int RC_COD_OPERACION { get; set; }
        public string RC_ARCHIVO { get; set; }
        public System.DateTime RC_FECHA_PROCESO { get; set; }
        public int RC_TOTAL_REGISTRO { get; set; }
        public decimal RC_TOTAL_DEBITO { get; set; }
        public decimal RC_TOTAL_CREDITO { get; set; }
        public decimal RC_TOTAL { get; set; }
        public int RC_ESTATUS_LOTE { get; set; }
        public System.DateTime RC_FECHA_CREACION { get; set; }
        public string RC_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> RC_FECHA_APROBACION { get; set; }
        public string RC_USUARIO_APROBADOR { get; set; }
        public Nullable<System.DateTime> RC_FECHA_MOD { get; set; }
        public string RC_USUARIO_MOD { get; set; }
        public Nullable<int> CA_ID_AREA { get; set; }
        public Nullable<int> EV_COD_EVENTO { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual AspNetUsers AspNetUsers1 { get; set; }
        public virtual AspNetUsers AspNetUsers2 { get; set; }
    }


    public class ReporteRegistroControlPartialModel
    {
        public string Supervisor { get; set; }
        public string NombreOperacion { get; set; }
        public string Lote { get; set; }
        public string Capturador { get; set; }
        public int TotalRegistro { get; set; }
        public decimal TotalDebito { get; set; }
        public decimal TotalCredito { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string FechaCreacion { get; set; }
        public string HoraCreacion { get; set; }

        public Int16 AreaId { get; set; }
    }

}
