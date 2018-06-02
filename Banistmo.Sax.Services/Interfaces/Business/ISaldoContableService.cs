using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Repository.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface ISaldoContableService  : IService<ReporteSaldoContableModel, SAX_SALDO_CONTABLE, IReporteSaldoContable>
    {
    }
}
