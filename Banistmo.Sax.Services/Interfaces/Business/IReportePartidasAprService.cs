﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Repository.Interfaces;

namespace Banistmo.Sax.Services.Interfaces.Business
{
   public  interface IReportePartidasAprService : IService <ReportePartidasAprModel, vi_PartidasApr, IReportePartidasApr>
    {
    }

    public interface IReportePartidasAprConciliableService : IService<ReportePartidasAprModel, vi_PartidasApr_Conciliadas, IReportePartidasAprConciliables>
    {
    }
}
