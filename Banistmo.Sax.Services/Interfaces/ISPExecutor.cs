using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces
{
    public interface ISPExecutor
    {
        UsuariosPorRolModel GetUsuarioPorRol();

    }
}
