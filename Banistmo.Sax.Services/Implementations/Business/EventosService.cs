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

        private IEventos eveService;
        public EventosService(IEventos service)
        : this(new Eventos())
        {
            eveService = service;
        }
        public int Insert_Eventos_EventosTempOperador(EventosModel ev)
        {
            SAX_EVENTO modelEvento = Mapper.Map<EventosModel, SAX_EVENTO>(ev);
            eveService = eveService ?? new Eventos();
            return eveService.Insert_Eventos_EventosTempOperador(modelEvento);
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

        public bool SupervidorAprueba_Evento(int eventoId, string userId)
        {
            
            eveService = eveService ?? new Eventos();
            return eveService.SupervidorAprueba_Evento(eventoId, userId);
        }

        public bool SupervidorRechaza_Evento(int eventoId)
        {
            eveService = eveService ?? new Eventos();
            return eveService.SupervidorRechaza_Evento(eventoId);
        }

        //public List<EventosModel> SearchByFilter(Int32 IdEmp, Int32 IdAreaOpe, string IdCuentaDb, string IdCuentaCR)
        //{
        //    var filter = eveService.SearchByFilter(IdEmp, IdAreaOpe, IdCuentaDb, IdCuentaCR);
        //    return Mapper.Map<List<SAX_EVENTO>, List<EventosModel>>(filter);
        //}

        //public List<EventosModel> GetAll()
        //{
        //    var getAll = eveService.GetAll();
        //    getAll.Select(c => new
        //    {
        //        c.SAX_AREA_OPERATIVA.CA_ID_AREA,

        //    });

        //    return Mapper.Map<List<SAX_EVENTO>, List<EventosModel>>(getAll);
        //}
    }
}
