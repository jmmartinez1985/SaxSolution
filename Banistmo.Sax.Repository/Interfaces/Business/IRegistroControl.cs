﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IRegistroControl : IRepository<SAX_REGISTRO_CONTROL>
    {
        void LoadFileData(SAX_REGISTRO_CONTROL control);
    }
}
