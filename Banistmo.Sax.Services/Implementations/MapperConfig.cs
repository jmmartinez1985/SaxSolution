using AutoMapper;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations
{
    public class MapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {

                cfg.CreateMap<SAX_DIAS_FERIADOS, DiasFeriadosModel>();
                cfg.CreateMap<DiasFeriadosModel, SAX_DIAS_FERIADOS>();
                cfg.CreateMap<AspNetUsers, AspNetUserModel>();
                cfg.CreateMap<AspNetUserModel, AspNetUsers>();

                //cfg.CreateMap<AspNetRoles, RolesModel>();
                //cfg.CreateMap<RolesModel, AspNetRoles>();

                cfg.CreateMap<SAX_AREA_OPERATIVA, AreaOperativaModel>();
                cfg.CreateMap<AreaOperativaModel, SAX_AREA_OPERATIVA>();

                cfg.CreateMap<SAX_EMPRESA, EmpresaModel>();
                cfg.CreateMap<EmpresaModel, SAX_EMPRESA>();

                cfg.CreateMap<SAX_MODULO, ModuloModel>();
                cfg.CreateMap<ModuloModel, SAX_MODULO>();

                cfg.CreateMap<SAX_MODULO_ROL, ModuloRolModel>();
                cfg.CreateMap<ModuloRolModel, SAX_MODULO_ROL>();

                cfg.CreateMap<SAX_USUARIO_AREA, UsuarioAreaModel>();
                cfg.CreateMap<UsuarioAreaModel, SAX_USUARIO_AREA>();

                cfg.CreateMap<SAX_USUARIO_EMPRESA, UsuarioEmpresaModel>();
                cfg.CreateMap<UsuarioEmpresaModel, SAX_USUARIO_EMPRESA>();

                //Eventos
                cfg.CreateMap<SAX_EVENTO, EventosModel>();
                cfg.CreateMap<EventosModel, SAX_EVENTO>();

                cfg.CreateMap<SAX_EVENTO_TEMP, EventosTempModel>();
                cfg.CreateMap<EventosTempModel, SAX_EVENTO_TEMP>();


                //RegistrosControl
                cfg.CreateMap<RegistroControlModel, SAX_REGISTRO_CONTROL>();
                cfg.CreateMap<SAX_REGISTRO_CONTROL, RegistroControlModel>();

                cfg.CreateMap<SAX_REGISTRO_CONTROL, OnlyRegistroControlModel>();
                cfg.CreateMap<OnlyRegistroControlModel,SAX_REGISTRO_CONTROL>();

                //Partidas
                cfg.CreateMap<SAX_PARTIDAS, PartidasModel>();
                cfg.CreateMap<PartidasModel, SAX_PARTIDAS>();

                //Parametros
                cfg.CreateMap<SAX_PARAMETRO, ParametroModel>();
                cfg.CreateMap<ParametroModel, SAX_PARAMETRO>();

                //Parametros_Temp
                cfg.CreateMap<SAX_PARAMETRO_TEMP, ParametroTempModel>();
                cfg.CreateMap<ParametroTempModel, SAX_PARAMETRO_TEMP>();

                //Supervisor
                cfg.CreateMap<SAX_SUPERVISOR, SupervisorModel>();
                cfg.CreateMap<SupervisorModel, SAX_SUPERVISOR>();

                //Supervisor_Temp
                cfg.CreateMap<SAX_SUPERVISOR_TEMP, SupervisorTempModel>();
                cfg.CreateMap<SupervisorTempModel, SAX_SUPERVISOR_TEMP>();

                cfg.CreateMap<SAX_AREA_CENCOSTO, AreaCentroCostoModel>();
                cfg.CreateMap<AreaCentroCostoModel, SAX_AREA_CENCOSTO>();

                cfg.CreateMap<SAX_CATALOGO_DETALLE, CatalogoDetalleModel>();
                cfg.CreateMap<CatalogoDetalleModel, SAX_CATALOGO_DETALLE>();

                cfg.CreateMap<SAX_CATALOGO, CatalogoModel>();
                cfg.CreateMap<CatalogoModel, SAX_CATALOGO>();

                cfg.CreateMap<SAX_CENTRO_COSTO, CentroCostoModel>();
                cfg.CreateMap<CentroCostoModel, SAX_CENTRO_COSTO>();

                cfg.CreateMap<SAX_COMPROBANTE, ComprobanteModel>();
                cfg.CreateMap<ComprobanteModel, SAX_COMPROBANTE>();

                cfg.CreateMap<SAX_COMPROBANTE_DETALLE, ComprobanteDetalleModel>();
                cfg.CreateMap<ComprobanteDetalleModel, SAX_COMPROBANTE_DETALLE>();

                cfg.CreateMap<SAX_CONCEPTO_COSTO, ConceptoCostoModel>();
                cfg.CreateMap<ConceptoCostoModel, SAX_CONCEPTO_COSTO>();

                cfg.CreateMap<SAX_CUENTA_CONTABLE, CuentaContableModel>();
                cfg.CreateMap<CuentaContableModel, SAX_CUENTA_CONTABLE>();

                cfg.CreateMap<SAX_EMPRESA_CENTRO, EmpresaCentroModel>();
                cfg.CreateMap<EmpresaCentroModel, SAX_EMPRESA_CENTRO>();


            });

        }
    }
}
