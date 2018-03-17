using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
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
    public class SupervisorTempService : ServiceBase<SupervisorTempModel, SAX_SUPERVISOR_TEMP, SupervisorTemp>, ISupervisorTempService
    {
        public SupervisorTempService()
            : this(new SupervisorTemp())
        {

        }

        public SupervisorTempService(SupervisorTemp objSupervisor)
            : base(objSupervisor)
        { }
    }
}
