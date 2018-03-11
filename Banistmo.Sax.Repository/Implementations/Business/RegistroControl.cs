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
    public class RegistroControl : RepositoryBase<SAX_REGISTRO_CONTROL>, IRegistroControl
    {

        public RegistroControl()
            : this(new SaxRepositoryContext())
        {
        }
        public RegistroControl(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        private readonly IPartidas partidas;

        public RegistroControl(IPartidas ipartidas)
        {
            partidas = ipartidas;
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> SearchFilters(SAX_REGISTRO_CONTROL obj)
        {
            return x => x.RC_REGISTRO_CONTROL == obj.RC_REGISTRO_CONTROL;
        }


        public void LoadFileData(SAX_REGISTRO_CONTROL control)
        {

        }

    }
}