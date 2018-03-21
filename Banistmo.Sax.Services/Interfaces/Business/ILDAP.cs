using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Models;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface ILDAP
    {
        UsuarioLDAPModel validaUsuarioLDAP(string usuario, string contraseña, string usuarioNuevoValidar = null);
    }
}
