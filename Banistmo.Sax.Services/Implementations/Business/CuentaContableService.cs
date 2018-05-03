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
            CtaContService = new CuentaContable();
        }
        public CuentaContableService(CuentaContable ao)
            : base(ao)
        { }

        private ICuentaContable CtaContService;
   

        

        
    }
}
