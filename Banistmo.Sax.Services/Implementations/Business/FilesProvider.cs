using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class FilesProvider : IFilesProvider
    {
        public List<ExcelDataModel> getDataFrom<T>(T input)
        {
            List<ExcelDataModel> list = new List<ExcelDataModel>();
            var ds = input as DataSet;
            foreach (var item in ds.Tables[0].AsEnumerable().Skip(1))
            {
                list.Add(new ExcelDataModel
                {
                    prop1 = (string) item.Field<string>(0),
                    prop2 = (double)item.Field<double>(1),
                    prop3 =(double) item.Field<double>(2),
                });
            }

            return list;
        }

        public void loadData(List<ExcelDataModel> input)
        {
            throw new NotImplementedException();
        }
    }

  
}
