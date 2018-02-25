using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Repository.Implementations.Business
{
    public interface IExcelData
    {
        void LoadBulk(List<ExcelData> data);
    }
}
