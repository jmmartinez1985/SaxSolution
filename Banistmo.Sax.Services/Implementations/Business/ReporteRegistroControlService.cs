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
using Banistmo.Sax.Repository.Implementations;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ReporteRegistroControlService : ServiceBase<ReporteRegistroControlModel, SAX_REGISTRO_CONTROL, ReporteRegistroControl>, IReporteRegistroControlService
    {

        public ReporteRegistroControlService()
            : this(new ReporteRegistroControl())
        {

        }

        public ReporteRegistroControlService(RepositoryBase<SAX_REGISTRO_CONTROL> obj) : base(obj)
        {
        }
    }
}
