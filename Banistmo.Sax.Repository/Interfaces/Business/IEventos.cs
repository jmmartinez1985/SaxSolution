using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Model;

namespace Banistmo.Sax.Repository.Interfaces.Business
{
    public interface IEventos : IRepository<SAX_EVENTO>
    {
        int Insert_Eventos_EventosTempOperador(SAX_EVENTO eve);

        bool Update_EventoTempOperador(SAX_EVENTO_TEMP eventoTempNuevo);

        SAX_EVENTO_TEMP Consulta_EventoTempOperador(int eventoid);

        bool Deshacer_EventoTempOperador(int eventoid);

        bool SupervidorAprueba_Evento(int eventoId,string userId);

        bool SupervidorRechaza_Evento(int eventoId);
        //List<SAX_EVENTO> SearchByFilter(Int32 IdEmp, Int32 IdAreaOpe, string IdCuentaDb, string IdCuentaCR);

        //List<SAX_EVENTO> GetAll();
    }
}
