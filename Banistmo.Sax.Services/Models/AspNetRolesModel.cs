using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class AspNetRolesModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Estatus { get; set; }

        public ICollection<ModuloRolModel> SAX_MODULO_ROL { get; set; }

        //public ICollection<AspNetUserModel> AspNetUsers { get; set; }
    }
}
