﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
using System.Linq.Expressions;

namespace Banistmo.Sax.Repository.Implementations.Business
{

    [Injectable]
    public class AreaCentroCosto : RepositoryBase<SAX_AREA_CENCOSTO>, IAreaCentroCosto
    {
        public AreaCentroCosto()
            : this(new SaxRepositoryContext())
        {
        }
        public AreaCentroCosto(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_AREA_CENCOSTO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_AREA_CENCOSTO, bool>> SearchFilters(SAX_AREA_CENCOSTO obj)
        {
            return x => x.AD_ID_REGISTRO == obj.AD_ID_REGISTRO;
        }
    }
}
