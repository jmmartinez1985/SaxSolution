using Banistmo.Sax.Services.Interfaces.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Repository.Implementations.Business;
using AutoMapper;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Common;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class ExcelDataService : IExcelDataService
    {
        private readonly IExcelData excelService;
        public ExcelDataService(IExcelData excel)
        {
            excelService = excel;
        }

        public void LoadBulk(List<ExcelDataModel> data)
        { 
            var model = Mapper.Map<List<ExcelDataModel>, List<ExcelData>>(data);
            excelService.LoadBulk(model);


        }
    }
}
