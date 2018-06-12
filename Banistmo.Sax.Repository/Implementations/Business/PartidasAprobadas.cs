﻿using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Common;
namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class PartidasAprobadas : RepositoryBase<vi_PartidasAprobadas>, IPartidasAprobadas
    {
        public PartidasAprobadas()
            : this(new SaxRepositoryContext())
        {
        }
        public PartidasAprobadas(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<vi_PartidasAprobadas, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }
        public override Expression<Func<vi_PartidasAprobadas, bool>> SearchFilters(vi_PartidasAprobadas obj)
        {
            return x => x.AREAOPERATIVADESC == obj.AREAOPERATIVADESC;
        }

        public List<vi_PartidasAprobadas> ConsultaPartidaPorAprobar(string codEnterprise,
                                                                        string reference,
                                                                        decimal? importe,
                                                                        DateTime? trxDateIni,
                                                                        DateTime? trxDateFin,
                                                                        string ctaAccount,
                                                                        int? userArea)
        {
                        
            try
            {
                int capManual = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL);
                int capInicial = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL);
                int status = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);               
                int aprobado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                int anulado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);
                int porAprobar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);

                DBModelEntities db = new DBModelEntities();
                var resultComprobante = (from p in db.vi_PartidasAprobadas
                                          join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
                                          join com in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals com.TC_ID_COMPROBANTE                                          
                                         where p.PA_STATUS_PARTIDA == aprobado
                                            || p.PA_STATUS_PARTIDA == anulado
                                             && p.PA_ESTADO_CONCILIA == 0
                                             && p.PA_REFERENCIA != ""
                                             && p.RC_COD_AREA == userArea
                                             && p.PA_IMPORTE == (importe == null ? p.PA_IMPORTE : importe)
                                             && p.PA_COD_EMPRESA == (codEnterprise == null ? p.PA_COD_EMPRESA : codEnterprise)
                                             && p.PA_CTA_CONTABLE == (ctaAccount == null ? p.PA_CTA_CONTABLE : ctaAccount)
                                             && p.PA_REFERENCIA == (reference == null ? p.PA_REFERENCIA : reference)
                                             && p.PA_FECHA_TRX >= (trxDateIni == null ? p.PA_FECHA_TRX : trxDateIni)
                                             && p.PA_FECHA_TRX <= (trxDateFin == null ? p.PA_FECHA_TRX : trxDateFin)
                                             && com.TC_COD_OPERACION == porAprobar
                                         select p).ToList();
                return resultComprobante;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
