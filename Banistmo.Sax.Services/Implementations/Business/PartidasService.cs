using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Banistmo.Sax.Services.Implementations.Rules;
using Banistmo.Sax.Services.Implementations.Rules.FileInput;
using System.Globalization;

namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class PartidasService : ServiceBase<PartidasModel, SAX_PARTIDAS, Partidas>, IPartidasService
    {

        private  IPartidas service;
        private  IPartidasService partidaService;
        private  ICentroCostoService centroCostoService;
        private  IEmpresaService empresaService;
        private  IConceptoCostoService conceptoCostoService;
        private  ICuentaContableService contableService;

        public PartidasService()
            : this(new Partidas())
        {

        }
        public PartidasService(Partidas ao)
            : base(ao)
        { }

        public PartidasService(Partidas ao, ICentroCostoService centroCostoSvc,
            IEmpresaService empresaSvc,
            IConceptoCostoService conceptoCostoSvc,
            ICuentaContableService contableSvc)
           : base(ao)
        {
            centroCostoService = centroCostoSvc;
            empresaService = empresaSvc;
            conceptoCostoService = conceptoCostoSvc;
            contableService = contableSvc;
        }

        public PartidasModel CreateSinglePartida(PartidasModel par)
        {

            centroCostoService = centroCostoService ?? new CentroCostoService();
            partidaService = partidaService ?? new PartidasService();
            conceptoCostoService = conceptoCostoService ?? new ConceptoCostoService();
            contableService = contableService ?? new CuentaContableService();
            empresaService = empresaService ?? new EmpresaService();


            IFormatProvider culture = new CultureInfo("en-US", true);
            string dateFormat = "MMddyyyy";
            //Counting number of record already exist.
            var counterRecords = partidaService.Count();

            var centroCostos = centroCostoService.GetAll();
            var conceptoCostos = conceptoCostoService.GetAll();
            var cuentas = contableService.GetAll();
            var empresa = empresaService.GetAll();

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(par, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(par, context, validationResults, true);
            ValidationList rules = new ValidationList();
            rules.Add(new FTSFOValidation(par, null));
            rules.Add(new FTFCIFOValidation(par, null));
            rules.Add(new COValidation(par, cuentas));
            rules.Add(new CEValidation(par, empresa));
            rules.Add(new CCValidations(par, centroCostos));
            rules.Add(new CONCEPCOSValidation(par, conceptoCostos));
            rules.Add(new IImporteValidation(par, null));
            if (!rules.IsValid)
                throw new Exception("No se cumple con la entrada de datos y las reglas de negocios");
            par.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CREADO);
            par.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + counterRecords;
            return base.Insert(par, true);
        }

        //public List<PartidasModel> ConsultaConciliacioneManualPorAprobar(DateTime? Fechatrx,
        //                                                             string empresaCod,
        //                                                             int? comprobanteId,
        //                                                             int? cuentaContableId,
        //                                                             decimal? importe)
        //{
        //    var modeloServ = service.ConsultaConciliacioneManualPorAprobar(Fechatrx, empresaCod, comprobanteId, cuentaContableId, importe);
        //    return Mapper.Map<List<SAX_PARTIDAS>, List<PartidasModel>>(modeloServ);
        //}
    }
}

