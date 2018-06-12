using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class ParameterReportePartidaModel
    {
        public int? tipoReporte { get; set; }
        public DateTime? fechaCargaIni { get; set; }
        public DateTime? fechaCargaFin { get; set; }
        public DateTime? fechaTransaccionIni { get; set; }
        public DateTime? fechaTransaccionFin { get; set; }
        public string cuentaContable { get; set; }
        public string  referencia { get; set; }
        public string usuarioCreador { get; set; }
        public int? areaOperativa { get; set; }

        const int maxPageSize = 20;
        public int pageNumber { get; set; } = 1;
        public int _pageSize { get; set; } = 10;
        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}