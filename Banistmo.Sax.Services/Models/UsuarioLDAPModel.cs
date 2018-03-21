using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class UsuarioLDAPModel
    {        
        public string userNumber { get; set; }
        //public string contraseña { get; set; }
        public string nombreCompleto { get; set; }
        public bool existe { get; set; }
        public string error { get; set; }
        
    }
}
