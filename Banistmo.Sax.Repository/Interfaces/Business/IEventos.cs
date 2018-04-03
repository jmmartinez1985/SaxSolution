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
        bool Insert_Eventos_EventosTempOperador(SAX_EVENTO eve, SAX_EVENTO_TEMP evetem);

        bool Update_EventoTempOperador(SAX_EVENTO_TEMP eventoTempNuevo);

        SAX_EVENTO_TEMP Consulta_EventoTempOperador(int eventoid);

        bool Deshacer_EventoTempOperador(int eventoid);
    }
}
