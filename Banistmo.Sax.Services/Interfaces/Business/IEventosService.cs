using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;


namespace Banistmo.Sax.Services.Interfaces.Business
{
    public interface IEventosService : IService<EventosModel, SAX_EVENTO, IEventos>
    {
        int Insert_Eventos_EventosTempOperador(EventosModel evetem);

        int Update_EventoTempOperador(EventosTempModel modelevtemp);

        SAX_EVENTO_TEMP Consulta_EventoTempOperador(int eventoid);

        bool Deshacer_EventoTempOperador(int eventoid);

        int SupervidorAprueba_Evento(int eventoId, string userId);

        int SupervidorRechaza_Evento(int eventoId, string userId);
        //List<EventosModel> SearchByFilter(Int32 IdEmp, Int32 IdAreaOpe, string IdCuentaDb, string IdCuentaCR);

        //List<EventosModel> GetAll();

    }
}
