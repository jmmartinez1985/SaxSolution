﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class ParametersCargaManual
    {
        public int RC_REGISTRO_CONTROL { get; set; }
        public string RC_COD_PARTIDA { get; set; }
        public string RC_USUARIO_NOMBRE { get; set; }
       
        public int PA_CONTADOR { get; set; }
       
        public string PA_COD_EMPRESA { get; set; }
       
        public System.DateTime PA_FECHA_CARGA { get; set; }
       
        public System.DateTime PA_FECHA_TRX { get; set; }
       
        public string PA_CENTRO_COSTO { get; set; }
       
        public string PA_COD_MONEDA { get; set; }
        
        public decimal PA_IMPORTE { get; set; }
       
        public string PA_REFERENCIA { get; set; }
      
        public string PA_EXPLICACION { get; set; }
        
        public string PA_PLAN_ACCION { get; set; }
        
        public string PA_CONCEPTO_COSTO { get; set; }
       
        public string PA_CAMPO_1 { get; set; }
       
        public string PA_CAMPO_2 { get; set; }
    
        public string PA_CAMPO_3 { get; set; }
       
        public string PA_CAMPO_4 { get; set; }
        
        public string PA_CAMPO_5 { get; set; }
        
        public string PA_CAMPO_6 { get; set; }
      
        public string PA_CAMPO_7 { get; set; }
      
        public string PA_CAMPO_8 { get; set; }
      
        public string PA_CAMPO_9 { get; set; }
      
        public string PA_CAMPO_10 { get; set; }
     
        public string PA_CAMPO_11 { get; set; }
     
        public string PA_CAMPO_12 { get; set; }
       
        public string PA_CAMPO_13 { get; set; }
        
        public string PA_CAMPO_14 { get; set; }
        
        public string PA_CAMPO_15 { get; set; }
       
        public string PA_CAMPO_16 { get; set; }
     
        public string PA_CAMPO_17 { get; set; }
       
        public string PA_CAMPO_18 { get; set; }
      
        public string PA_CAMPO_19 { get; set; }
      
        public string PA_CAMPO_20 { get; set; }
        
        public string PA_CAMPO_21 { get; set; }
        
        public string PA_CAMPO_22 { get; set; }
    
        public string PA_CAMPO_23 { get; set; }
     
        public string PA_CAMPO_24 { get; set; }
       
        public string PA_CAMPO_25 { get; set; }
        
        public string PA_CAMPO_26 { get; set; }
     
        public string PA_CAMPO_27 { get; set; }
      
        public string PA_CAMPO_28 { get; set; }
   
        public string PA_CAMPO_29 { get; set; }
      
        public string PA_CAMPO_30 { get; set; }
       
        public string PA_CAMPO_31 { get; set; }
       
        public string PA_CAMPO_32 { get; set; }
     
        public string PA_CAMPO_33 { get; set; }
     
        public string PA_CAMPO_34 { get; set; }
      
        public string PA_CAMPO_35 { get; set; }
     
        public string PA_CAMPO_36 { get; set; }
      
        public string PA_CAMPO_37 { get; set; }
    
        public string PA_CAMPO_38 { get; set; }
      
        public string PA_CAMPO_39 { get; set; }
     
        public string PA_CAMPO_40 { get; set; }
     
        public string PA_CAMPO_41 { get; set; }
    
        public string PA_CAMPO_42 { get; set; }
    
        public string PA_CAMPO_43 { get; set; }
    
        public string PA_CAMPO_44 { get; set; }
      
        public string PA_CAMPO_45 { get; set; }
     
        public string PA_CAMPO_46 { get; set; }
    
        public string PA_CAMPO_47 { get; set; }
     
        public string PA_CAMPO_48 { get; set; }
  
        public string PA_CAMPO_49 { get; set; }
      
        public string PA_CAMPO_50 { get; set; }
       
        public System.DateTime PA_FECHA_CREACION { get; set; }
      
        public string PA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> PA_FECHA_MOD { get; set; }
        public string PA_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> PA_FECHA_APROB { get; set; }
        public string PA_USUARIO_APROB { get; set; }
        public int PA_STATUS_PARTIDA { get; set; }
        public string PA_APLIC_ORIGEN { get; set; }
        public int PA_TIPO_CONCILIA { get; set; }
        public int PA_ESTADO_CONCILIA { get; set; }
        public Nullable<decimal> PA_IMPORTE_PENDIENTE { get; set; }
        public Nullable<System.DateTime> PA_FECHA_CONCILIA { get; set; }
        public Nullable<System.DateTime> PA_FECHA_ANULACION { get; set; }
        public Nullable<int> PA_DIAS_ANTIGUEDAD { get; set; }
        public int PA_ORIGEN_REFERENCIA { get; set; }
    }
}