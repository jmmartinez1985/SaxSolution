using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces.Business
{
 
    public interface IFilesProvider
    {
        List<ExcelDataModel> getDataFrom<T>(T input);

        void loadData (List<ExcelDataModel> input);


    }
}
