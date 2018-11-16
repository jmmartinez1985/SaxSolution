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
    public class SaldoNoConciliableService : ServiceBase<SaldoContableNoConciliableModel, SAX_SALDO_NOCONCILIABLE, SaldoNoConciliable>, ISaldoNoConciliableService
    {
        private  ISaldoNoConciliable service;
        public SaldoNoConciliableService()
            : this(new SaldoNoConciliable())
        {

        }
        public SaldoNoConciliableService(SaldoNoConciliable em)
            : base(em)
        { }
    }
}
