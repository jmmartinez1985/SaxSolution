using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace Banistmo.Sax.WebApi.Models
{
    // Models used as parameters to AccountController actions.
    public class ReporteSupervisorModel
    {
        public string LimiteInferior { get; set; }
        public string LimiteSuperior { get; set; }
        public int? AreaOperativa { get; set; }
        public int? Empresa { get; set; }
        public string UsuarioAprobador { get; set; }
        public string UsuarioSupervisor { get; set; }
    }

    public class AprobacionParametrosModel
    {
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
    }
}
