using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Models
{
    public class RolModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Estatus { get; set; }
        public string Description { get; set; }
        public string Discriminator { get; set; }
    }
}
