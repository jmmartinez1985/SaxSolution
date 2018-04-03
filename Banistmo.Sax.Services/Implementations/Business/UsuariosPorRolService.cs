using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Services.Interfaces;
using System.Linq.Expressions;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class UsuariosPorRolService : ServiceBase<UsuariosPorRolModel, SAX_USUARIOS_POR_ROL_Result, UsuariosPorRol>, IUsuariosPorRoleService
    {

        public UsuariosPorRolService()
            : this(new UsuariosPorRol())
        {

        }
        public UsuariosPorRolService(UsuariosPorRol usrrol)
            : base(usrrol)
        { }


        public List<UsuariosPorRolModel> GetReporte()
        {
            return this.ExecuteProcedure("SAX_USUARIOS_POR_ROL", new object[0]);
        }

 
    }
}
