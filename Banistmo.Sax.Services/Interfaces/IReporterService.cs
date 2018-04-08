using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Interfaces
{
    public interface IReporterService
    {
        void CreateReport<T>(List<string[]> header, List<T> data) where T: class;
    }
}
