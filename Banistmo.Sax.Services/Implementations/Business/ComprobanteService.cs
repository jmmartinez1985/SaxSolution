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
using Banistmo.Sax.Repository.Interfaces.Business;

namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class ComprobanteService : ServiceBase<ComprobanteModel, SAX_COMPROBANTE, Comprobante>, IComprobanteService
    {
        private readonly IComprobante service;
        public ComprobanteService()
            : this(new Comprobante())
        {
            service = service ?? new Comprobante();
        }
        public ComprobanteService(Comprobante ao)
            : base(ao)
        { }

        public ComprobanteService(Comprobante ao, IComprobante svc)
            : base(ao)
        {
            service = svc;
        }

        public bool AnularComprobante(int comprobante, string userName)
        {
            return service.AnularComprobante(comprobante,userName);
        }
    }
}
