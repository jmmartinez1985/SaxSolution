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
using AutoMapper;
using Banistmo.Sax.Repository.Interfaces.Business;

namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class CuentaContableService : ServiceBase<CuentaContableModel, SAX_CUENTA_CONTABLE, CuentaContable>, ICuentaContableService
    {
        public CuentaContableService()
            : this(new CuentaContable())
        {
            CtaContService = CtaContService ?? new CuentaContable();
        }
        public CuentaContableService(CuentaContable ao)
            : base(ao)
        { }

        private readonly ICuentaContable CtaContService;

        public bool conciliaCuenta(string cta)
        {
            var result = base.GetSingle(c => c.CO_CUENTA_CONTABLE == cta && c.CO_COD_CONCILIA == "Y");
            if (result == null)
                return false;
            return true;
        }
    }
}
