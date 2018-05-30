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

                    cloneComp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.ANULADO).ToString();
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
                comp.TC_ESTATUS = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CONCILIADO).ToString();
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
                    comp.TC_COD_OPERACION = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.SI).ToString();
                }
                else
                    comp.TC_COD_OPERACION = Convert.ToInt16(BusinessEnumerations.TipoConciliacion.SI).ToString();
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

        public List<SAX_COMPROBANTE> ConsultaComprobanteConciliada()
        {
            try
            {
                DBModelEntities db = new DBModelEntities();
                var resultComprobante = db.usp_consulta_comprobante_por_Anular().Select(x => new SAX_COMPROBANTE
                {
                    TC_COD_COMPROBANTE = x.TC_COD_COMPROBANTE,
                    TC_COD_OPERACION = x.TC_COD_OPERACION,
                    TC_ESTATUS = x.TC_ESTATUS,
                    TC_FECHA_APROBACION = x.TC_FECHA_APROBACION,
                    TC_FECHA_CREACION = x.TC_FECHA_CREACION,
                    TC_FECHA_MOD = x.TC_FECHA_MOD,
                    TC_FECHA_PROCESO = x.TC_FECHA_PROCESO,
                    TC_FECHA_RECHAZO = x.TC_FECHA_RECHAZO,
                    TC_ID_COMPROBANTE = x.TC_ID_COMPROBANTE,
                    TC_TOTAL = x.TC_TOTAL,
                    TC_TOTAL_CREDITO = x.TC_TOTAL_CREDITO,
                    TC_TOTAL_DEBITO = x.TC_TOTAL_DEBITO,
                    TC_TOTAL_REGISTRO = x.TC_TOTAL_REGISTRO,
                    TC_USUARIO_APROBADOR = x.TC_USUARIO_APROBADOR,
                    TC_USUARIO_CREACION = x.TC_USUARIO_CREACION,
                    TC_USUARIO_MOD = x.TC_USUARIO_MOD,
                    TC_USUARIO_RECHAZO = x.TC_USUARIO_RECHAZO

                }).ToList();

                return resultComprobante;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
