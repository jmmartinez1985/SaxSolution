using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
using System.Linq.Expressions;
using EntityFramework.Utilities;
using System.Transactions;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class Eventos : RepositoryBase<SAX_EVENTO>, IEventos
    {
        public Eventos()
             : this(new SaxRepositoryContext())
        {
        }
        public Eventos(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        private readonly IEventosTemp evtempService;
        private SAX_EVENTO_TEMP eventotempactual;

        public Eventos(IEventosTemp evtemp)
        {
            evtempService = evtemp;
        }

        public override Expression<Func<SAX_EVENTO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_EVENTO, bool>> SearchFilters(SAX_EVENTO obj)
        {
            return x => x.EV_COD_EVENTO == obj.EV_COD_EVENTO;
        }

        private void Insert(SAX_EVENTO_TEMP eventoTemp)
        {
            throw new NotImplementedException();
        }


        public bool Insert_Eventos_EventosTempOperador(SAX_EVENTO evento, SAX_EVENTO_TEMP eventoTemp)
        {
            bool result = false;
            try
            {

                using (var trx = new TransactionScope())
                {

                    var eventoExiste = evtempService.GetSingle(x => x.EV_COD_EVENTO == evento.EV_COD_EVENTO);
                    if (eventoExiste == null)
                    {
                        //Insertamos Evento
                        var ev = new Eventos();
                        evento.EV_ESTATUS = 0;                        
                        ev.Insert(evento);
                        
                        //Insertamos EventoTemp
                        int id = evento.EV_COD_EVENTO;
                        eventoTemp.EV_COD_EVENTO = id;
                        eventoTemp.EV_ESTATUS = 2;                        
                        evtempService.Insert(eventoTemp);

                        trx.Complete();
                        result = true;
                    }                   
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public bool Update_EventoTempOperador(SAX_EVENTO_TEMP eventoTempNuevo)
        {
            bool result = false;
            try
            {

                using (var trx = new TransactionScope())
                {
                    //Actualizamos tabla Evento con status = 0 = pendiente
                    var evt = new Eventos();
                    var eventoactual = evt.GetSingle(x => x.EV_COD_EVENTO == eventoTempNuevo.EV_COD_EVENTO);
                    if (eventoactual != null)
                    {
                        var eventonuevo = eventoactual;
                        eventonuevo.EV_ESTATUS = 0;
                        evt.Update(eventoactual, eventonuevo);
                    }

                    //Actualizamos tabla EventoTemp con valores nuevos y con status = 2 = por aprobar
                    var evtmp = new EventosTemp();
                    var eventotempactual = evtmp.GetSingle(x => x.EV_COD_EVENTO == eventoTempNuevo.EV_COD_EVENTO);
                    if (eventotempactual != null)
                    {                        
                        eventoTempNuevo.EV_COD_EVENTO_TEMP = eventotempactual.EV_COD_EVENTO_TEMP;
                        eventoTempNuevo.EV_ESTATUS = 2;
                        evtmp.Update(eventotempactual, eventoTempNuevo);                        
                    }

                    trx.Complete();
                    result = true;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public SAX_EVENTO_TEMP Consulta_EventoTempOperador(int eventoid)
        {
            try
            {
                var evtmp = new EventosTemp();
                eventotempactual = evtmp.GetSingle(x => x.EV_COD_EVENTO == eventoid);
            }
            catch (Exception ex)
            {
                eventotempactual = null;
            }
            return eventotempactual;
        }


    }
}
