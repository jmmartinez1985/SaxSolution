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
                //Usuario
                cfg.CreateMap<SAX_USUARIO, UserModel>();
                cfg.CreateMap<UserModel, SAX_USUARIO>();
                //Excel
                //cfg.CreateMap<ExcelData, ExcelDataModel>();
                //cfg.CreateMap<ExcelDataModel, ExcelData>();
                cfg.CreateMap<SAX_DIAS_FERIADOS, DiasFeriadosModel>();
                cfg.CreateMap<DiasFeriadosModel, SAX_DIAS_FERIADOS>();
                cfg.CreateMap<AspNetUsers, AspNetUserModel>();
                cfg.CreateMap<AspNetUserModel, AspNetUsers>();

                cfg.CreateMap<AspNetRoles, RolesModel>();
                cfg.CreateMap<RolesModel, AspNetRoles>();

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

                cfg.CreateMap<SAX_USUARIO_ROL, UsuarioRolModel>();
                cfg.CreateMap<UsuarioRolModel, SAX_USUARIO_ROL>();

                cfg.CreateMap<SAX_USUARIO_EMPRESA, UsuarioEmpresaModel>();
                cfg.CreateMap<UsuarioEmpresaModel, SAX_USUARIO_EMPRESA>();

            });

        }
    }
}
