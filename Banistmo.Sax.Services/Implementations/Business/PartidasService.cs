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
using System.ComponentModel.DataAnnotations;
using Banistmo.Sax.Services.Implementations.Rules;
using Banistmo.Sax.Services.Implementations.Rules.FileInput;
using System.Globalization;

namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class PartidasService : ServiceBase<PartidasModel, SAX_PARTIDAS, Partidas>, IPartidasService
    {

        private readonly IPartidasService partidaService;
        private readonly ICentroCostoService centroCostoService;
        private readonly IEmpresaService empresaService;
        private readonly IConceptoCostoService conceptoCostoService;
        private readonly ICuentaContableService contableService;

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
            par.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumations.EstatusCarga.CREADO).ToString();
            par.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + counterRecords;
            return base.Insert(par, true);
        }
    }
}

