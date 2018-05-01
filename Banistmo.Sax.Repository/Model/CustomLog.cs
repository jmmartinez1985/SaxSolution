using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Repository.Model
{
    public class CustomLog

    {

        public Guid Id { get; set; }

        public Guid GuidGroup { get; set; }

        public string Action { get; set; }

        public string TableName { get; set; }

        public string PrimaryKey { get; set; }

        public string ColumnName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public DateTime Date { get; set; }

        public String UserId { get; set; }

    }
}
