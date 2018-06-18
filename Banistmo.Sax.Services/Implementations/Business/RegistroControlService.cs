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
using System.Data.Entity;

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
        private readonly IMonedaService monedaService;

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
            monedaService = monedaService ?? new MonedaService();
        }

        public RegistroControlService(RegistroControl ao, IFilesProvider provider, IPartidasService partSvc, ICuentaContableService ctaSvc, ICentroCostoService centroCosSvc, IEmpresaService empSvc,
            IConceptoCostoService cocosSvc)
            : base(ao)
        {
            registroControl = ao ?? new RegistroControl();
            fileProvider = provider ?? new FilesProvider();
            partidaService = partSvc ?? new PartidasService();
            ctaService = ctaSvc ?? new CuentaContableService();
            centroCostoService = centroCosSvc ?? new CentroCostoService();
            empresaService = empSvc ?? new EmpresaService();
            conceptoCostoService = cocosSvc ?? new ConceptoCostoService();
            monedaService = monedaService ?? new MonedaService();
        }

        public RegistroControlContent CreateSinglePartidas(RegistroControlModel control, PartidaManualModel partida, int tipoOperacion)
        {
            int counter = 1;
            DateTime todays = DateTime.Now.Date;
            var counterRecord = base.Count(c => DbFunctions.TruncateTime(c.RC_FECHA_CREACION) == todays);
            string dateFormat = "yyyyMMdd";
            string refFormat = "yyyyMMdd";
            var model = new List<SAX_PARTIDAS>();
            var registroContext = new RegistroControlContent();
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();

            var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
            var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
            var cuentas = ctaService.GetAllFlatten<CuentaContableModel>();
            var empresa = empresaService.GetAllFlatten<EmpresaModel>();
            List<MonedaModel> lstMoneda = monedaService.GetAllFlatten<MonedaModel>();
            CuentaContableModel cuenta_debito = cuentas.Where(x => x.CO_ID_CUENTA_CONTABLE == partida.EV_CUENTA_DEBITO).FirstOrDefault();
            CuentaContableModel cuenta_credito = cuentas.Where(x => x.CO_ID_CUENTA_CONTABLE == partida.EV_CUENTA_CREDITO).FirstOrDefault(); 
            string codeOperacion = string.Empty;
            if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL))
                codeOperacion = "I";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA))
                codeOperacion = "D";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
                codeOperacion = "M";

            var sequence = System.DateTime.Now.Date.ToString(dateFormat) + codeOperacion + (counterRecord + 1);

            control.CA_ID_AREA = control.CA_ID_AREA;
            control.RC_COD_EVENTO = partida.PA_EVENTO;
            control.RC_COD_OPERACION = tipoOperacion;
            control.RC_COD_PARTIDA = sequence + 1;

            control.RC_USUARIO_CREACION = control.RC_COD_USUARIO;

            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);

            var partidaDebito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            if(cuenta_debito != null && !string.IsNullOrEmpty(cuenta_debito.CO_CUENTA_CONTABLE))
                partidaDebito.PA_CTA_CONTABLE = cuenta_debito.CO_CUENTA_CONTABLE.Trim()+ cuenta_debito.CO_COD_AUXILIAR.Trim() + cuenta_debito.CO_NUM_AUXILIAR.Trim();
            //partidaDebito.PA_IMPORTE = decimal.Parse(partida.PA_DEBITO);
            //partidaDebito.PA_CTA_CONTABLE = partida.PA_DEBITO.Trim() + partida.PA_NOMBRE_D.Trim();
            //validaCta(partida.PA_NOMBRE_D, ref partidaDebito);
            partidaDebito.PA_FECHA_ANULACION = DateTime.Now;
            partidaDebito.PA_FECHA_CREACION = DateTime.Now;
            partidaDebito.PA_FECHA_CONCILIA = DateTime.Now;
            list.Add(partidaDebito);

            var partidaCredito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            if (cuenta_credito != null && !string.IsNullOrEmpty(cuenta_credito.CO_CUENTA_CONTABLE))
                partidaCredito.PA_CTA_CONTABLE = cuenta_credito.CO_CUENTA_CONTABLE.Trim() + cuenta_credito.CO_COD_AUXILIAR.Trim() + cuenta_credito.CO_NUM_AUXILIAR.Trim();
            //partidaDebito.PA_CTA_CONTABLE = partida.PA_CREDITO.Trim()+partida.PA_NOMBRE_C.Trim();
            //validaCta(partida.PA_NOMBRE_C, ref partidaCredito);
            //partidaDebito.PA_IMPORTE = decimal.Parse(partida.PA_CREDITO);
            partidaCredito.PA_FECHA_ANULACION = DateTime.Now;
            partidaCredito.PA_FECHA_CREACION = DateTime.Now;
            partidaCredito.PA_FECHA_CONCILIA = DateTime.Now;
            list.Add(partidaCredito);


            DateTime today = DateTime.Now;
            var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);

            control.RC_TOTAL_REGISTRO = list.Count;
            control.RC_USUARIO_CREACION = control.RC_USUARIO_CREACION;

            var credito = partida.PA_IMPORTE;
            var debito = (partida.PA_IMPORTE * -1);

            control.RC_TOTAL_CREDITO = credito;
            control.RC_TOTAL_DEBITO = debito;
            control.RC_TOTAL = credito + debito;
            control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now.Date;

            var mensaje = string.Empty;
            decimal monto = 0;

            foreach (var item in list)
            {
                String PA_REFERENCIA = string.Empty;
                try
                {
                    var referenciaEmbedded = item.PA_REFERENCIA;
                    var cuenta = item.PA_CTA_CONTABLE;
                    var importe = item.PA_IMPORTE;
                    var singleCuenta = ctaService.GetSingle(c => (c.CO_CUENTA_CONTABLE.Trim() + c.CO_COD_AUXILIAR.Trim() + c.CO_NUM_AUXILIAR.Trim()) == cuenta.Trim());
                    var fechaCarga = item.PA_FECHA_CARGA;
                    if (fechaCarga == null)
                        throw new Exception("Debe contener una fecha de carga para las partidas.");
                    if (singleCuenta.CO_COD_CONCILIA.Equals("S"))
                    {
                        if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe > 0)
                        {
                            item.PA_REFERENCIA = fechaCarga.Date.ToString(refFormat) + counter.ToString().PadLeft(5, '0');
                        }
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                        {
                            var refval = registroControl.IsValidReferencia(referenciaEmbedded, ref monto);
                            if (!(refval == "S"))
                            {
                                mensaje = $"La referencia es invalida: {referenciaEmbedded}";
                                throw new Exception();
                            }
                            if (importe > monto)
                            {
                                mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}";
                                throw new Exception();
                            }
                        }
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                        {
                            item.PA_REFERENCIA = fechaCarga.Date.ToString(refFormat) + counter.ToString().PadLeft(5, '0');
                        }
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe > 0)
                        {
                            var refval = registroControl.IsValidReferencia(referenciaEmbedded, ref monto);
                            if (!(refval == "S"))
                            {
                                mensaje = $"La referencia es invalida: {referenciaEmbedded}";
                                throw new Exception();
                            }
                            if (importe > monto)
                            {
                                mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}";
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
                fileProvider.ValidaReglasCarga(counter, ref list, ref listError, item,Convert.ToInt16( BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL), centroCostos, conceptoCostos, cuentas, empresa, list, lstMoneda);
                counter++;
                counterRecords += 1;
            }

            //Validaciones globales por Saldos Balanceados por Moneda y Empresa
            var monedaError = new List<EmpresaMonedaValidationModel>();
            bool validaSaldoMoneda = partidaService.isSaldoValidoMonedaEmpresa(list, ref monedaError);
            if (!validaSaldoMoneda)
            {
                monedaError.ForEach(x =>
                {
                    listError.Add(new MessageErrorPartida { Columna = "global", Linea = counter++, Mensaje = $"Partida desbalanceada en la empresa: {x.DescripcionEmpresa} y moneda {x.DescripcionMoneda}" });
                });
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
            string codeOperacion = string.Empty;
            if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL))
                codeOperacion = "I";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA))
                codeOperacion = "D";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
                codeOperacion = "M";
            DateTime todays = DateTime.Now.Date;
            var counterRecord = base.Count(c => DbFunctions.TruncateTime(c.RC_FECHA_CREACION) == todays);
            string dateFormat = "yyyyMMdd";
            var model = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(excelData);
            var firstElement = model.FirstOrDefault();
            //var tipoCarga = firstElement.PA_FECHA_CARGA < System.DateTime.Now.Date ? Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL).ToString() : Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA).ToString();
            control.CA_ID_AREA = control.CA_ID_AREA;
            control.RC_ARCHIVO = this.FileName;
            control.RC_COD_OPERACION = tipoOperacion;
            control.RC_COD_PARTIDA = System.DateTime.Now.Date.ToString(dateFormat) + codeOperacion + ((counterRecord + 1).ToString("0000"));
            //El lenght de este campo esta incorrecto
            control.RC_COD_USUARIO = control.RC_USUARIO_CREACION;
            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            control.RC_TOTAL_REGISTRO = model.Count;
            control.RC_USUARIO_CREACION = firstElement.PA_USUARIO_CREACION;

            var credito = model.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
            var debito = model.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
            if ((credito + debito) > 0 | (credito + debito) < 0)
            {
                throw new Exception("Carga no balanceada en debitos o creditos.");
            }
            control.RC_TOTAL_CREDITO = credito;
            control.RC_TOTAL_DEBITO = debito;

            control.RC_TOTAL = credito + debito;
            control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = DateTime.Now.Date;

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

        public string IsValidReferencia(string referencia, ref decimal monto)
        {
            return registroControl.IsValidReferencia(referencia, ref monto);
        }

        public bool removeRegistro(int registro)
        {
            return registroControl.removeRegistro(registro);
        }

        public bool AprobarRegistro(int registro, string userName)
        {
            return registroControl.AprobarRegistro(registro, userName);
        }

        public bool RechazarRegistro(int registro, string userName)
        {
            return registroControl.RechazarRegistro(registro, userName);
        }

        public string IsValidReferencia(string referencia, string empresa, string moneda, string cuenta_contable, decimal monto_saldo, ref decimal monto)
        {
            return registroControl.IsValidReferencia(referencia, empresa, moneda, cuenta_contable, monto_saldo, ref monto);
        }

        public string FileName { get; set; }
    }
}
