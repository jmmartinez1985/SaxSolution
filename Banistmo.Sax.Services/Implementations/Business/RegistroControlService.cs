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
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Helpers;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class RegistroControlService : ServiceBase<RegistroControlModel, SAX_REGISTRO_CONTROL, RegistroControl>, IRegistroControlService
    {

        private readonly IRegistroControl registroControl;
        private readonly IFilesProvider fileProvider;
        private readonly IPartidasService partidaService;

        public RegistroControlService()
            : this(new RegistroControl())
        {
        }
        public RegistroControlService(RegistroControl ao)
            : base(ao)
        {
            registroControl = ao;
            fileProvider = fileProvider ?? new FilesProvider();
            partidaService = partidaService ?? new PartidasService();
        }

        public RegistroControlService(RegistroControl ao, IFilesProvider provider, IPartidasService partSvc)
            : base(ao)
        {
            registroControl = ao ?? new RegistroControl();
            fileProvider = provider ?? new FilesProvider();
            partidaService = partSvc ?? new PartidasService();
        }

        public RegistroControlContent CreateSinglePartidas(RegistroControlModel control, PartidaManualModel partida)
        {
            int counter = 1;
            var counterRecord = base.Count();
            string dateFormat = "yyyyMMdd";
            var model = new List<SAX_PARTIDAS>();
            var registroContext = new RegistroControlContent();
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();

            var tipoCarga = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL).ToString();
            control.RC_COD_AREA = control.RC_COD_AREA;
            control.RC_COD_EVENTO = partida.PA_EVENTO;
            control.RC_COD_OPERACION = tipoCarga;
            control.RC_COD_PARTIDA = System.DateTime.Now.Date.ToString(dateFormat) + tipoCarga + counterRecord + 1;

            control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CREADO).ToString();

            var partidaDebito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            list.Add(partidaDebito);
            var partidaCredito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            list.Add(partidaCredito);

            DateTime today = DateTime.Now;
            var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);

            control.RC_TOTAL_REGISTRO = list.Count;
            control.RC_USUARIO_CREACION = control.RC_USUARIO_CREACION;

            var credito = decimal.Parse( partida.PA_CREDITO);
            var debito = decimal.Parse(partida.PA_DEBITO);

            control.RC_TOTAL_CREDITO = credito;
            control.RC_TOTAL_DEBITO = debito;
            control.RC_TOTAL = credito + debito;
            control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now;
            foreach (var item in list)
            {
                fileProvider.ValidateInput(counter, ref list, ref listError, item);
                counter++;
                counterRecords += 1;
            }
            registroContext.ListPartidas = list;
            registroContext.ListError = listError;
            control.SAX_PARTIDAS = list;

            if(listError.Count > 0)
            {
                var modelRegistroTo = Mapper.Map<RegistroControlModel, SAX_REGISTRO_CONTROL>(control);
                var modelPart = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(list);
                modelRegistroTo.SAX_PARTIDAS = modelPart;
                var registro = registroControl.LoadFileData(modelRegistroTo);
                var returnmodel = Mapper.Map<SAX_REGISTRO_CONTROL, RegistroControlModel>(registro);
            }
            return registroContext;
        }

        public RegistroControlModel LoadFileData(RegistroControlModel control, List<PartidasModel> excelData)
        {

            var counterRecord = base.Count();
            string dateFormat = "yyyyMMdd";
            var model = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(excelData);
            var firstElement = model.FirstOrDefault();
            var tipoCarga = firstElement.PA_FECHA_CARGA < System.DateTime.Now.Date ? Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL).ToString() : Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA).ToString();
            control.RC_COD_AREA = control.RC_COD_AREA;
            control.RC_COD_EVENTO = "";
            control.RC_COD_OPERACION = tipoCarga;
            control.RC_COD_PARTIDA = System.DateTime.Now.Date.ToString(dateFormat) + tipoCarga + counterRecord + 1;
            //El lenght de este campo esta incorrecto
            control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            //control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CREADO).ToString();
            control.RC_TOTAL_REGISTRO = model.Count;
            control.RC_USUARIO_CREACION = firstElement.PA_USUARIO_CREACION;

            var credito = model.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
            var debito = model.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));

            control.RC_TOTAL_CREDITO = credito;
            control.RC_TOTAL_DEBITO = debito;

            control.RC_TOTAL = credito + debito;
            control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now;

            control.SAX_PARTIDAS = excelData;

            var modelRegistroTo = Mapper.Map<RegistroControlModel, SAX_REGISTRO_CONTROL>(control);
            modelRegistroTo.SAX_PARTIDAS = model;

            var registro = registroControl.LoadFileData(modelRegistroTo);
            var returnmodel = Mapper.Map<SAX_REGISTRO_CONTROL, RegistroControlModel>(registro);

            return returnmodel;

        }
    }
}
