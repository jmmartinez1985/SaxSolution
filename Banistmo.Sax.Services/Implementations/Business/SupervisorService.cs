using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations;
using Banistmo.Sax.Common;
using AutoMapper;
using Banistmo.Sax.Repository.Interfaces.Business;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class SupervisorService : ServiceBase<SupervisorModel, SAX_SUPERVISOR, Supervisor>, ISupervisorService
    {

        private readonly ISupervisor ISupervisor;

        public SupervisorService()
            : this(new Supervisor())
        {

        }

        public SupervisorService(Supervisor objSupervisor)
            : base(objSupervisor)
        { }

        public SupervisorService(ISupervisor objISupervisor)
      : this(new Supervisor())
        {
            ISupervisor = objISupervisor;
        }

        public SupervisorModel InsertSupervisor(SupervisorModel supervisor)
        {
            var model = Mapper.Map< SupervisorModel, SAX_SUPERVISOR>(supervisor);
            var modelresult = ISupervisor.InsertSupervisor(model);
            return Mapper.Map<SAX_SUPERVISOR,SupervisorModel >(modelresult);
        }
    }
}
