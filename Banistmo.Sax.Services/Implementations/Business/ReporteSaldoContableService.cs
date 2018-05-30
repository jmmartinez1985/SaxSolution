using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ReporteSaldoContableService : ServiceBase<ReporteSaldoContableModel, SAX_SALDO_CONTABLE, ReporteSaldoContable>, ISaldoContableService
    {

        public ReporteSaldoContableService()
            : this(new ReporteSaldoContable())
        {

        }
        public ReporteSaldoContableService(ReporteSaldoContable ao)
            : base(ao)
        { }
    }
}
