//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Banistmo.Sax.Repository.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUsers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetUsers()
        {
            this.AspNetUserClaims = new HashSet<AspNetUserClaims>();
            this.AspNetUserLogins = new HashSet<AspNetUserLogins>();
            this.AspNetUserRoles = new HashSet<AspNetUserRoles>();
            this.SAX_AREA_OPERATIVA = new HashSet<SAX_AREA_OPERATIVA>();
            this.SAX_AREA_OPERATIVA1 = new HashSet<SAX_AREA_OPERATIVA>();
            this.SAX_CATALOGO = new HashSet<SAX_CATALOGO>();
            this.SAX_CATALOGO1 = new HashSet<SAX_CATALOGO>();
            this.SAX_CATALOGO_DETALLE = new HashSet<SAX_CATALOGO_DETALLE>();
            this.SAX_CATALOGO_DETALLE1 = new HashSet<SAX_CATALOGO_DETALLE>();
            this.SAX_CENTRO_COSTO = new HashSet<SAX_CENTRO_COSTO>();
            this.SAX_CENTRO_COSTO1 = new HashSet<SAX_CENTRO_COSTO>();
            this.SAX_COMPROBANTE = new HashSet<SAX_COMPROBANTE>();
            this.SAX_COMPROBANTE1 = new HashSet<SAX_COMPROBANTE>();
            this.SAX_COMPROBANTE2 = new HashSet<SAX_COMPROBANTE>();
            this.SAX_COMPROBANTE_DETALLE = new HashSet<SAX_COMPROBANTE_DETALLE>();
            this.SAX_COMPROBANTE_DETALLE1 = new HashSet<SAX_COMPROBANTE_DETALLE>();
            this.SAX_CONCEPTO_COSTO = new HashSet<SAX_CONCEPTO_COSTO>();
            this.SAX_CONCEPTO_COSTO1 = new HashSet<SAX_CONCEPTO_COSTO>();
            this.SAX_CUENTA_CONTABLE = new HashSet<SAX_CUENTA_CONTABLE>();
            this.SAX_CUENTA_CONTABLE1 = new HashSet<SAX_CUENTA_CONTABLE>();
            this.SAX_DIAS_FERIADOS = new HashSet<SAX_DIAS_FERIADOS>();
            this.SAX_DIAS_FERIADOS1 = new HashSet<SAX_DIAS_FERIADOS>();
            this.SAX_EMPRESA_CENTRO = new HashSet<SAX_EMPRESA_CENTRO>();
            this.SAX_EMPRESA_CENTRO1 = new HashSet<SAX_EMPRESA_CENTRO>();
            this.SAX_EVENTO = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO1 = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO2 = new HashSet<SAX_EVENTO>();
            this.SAX_EVENTO_TEMP = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_EVENTO_TEMP1 = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_EVENTO_TEMP2 = new HashSet<SAX_EVENTO_TEMP>();
            this.SAX_LOG_USUARIO = new HashSet<SAX_LOG_USUARIO>();
            this.SAX_LOG_USUARIO1 = new HashSet<SAX_LOG_USUARIO>();
            this.SAX_MODULO = new HashSet<SAX_MODULO>();
            this.SAX_MODULO1 = new HashSet<SAX_MODULO>();
            this.SAX_MODULO_ROL = new HashSet<SAX_MODULO_ROL>();
            this.SAX_MODULO_ROL1 = new HashSet<SAX_MODULO_ROL>();
            this.SAX_MONEDA = new HashSet<SAX_MONEDA>();
            this.SAX_MONEDA1 = new HashSet<SAX_MONEDA>();
            this.SAX_PARAMETRO = new HashSet<SAX_PARAMETRO>();
            this.SAX_PARAMETRO1 = new HashSet<SAX_PARAMETRO>();
            this.SAX_PARAMETRO2 = new HashSet<SAX_PARAMETRO>();
            this.SAX_PARAMETRO_TEMP = new HashSet<SAX_PARAMETRO_TEMP>();
            this.SAX_PARAMETRO_TEMP1 = new HashSet<SAX_PARAMETRO_TEMP>();
            this.SAX_PARAMETRO_TEMP2 = new HashSet<SAX_PARAMETRO_TEMP>();
            this.SAX_PARTIDAS = new HashSet<SAX_PARTIDAS>();
            this.SAX_PARTIDAS1 = new HashSet<SAX_PARTIDAS>();
            this.SAX_PARTIDAS2 = new HashSet<SAX_PARTIDAS>();
            this.SAX_REGISTRO_CONTROL = new HashSet<SAX_REGISTRO_CONTROL>();
            this.SAX_REGISTRO_CONTROL1 = new HashSet<SAX_REGISTRO_CONTROL>();
            this.SAX_REGISTRO_CONTROL2 = new HashSet<SAX_REGISTRO_CONTROL>();
            this.SAX_SALDO_CONTABLE = new HashSet<SAX_SALDO_CONTABLE>();
            this.SAX_SALDO_CONTABLE1 = new HashSet<SAX_SALDO_CONTABLE>();
            this.SAX_SUPERVISOR = new HashSet<SAX_SUPERVISOR>();
            this.SAX_SUPERVISOR1 = new HashSet<SAX_SUPERVISOR>();
            this.SAX_SUPERVISOR2 = new HashSet<SAX_SUPERVISOR>();
            this.SAX_SUPERVISOR3 = new HashSet<SAX_SUPERVISOR>();
            this.SAX_SUPERVISOR_TEMP = new HashSet<SAX_SUPERVISOR_TEMP>();
            this.SAX_SUPERVISOR_TEMP1 = new HashSet<SAX_SUPERVISOR_TEMP>();
            this.SAX_SUPERVISOR_TEMP2 = new HashSet<SAX_SUPERVISOR_TEMP>();
            this.SAX_SUPERVISOR_TEMP3 = new HashSet<SAX_SUPERVISOR_TEMP>();
            this.SAX_USUARIO_AREA = new HashSet<SAX_USUARIO_AREA>();
            this.SAX_USUARIO_AREA1 = new HashSet<SAX_USUARIO_AREA>();
            this.SAX_USUARIO_EMPRESA = new HashSet<SAX_USUARIO_EMPRESA>();
            this.SAX_USUARIO_EMPRESA1 = new HashSet<SAX_USUARIO_EMPRESA>();
            this.SAX_USUARIO_EMPRESA2 = new HashSet<SAX_USUARIO_EMPRESA>();
        }
    
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Level { get; set; }
        public System.DateTime JoinDate { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public Nullable<int> Estatus { get; set; }
        public string NumColaborador { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_AREA_OPERATIVA> SAX_AREA_OPERATIVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_AREA_OPERATIVA> SAX_AREA_OPERATIVA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CATALOGO> SAX_CATALOGO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CATALOGO> SAX_CATALOGO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CATALOGO_DETALLE> SAX_CATALOGO_DETALLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CATALOGO_DETALLE> SAX_CATALOGO_DETALLE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CENTRO_COSTO> SAX_CENTRO_COSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CENTRO_COSTO> SAX_CENTRO_COSTO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_COMPROBANTE> SAX_COMPROBANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_COMPROBANTE> SAX_COMPROBANTE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_COMPROBANTE> SAX_COMPROBANTE2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_COMPROBANTE_DETALLE> SAX_COMPROBANTE_DETALLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_COMPROBANTE_DETALLE> SAX_COMPROBANTE_DETALLE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CONCEPTO_COSTO> SAX_CONCEPTO_COSTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CONCEPTO_COSTO> SAX_CONCEPTO_COSTO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CUENTA_CONTABLE> SAX_CUENTA_CONTABLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_CUENTA_CONTABLE> SAX_CUENTA_CONTABLE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_DIAS_FERIADOS> SAX_DIAS_FERIADOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_DIAS_FERIADOS> SAX_DIAS_FERIADOS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EMPRESA_CENTRO> SAX_EMPRESA_CENTRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EMPRESA_CENTRO> SAX_EMPRESA_CENTRO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO> SAX_EVENTO2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_LOG_USUARIO> SAX_LOG_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_LOG_USUARIO> SAX_LOG_USUARIO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MODULO> SAX_MODULO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MODULO> SAX_MODULO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MODULO_ROL> SAX_MODULO_ROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MODULO_ROL> SAX_MODULO_ROL1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MONEDA> SAX_MONEDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_MONEDA> SAX_MONEDA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARAMETRO> SAX_PARAMETRO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARAMETRO> SAX_PARAMETRO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARAMETRO> SAX_PARAMETRO2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARAMETRO_TEMP> SAX_PARAMETRO_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARAMETRO_TEMP> SAX_PARAMETRO_TEMP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARAMETRO_TEMP> SAX_PARAMETRO_TEMP2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARTIDAS> SAX_PARTIDAS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARTIDAS> SAX_PARTIDAS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_PARTIDAS> SAX_PARTIDAS2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_REGISTRO_CONTROL> SAX_REGISTRO_CONTROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_REGISTRO_CONTROL> SAX_REGISTRO_CONTROL1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_REGISTRO_CONTROL> SAX_REGISTRO_CONTROL2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SALDO_CONTABLE> SAX_SALDO_CONTABLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SALDO_CONTABLE> SAX_SALDO_CONTABLE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR> SAX_SUPERVISOR3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_AREA> SAX_USUARIO_AREA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_AREA> SAX_USUARIO_AREA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_EMPRESA> SAX_USUARIO_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_EMPRESA> SAX_USUARIO_EMPRESA1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAX_USUARIO_EMPRESA> SAX_USUARIO_EMPRESA2 { get; set; }
    }
}
