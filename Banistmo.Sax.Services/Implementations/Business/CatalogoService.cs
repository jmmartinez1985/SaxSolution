﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;


namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class CatalogoService : ServiceBase<CatalogoModel, SAX_CATALOGO, Catalogo>, ICatalogoService
    {
        public CatalogoService()
            : this(new Catalogo())
        {

        }
        public CatalogoService(Catalogo ao)
            : base(ao)
        { }
    }
}