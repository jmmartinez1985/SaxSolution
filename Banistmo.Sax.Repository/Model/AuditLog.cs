//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Banistmo.Sax.Repository.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuditLog
    {
        public System.Guid Id { get; set; }
        public System.Guid GuidGroup { get; set; }
        public string AuditType { get; set; }
        public string TableName { get; set; }
        public string PK { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
