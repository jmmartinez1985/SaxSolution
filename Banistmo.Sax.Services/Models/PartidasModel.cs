using Banistmo.Sax.Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class PartidasModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo PA_REGISTRO no puede estar vacío.")]
        public int PA_REGISTRO { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo RC_REGISTRO_CONTROL no puede estar vacío.")]
        public int RC_REGISTRO_CONTROL { get; set; }
        public string RC_COD_PARTIDA { get; set; }
        public string RC_USUARIO_NOMBRE { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo RC_REGISTRO_CONTROL no puede estar vacío.")]
        public int PA_CONTADOR { get; set; }
        [Display(Name = "Empresa")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo empresa no puede estar vacío.")]
        [StringLength(3, ErrorMessage = "El campo Empresa no puede tener más de 3 caracteres.")]
        public string PA_COD_EMPRESA { get; set; }
        [Display(Name = "Fecha de  carga")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo fecha de carga no puede estar vacío.")]
        public System.DateTime PA_FECHA_CARGA { get; set; }
        [Display(Name = "Fecha transacción")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo fecha de transacción no puede estar vacío.")]
        public System.DateTime PA_FECHA_TRX { get; set; }
        [Display(Name ="Cuenta contable")]
        [StringLength(30, ErrorMessage = "El campo cuenta contable no puede tener más de 30 caracteres.")]
        public string PA_CTA_CONTABLE { get; set; }
        [Display(Name = "Centro de costo")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo centro de costo no puede estar vacío.")]
        [StringLength(5, ErrorMessage = "El campo centro de costo no puede tener más de 5 caracteres.")]
        public string PA_CENTRO_COSTO { get; set; }
        [Display(Name = "Moneda")]
        [Required (ErrorMessage = "El campo moneda es requerido")]
        [StringLength(4, ErrorMessage = "El campo moneda no puede tener más de 4 caracteres.")]
        public string PA_COD_MONEDA { get; set; }
        [Display(Name = "Importe")]
        [Required(ErrorMessage = "El campo importe es requerido.")]
        public decimal PA_IMPORTE { get; set; }
        [Display(Name = "Referencia")]
        [StringLength(13, ErrorMessage = "El campo referencia no puede tener más de 13 caracteres.")]
        public string PA_REFERENCIA { get; set; }
        [Display(Name = "Explicación")]
        [StringLength(700, ErrorMessage = "El campo explicación no puede tener más de 700 caracteres.")]
        public string PA_EXPLICACION { get; set; }
        [Display(Name = "Plan de acción")]
        [StringLength(700, ErrorMessage = "El campo plan de acción no puede tener más de 700 caracteres.")]
        public string PA_PLAN_ACCION { get; set; }
        [Display(Name = "Concepto de costo")]
        [StringLength(7, ErrorMessage = "El campo concepto de costo no puede tener más de 7 caracteres.")]
        public string PA_CONCEPTO_COSTO { get; set; }
        [Display(Name = "Campo 1")]
        [StringLength(50, ErrorMessage = "El Campo 1 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_1 { get; set; }
        [Display(Name = "Campo 2")]
        [StringLength(50, ErrorMessage = "El Campo 2 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_2 { get; set; }
        [Display(Name = "Campo 3")]
        [StringLength(50, ErrorMessage = "El Campo 3 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_3 { get; set; }
        [Display(Name = "Campo 4")]
        [StringLength(50, ErrorMessage = "El Campo 4 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_4 { get; set; }
        [Display(Name = "Campo 5")]
        [StringLength(50, ErrorMessage = "El Campo 5 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_5 { get; set; }
        [Display(Name = "Campo 6")]
        [StringLength(50, ErrorMessage = "El Campo 6 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_6 { get; set; }
        [Display(Name = "Campo 7")]
        [StringLength(50, ErrorMessage = "El Campo 7 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_7 { get; set; }
        [Display(Name = "Campo 8")]
        [StringLength(50, ErrorMessage = "El Campo 8 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_8 { get; set; }
        [Display(Name = "Campo 9")]
        [StringLength(50, ErrorMessage = "El Campo 9 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_9 { get; set; }
        [Display(Name = "Campo 10")]
        [StringLength(50, ErrorMessage = "El Campo 10 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_10 { get; set; }
        [Display(Name = "Campo 11")]
        [StringLength(50, ErrorMessage = "El Campo 11 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_11 { get; set; }
        [Display(Name = "Campo 12")]
        [StringLength(50, ErrorMessage = "El Campo 12 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_12 { get; set; }
        [Display(Name = "Campo 13")]
        [StringLength(50, ErrorMessage = "El Campo 13 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_13 { get; set; }
        [Display(Name = "Campo 14")]
        [StringLength(50, ErrorMessage = "El Campo 14 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_14 { get; set; }
        [Display(Name = "Campo 15")]
        [StringLength(50, ErrorMessage = "El Campo 15 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_15 { get; set; }
        [Display(Name = "Campo 16")]
        [StringLength(50, ErrorMessage = "El Campo 16 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_16 { get; set; }
        [Display(Name = "Campo 17")]
        [StringLength(50, ErrorMessage = "El Campo 17 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_17 { get; set; }
        [Display(Name = "Campo 18")]
        [StringLength(50, ErrorMessage = "El Campo 18 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_18 { get; set; }
        [Display(Name = "Campo 19")]
        [StringLength(50, ErrorMessage = "El Campo 19 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_19 { get; set; }
        [Display(Name = "Campo 20")]
        [StringLength(50, ErrorMessage = "El Campo 20 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_20 { get; set; }
        [Display(Name = "Campo 21")]
        [StringLength(50, ErrorMessage = "El Campo 21 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_21 { get; set; }
        [Display(Name = "Campo 22")]
        [StringLength(50, ErrorMessage = "El Campo 22 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_22 { get; set; }
        [Display(Name = "Campo 23")]
        [StringLength(50, ErrorMessage = "El Campo 23 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_23 { get; set; }
        [Display(Name = "Campo 24")]
        [StringLength(50, ErrorMessage = "El Campo 24 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_24 { get; set; }
        [Display(Name = "Campo 25")]
        [StringLength(50, ErrorMessage = "El Campo 25 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_25 { get; set; }
        [Display(Name = "Campo 26")]
        [StringLength(50, ErrorMessage = "El Campo 26 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_26 { get; set; }
        [Display(Name = "Campo 27")]
        [StringLength(50, ErrorMessage = "El Campo 27 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_27 { get; set; }
        [Display(Name = "Campo 28")]
        [StringLength(50, ErrorMessage = "El Campo 28 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_28 { get; set; }
        [Display(Name = "Campo 29")]
        [StringLength(50, ErrorMessage = "El Campo 29 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_29 { get; set; }
        [Display(Name = "Campo 30")]
        [StringLength(50, ErrorMessage = "El Campo 30 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_30 { get; set; }
        [Display(Name = "Campo 31")]
        [StringLength(50, ErrorMessage = "El Campo 31 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_31 { get; set; }
        [Display(Name = "Campo 32")]
        [StringLength(50, ErrorMessage = "El Campo 32 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_32 { get; set; }
        [Display(Name = "Campo 33")]
        [StringLength(50, ErrorMessage = "El Campo 33 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_33 { get; set; }
        [Display(Name = "Campo 34")]
        [StringLength(50, ErrorMessage = "El Campo 34 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_34 { get; set; }
        [Display(Name = "Campo 35")]
        [StringLength(50, ErrorMessage = "El Campo 35 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_35 { get; set; }
        [Display(Name = "Campo 36")]
        [StringLength(50, ErrorMessage = "El Campo 36 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_36 { get; set; }
        [Display(Name = "Campo 37")]
        [StringLength(50, ErrorMessage = "El Campo 37 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_37 { get; set; }
        [Display(Name = "Campo 38")]
        [StringLength(50, ErrorMessage = "El Campo 38 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_38 { get; set; }
        [Display(Name = "Campo 39")]
        [StringLength(50, ErrorMessage = "El Campo 39 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_39 { get; set; }
        [Display(Name = "Campo 40")]
        [StringLength(50, ErrorMessage = "El Campo 40 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_40 { get; set; }
        [Display(Name = "Campo 41")]
        [StringLength(50, ErrorMessage = "El Campo 41 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_41 { get; set; }
        [Display(Name = "Campo 42")]
        [StringLength(50, ErrorMessage = "El Campo 42 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_42 { get; set; }
        [Display(Name = "Campo 43")]
        [StringLength(50, ErrorMessage = "El Campo 43 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_43 { get; set; }
        [Display(Name = "Campo 44")]
        [StringLength(50, ErrorMessage = "El Campo 44 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_44 { get; set; }
        [Display(Name = "Campo 45")]
        [StringLength(50, ErrorMessage = "El Campo 45 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_45 { get; set; }
        [Display(Name = "Campo 46")]
        [StringLength(50, ErrorMessage = "El Campo 46 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_46 { get; set; }
        [Display(Name = "Campo 47")]
        [StringLength(50, ErrorMessage = "El Campo 47 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_47 { get; set; }
        [Display(Name = "Campo 48")]
        [StringLength(50, ErrorMessage = "El Campo 48 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_48 { get; set; }
        [Display(Name = "Campo 49")]
        [StringLength(50, ErrorMessage = "El Campo 49 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_49 { get; set; }
        [Display(Name = "Campo 50")]
        [StringLength(50, ErrorMessage = "El Campo 50 no puede tener más de 50 caracteres.")]
        public string PA_CAMPO_50 { get; set; }
        [Required (ErrorMessage = "El campo Fecha creacion es requerido.")]
        public System.DateTime PA_FECHA_CREACION { get; set; }
        [Required(ErrorMessage = "El campo usuario creación es requerido."), StringLength(256)]
        public string PA_USUARIO_CREACION { get; set; }
        public Nullable<System.DateTime> PA_FECHA_MOD { get; set; }
        [StringLength(256)]
        public string PA_USUARIO_MOD { get; set; }
        public Nullable<System.DateTime> PA_FECHA_APROB { get; set; }
        [StringLength(256)]
        public string PA_USUARIO_APROB { get; set; }        
        public int PA_STATUS_PARTIDA { get; set; }
        [StringLength(10)]
        public string PA_APLIC_ORIGEN { get; set; }       
        public int PA_TIPO_CONCILIA { get; set; }       
        public int PA_ESTADO_CONCILIA { get; set; }
        public Nullable<decimal> PA_IMPORTE_PENDIENTE { get; set; }
        public Nullable<System.DateTime> PA_FECHA_CONCILIA { get; set; }
        public Nullable<System.DateTime> PA_FECHA_ANULACION { get; set; }
        public Nullable<int> PA_DIAS_ANTIGUEDAD { get; set; }       
        public int PA_ORIGEN_REFERENCIA { get; set; }

        public string TC_COD_COMPROBANTE { get; set; }
        //public virtual AspNetUserModel AspNetUsers { get; set; }
        //public virtual AspNetUserModel AspNetUsers1 { get; set; }
        //public virtual AspNetUserModel AspNetUsers2 { get; set; }
        //public virtual ComprobanteDetalleModel SAX_COMPROBANTE_DETALLE { get; set; }
        //public virtual RegistroControlModel SAX_REGISTRO_CONTROL { get; set; }
    }
}
