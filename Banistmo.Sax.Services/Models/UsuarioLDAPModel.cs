using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class UsuarioLDAPModel
    {        
        public string UserNumber { get; set; }
        //public string contraseña { get; set; }
        public string NombreCompleto { get; set; }
        public bool Existe { get; set; }
        public string Error { get; set; }

        public string Mail { get; set; }

    }
}
