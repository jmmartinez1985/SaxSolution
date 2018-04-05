using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using AutoMapper;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ModuloRolService : ServiceBase<ModuloRolModel, SAX_MODULO_ROL, ModuloRol>, IModuloRolService
    {

        private readonly IModuloRol moduleRolService;
        public ModuloRolService()
            : this(new ModuloRol())
        {

        }
        public ModuloRolService(ModuloRol mr)
            : base(mr)
        { }

        public ModuloRolService(IModuloRol srvModuleRole)
        : this(new ModuloRol())
        {
            moduleRolService = srvModuleRole;
        }

        public void CreateAndRemove(List<ModuloRolModel> create, List<int> remove)
        {
            List<SAX_MODULO_ROL> modelA = Mapper.Map<List<ModuloRolModel>, List<SAX_MODULO_ROL>>(create);
            moduleRolService.CreateAndRemove(modelA, remove);
        }
    }
}

