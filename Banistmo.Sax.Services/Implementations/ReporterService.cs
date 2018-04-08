using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations
{
    [Injectable]
    public class ReporterService : IReporterService
    {
        public void CreateReport<T>(List<string[]> header, List<T> data) where T : class
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                var wks = excel.Workbook.Worksheets.Add("Info Report");
                if (header.Count == 1)
                {
                    //string headerRange = "A1:" + Char.ConvertFromUtf32(header[0].Length + 64) + "1";
                    //wks.Cells[headerRange].LoadFromArrays(header);
                    for (int i = 0; i < header.Count(); i++)
                    {
                        wks.Cells[1, i + 1].Value = header[i];
                    }
                }
                else
                {
                    var t = typeof(T);
                    var Headings = t.GetProperties();
                    for (int i = 0; i < Headings.Count(); i++)
                    {
                        wks.Cells[1, i + 1].Value = Headings[i].Name;
                    }
                }
                if (data.Count() > 0)
                {
                    wks.Cells["A2"].LoadFromCollection(data);
                }
                var reportPath = ConfigurationManager.AppSettings["reportFolder"].ToString();
                FileInfo excelFile = new FileInfo(reportPath + "report.xlsx");
                excel.SaveAs(excelFile);
            }
        }
    }
}
