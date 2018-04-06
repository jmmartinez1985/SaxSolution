using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class OnlyRegistroControlService: ServiceBase<OnlyRegistroControlModel, SAX_REGISTRO_CONTROL, OnlyRegistroControl>, IOnlyRegistroControlService
    {
        public OnlyRegistroControlService()
            : this(new OnlyRegistroControl())
        {

        }
        public OnlyRegistroControlService(OnlyRegistroControl reco)
            : base(reco)
        { }
    }
}
