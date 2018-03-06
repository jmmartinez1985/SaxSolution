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

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class UsuarioRolService : ServiceBase<UsuarioRolModel, SAX_USUARIO_ROL, UsuarioRol>, IUsuarioRolService
    {
        public UsuarioRolService()
            : this(new UsuarioRol())
        {

        }
        public UsuarioRolService(UsuarioRol ur)
            : base(ur)
        { }
    }
}
