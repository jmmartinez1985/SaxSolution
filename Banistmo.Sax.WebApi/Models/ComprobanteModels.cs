using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Banistmo.Sax.WebApi.Models
{
    public class ComprobanteModels
    {
        public DateTime? FechaCreacion { get; set; }
        public string empresaCod { get; set; }
        public int? comprobanteId { get; set; }
        public int? cuentaContableId { get; set; }
        public decimal? importe { get; set; }
        public string referencia { get; set; }
        public int? areaOpe { get; set; }

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