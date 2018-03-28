using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using AutoMapper;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class RegistroControlService : ServiceBase<RegistroControlModel, SAX_REGISTRO_CONTROL, RegistroControl>, IRegistroControlService
    {
        public RegistroControlService()
            : this(new RegistroControl())
        {

        }
        public RegistroControlService(RegistroControl ao)
            : base(ao)
        {


        }

        public RegistroControlModel LoadFileData(RegistroControlModel control, List<PartidasModel> excelData)
        {

            var model = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(excelData);
            var firstElement = model.FirstOrDefault();
            control.RC_COD_AREA = "01";
            control.RC_COD_EVENTO   = "";
            control.RC_COD_OPERACION = firstElement.PA_FECHA_CARGA < System.DateTime.Now.Date ? "I" : "M";
            control.RC_COD_PARTIDA = "I01";
            //El lenght de este campo esta incorrecto
            control.RC_COD_USUARIO = "50062048";
            //control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_ESTATUS_LOTE = "1";
            control.RC_TOTAL_REGISTRO = model.Count;
            control.RC_USUARIO_CREACION = firstElement.PA_USUARIO_CREACION;
            control.RC_TOTAL_CREDITO = model.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0 ));
            control.RC_TOTAL_DEBITO = model.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element)); 
            control.RC_TOTAL = model.Count;
            //control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            //control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now;
          
            control.SAX_PARTIDAS = excelData;

            var modelRegistroTo = Mapper.Map<RegistroControlModel, SAX_REGISTRO_CONTROL>(control);
            modelRegistroTo.SAX_PARTIDAS = model;

            return base.Insert(control, true);

        }
    }
}
