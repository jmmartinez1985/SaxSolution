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
using System.Data;
using OfficeOpenXml.Table;

namespace Banistmo.Sax.Services.Implementations
{
    [Injectable]
    public class ReporterService : IReporterService
    {
        public void CreateReport(DataTable table, string filename = "report")
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                var wks = excel.Workbook.Worksheets.Add("Info Report");
                wks.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Light10);
                var reportPath = ConfigurationManager.AppSettings["reportFolder"].ToString();
                FileInfo excelFile = new FileInfo(reportPath + $"{filename}.xlsx");
                excel.SaveAs(excelFile);
            }
        }

        public void CreateReport<T>(List<string[]> header, IEnumerable<T> data, string filename) where T : class
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                var wks = excel.Workbook.Worksheets.Add("Info Report");
                if (header.Count == 1)
                {
                    string headerRange = "A1:" + Char.ConvertFromUtf32(header[0].Length + 64) + "1";
                    wks.Cells[headerRange].LoadFromArrays(header);
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
                FileInfo excelFile = new FileInfo(reportPath + $"{filename}.xlsx");
                excel.SaveAs(excelFile);
            }
        }

        public byte[] CreateReportBinary(DataTable table, string filename = "report")
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                var wks = excel.Workbook.Worksheets.Add("Info Report");
                wks.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Light10);
                return excel.GetAsByteArray();
            }
        }

        public byte[] CreateReportBinary<T>(List<string[]> header, IEnumerable<T> data, string filename = "report") where T : class
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
                FileInfo excelFile = new FileInfo(reportPath + $"{filename}.xlsx");
                return excel.GetAsByteArray();
            }
        }
    }
}
