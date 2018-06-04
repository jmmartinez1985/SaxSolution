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

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class Partidas : RepositoryBase<SAX_PARTIDAS>, IPartidas
    {
        public Partidas()
            : this(new SaxRepositoryContext())
        {
        }
        public Partidas(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public override Expression<Func<SAX_PARTIDAS, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_PARTIDAS, bool>> SearchFilters(SAX_PARTIDAS obj)
        {
            return x => x.PA_REGISTRO == obj.PA_REGISTRO;
        }

        //public List<Model.vi_PartidasAprobadas> ConsultaConciliacioneManualPorAprobar(DateTime? Fechatrx,
        //                                                                string empresaCod,                                                                    
        //                                                                int? areaPartida)
        //{
        //    try
        //    {
        //        int capManual = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL);
        //        int capInicial = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL);
        //        int status = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
        //        DateTime? fechaTrx = (Fechatrx == null ? Fechatrx : Fechatrx.Value.Date);

        //        DBModelEntities db = new DBModelEntities();
        //        var resultComprobante = (from p in db.vi_PartidasAprobadas
        //                                 //join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
        //                                 //join com in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals com.TC_ID_COMPROBANTE
        //                                 //join cc in db.SAX_CUENTA_CONTABLE on p.PA_CTA_CONTABLE equals cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR
        //                                 where  p.RC_COD_OPERACION == capManual
        //                                     && p.RC_COD_OPERACION == capInicial                                           
        //                                     && p.PA_FECHA_CREACION.Value.Year == DateTime.Now.Year
        //                                     && p.PA_FECHA_TRX.Value.Month == DateTime.Now.Month
        //                                     && p.PA_FECHA_TRX == fechaTrx
        //                                     && p.PA_STATUS_PARTIDA == status
        //                                     && p.PA_ESTADO_CONCILIA == Convert.ToInt32(BusinessEnumerations.EstatusCarga.POR_CONCILIAR)
        //                                     && p.RC_COD_AREA == (areaPartida == null? p.RC_COD_AREA:areaPartida)
        //                                 select p).ToList();
        //        return resultComprobante;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
    }
}
