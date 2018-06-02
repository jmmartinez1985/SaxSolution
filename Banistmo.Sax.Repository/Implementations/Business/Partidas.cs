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

        public List<SAX_PARTIDAS> ConsultaConciliacioneManualPorAprobar(DateTime? Fechatrx,
                                                                     string empresaCod,
                                                                     int? comprobanteId,
                                                                     int? cuentaContableId,
                                                                     decimal? importe)
        {
            try
            {
                DBModelEntities db = new DBModelEntities();
                var resultComprobante = (from p in db.SAX_PARTIDAS
                                         join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
                                         join com in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals com.TC_ID_COMPROBANTE
                                         join cc in db.SAX_CUENTA_CONTABLE on p.PA_CTA_CONTABLE equals cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR
                                         where  p.PA_TIPO_CONCILIA == Convert.ToInt32(BusinessEnumerations.EstatusCarga.MANUAL)
                                             && p.PA_FECHA_CREACION.Year == DateTime.Now.Year
                                             && p.PA_FECHA_CREACION.Month == DateTime.Now.Month
                                             && p.PA_FECHA_TRX == (Fechatrx.Value.Date == null? p.PA_FECHA_TRX: Fechatrx.Value.Date)
                                             && p.PA_ESTADO_CONCILIA == Convert.ToInt32(BusinessEnumerations.EstatusCarga.POR_CONCILIAR)
                                             && com.TC_ID_COMPROBANTE == (comprobanteId == null ? com.TC_ID_COMPROBANTE : comprobanteId)
                                             && p.PA_COD_EMPRESA == (empresaCod == null ? p.PA_COD_EMPRESA : empresaCod)
                                             && cc.CO_ID_CUENTA_CONTABLE == (cuentaContableId == null ? cc.CO_ID_CUENTA_CONTABLE : cuentaContableId)
                                             && com.TC_TOTAL_DEBITO == (importe == null ? com.TC_TOTAL : importe)
                                            
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
