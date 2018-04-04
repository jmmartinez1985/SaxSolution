using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IUsuariosPorRoleService: IService<UsuariosPorRolModel, SAX_USUARIOS_POR_ROL_Result, IUsuariosPorRol>
    {
        List<UsuariosPorRolModel> GetReporte();
    }
}
