using Banistmo.Sax.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Services.Implementations
{

    [Injectable]
    public class SPExecutor : ISPExecutor
    {
        private readonly IProcedureExecuter executer;

        public SPExecutor(IProcedureExecuter exeSvc) {
            executer = exeSvc;
        }

        public UsuariosPorRolModel GetUsuarioPorRol()
        {
            var res =  executer.ExecuteProcedure<SAX_USUARIOS_POR_ROL_Result>("SAX_USUARIOS_POR_ROL", new object[0]).ToList();;
            throw new NotImplementedException();
        }
    }
}
