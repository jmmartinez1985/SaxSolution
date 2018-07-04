using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Banistmo.Sax.WebApi.Models
{
    public class PagingParameterModel
    {
        public string codEnterprise { get; set; }

        public string reference { get; set; }
        public decimal? importe { get; set; }
        public decimal? importeDesde { get; set; }

        public decimal? importeHasta { get; set; }

        public DateTime? trxDateIni { get; set; }

        public DateTime? trxDateFin { get; set; }

        public int? tipoOperacion { get; set; }

        public string lote { get; set; }

        public string idCapturador { get; set; }

        public string ctaAccount { get; set; }

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