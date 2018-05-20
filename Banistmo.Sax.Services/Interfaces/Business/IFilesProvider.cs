﻿using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Helpers;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
 
    public interface IFilesProvider
    {
        PartidasContent cargaInicial<T>(T input, string userId);

        PartidasContent cargaMasiva<T>(T input, string userId);

        void ValidateInput(int counter, ref List<PartidasModel> list, ref List<MessageErrorPartida> listError, PartidasModel partidaModel);
    }
}
