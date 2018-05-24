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
        private readonly ICuentaContableService ctaService;

        private readonly ICentroCostoService centroCostoService;
        private readonly IEmpresaService empresaService;
        private readonly IConceptoCostoService conceptoCostoService;

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
            ctaService = ctaService ?? new CuentaContableService();
            centroCostoService = centroCostoService ?? new CentroCostoService();
            empresaService = empresaService ?? new EmpresaService();
            conceptoCostoService = conceptoCostoService ?? new ConceptoCostoService();

        }

        public RegistroControlService(RegistroControl ao, IFilesProvider provider, IPartidasService partSvc, ICuentaContableService ctaSvc, ICentroCostoService centroCosSvc, IEmpresaService empSvc, IConceptoCostoService cocosSvc)
            : base(ao)
        {
            registroControl = ao ?? new RegistroControl();
            fileProvider = provider ?? new FilesProvider();
            partidaService = partSvc ?? new PartidasService();
            ctaService = ctaSvc ?? new CuentaContableService();
            centroCostoService = centroCosSvc ?? new CentroCostoService();
            empresaService = empSvc ?? new EmpresaService();
            conceptoCostoService = cocosSvc ?? new ConceptoCostoService();
        }

        public RegistroControlContent CreateSinglePartidas(RegistroControlModel control, PartidaManualModel partida, int tipoOperacion)
        {
            int counter = 1;
            var counterRecord = base.Count();
            string dateFormat = "yyyyMMdd";
            var model = new List<SAX_PARTIDAS>();
            var registroContext = new RegistroControlContent();
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();

            var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
            var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
            var cuentas = ctaService.GetAllFlatten<CuentaContableModel>();
            var empresa = empresaService.GetAllFlatten<EmpresaModel>();


            var tipoCarga = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL).ToString();
            var sequence = System.DateTime.Now.Date.ToString(dateFormat) + tipoCarga + counterRecord;

            control.RC_COD_AREA = control.RC_COD_AREA;
            control.RC_COD_EVENTO = partida.PA_EVENTO;
            control.RC_COD_OPERACION = tipoCarga;
            control.RC_COD_PARTIDA = sequence + 1;

            control.RC_USUARIO_CREACION = control.RC_COD_USUARIO;

            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR).ToString();

            var partidaDebito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            //partidaDebito.PA_IMPORTE = decimal.Parse(partida.PA_DEBITO);
            partidaDebito.PA_CTA_CONTABLE = partida.PA_NOMBRE_D;
            //validaCta(partida.PA_NOMBRE_D, ref partidaDebito);
            list.Add(partidaDebito);

            var partidaCredito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            partidaDebito.PA_CTA_CONTABLE = partida.PA_NOMBRE_C;
            //validaCta(partida.PA_NOMBRE_C, ref partidaCredito);
            //partidaDebito.PA_IMPORTE = decimal.Parse(partida.PA_CREDITO);
            list.Add(partidaCredito);



            DateTime today = DateTime.Now;
            var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);

            control.RC_TOTAL_REGISTRO = list.Count;
            control.RC_USUARIO_CREACION = control.RC_USUARIO_CREACION;

            var credito = partida.PA_IMPORTE;
            var debito = partida.PA_IMPORTE;

            control.RC_TOTAL_CREDITO = credito;
            control.RC_TOTAL_DEBITO = debito;
            control.RC_TOTAL = credito + debito;
            control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now;

            var mensaje = string.Empty;
            foreach (var item in list)
            {
                String PA_REFERENCIA = string.Empty;
                try
                {
                    var referenciaEmbedded = item.PA_REFERENCIA;
                    var cuenta = item.PA_CTA_CONTABLE;
                    var importe = item.PA_IMPORTE;
                    var singleCuenta = ctaService.GetSingle(c => (c.CO_CUENTA_CONTABLE + c.CO_COD_AUXILIAR + c.CO_NUM_AUXILIAR) == cuenta);
                    if (singleCuenta.CO_COD_CONCILIA.Equals("S"))
                    {
                        if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe > 0)
                        {
                            item.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + counter.ToString().PadLeft(5, '0');
                        }
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                        {
                            if (registroControl.IsValidReferencia(referenciaEmbedded) == "S")
                                continue;
                            else
                            {
                                mensaje = "La referencia es invalida";
                                throw new Exception();
                            }
                        }
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                        {
                            item.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + counter.ToString().PadLeft(5, '0');
                        }
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe > 0)
                        {
                            if (registroControl.IsValidReferencia(referenciaEmbedded) == "S")
                                continue;
                            else
                            {
                                mensaje = "La referencia es invalida";
                                throw new Exception();
                            }
                        }
                        else
                        {
                            mensaje = "No se cumple con una referencia valida por Naturaleza ni Importe";
                            throw new Exception();
                        }
                        //EXEC SP de VALIDACION
                    }
                    else
                    {
                        PA_REFERENCIA = referenciaEmbedded;
                    }
                }
                catch (Exception e)
                {
                    listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                }
                fileProvider.ValidaReglasCarga(counter, ref list, ref listError, item, 3, centroCostos, conceptoCostos,cuentas,empresa, list);
                counter++;
                counterRecords += 1;
            }
            registroContext.ListPartidas = list;
            registroContext.ListError = listError;
            control.SAX_PARTIDAS = list;

            if (listError.Count == 0)
            {
                var modelRegistroTo = Mapper.Map<RegistroControlModel, SAX_REGISTRO_CONTROL>(control);
                var modelPart = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(list);
                modelRegistroTo.SAX_PARTIDAS = modelPart;
                var registro = registroControl.LoadFileData(modelRegistroTo);
                var returnmodel = Mapper.Map<SAX_REGISTRO_CONTROL, RegistroControlModel>(registro);
            }
            return registroContext;
        }

        public RegistroControlModel LoadFileData(RegistroControlModel control, List<PartidasModel> excelData, int tipoOperacion)
        {

            var counterRecord = base.Count();
            string dateFormat = "yyyyMMdd";
            var model = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(excelData);
            var firstElement = model.FirstOrDefault();
            //var tipoCarga = firstElement.PA_FECHA_CARGA < System.DateTime.Now.Date ? Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL).ToString() : Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA).ToString();
            control.RC_COD_AREA = control.RC_COD_AREA;
            control.RC_COD_EVENTO = "";
            control.RC_COD_OPERACION = tipoOperacion.ToString();
            control.RC_COD_PARTIDA = System.DateTime.Now.Date.ToString(dateFormat) + tipoOperacion + counterRecord + 1;
            //El lenght de este campo esta incorrecto
            control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            //control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR).ToString();
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

        public bool IsValidLoad(DateTime fecha)
        {
            return registroControl.IsValidLoad(fecha);
        }

        public string IsValidReferencia(string referencia)
        {
            return registroControl.IsValidReferencia(referencia);
        }
    }
}
