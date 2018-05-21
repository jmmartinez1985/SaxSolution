﻿using System;
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

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class RegistroControl : RepositoryBase<SAX_REGISTRO_CONTROL>, IRegistroControl
    {

        public RegistroControl()
            : this(new SaxRepositoryContext())
        {
        }
        public RegistroControl(IRepositoryContext repositoryContext)
            : base(repositoryContext)
        {
            newContext = repositoryContext;
        }
        private readonly IPartidas partidas;
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
            var value = newContext.ObjectContext.Database.SqlQuery<Forker>("usp_fecha_proceso", fecha).ToList();
            var res = value.FirstOrDefault();
            if (res == null)
                return false;
            if (res.DIA_HABIL == "S")
                return true;
            return false;
        }

        public string IsValidReferencia(string referencia)
        {
            var value = newContext.ObjectContext.Database.SqlQuery<ReferenciaForker>("usp_buscar_referencia", referencia).ToList();
            var res = value.FirstOrDefault();
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
        }
    }
}
