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
    public class ModuloService : ServiceBase<ModuloModel, SAX_MODULO, Modulo>, IModuloService
    {
        public ModuloService()
            : this(new Modulo())
        {

        }
        public ModuloService(Modulo mo)
            : base(mo)
        { }
    }
}
