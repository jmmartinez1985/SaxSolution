using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Model;
using EntityFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    [Injectable]
    public class ExcelDataImpl : IExcelData
    {
        public void LoadBulk(List<ExcelData> data)
        {
            using (var db = new DBModelEntities())
            {
                EFBatchOperation.For(db, db.ExcelData).InsertAll(data);
            }
        }
    }
}
