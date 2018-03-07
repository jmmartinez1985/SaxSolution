﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IUsuarioEmpresa : IRepository<SAX_USUARIO_EMPRESA>
    {
        void CreateAndRemove(List<SAX_USUARIO_EMPRESA> create, List<int> remove);
    }
}
