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
    public class ParametrosService : ServiceBase<ParametroModel , SAX_PARAMETRO, Parametro>, IParametroService
    {
        public ParametrosService()
            : this(new Parametro())
        {

        }

        public ParametrosService(Parametro objPar)
            : base(objPar)
        { }
    }
}