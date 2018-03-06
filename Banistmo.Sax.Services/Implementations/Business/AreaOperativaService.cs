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
    public class AreaOperativaService : ServiceBase<AreaOperativaModel, SAX_AREA_OPERATIVA, AreaOperativa>, IAreaOperativaService
    {
        public AreaOperativaService()
            : this(new AreaOperativa())
        {

        }
        public AreaOperativaService(AreaOperativa ao)
            : base(ao)
        { }
    }
   
}
