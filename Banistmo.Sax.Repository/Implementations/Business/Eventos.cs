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

        private readonly IEmpresa InjectIemp;
        private readonly IAreaOperativa InjectIareaOpe;
        private readonly ICuentaContable InjectIctaCont;

        public Eventos(IEventosTemp evtemp, IEmpresa emp, IAreaOperativa aOpe, ICuentaContable ctaCont)
        {
            evtempService = evtemp;
            InjectIemp = emp;
            InjectIareaOpe = aOpe;
            InjectIctaCont = ctaCont;
        }

        public override Expression<Func<SAX_EVENTO, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public List<SAX_EVENTO> GetAll()
        {
            try
            {
                DBModelEntities db = new DBModelEntities();
                var result = (from s in db.SAX_EVENTO
                              select s).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);            
            }            
        }

        public override Expression<Func<SAX_EVENTO, bool>> SearchFilters(SAX_EVENTO obj)
        {
            return x => x.EV_COD_EVENTO == obj.EV_COD_EVENTO;
        }

        public List<SAX_EVENTO> SearchByFilter (Int32 IdEmp, Int32 IdAreaOpe, string IdCuentaDb, string IdCuentaCR)
        {           
            try
            {                
                DBModelEntities db = new DBModelEntities();
                var result = (from s in db.SAX_EVENTO
                         where s.CE_ID_EMPRESA == (IdEmp == 0 ? s.CE_ID_EMPRESA : IdEmp) 
                            && s.EV_ID_AREA == (IdAreaOpe == 0 ? s.EV_ID_AREA : IdAreaOpe)
                            && s.EV_CUENTA_CREDITO == (IdCuentaCR == "null" ? s.EV_CUENTA_CREDITO : IdCuentaCR)
                            && s.EV_CUENTA_DEBITO == (IdCuentaDb == "null" ? s.EV_CUENTA_DEBITO : IdCuentaDb)
                         select s).ToList();
                return result;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        private void Insert(SAX_EVENTO_TEMP eventoTemp)
        {
            throw new NotImplementedException();
        }


        public bool Insert_Eventos_EventosTempOperador(SAX_EVENTO evento)
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
                        evento.EV_ESTATUS = Convert.ToInt32(RegistryState.Pendiente);                        
                        ev.Insert(evento);
                                                
                        //Insertamos EventoTemp
                        int id = evento.EV_COD_EVENTO;
                        var evtmp = mapeoEntidadEventoTemporal(evento, id, Convert.ToInt32(RegistryState.PorAprobar));                                              
                        evtempService.Insert(evtmp);

                        trx.Complete();
                        result = true;
                    }  
                    else
                    {
                        var eventoTemp = evtempService.GetSingle(x => x.EV_COD_EVENTO == evento.EV_COD_EVENTO);
                        evtempService.Update(eventoTemp, mapeoEntidadEventoTemporal(evento, evento.EV_COD_EVENTO, Convert.ToInt32(RegistryState.PorAprobar)));

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo crear el nuevo Evento. " + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Actualiza la tabal Evento temporal con valores nuevos y actualiza el status de la tabla evento 
        /// con status = 0 = Pendiente
        /// </summary>
        /// <param name="eventoTempNuevo"></param>
        /// <returns></returns>
        public bool Update_EventoTempOperador(SAX_EVENTO_TEMP eventoTempNuevo)
        {
            bool actualizado = false;
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
                    actualizado = true;
                }
            }
            catch (Exception ex)
            {

            }
            return actualizado;
        }

        public SAX_EVENTO_TEMP Consulta_EventoTempOperador(int eventoidConsulta)
        {
            try
            {
                var evtmp = new EventosTemp();
                eventotempactual = evtmp.GetSingle(x => x.EV_COD_EVENTO == eventoidConsulta);
            }
            catch (Exception ex)
            {
                eventotempactual = null;
            }
            return eventotempactual;
        }

        public bool Deshacer_EventoTempOperador(int eventoidDeshacer)
        {
            bool Deshacer = false;
            try
            {
                //Obtenemos los valores de los eventos sin modificar para actualizar la tabla de eventos temporal
                var evt = new Eventos();
                var eventoSinMod = evt.GetSingle(x => x.EV_COD_EVENTO == eventoidDeshacer);
                
                //Obtenermos la tabla evento temporal para actualizar los datos con la tabla sin modificar
                var evtmp = new EventosTemp();
                var eventotempactual = evtmp.GetSingle(x => x.EV_COD_EVENTO == eventoSinMod.EV_COD_EVENTO);

                using (var trx = new TransactionScope())
                {
                    if (eventotempactual != null && eventoSinMod != null)
                    {
                        evtmp.Update(eventotempactual, mapeoEntidadEventoTemporal(eventoSinMod, eventotempactual.EV_COD_EVENTO_TEMP, Convert.ToInt32(RegistryState.Aprobado)));
                        evt.Attach(eventoSinMod);
                    }
                    trx.Complete();
                    Deshacer = true;
                }
            }
            catch (Exception ex)
            {

            }
            return Deshacer;
        }

        private SAX_EVENTO_TEMP mapeoEntidadEventoTemporal (SAX_EVENTO evt, int codEventoTemporal, int status)
        {
            var evtReturn = new SAX_EVENTO_TEMP();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            //evtReturn.EV_COD_AUXILIARC = evt.EV_COD_AUXILIARC;
            //evtReturn.EV_COD_AUXILIARD = evt.EV_COD_AUXILIARD;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO;
            evtReturn.EV_COD_EVENTO_TEMP = codEventoTemporal;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = status;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            //evtReturn.EV_NUM_AUXILIARC = evt.EV_NUM_AUXILIARC;
            //evtReturn.EV_NUM_AUXILIARD = evt.EV_NUM_AUXILIARD;
            evtReturn.EV_REFERENCIA = evt.EV_REFERENCIA;
            //evtReturn.EV_TIPO_ACCION = evt.EV_TIPO_ACCION;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            
            return evtReturn;
        }

        public bool SupervidorAprueba_Evento(int eventoIdAprueba, string userId )
        {
            try
            {
                //Obtenermos la tabla evento para actualizar los datos con la tabla sin modificar
                var evt = new Eventos();
                var eventoActual = evt.GetSingle(x => x.EV_COD_EVENTO == eventoIdAprueba);
                //Obtenermos la tabla evento Temporal para actualizar los datos con la tabla sin modificar
                var evtmp = new EventosTemp();
                var eventoTempActual = evtmp.GetSingle(x => x.EV_COD_EVENTO == eventoIdAprueba);
                using (var trx = new TransactionScope())
                {
                    if (eventoActual != null && eventoTempActual != null)
                    {
                        var ev = mapeoEntidadEvento(eventoTempActual, eventoIdAprueba, Convert.ToInt16(RegistryState.Aprobado));
                        ev.EV_USUARIO_APROBADOR = userId;
                        //Actualizamos Evento con valores de Eventos Temporal
                        evt.Update(eventoActual, ev);
                        trx.Complete();
                        return true;
                    }
                    else
                    {
                        throw new Exception("No se encuentra, Evento para Aprobar");
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private SAX_EVENTO mapeoEntidadEvento(SAX_EVENTO_TEMP evt, int codEvento, int status)
        {
            var evtReturn = new SAX_EVENTO();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            //evtReturn.EV_COD_AUXILIARC = evt.EV_COD_AUXILIARC;
            //evtReturn.EV_COD_AUXILIARD = evt.EV_COD_AUXILIARD;
            evtReturn.EV_COD_EVENTO = codEvento;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = status;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            //evtReturn.EV_NUM_AUXILIARC = evt.EV_NUM_AUXILIARC;
            //evtReturn.EV_NUM_AUXILIARD = evt.EV_NUM_AUXILIARD;
            evtReturn.EV_REFERENCIA = evt.EV_REFERENCIA;
            //evtReturn.EV_TIPO_ACCION = evt.EV_TIPO_ACCION;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;

            return evtReturn;
        }

        public bool SupervidorRechaza_Evento(int eventoIdRechaza)
        {
            bool rechazado = false;
            //Obtenermos la tabla evento para actualizar los datos con la tabla sin modificar
            var evt = new Eventos();
            var eventoActual = evt.GetSingle(x => x.EV_COD_EVENTO == eventoIdRechaza);
            //Obtenermos la tabla evento Temporal para actualizar los datos con la tabla sin modificar
            var evtmp = new EventosTemp();
            var eventoTempActual = evtmp.GetSingle(x => x.EV_COD_EVENTO == eventoIdRechaza);
            using (var trx = new TransactionScope())
            {
                if (eventoActual != null && eventoTempActual != null)
                {
                    evtmp.Update(eventoTempActual, mapeoEntidadEventoTemporal(eventoActual, eventoIdRechaza, Convert.ToInt16(RegistryState.Aprobado))); 
                }
            }
            return rechazado;
        }

        public enum RegistryState
        {
            Pendiente = 0,
            Aprobado = 1,
            PorAprobar = 2,
            Eliminado = 3
        }
    }
}
