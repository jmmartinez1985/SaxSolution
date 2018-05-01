using Banistmo.Sax.Repository.Interfaces;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Repository.Structure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//SA: JMMB
namespace Banistmo.Sax.Repository
{
    public class SaxRepositoryContext : IRepositoryContext
    {
        private const string OBJECT_CONTEXT_KEY = "Banistmo.Sax.Repository.Model";
        public IDbSet<T> GetObjectSet<T>()
            where T : class
        {
            return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY).Set<T>();
        }

        /// <summary>
        /// Returns the active object context
        /// </summary>
        public DbContext ObjectContext
        {
            get
            {
                return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY);
            }
        }

        public int SaveChanges()
        {
            if (ConfigurationManager.AppSettings["auditLog"] == "true")
            {
                var manager = ((IObjectContextAdapter)ObjectContext).ObjectContext.ObjectStateManager;
                try
                {
                    var result = GetLogEntries(manager);
                    foreach (var log in result)
                    {
                        var auditLog = new AuditLog() //fill the AuditLog entity of EF
                        {
                            Id = log.Id,
                            GuidGroup = log.GuidGroup,
                            AuditType = log.Action,
                            TableName = log.TableName,
                            PK = log.PrimaryKey,
                            ColumnName = log.ColumnName,
                            OldValue = log.OldValue,
                            NewValue = log.NewValue,
                            Date = log.Date,
                            UserId = log.UserId.ToString()
                        };
                        this.ObjectContext.Set<AuditLog>().Add(auditLog);
                    }
                }
                catch (Exception ex)
                {
                    //No break app while logging failed
                }
            }
            return this.ObjectContext.SaveChanges();
        }

        public void Terminate()
        {
            ContextManager.SetRepositoryContext(null, OBJECT_CONTEXT_KEY);
        }

        public static List<CustomLog> GetLogEntries(ObjectStateManager entities)
        {
            List<CustomLog> listLogs = new List<CustomLog>();
            var entries = entities.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted);
            var user = HttpContext.Current.User.Identity.Name;
            foreach (var entry in entries)
            {
                var tableName = entry.Entity.GetType().Name;
                if (tableName.Contains("SAX_PARTIDAS"))
                    return new List<CustomLog>();
                var pk = GetPrimaryKeys(entry);
                if (entry.State == EntityState.Added)
                {
                    var newgroup = Guid.NewGuid();
                    var currentEntry = entities.GetObjectStateEntry(entry.EntityKey);
                    var currentValues = currentEntry.CurrentValues;
                    for (var i = 0; i < currentValues.FieldCount; i++)
                    {
                        var propName = currentValues.DataRecordInfo.FieldMetadata[i].FieldType.Name;
                        var newValue = currentValues[propName].ToString();
                        var log = new CustomLog()
                        {
                            Id = Guid.NewGuid(),
                            GuidGroup = newgroup,
                            Action = "I",
                            TableName = tableName,
                            PrimaryKey = pk,
                            ColumnName = propName,
                            OldValue = null,
                            NewValue = newValue,
                            Date = DateTime.Now,
                            UserId = user
                        };
                        listLogs.Add(log);
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    var currentEntry = entities.GetObjectStateEntry(entry.EntityKey);
                    var currentValues = currentEntry.CurrentValues;
                    var originalValues = currentEntry.OriginalValues;
                    var properties = currentEntry.GetModifiedProperties();
                    var updgroup = Guid.NewGuid();
                    foreach (var propName in properties)
                    {
                        var oldValue = originalValues[propName].ToString();
                        var newValue = currentValues[propName].ToString();
                        if (oldValue == newValue) continue;
                        var log = new CustomLog()
                        {
                            Id = Guid.NewGuid(),
                            GuidGroup = updgroup,
                            Action = "M",
                            TableName = tableName,
                            PrimaryKey = pk,
                            ColumnName = propName,
                            OldValue = oldValue,
                            NewValue = newValue,
                            Date = DateTime.Now,
                            UserId = user
                        };
                        listLogs.Add(log);
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var currentEntry = entities.GetObjectStateEntry(entry.EntityKey);
                    var originalValues = currentEntry.OriginalValues;
                    var delgroup = Guid.NewGuid();
                    for (var i = 0; i < originalValues.FieldCount; i++)
                    {
                        var oldValue = originalValues[i].ToString();
                        var log = new CustomLog()
                        {
                            Id = Guid.NewGuid(),
                            GuidGroup = delgroup,
                            Action = "D",
                            TableName = tableName,
                            PrimaryKey = pk,
                            ColumnName = null,
                            OldValue = oldValue,
                            NewValue = null,
                            Date = DateTime.Now,
                            UserId = user
                        };
                        listLogs.Add(log);
                    }
                }
            }
            return listLogs;

        }

        private static string GetPrimaryKeys(ObjectStateEntry entry)
        {
            string pk = string.Empty;
            if (entry.EntityKey == null || entry.EntityKey.EntityKeyValues == null || entry.EntityKey.EntityKeyValues.Length == 0) return "N/A";
            foreach (var keyValue in entry.EntityKey.EntityKeyValues)
            {
                pk += string.Format("{0}={1};", keyValue.Key, keyValue.Value);
            }
            return pk;

        }

    }



}
