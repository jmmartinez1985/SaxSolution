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
    public class EventosTemp : RepositoryBase<SAX_EVENTO_TEMP>, IEventosTemp
    {
        public override Expression<Func<SAX_EVENTO_TEMP, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_EVENTO_TEMP, bool>> SearchFilters(SAX_EVENTO_TEMP obj)
        {
            throw new NotImplementedException();
        }

       
    }
}