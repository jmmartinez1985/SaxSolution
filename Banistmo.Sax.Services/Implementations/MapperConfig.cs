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
            Mapper.Initialize(cfg => {
                //PAIS
                cfg.CreateMap<PA_PAISES, PaisModel>();
                cfg.CreateMap<PaisModel, PA_PAISES>();
                //PROVINCIA
                cfg.CreateMap<PR_PROVINCIA, ProvinciaModel>();
                cfg.CreateMap<ProvinciaModel, PR_PROVINCIA>();
                //Usuario
                cfg.CreateMap<SAX_USUARIO, UserModel>();
                cfg.CreateMap<UserModel, SAX_USUARIO>();

            });

        }
    }
}
