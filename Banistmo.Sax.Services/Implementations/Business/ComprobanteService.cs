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

        public bool ConciliacionManual(List<int> partidas, string userName)
        {
            return service.ConciliacionManual(partidas, userName);
        }

        public void RechazarAnulacion(ComprobanteModel comprobante, string userName)
        {
            comprobante.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO).ToString();
            base.Update(comprobante);
        }

        public void SolitarAnulacion(ComprobanteModel comprobante, string userName)
        {
            comprobante.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_ANULAR).ToString();
            base.Update(comprobante);
        }

        public List<ComprobanteModel> ConsultaComprobanteConciliadaServ()
        {
            var modeloServ = service.ConsultaComprobanteConciliada();
            return Mapper.Map<List<SAX_COMPROBANTE>, List<ComprobanteModel>>(modeloServ);
        }
    }
}
