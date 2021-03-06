﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Repository.Implementations;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ReportePartidasAprService : ServiceBase<ReportePartidasAprModel, vi_PartidasApr, ReportePartidasApr>, IReportePartidasAprService
    {
        //public ReportePartidasAprService()
        //{
        //}

        private IReportePartidasApr IParam;
        public ReportePartidasAprService()
            : this(new ReportePartidasApr())
        {

        }

        public ReportePartidasAprService(RepositoryBase<vi_PartidasApr> obj) : base(obj)
        {
        }
    }

    [Injectable ]
    public class ReportePartidasAprConciliablesService : ServiceBase<ReportePartidasAprModel, vi_PartidasApr_Conciliadas, ReportePartidasAprConciliables>, IReportePartidasAprConciliableService
    {
        private IReportePartidasAprConciliables  IParam;

        public ReportePartidasAprConciliablesService(RepositoryBase<vi_PartidasApr_Conciliadas> obj) : base(obj)
        {
        }

        public ReportePartidasAprConciliablesService()
            : this(new ReportePartidasAprConciliables())
        {

        }

        //public ReportePartidasAprConciliablesService(RepositoryBase<vi_PartidasApr_Conciliadas> obj) : base(obj)
        //{
        //}

    }
}
