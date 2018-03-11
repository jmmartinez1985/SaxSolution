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
        { }

        public void LoadFileData(RegistroControlModel control, List<PartidasModel> excelData)
        {

            var model = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(excelData);
            var firstElement = model.FirstOrDefault();
            control.RC_COD_AREA = "01";
            control.RC_COD_EVENTO   = "01";
            control.RC_COD_OPERACION = "I";
            control.RC_COD_PARTIDA = "I01";
            control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_ESTATUS_LOTE = "1";
            control.RC_TOTAL_REGISTRO = model.Count;
            control.RC_USUARIO_CREACION = firstElement.PA_USUARIO_CREACION;
            control.RC_TOTAL_CREDITO = 0;
            control.RC_TOTAL_DEBITO = 0;
            control.RC_TOTAL = 0;
            control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now;
            control.SAX_PARTIDAS = excelData;

            var modelRegistroTo = Mapper.Map<RegistroControlModel, SAX_REGISTRO_CONTROL>(control);
            modelRegistroTo.SAX_PARTIDAS = model;

            base.Insert(control);

        }
    }
}
