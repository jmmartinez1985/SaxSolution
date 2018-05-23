﻿using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations;
using Banistmo.Sax.Common;
namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class EmpresaAreasCentroCostoService : ServiceBase<EmpresaAreasCentroCostoModel, vi_EmpresaAreasCentroCosto, EmpresaAreasCentroCosto>, IEmpresaAreasCentroCostoService
    {
        public EmpresaAreasCentroCostoService()
            : this(new EmpresaAreasCentroCosto())
        { }
        public EmpresaAreasCentroCostoService(EmpresaAreasCentroCosto eacc)
            : base(eacc)
        { }
    }
}
