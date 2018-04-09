using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces
{
    public interface IReporterService
    {
        void CreateReport<T>(List<string[]> header, IEnumerable<T> data, string filename = "report") where T: class;

        byte[] CreateReportBinary<T>(List<string[]> header, IEnumerable<T> data, string filename = "report") where T : class;

        void CreateReport(DataTable table, string filename = "report");

        byte[] CreateReportBinary(DataTable table, string filename = "report");
    }
}
