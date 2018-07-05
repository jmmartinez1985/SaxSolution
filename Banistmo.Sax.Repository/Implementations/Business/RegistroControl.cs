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
using EntityFramework.Utilities;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.Entity;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class RegistroControl : RepositoryBase<SAX_REGISTRO_CONTROL>, IRegistroControl
    {
        const string refFormat = "yyyyMMdd";

        public RegistroControl()
            : this(new SaxRepositoryContext())
        {
        }
        public RegistroControl(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            newContext = repositoryContext;
        }
        private IPartidas partidas;
        private readonly IRepositoryContext newContext;

        public RegistroControl(IPartidas ipartidas)
        {
            partidas = ipartidas;
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> GetFilters()
        {
            throw new NotImplementedException();
        }

        public override Expression<Func<SAX_REGISTRO_CONTROL, bool>> SearchFilters(SAX_REGISTRO_CONTROL obj)
        {
            return x => x.RC_REGISTRO_CONTROL == obj.RC_REGISTRO_CONTROL;
        }
        public SAX_REGISTRO_CONTROL LoadFileData(SAX_REGISTRO_CONTROL control)
        {
            SAX_REGISTRO_CONTROL registro = null;
            try
            {
                using (var trx = new TransactionScope())
                {
                    using (var db = new DBModelEntities())
                    {
                        db.Database.CommandTimeout = 200000;
                        db.Configuration.LazyLoadingEnabled = false;
                        var partidas = control.SAX_PARTIDAS.ToList();
                        control.SAX_PARTIDAS = null;
                        registro = base.Insert(control, true);
                        partidas.ForEach(c => c.RC_REGISTRO_CONTROL = registro.RC_REGISTRO_CONTROL);
                        EFBatchOperation.For(db, db.SAX_PARTIDAS).InsertAll(partidas, batchSize: 1500);
                    }
                    trx.Complete();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return registro;
        }

        private void InsertItems<T>(IEnumerable<T> items, string schema, string tableName, IList<ColumnMapping> properties, DbConnection storeConnection, int? batchSize)
        {
            using (var reader = new EFDataReader<T>(items, properties))
            {
                var con = storeConnection as SqlConnection;
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                using (SqlBulkCopy copy = new SqlBulkCopy(con) { BulkCopyTimeout = 0 })
                {
                    copy.BatchSize = Math.Min(reader.RecordsAffected, batchSize ?? 15000); //default batch size
                    if (!string.IsNullOrWhiteSpace(schema))
                    {
                        copy.DestinationTableName = string.Format("[{0}].[{1}]", schema, tableName);
                    }
                    else
                    {
                        copy.DestinationTableName = "[" + tableName + "]";
                    }

                    copy.NotifyAfter = 0;

                    foreach (var i in Enumerable.Range(0, reader.FieldCount))
                    {
                        copy.ColumnMappings.Add(i, properties[i].NameInDatabase);
                    }
                    copy.WriteToServer(reader);
                    copy.Close();
                }
            }
        }

        public bool IsValidLoad(DateTime fecha)
        {
            object[] parameters = new object[] { new SqlParameter("i_fecha_proceso", fecha) };
            var value = newContext.ObjectContext.Database.SqlQuery<Forker>("usp_fecha_proceso @i_fecha_proceso", parameters).ToList();
            var res = value.FirstOrDefault();
            if (res == null)
                return false;
            if (res.DIA_HABIL == "S")
                return true;
            return false;
        }

        public string IsValidReferencia(string referencia, ref decimal monto)
        {
            object[] parameters = new object[] { new SqlParameter("i_referencia", referencia) };
            var value = newContext.ObjectContext.Database.SqlQuery<ReferenciaForker>("usp_buscar_referencia @i_referencia", parameters).ToList();
            var res = value.FirstOrDefault();
            if (res.IMPORTE != null)
            {
                monto = Convert.ToDecimal(res.IMPORTE);
            }
            if (res == null)
                return "";
            return res.REFERENCIA;
        }

        public bool removeRegistro(int registro)
        {
            try
            {
                using (var trx = new TransactionScope())
                {
                    using (var db = new DBModelEntities())
                    {
                        db.Database.CommandTimeout = 200000;
                        db.Configuration.LazyLoadingEnabled = false;
                        var validCreado = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CREADO);
                        var validPorAprobar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
                        EFBatchOperation.For(db, db.SAX_PARTIDAS).Where(p => p.RC_REGISTRO_CONTROL == registro && (p.PA_STATUS_PARTIDA == validCreado)
                        || p.PA_STATUS_PARTIDA == validPorAprobar).Delete();
                        EFBatchOperation.For(db, db.SAX_REGISTRO_CONTROL).Where(p => p.RC_REGISTRO_CONTROL == registro).Delete();
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

        public bool AprobarRegistro(int registro, string userName)
        {
            try
            {
                var control = base.GetSingle(c => c.RC_REGISTRO_CONTROL == registro);
                if (control == null || control.SAX_PARTIDAS.Count == 0)
                    throw new Exception("Registro control no es valido o no se cuenta con partidas asociadas.");
                var partidas = control.SAX_PARTIDAS;
                var cloneReg = control.CloneEntity();
                cloneReg.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                cloneReg.RC_USUARIO_APROBADOR = userName;
                cloneReg.RC_FECHA_APROBACION = DateTime.Now.Date;
                using (var trx = new TransactionScope())
                {
                    using (var db = new DBModelEntities())
                    {
                        db.Database.CommandTimeout = 200000;
                        db.Configuration.LazyLoadingEnabled = false;
                        base.Update(control, cloneReg);
                        partidas = ReferenceAsign(control.RC_COD_OPERACION, partidas, userName, partidas.FirstOrDefault().RC_REGISTRO_CONTROL);
                        EFBatchOperation.For(db, db.SAX_PARTIDAS).UpdateAll(partidas, x => x.ColumnsToUpdate(y => y.PA_FECHA_APROB, z => z.PA_USUARIO_APROB, t => t.PA_STATUS_PARTIDA, u => u.PA_REFERENCIA), null, 1500);
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

        private ICollection<SAX_PARTIDAS> ReferenceAsign(int tipoOperacion, ICollection<SAX_PARTIDAS> partidasList, string userName, int idPartida)
        {
            string codeOperacion = string.Empty;
            CuentaContable cuentas = new CuentaContable();
            Empresa emp = new Empresa();
            AreaOperativa area = new AreaOperativa();
            RegistroControl regCtrl = new RegistroControl();
            if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL))
                codeOperacion = "I";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA))
                codeOperacion = "D";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
                codeOperacion = "M";

            DateTime todays = DateTime.Now.Date;

            var refAut = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);

            partidas = partidas ?? new Partidas();
            var aproveed = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);

            switch (tipoOperacion)
            {
                case 21:
                    var groupByFechaTrx = partidasList.GroupBy(c => c.PA_FECHA_TRX);

                    
                    
                    foreach (var item in groupByFechaTrx)
                    {
                        //int intcounter = 1;
                        var counterRecord = partidas.Count(c => DbFunctions.TruncateTime(c.PA_FECHA_TRX) == DbFunctions.TruncateTime(item.Key)
                                                           && c.PA_STATUS_PARTIDA == aproveed) + 1;
                        var itemgroup = item.Key;
                        foreach (var internalcol in item)
                        {
                            if (string.IsNullOrEmpty(internalcol.PA_REFERENCIA) | internalcol.PA_REFERENCIA == "")
                                if (internalcol.PA_ORIGEN_REFERENCIA == refAut)
                                {
                                    internalcol.PA_REFERENCIA = itemgroup.ToString(refFormat) + counterRecord.ToString().PadLeft(5, '0');
                                    counterRecord++;
                                }
                          
                            internalcol.PA_FECHA_APROB = DateTime.Now.Date;
                            internalcol.PA_USUARIO_APROB = userName;
                            internalcol.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                            //intcounter++;
                            

                        }
                    }
                    break;
                case 22:
                    var groupByFechaTrx2 = partidasList.GroupBy(c => c.PA_FECHA_TRX);

                   
                    foreach (var item in groupByFechaTrx2)
                    {
                        //int intcounter = 1;
                        var counterRecord = partidas.Count(c => DbFunctions.TruncateTime(c.PA_FECHA_TRX) == DbFunctions.TruncateTime(item.Key)
                                                           && c.PA_STATUS_PARTIDA == aproveed) + 1;
                        var itemgroup = item.Key;
                        foreach (var internalcol in item)
                        {
                            if (string.IsNullOrEmpty(internalcol.PA_REFERENCIA) | internalcol.PA_REFERENCIA == "")
                            {
                                if (internalcol.PA_ORIGEN_REFERENCIA == refAut)
                                {
                                    internalcol.PA_REFERENCIA = itemgroup.ToString(refFormat) + counterRecord.ToString().PadLeft(5, '0');
                                    counterRecord++;
                                }
 
                            }
                            internalcol.PA_FECHA_APROB = DateTime.Now.Date;
                            internalcol.PA_USUARIO_APROB = userName;
                            internalcol.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                            //intcounter++;
                           
                        }
                    }
                    break;
                case 23:
                    var groupByFechaTrx3 = partidasList.GroupBy(c => c.PA_FECHA_TRX);


                    foreach (var item in groupByFechaTrx3)
                    {
                        //int intcounter = 1;
                        var counterRecord = partidas.Count(c => DbFunctions.TruncateTime(c.PA_FECHA_TRX) == DbFunctions.TruncateTime(item.Key)
                                                            && c.PA_STATUS_PARTIDA == aproveed) + 1;
                        var itemgroup = item.Key;
                        foreach (var internalcol in item)
                        {
                            if (string.IsNullOrEmpty(internalcol.PA_REFERENCIA) | internalcol.PA_REFERENCIA == "")
                            {
                                if (internalcol.PA_ORIGEN_REFERENCIA == refAut)
                                {
                                    internalcol.PA_REFERENCIA = itemgroup.ToString(refFormat) + counterRecord.ToString().PadLeft(5, '0');
                                    counterRecord++;
                                }
                            }
                            internalcol.PA_FECHA_APROB = DateTime.Now.Date;
                            internalcol.PA_USUARIO_APROB = userName;
                            internalcol.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.APROBADO);
                            //intcounter++;
                           
                        }
                    }
                    break;
                default:
                    break;
            }

            return partidasList;
        }

        public bool RechazarRegistro(int registro, string userName)
        {
            try
            {
                var control = base.GetSingle(c => c.RC_REGISTRO_CONTROL == registro);
                if (control == null || control.SAX_PARTIDAS.Count == 0)
                    throw new Exception("Registro control no es valido o no se cuenta con partidas asociadas.");
                var partidas = control.SAX_PARTIDAS;
                var cloneReg = control.CloneEntity();

                cloneReg.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.RECHAZADO);
                cloneReg.RC_USUARIO_MOD = userName;
                cloneReg.RC_FECHA_MOD = DateTime.Now;

                using (var trx = new TransactionScope())
                {
                    using (var db = new DBModelEntities())
                    {
                        db.Database.CommandTimeout = 200000;
                        db.Configuration.LazyLoadingEnabled = false;
                        base.Update(control, cloneReg);
                        partidas.ToList().ForEach(c =>
                        {
                            c.PA_FECHA_MOD = DateTime.Now.Date;
                            c.PA_USUARIO_MOD = userName;
                            c.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.RECHAZADO);
                        });
                        EFBatchOperation.For(db, db.SAX_PARTIDAS).UpdateAll(partidas, x => x.ColumnsToUpdate(
                            y => y.PA_FECHA_MOD,
                            z => z.PA_USUARIO_MOD,
                            t => t.PA_STATUS_PARTIDA), null, 1500);
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

        public string IsValidReferencia(string referencia, string empresa, string moneda, string cuenta_contable, decimal monto_saldo, ref decimal monto, ref int tipo_error)
        {
            List<object> parameters = new List<object>();
            parameters.Add(new SqlParameter("i_referencia", referencia));
            parameters.Add(new SqlParameter("i_empresa", empresa));
            parameters.Add(new SqlParameter("i_moneda", moneda));
            parameters.Add(new SqlParameter("i_cta_contable", cuenta_contable));
            parameters.Add(new SqlParameter("i_importe", monto_saldo));
            var value = newContext.ObjectContext.Database.SqlQuery<ReferenciaForker>("USP_BUSCAR_REFERENCIA @i_referencia,@i_empresa,@i_moneda,@i_cta_contable,@i_importe", parameters.ToArray()).ToList();
            var res = value.FirstOrDefault();
            if (res.IMPORTE != null)
            {
                monto = Convert.ToDecimal(res.IMPORTE);
            }

            if (res.IMPORTE != null)
            {
                tipo_error = Convert.ToInt16(res.TIPO_ERROR);
            }
            if (res == null)
                return "";
            return res.REFERENCIA;
        }

        internal class Forker
        {
            public string DIA_HABIL { get; set; }
        }

        internal class ReferenciaForker
        {
            public string REFERENCIA { get; set; }
            public decimal? IMPORTE { get; set; }
            public int? TIPO_ERROR { get; set; }

        }
    }
}
