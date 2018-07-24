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
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace Banistmo.Sax.Repository.Implementations.Business
{

    [Injectable]
    public class Comprobante : RepositoryBase<SAX_COMPROBANTE>, IComprobante
    {

        private readonly IComprobanteDetalle cdService;
        private readonly IPartidas parService;
        private readonly IUsuarioArea usuarioAreaService;

        public Comprobante()
            : this(new SaxRepositoryContext())
        {
        }
        public Comprobante(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            cdService = cdService ?? new ComprobanteDetalle();
            parService = parService ?? new Partidas();
            usuarioAreaService = usuarioAreaService ?? new UsuarioArea();
        }

        public Comprobante(IRepositoryContext repositoryContext, IComprobanteDetalle detalle, IPartidas partida, IUsuarioArea usuarioArea)
            : base(repositoryContext)
        {
            cdService = detalle ?? new ComprobanteDetalle();
            parService = partida ?? new Partidas();
            usuarioAreaService = usuarioArea ?? new UsuarioArea();
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
                                clonePart.PA_FECHA_ANULACION = DateTime.Now.Date;
                                clonePart.PA_USUARIO_MOD = userName;
                                clonePart.PA_ESTADO_CONCILIA = Convert.ToInt16(BusinessEnumerations.Concilia.NO);
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

                var usuarioArea = usuarioAreaService.GetSingle(x => x.US_ID_USUARIO == userName);
                if (usuarioArea == null)
                    throw new Exception("No se puede crear el comprobante de la conciliacion manual, porque el usuario no tiene area operativa");

                string dateFormat = "yyyyMMdd";
                var filterPartidas = parService.GetAll(c => partidas.Contains(c.PA_REGISTRO));
                if (filterPartidas.Count == 0)
                    throw new Exception();
                var comp = new SAX_COMPROBANTE();
                var detalle = new List<SAX_COMPROBANTE_DETALLE>();
                comp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
                comp.TC_USUARIO_CREACION = userName;
                comp.TC_FECHA_CREACION = DateTime.Now;
                comp.TC_FECHA_PROCESO = DateTime.Now;
                comp.TC_TOTAL_REGISTRO = partidas.Count;
                var now = DateTime.Now.Date;
                var countcomp = base.Count(c => DbFunctions.TruncateTime(c.TC_FECHA_PROCESO) == now);

                comp.TC_COD_COMPROBANTE = System.DateTime.Now.Date.ToString(dateFormat) + ((countcomp + 1).ToString("00000"));

                var credito = filterPartidas.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
                var debito = filterPartidas.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
                comp.TC_TOTAL_CREDITO = credito;
                comp.TC_TOTAL_DEBITO = debito;
                comp.TC_TOTAL = credito + debito;

                comp.TC_FECHA_APROBACION = DateTime.Now;
                comp.TC_USUARIO_CREACION = userName;
                comp.TC_USUARIO_MOD = userName;
                comp.TC_FECHA_MOD = DateTime.Now;
                comp.CA_ID_AREA = usuarioArea.CA_ID_AREA;

                if ((credito + debito) == 0)
                {
                    comp.TC_COD_OPERACION = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.MANUAL);
                }
                else
                    throw new Exception();
                foreach (var item in filterPartidas)
                {
                    detalle.Add(new SAX_COMPROBANTE_DETALLE
                    {
                        PA_REGISTRO = item.PA_REGISTRO,
                        TD_FECHA_CREACION = DateTime.Now,
                        TD_USUARIO_CREACION = userName,
                        TD_FECHA_MOD = DateTime.Now,
                        TD_USUARIO_MOD = userName,
                    });
                    var clonePart = item.CloneEntity();
                    var partEntity = item;
                    clonePart.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ERRADO);
                    clonePart.PA_TIPO_CONCILIA = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.MANUAL);
                    parService.Update(partEntity, clonePart);
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

        public IQueryable<SAX_CUENTA_CONTABLE> ListarCuentasContables(string userId)
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
                              select cc);

                if (result.Count() > 0)
                {
                    var resultFormated = result.Select(x => new SAX_CUENTA_CONTABLE
                    {
                        CO_CUENTA_CONTABLE = x.CO_CUENTA_CONTABLE,
                        CO_COD_AUXILIAR = x.CO_COD_AUXILIAR,
                        CO_NUM_AUXILIAR = x.CO_NUM_AUXILIAR,
                        CO_COD_CONCILIA = x.CO_COD_CONCILIA,
                        CO_ID_CUENTA_CONTABLE = x.CO_ID_CUENTA_CONTABLE
                    });
                    return resultFormated;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IQueryable<SAX_COMPROBANTE> ConsultaComprobanteConciliada(DateTime? FechaCreacion,
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
            try
            {
                int autonomia = Convert.ToInt16(BusinessEnumerations.EstatusCarga.AUTOMATICA);
                int manual = Convert.ToInt16(BusinessEnumerations.EstatusCarga.MANUAL);
                int status = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                int status1 = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_ANULAR);
                int status2 = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO);

                DateTime? fechaTrx = (FechaCreacion == null ? FechaCreacion : FechaCreacion.Value.Date);

                DBModelEntities db = new DBModelEntities();
                if (statusCondi == Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO))
                {
                    var resultComprobante = (from p in db.SAX_PARTIDAS
                                             join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
                                             join com in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals com.TC_ID_COMPROBANTE
                                             join rc in db.SAX_REGISTRO_CONTROL on p.RC_REGISTRO_CONTROL equals rc.RC_REGISTRO_CONTROL
                                             join cc in db.SAX_CUENTA_CONTABLE on p.PA_CTA_CONTABLE equals cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR
                                             where (p.PA_TIPO_CONCILIA == autonomia
                                                 || p.PA_TIPO_CONCILIA == manual)
                                                 //Activar para  pruebas en UAT vmuillo
                                                 //&& p.PA_FECHA_CREACION.Year == DateTime.Now.Year
                                                 //&& p.PA_FECHA_CREACION.Month == DateTime.Now.Month
                                                 && p.PA_FECHA_TRX == (fechaTrx == null ? p.PA_FECHA_TRX : fechaTrx)
                                                 && com.TC_ESTATUS == status
                                                 && com.TC_ID_COMPROBANTE == (comprobanteId == null ? com.TC_ID_COMPROBANTE : comprobanteId)
                                                 && p.PA_COD_EMPRESA == (empresaCod == null ? p.PA_COD_EMPRESA : empresaCod)
                                                 && cc.CO_ID_CUENTA_CONTABLE == (cuentaContableId == null ? cc.CO_ID_CUENTA_CONTABLE : cuentaContableId)
                                                 && com.TC_TOTAL == (importe == null ? com.TC_TOTAL : importe)
                                                 && p.PA_REFERENCIA == (referencia == null ? p.PA_REFERENCIA : referencia)
                                                 && com.CA_ID_AREA == areaOpe
                                             select com).Distinct();
                    return resultComprobante;
                }
                else 
                {
                    var resultComprobante1 = (from p in db.SAX_PARTIDAS
                                             join ct in db.SAX_COMPROBANTE_DETALLE on p.PA_REGISTRO equals ct.PA_REGISTRO
                                             join com in db.SAX_COMPROBANTE on ct.TC_ID_COMPROBANTE equals com.TC_ID_COMPROBANTE
                                             join rc in db.SAX_REGISTRO_CONTROL on p.RC_REGISTRO_CONTROL equals rc.RC_REGISTRO_CONTROL
                                             join cc in db.SAX_CUENTA_CONTABLE on p.PA_CTA_CONTABLE equals cc.CO_CUENTA_CONTABLE + cc.CO_COD_AUXILIAR + cc.CO_NUM_AUXILIAR
                                             where (p.PA_TIPO_CONCILIA == autonomia
                                                 || p.PA_TIPO_CONCILIA == manual)
                                                 //&& p.PA_FECHA_CREACION.Year == DateTime.Now.Year
                                                 //&& p.PA_FECHA_CREACION.Month == DateTime.Now.Month
                                                 && p.PA_FECHA_TRX == (fechaTrx == null ? p.PA_FECHA_TRX : fechaTrx)                                                 
                                                 && com.TC_ESTATUS == status1                                                 
                                                 && com.TC_ID_COMPROBANTE == (comprobanteId == null ? com.TC_ID_COMPROBANTE : comprobanteId)
                                                 && p.PA_COD_EMPRESA == (empresaCod == null ? p.PA_COD_EMPRESA : empresaCod)
                                                 && p.PA_USUARIO_CREACION == (capturador == null ? p.PA_USUARIO_CREACION : capturador)
                                                 && cc.CO_ID_CUENTA_CONTABLE == (cuentaContableId == null ? cc.CO_ID_CUENTA_CONTABLE : cuentaContableId)
                                                 && com.TC_TOTAL == (importe == null ? com.TC_TOTAL : importe)
                                                 && p.PA_REFERENCIA == (referencia == null ? p.PA_REFERENCIA : referencia)
                                                 && com.TC_COD_COMPROBANTE == (lote == null ? com.TC_COD_COMPROBANTE : lote)
                                                 && com.CA_ID_AREA == areaOpe
                                             select com).Distinct();
                    return resultComprobante1;
                }
               
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool SolicitarAnulaciones(List<int> comprobantes, string userName)
        {
            try
            {
                var comps = base.GetAll(c => comprobantes.Contains(c.TC_ID_COMPROBANTE));
                if (comps == null || comps.Count == 0)
                    throw new Exception("Comprobante no encontrados en el control.");

                using (var trx = new TransactionScope())
                {
                    using (var db = new DBModelEntities())
                    {
                        foreach (var item in comps)
                        {
                            var cloneComp = item.CloneEntity();
                            cloneComp.TC_FECHA_MOD = System.DateTime.Now.Date;
                            cloneComp.TC_FECHA_APROBACION = DateTime.Now.Date;
                            cloneComp.TC_USUARIO_MOD = userName;
                            cloneComp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_ANULAR);
                            base.Update(item, cloneComp);
                        }
                    }
                    trx.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public bool AprobarComprobante(int idComprobante, string userName)
        {
            try
            {
                var comp = base.GetSingle(c => c.TC_ID_COMPROBANTE == idComprobante);
                if (comp != null)
                {
                    var cloneComp = comp.CloneEntity();

                    cloneComp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO);
                    cloneComp.TC_USUARIO_MOD = userName;
                    cloneComp.TC_FECHA_MOD = DateTime.Now;

                    var detalles = cdService.GetAll(c => c.TC_ID_COMPROBANTE == idComprobante).ToList();

                    using (var trx = new TransactionScope())
                    {
                        using (var db = new DBModelEntities())
                        {
                            base.Update(comp, cloneComp);
                            detalles.ForEach(c =>
                            {
                                var clonePart = c.SAX_PARTIDAS.CloneEntity();
                                var partEntity = c.SAX_PARTIDAS;
                                clonePart.PA_FECHA_MOD = DateTime.Now.Date;
                                clonePart.PA_USUARIO_MOD = userName;
                                clonePart.PA_TIPO_CONCILIA = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.MANUAL);
                                clonePart.PA_FECHA_CONCILIA = DateTime.Now.Date;
                                clonePart.PA_ESTADO_CONCILIA = Convert.ToInt16(BusinessEnumerations.Concilia.SI);
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
                throw new Exception("No se puede aprobar el comprobante, contacte al administrador.");
            }
        }

        public bool RechazarComprobante(int idComprobante, string userName)
        {
            try
            {
                var comp = base.GetSingle(c => c.TC_ID_COMPROBANTE == idComprobante);
               
                if (comp != null)
                {
                    var detalle = comp.SAX_COMPROBANTE_DETALLE.Select(x=>x.PA_REGISTRO).ToList();
                    if (detalle !=null && detalle.Count == 0)
                        throw new Exception();
                    var filterPartidas = parService.GetAll(c => detalle.Contains(c.PA_REGISTRO));
                    if (filterPartidas.Count == 0)
                        throw new Exception();

                    var cloneComp = comp.CloneEntity();

                    cloneComp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.RECHAZADO);
                    cloneComp.TC_USUARIO_MOD = userName;
                    cloneComp.TC_FECHA_MOD = DateTime.Now;

                    var detalles = cdService.GetAll(c => c.TC_ID_COMPROBANTE == idComprobante).ToList();

                    using (var trx = new TransactionScope())
                    {
                        using (var db = new DBModelEntities())
                        {
                            base.Update(comp, cloneComp);
                            detalles.ForEach(c =>
                            {
                                var clonePart = c.SAX_PARTIDAS.CloneEntity();
                                var partEntity = c.SAX_PARTIDAS;
                                clonePart.PA_FECHA_MOD = DateTime.Now.Date;
                                clonePart.PA_USUARIO_MOD = userName;
                                clonePart.PA_TIPO_CONCILIA = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.MANUAL);
                                clonePart.PA_FECHA_CONCILIA = DateTime.Now.Date;
                                clonePart.PA_ESTADO_CONCILIA = Convert.ToInt16(BusinessEnumerations.Concilia.NO);
                                parService.Update(partEntity, clonePart);
                            });
                        }
                        trx.Complete();
                    }

                    foreach (var item in filterPartidas)
                    {
                        var clonePart = item.CloneEntity();
                        var partEntity = item;
                        clonePart.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                        clonePart.PA_TIPO_CONCILIA = null;
                        parService.Update(partEntity, clonePart);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw new Exception("No se puede rechazar el comprobante, contacte al administrador.");
            }
        }


    }
}
