using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;

namespace Banistmo.Sax.Services.Helpers
{
    public class PartidasContent
    {
        public List<PartidasModel> ListPartidas { get; set; }
        public List<MessageErrorPartida> ListError { get; set; }
    }
}
