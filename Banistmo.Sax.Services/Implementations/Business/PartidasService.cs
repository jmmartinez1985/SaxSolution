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
    }
}

