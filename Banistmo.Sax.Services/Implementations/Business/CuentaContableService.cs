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

        }
        public CuentaContableService(CuentaContable ao)
            : base(ao)
        { }

        private ICuentaContable CtaContService;
        public CuentaContableService(ICuentaContable service)
            : this(new CuentaContable())
        {
            CtaContService = service;
        }

        public List<CuentaContableModel> ConsultaCuentaDb()
        {
            var dedito = CtaContService.ConsultaCuentaDb();
            return Mapper.Map<List<SAX_CUENTA_CONTABLE>,List<CuentaContableModel>>(dedito);
        }

        public List<CuentaContableModel> ConsultaCuentaCr()
        {
           
            var credito = CtaContService.ConsultaCuentaCr();            
            return Mapper.Map<List<SAX_CUENTA_CONTABLE>, List<CuentaContableModel>>(credito);            
        }

        
    }
}
