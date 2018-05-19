﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DBModelEntities : DbContext
    {
        public DBModelEntities()
            : base("name=DBModelEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<SAX_AREA_CENCOSTO> SAX_AREA_CENCOSTO { get; set; }
        public virtual DbSet<SAX_AREA_OPERATIVA> SAX_AREA_OPERATIVA { get; set; }
        public virtual DbSet<SAX_CALENDARIO> SAX_CALENDARIO { get; set; }
        public virtual DbSet<SAX_CATALOGO> SAX_CATALOGO { get; set; }
        public virtual DbSet<SAX_CATALOGO_DETALLE> SAX_CATALOGO_DETALLE { get; set; }
        public virtual DbSet<SAX_CENTRO_COSTO> SAX_CENTRO_COSTO { get; set; }
        public virtual DbSet<SAX_COMPROBANTE> SAX_COMPROBANTE { get; set; }
        public virtual DbSet<SAX_COMPROBANTE_DETALLE> SAX_COMPROBANTE_DETALLE { get; set; }
        public virtual DbSet<SAX_CONCEPTO_COSTO> SAX_CONCEPTO_COSTO { get; set; }
        public virtual DbSet<SAX_CUENTA_CONTABLE> SAX_CUENTA_CONTABLE { get; set; }
        public virtual DbSet<SAX_DIAS_FERIADOS> SAX_DIAS_FERIADOS { get; set; }
        public virtual DbSet<SAX_EMPRESA> SAX_EMPRESA { get; set; }
        public virtual DbSet<SAX_EMPRESA_CENTRO> SAX_EMPRESA_CENTRO { get; set; }
        public virtual DbSet<SAX_EVENTO> SAX_EVENTO { get; set; }
        public virtual DbSet<SAX_EVENTO_TEMP> SAX_EVENTO_TEMP { get; set; }
        public virtual DbSet<SAX_LOG_USUARIO> SAX_LOG_USUARIO { get; set; }
        public virtual DbSet<SAX_MODULO> SAX_MODULO { get; set; }
        public virtual DbSet<SAX_MODULO_ROL> SAX_MODULO_ROL { get; set; }
        public virtual DbSet<SAX_MONEDA> SAX_MONEDA { get; set; }
        public virtual DbSet<SAX_PARAMETRO> SAX_PARAMETRO { get; set; }
        public virtual DbSet<SAX_PARAMETRO_ARCHIVO> SAX_PARAMETRO_ARCHIVO { get; set; }
        public virtual DbSet<SAX_PARAMETRO_TEMP> SAX_PARAMETRO_TEMP { get; set; }
        public virtual DbSet<SAX_PARTIDAS> SAX_PARTIDAS { get; set; }
        public virtual DbSet<SAX_PARTIDAS_TEMP> SAX_PARTIDAS_TEMP { get; set; }
        public virtual DbSet<SAX_REGISTRO_CONTROL> SAX_REGISTRO_CONTROL { get; set; }
        public virtual DbSet<SAX_SALDO_CONTABLE> SAX_SALDO_CONTABLE { get; set; }
        public virtual DbSet<SAX_SUPERVISOR> SAX_SUPERVISOR { get; set; }
        public virtual DbSet<SAX_SUPERVISOR_TEMP> SAX_SUPERVISOR_TEMP { get; set; }
        public virtual DbSet<SAX_USUARIO_AREA> SAX_USUARIO_AREA { get; set; }
        public virtual DbSet<SAX_USUARIO_EMPRESA> SAX_USUARIO_EMPRESA { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<vi_PartidasAprobadas> vi_PartidasAprobadas { get; set; }
    
        public virtual ObjectResult<SAX_REPORTE_ROLES_MENU_Result> SAX_REPORTE_ROLES_MENU()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SAX_REPORTE_ROLES_MENU_Result>("SAX_REPORTE_ROLES_MENU");
        }
    
        public virtual ObjectResult<SAX_REPORTE_USUARIO_Result> SAX_REPORTE_USUARIO()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SAX_REPORTE_USUARIO_Result>("SAX_REPORTE_USUARIO");
        }
    
        public virtual ObjectResult<SAX_REPORTE_USUARIOS_Result> SAX_REPORTE_USUARIOS()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SAX_REPORTE_USUARIOS_Result>("SAX_REPORTE_USUARIOS");
        }
    
        public virtual ObjectResult<SAX_ROLES_POR_USUARIO_Result> SAX_ROLES_POR_USUARIO(string userId)
        {
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SAX_ROLES_POR_USUARIO_Result>("SAX_ROLES_POR_USUARIO", userIdParameter);
        }
    
        public virtual ObjectResult<SAX_USUARIOS_POR_ROL_Result> SAX_USUARIOS_POR_ROL()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SAX_USUARIOS_POR_ROL_Result>("SAX_USUARIOS_POR_ROL");
        }
    
        public virtual int usp_fecha_proceso(Nullable<System.DateTime> i_fecha_proceso, ObjectParameter o_dia_habil)
        {
            var i_fecha_procesoParameter = i_fecha_proceso.HasValue ?
                new ObjectParameter("i_fecha_proceso", i_fecha_proceso) :
                new ObjectParameter("i_fecha_proceso", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_fecha_proceso", i_fecha_procesoParameter, o_dia_habil);
        }
    
        public virtual int USP_SELECT_CUENTAS_CONTABLES()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("USP_SELECT_CUENTAS_CONTABLES");
        }
    }
}
