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
using System.Transactions;

namespace Banistmo.Sax.Repository.Implementations.Business
{

    [Injectable]
    public class Comprobante : RepositoryBase<SAX_COMPROBANTE>, IComprobante
    {

        private readonly IComprobanteDetalle cdService;
        private readonly IPartidas parService;

        public Comprobante()
            : this(new SaxRepositoryContext())
        {
        }
        public Comprobante(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            cdService = cdService ?? new ComprobanteDetalle();
            parService = parService ?? new Partidas();
        }

        public Comprobante(IRepositoryContext repositoryContext, IComprobanteDetalle detalle, IPartidas partida)
            : base(repositoryContext)
        {
            cdService = detalle ?? new ComprobanteDetalle();
            parService = partida ?? new Partidas();
        }

        public bool AnularComprobante(int comprobante, string userName)
        {
            try
            {
                var comp = base.GetSingle(c => c.TC_ID_COMPROBANTE == comprobante);
                if (comp != null)
                {
                    var cloneComp = comp.CloneEntity();

                    cloneComp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);
                    cloneComp.TC_USUARIO_MOD = userName;
                    cloneComp.TC_FECHA_MOD = DateTime.Now;

                    var detalles = cdService.GetAll(c => c.TC_ID_COMPROBANTE == comprobante).ToList();

                    using (var trx = new TransactionScope())
                    {
                        using (var db = new DBModelEntities())
                        {
                            base.Update(comp, cloneComp);
                            detalles.ForEach(c =>
                            {
                                var clonePart = c.SAX_PARTIDAS.CloneEntity();
                                var partEntity = c.SAX_PARTIDAS;
                                clonePart.PA_FECHA_ANULACION = DateTime.Now;
                                clonePart.PA_USUARIO_MOD = userName;
                                clonePart.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);
                                parService.Update(partEntity, clonePart);
                            });
                        }
                        trx.Complete();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw new Exception("No se puede anular el comprobante, contacte al administrador.");
            }
        }

        public bool ConciliacionManual(List<int> partidas, string userName)
        {
            try
            {
                var filterPartidas = parService.GetAll(c => partidas.Contains(c.PA_REGISTRO));
                if (filterPartidas.Count == 0)
                    throw new Exception();
                var comp = new SAX_COMPROBANTE();
                var detalle = new List<SAX_COMPROBANTE_DETALLE>();
                comp.TC_ESTATUS =Convert.ToInt16( BusinessEnumerations.EstatusCarga.CONCILIADO);
                comp.TC_USUARIO_CREACION = userName;
                comp.TC_FECHA_CREACION = DateTime.Now;
                comp.TC_FECHA_PROCESO = DateTime.Now;
                comp.TC_TOTAL_REGISTRO = partidas.Count;

                var credito = filterPartidas.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
                var debito = filterPartidas.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
                comp.TC_TOTAL_CREDITO = credito;
                comp.TC_TOTAL_DEBITO = debito;
                comp.TC_TOTAL = credito + debito;

                comp.TC_FECHA_APROBACION = DateTime.Now;
                comp.TC_USUARIO_CREACION = userName;
                comp.TC_USUARIO_MOD = userName;
                comp.TC_FECHA_MOD = DateTime.Now;

                if ((credito + debito) == 0)
                {
                    //REVISAR 
                    comp.TC_COD_OPERACION = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.SI);
                }
                else
                    comp.TC_COD_OPERACION = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.SI);
                foreach (var item in filterPartidas)
                {
                    detalle.Add(new SAX_COMPROBANTE_DETALLE
                    {
                        PA_REGISTRO = item.PA_REGISTRO,
                        TD_FECHA_CREACION = DateTime.Now,
                        TD_USUARIO_CREACION = userName,
                        TD_FECHA_MOD = DateTime.Now,
                        TD_USUARIO_MOD = userName
                    });
                }
                comp.SAX_COMPROBANTE_DETALLE = detalle;
                base.Insert(comp);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("No se puede crear el comprobante de la conciliacion manual, contacte al administrador.");
            }
        }

        public override Expression<Func<SAX_COMPROBANTE, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_COMPROBANTE, bool>> SearchFilters(SAX_COMPROBANTE obj)
        {
            return x => x.TC_ID_COMPROBANTE == obj.TC_ID_COMPROBANTE;
        }
               
        public List<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId)
        {
            try
            {
                DBModelEntities db = new DBModelEntities();
                var result = (from p in db.SAX_PARTIDAS_TEMP
                              join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
                              join c in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals c.TC_ID_COMPROBANTE
                              join cc in db.SAX_CUENTA_CONTABLE on p.PA_CTA_CONTABLE equals cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR
                              where c.TC_ESTATUS == 37
                              && p.PA_USUARIO_CREACION == userId
                              select cc).Distinct();

                if (result.Count() > 0)
                {
                    var resultFormated =  result.Select(x => new SAX_CUENTA_CONTABLE
                    {
                        CO_CUENTA_CONTABLE = x.CO_CUENTA_CONTABLE,
                        CO_COD_AUXILIAR = x.CO_COD_AUXILIAR,
                        CO_NUM_AUXILIAR = x.CO_NUM_AUXILIAR,
                        CO_COD_CONCILIA = x.CO_COD_CONCILIA,
                        CO_ID_CUENTA_CONTABLE = x.CO_ID_CUENTA_CONTABLE
                    }).ToList();
                    return resultFormated;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SAX_COMPROBANTE> ConsultaComprobanteConciliada(DateTime? FechaCreacion,
                                                                      string empresaCod,
                                                                      int? comprobanteId,
                                                                      int? cuentaContableId,
                                                                      decimal? importe,
                                                                      string referencia)
        {
            try
            {
                DBModelEntities db = new DBModelEntities();
                var resultComprobante = (from p in db.SAX_PARTIDAS_TEMP
                                         join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
                                         join com in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals com.TC_ID_COMPROBANTE
                                         join cc in db.SAX_CUENTA_CONTABLE on p.PA_CTA_CONTABLE equals cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR
                                         where p.PA_TIPO_CONCILIA == 41
                                             || p.PA_TIPO_CONCILIA == 42
                                             && p.PA_FECHA_CREACION.Value.Year == (FechaCreacion == null ? p.PA_FECHA_CREACION.Value.Year : FechaCreacion.Value.Year)
                                             && p.PA_FECHA_CREACION.Value.Month == (FechaCreacion == null ? p.PA_FECHA_CREACION.Value.Month : FechaCreacion.Value.Month)
                                             && com.TC_ESTATUS == Convert.ToInt32(BusinessEnumerations.EstatusCarga.CONCILIADO.ToString())
                                             && com.TC_ID_COMPROBANTE == (comprobanteId == null ? com.TC_ID_COMPROBANTE : comprobanteId)
                                             && p.PA_COD_EMPRESA == (empresaCod == null ? p.PA_COD_EMPRESA : empresaCod)
                                             && cc.CO_ID_CUENTA_CONTABLE == (cuentaContableId == null ? cc.CO_ID_CUENTA_CONTABLE : cuentaContableId)
                                             && com.TC_TOTAL == (importe == null ? com.TC_TOTAL : importe)
                                             && p.PA_REFERENCIA == (referencia == null ? p.PA_REFERENCIA : referencia)
                                         select com).ToList();
                return resultComprobante;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
