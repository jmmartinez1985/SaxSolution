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
using System.Transactions;

namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class ComprobanteService : ServiceBase<ComprobanteModel, SAX_COMPROBANTE, Comprobante>, IComprobanteService
    {
        private readonly IComprobante service;
        public ComprobanteService()
            : this(new Comprobante())
        {
            service = service ?? new Comprobante();
        }
        public ComprobanteService(Comprobante ao)
            : base(ao)
        { }

        public ComprobanteService(Comprobante ao, IComprobante svc)
            : base(ao)
        {
            service = svc;
        }

        public bool AnularComprobante(int comprobante, string userName)
        {
            return service.AnularComprobante(comprobante,userName);
        }

        public bool ConciliacionManual(List<int> partidas, string userName)
        {
            return service.ConciliacionManual(partidas, userName);
        }

        public void RechazarAnulacion(ComprobanteModel comprobante, string userName)
        {
            ComprobanteDetalle cdService = new ComprobanteDetalle();
            IPartidas parService = new Partidas();
            comprobante.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO).ToString();
            comprobante.TC_FECHA_RECHAZO = DateTime.Now;
            comprobante.TC_USUARIO_RECHAZO = userName;
            //Prueba
            using (var trx = new TransactionScope())
            {
                using (var db = new DBModelEntities())
                {

                        var detalles = cdService.GetAll(c => c.TC_ID_COMPROBANTE == comprobante.TC_ID_COMPROBANTE).ToList();
                        detalles.ForEach(c =>
                        {
                            var clonePart = c.SAX_PARTIDAS.CloneEntity();
                            var partEntity = c.SAX_PARTIDAS;
                            clonePart.PA_FECHA_ANULACION = null;
                            clonePart.PA_USUARIO_ANULACION = null;
                            parService.Update(partEntity, clonePart);
                        });
                    }
                trx.Complete();
            }
            base.Update(comprobante);
        }

        public void SolitarAnulacion(ComprobanteModel comprobante, string userName)
        {
            comprobante.TC_FECHA_MOD = System.DateTime.Now;
            comprobante.TC_USUARIO_MOD = userName;
            comprobante.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_ANULAR).ToString();
            base.Update(comprobante);
        }

        public IQueryable<SAX_COMPROBANTE> ConsultaComprobanteConciliadaServ(DateTime? FechaCreacion,
                                                                        string empresaCod,
                                                                        int? comprobanteId,
                                                                        int? cuentaContableId,
                                                                        decimal? importe,
                                                                        string referencia,
                                                                        int? areaOpe,
                                                                        string lote,
                                                                        string capturador,
                                                                        int? statusCondi)
        {
            var modeloServ = service.ConsultaComprobanteConciliada(FechaCreacion, empresaCod, comprobanteId, cuentaContableId, importe, referencia, areaOpe,lote, capturador, statusCondi);
            return modeloServ;
            //return Mapper.Map<List<SAX_COMPROBANTE>, List<ComprobanteModel>>(modeloServ);
        }

        public IQueryable<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId)
        {
            var modeloServ = service.ListarCuentasContables(userId);
            return modeloServ;
        }

        public bool SolicitarAnulaciones(List<int> comprobantes, string userName)
        {
            return service.SolicitarAnulaciones(comprobantes, userName);
        }

        public bool AprobarComprobante(int idComprobantes, string userName)
        {
            return service.AprobarComprobante(idComprobantes, userName);
        }

        public bool RechazarComprobante(int idComprobantes, string userName)
        {
            return service.RechazarComprobante(idComprobantes, userName);
        }
    }
}
