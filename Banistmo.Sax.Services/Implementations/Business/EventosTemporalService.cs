using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using AutoMapper;
using Banistmo.Sax.Repository.Interfaces.Business;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class EventosTemporalService : ServiceBase<EventosTempModel, SAX_EVENTO_TEMP, EventosTemp>, IEventosTempService
    {
        public EventosTemporalService()
            : this(new EventosTemp())
        {

        }
        public EventosTemporalService(EventosTemp evt)
            : base(evt)
        { }

        private IEventosTemp eveService;
        public EventosTemporalService(IEventosTemp service)
        : this(new EventosTemp())
        {
            eveService = service;
        }
    }
}
