using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface ISupervisor : IRepository<SAX_SUPERVISOR>
    {
        SAX_SUPERVISOR InsertSupervisor(SAX_SUPERVISOR supervisor);
    }
}
