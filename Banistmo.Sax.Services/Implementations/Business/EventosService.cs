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
    public class EventosService : ServiceBase<EventosModel, SAX_EVENTO, Eventos>, IEventosService
    {
        public EventosService()
            : this(new Eventos())
        {

        }
        public EventosService(Eventos evento)
            : base(evento)
        { }

        private readonly IEventos eveService;
        public EventosService(IEventos service)
        : this(new Eventos())
        {
            eveService = service;
        }
        public bool Insert_Eventos_EventosTempOperador(EventosModel ev, EventosTempModel evtem)
        {
            SAX_EVENTO modelA = Mapper.Map<EventosModel, SAX_EVENTO>(ev);
            SAX_EVENTO_TEMP modelB = Mapper.Map<EventosTempModel, SAX_EVENTO_TEMP>(evtem);
            return eveService.Insert_Eventos_EventosTempOperador(modelA, modelB);
        }

        public bool Update_EventoTempOperador(EventosTempModel eventoTempNuevo)
        {
            SAX_EVENTO_TEMP modelevtemp = Mapper.Map<EventosTempModel, SAX_EVENTO_TEMP>(eventoTempNuevo);
            return eveService.Update_EventoTempOperador(modelevtemp);
        }

        public SAX_EVENTO_TEMP Consulta_EventoTempOperador(int eventoid)
        {
            return eveService.Consulta_EventoTempOperador(eventoid);
        }

        public bool Deshacer_EventoTempOperador(int eventoid)
        {
            return eveService.Deshacer_EventoTempOperador(eventoid);
        }
    }
}
