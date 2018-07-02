using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Implementations.Rules;
using Banistmo.Sax.Services.Implementations.Rules.FileInput;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Services.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Web.Configuration;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class FilesProvider : IFilesProvider
    {
        private readonly IPartidasService partidaService;
        private readonly ICentroCostoService centroCostoService;
        private readonly IEmpresaService empresaService;
        private readonly IMonedaService monedaService;
        private readonly IConceptoCostoService conceptoCostoService;
        private readonly ICuentaContableService contableService;
        private IRegistroControlService registroService;
        private IAreaOperativaService areaOperativaService;

        const string dateFormat = "MMddyyyy";
        const string refFormat = "yyyyMMdd";


        public FilesProvider(
            IPartidasService partidaSvc,
            ICentroCostoService centroCostoSvc,
            IEmpresaService empresaSvc,
            IConceptoCostoService conceptoCostoSvc,
            ICuentaContableService contableSvc,
            IMonedaService monedaSvc,
            IAreaOperativaService areaSvc
            )
        {
            partidaService = partidaSvc;
            centroCostoService = centroCostoSvc;
            empresaService = empresaSvc;
            conceptoCostoService = conceptoCostoSvc;
            contableService = contableSvc;
            monedaService = monedaSvc;
            areaOperativaService = areaSvc;
        }

        public FilesProvider()
        {
            partidaService = partidaService ?? new PartidasService();
            centroCostoService = centroCostoService ?? new CentroCostoService();
            empresaService = empresaService ?? new EmpresaService();
            conceptoCostoService = conceptoCostoService ?? new ConceptoCostoService();
            contableService = contableService ?? new CuentaContableService();
            monedaService = monedaService ?? new MonedaService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            //registroService = registroService ?? new RegistroControlService();

        }



        public void ValidaReglasCarga(int counter, ref List<PartidasModel> list, ref List<MessageErrorPartida> listError, PartidasModel partidaModel, int carga, List<CentroCostoModel> centroCostos, List<ConceptoCostoModel> conCostos, List<CuentaContableModel> ctaContables, List<EmpresaModel> empresa, List<PartidasModel> partidas, List<MonedaModel> listaMoneda)
        {
            var context = new ValidationContext(partidaModel, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(partidaModel, context, validationResults, true);
            if (!isValid)
            {
                foreach (var error in validationResults)
                {
                    listError.Add(new MessageErrorPartida() { Linea = counter + 1, Mensaje = error.ErrorMessage });
                }
            }
            SaldoCuentaValidationModel saldoCuenta = new SaldoCuentaValidationModel() { PartidasList = partidas, CuentasList = ctaContables };
            ValidationList rules = new ValidationList();
            if (carga == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA) || carga == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
            {
                //masiva
                rules.Add(new FTSFOValidation(partidaModel, null));
                rules.Add(new FTFCIFOValidation(partidaModel, null));
                rules.Add(new COValidation(partidaModel, ctaContables));
                rules.Add(new CEValidation(partidaModel, empresa));
                rules.Add(new CCValidations(partidaModel, centroCostos));
                rules.Add(new CONCEPCOSValidation(partidaModel, conCostos));
                rules.Add(new IImporteValidation(partidaModel, null));
                rules.Add(new DIFCTAValidation(partidaModel, null));
                //rules.Add(new FINCTAValidation(partidaModel, null));
                rules.Add(new CONCEPTO5152Validation(partidaModel, conCostos, empresa));
                rules.Add(new SALCTAValidation(partidaModel, saldoCuenta, partidas));
                rules.Add(new MONEDAValidation(partidaModel, listaMoneda));
                rules.Add(new EXPLICValidation(partidaModel, null));

            }
            else
            {
                //Inicial
                rules.Add(new FTSFOValidation(partidaModel, null));
                //rules.Add(new FTFCIFOValidation(partidaModel, null));
                rules.Add(new COValidation(partidaModel, ctaContables));
                rules.Add(new CEValidation(partidaModel, empresa));
                rules.Add(new CCValidations(partidaModel, centroCostos));
                rules.Add(new CONCEPCOSValidation(partidaModel, conCostos));
                rules.Add(new IImporteValidation(partidaModel, null));
                rules.Add(new DIFCTAValidation(partidaModel, null));
                rules.Add(new FINCTAValidation(partidaModel, null));
                rules.Add(new SALCTAValidation(partidaModel, saldoCuenta, partidas));
            }
            if (rules.IsValid && isValid) {
                if (carga != Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
                    list.Add(partidaModel);
            } 
            else if (!rules.IsValid)
            {
                foreach (var error in rules)
                {
                    if(!error.IsValid)
                    listError.Add(new MessageErrorPartida() { Linea = counter + 1, Mensaje = error.Message, Columna = error.Columna});
                }
            }
        }

        public PartidasContent cargaInicial<T>(T input, string userId, int areaId)
        {
            int counter = 1;
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();
            try
            {
                //Counting number of record already exist.
                DateTime today = DateTime.Now;
                IFormatProvider culture = new CultureInfo("en-US", true);
                string mensaje = string.Empty;
                int codAreaGenerica = Convert.ToInt16(WebConfigurationManager.AppSettings["areaOperativaGenerica"]);
                //var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);
                var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
                var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
                var cuentas = contableService.GetAllFlatten<CuentaContableModel>();
                var empresa = empresaService.GetAllFlatten<EmpresaModel>();
                var areaGenerica = areaOperativaService.GetSingle(x=>x.CA_COD_AREA== codAreaGenerica);
                List<MonedaModel> lstMoneda = monedaService.GetAllFlatten<MonedaModel>();
                registroService = registroService ?? new RegistroControlService();

                var ds = input as DataSet;

                var finalList = FillDataToList(ds, userId, ref listError);

                var consolidatedReference = partidaService.getConsolidaReferencias(finalList);
                decimal montoConsolidado = 0;

                var reorder = finalList.OrderBy(c => c.PA_FECHA_TRX).GroupBy(c => c.PA_FECHA_TRX);
                foreach (var item in reorder)
                {
                    int internalcounter = 1;
                    foreach (var iteminner in item)
                    {
                        mensaje = string.Empty;
                        String PA_REFERENCIA = string.Empty;
                        CuentaContableModel singleCuenta = null;
                        string cuentaCruda = String.Empty;
                        try
                        {
                            var referenciaEmbedded = iteminner.PA_REFERENCIA;
                            if (string.IsNullOrEmpty(iteminner.PA_CTA_CONTABLE))
                            {
                                mensaje = $"La cuenta contable no puede estar en blanco";
                                throw new CuentaContableException();
                            }

                            if (!string.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                            {
                                mensaje = $"No se pueden colocar referencias";
                                throw new ReferenciaInicialException();
                            }
                            cuentaCruda = iteminner.PA_CTA_CONTABLE.Trim().ToUpper();
                            iteminner.PA_COD_EMPRESA = iteminner.PA_COD_EMPRESA == null ? string.Empty : iteminner.PA_COD_EMPRESA;
                            var importe = iteminner.PA_IMPORTE;
                            var empresaSingle = empresa.FirstOrDefault(x => x.CE_COD_EMPRESA.Trim() == iteminner.PA_COD_EMPRESA.Trim());
                            if (empresaSingle == null) {
                                throw new EmpresaException($"La empresa {iteminner.PA_COD_EMPRESA} no existe en el sistema.");
                                
                            }
                            singleCuenta = cuentas.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim().ToUpper() + c.CO_COD_AUXILIAR.Trim().ToUpper() + c.CO_NUM_AUXILIAR.Trim().ToUpper()) == cuentaCruda && (c.CA_ID_AREA == areaId || c.CA_ID_AREA == areaGenerica.CA_ID_AREA) && c.CE_ID_EMPRESA == empresaSingle.CE_ID_EMPRESA);
                            if (singleCuenta == null)
                            {
                                throw new CuentaContableAreaException($"La cuenta contable{cuentaCruda} no existe en el sistema. Favor verificar nombre de la cuenta, la empresa y el area seleccionada.");

                            }
                            var fechaTrx = iteminner.PA_FECHA_TRX;
                            decimal monto = 0;
                            //if (fechaTrx == null)
                            //    throw new Exception("Debe contener una fecha de transaccion para las partidas.");

                            if (singleCuenta.CO_COD_CONCILIA.Equals("1"))
                            {
                                if (string.IsNullOrEmpty(singleCuenta.CO_COD_NATURALEZA))
                                    throw new CodNaturalezaException("Código de naturaleza es inválido.");
                                if (string.IsNullOrEmpty(singleCuenta.CO_COD_CONCILIA))
                                    throw new CodNaturalezaException("Código de concilia es inválido.");
                                if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe > 0)
                                {
                                    if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                                    {
                                        mensaje = $"La referencia tiene que estar en blanco ";
                                        throw new Exception();
                                    }
                                    iteminner.PA_REFERENCIA = "";
                                    //iteminner.PA_REFERENCIA = fechaTrx.Date.ToString(refFormat) + internalcounter.ToString().PadLeft(5, '0');
                                    iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                                {

                                    //mensaje = $"La referencia tiene que estar en blanco. Cuenta debito con importe negativo";
                                    
                                        throw new ReferenciaException($"Cuenta debito {cuentaCruda}  con importe negativo");
                                    //ROMPO
                                    //var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                                    //montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                                    //var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), montoConsolidado, ref monto);
                                    //if (!(refval == "S"))
                                    //{
                                    //    mensaje = $"La referencia es invalida: {referenciaEmbedded}";
                                    //    throw new Exception();
                                    //}
                                    //if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                                    //{
                                    //    mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}";
                                    //    throw new Exception();
                                    //}
                                    //iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                                    //throw new Exception("La partida no cumple con la naturaleza de la cuenta.");

                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                                {
                                    if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                                    {
                                        mensaje = $"La referencia tiene que estar en blanco. Cuenta credito {cuentaCruda} con importe negativo.";
                                        throw new Exception();
                                    }
                                    iteminner.PA_REFERENCIA = "";
                                    //iteminner.PA_REFERENCIA = fechaTrx.Date.ToString(refFormat) + internalcounter.ToString().PadLeft(5, '0');
                                    iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe > 0)
                                {

                                    //ROMPO
                                    
                                        throw new ReferenciaException($"Cuenta credito {cuentaCruda} con importe positivo.");
                                    //if (String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                                    //{
                                    //    mensaje = $"La referencia no puede estar en blanco para la cuenta " + cuentaCruda;
                                    //    throw new Exception();
                                    //}
                                    //var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                                    //montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                                    //var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), montoConsolidado, ref monto);
                                    //if (!(refval == "S"))
                                    //{
                                    //    mensaje = $"La referencia es invalida: {referenciaEmbedded}";
                                    //    throw new Exception();
                                    //}
                                    //if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                                    //{
                                    //    mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}";
                                    //    throw new Exception();
                                    //}
                                    //iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                                    //throw new ReferenciaException("La partida no cumple con la naturaleza de la cuenta.");
                                }
                                else
                                {
                                    mensaje = "No se cumple con una referencia valida por Naturaleza ni Importe";
                                    throw new Exception();
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(referenciaEmbedded))
                                    referenciaEmbedded = "NOCONCILIA";
                                PA_REFERENCIA = referenciaEmbedded;
                                iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                            }
                        }
                        catch (Exception e)
                        {
                    
                            if (e is CuentaContableException)
                            {
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "Cuenta Contable" });
                                mensaje = string.Empty;
                            }
                            if (e is CodNaturalezaException)
                            {
                                mensaje = $"Validar naturaleza de cuenta contable {cuentaCruda}.";
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "REFERENCIA" });
                                mensaje = string.Empty;
                            }
                            if (e is CodConciliaException)
                            {
                                mensaje = $"Validar conciliación de cuenta contable {cuentaCruda}.";
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = " REFERENCIA" });
                                mensaje = string.Empty;
                            }

                            if (e is ReferenciaException)
                            {
                                mensaje = $"Validar conciliación de cuenta contable {cuentaCruda}.";
                                
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = " REFERENCIA" });
                                mensaje = string.Empty;
                            }

                            if (e is ReferenciaInicialException)
                            {
                                mensaje = $"No se pueden colocar referencias en carga inicial.";

                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = " REFERENCIA" });
                                mensaje = string.Empty;
                            }
                            if (e is CuentaContableAreaException) {
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = " Cuenta Contable" });
                                mensaje = string.Empty;
                            }
                            if (e is EmpresaException)
                            {
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = " Empresa" });
                                mensaje = string.Empty;
                            }
                            if (singleCuenta == null)
                            {
                                mensaje = $"No se puede encontrar la cuenta contable {cuentaCruda} para calcular la referencia.";
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = " REFERENCIA" });
                                mensaje = string.Empty;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(mensaje))
                                listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "REFERENCIA" });
                                mensaje = string.Empty;
                            }

                        }
                        ValidaReglasCarga(counter, ref list, ref listError, iteminner, 2, centroCostos, conceptoCostos, cuentas, empresa, finalList, lstMoneda);
                        counter += 1;
                        internalcounter += 1;
                    }
                }

                //Validaciones globales por Saldos Balanceados por Moneda y Empresa
                var monedaError = new List<EmpresaMonedaValidationModel>();
                bool validaSaldoMoneda = partidaService.isSaldoValidoMonedaEmpresa(finalList, ref monedaError);
                if (!validaSaldoMoneda)
                {
                    monedaError.ForEach(x =>
                    {
                        listError.Add(new MessageErrorPartida { Columna = "global", Linea = counter++, Mensaje = $"Partida desbalanceada en la empresa: {x.DescripcionEmpresa} y moneda {x.DescripcionMoneda}" });
                    });
                }
                partidas.ListPartidas = list;
                partidas.ListError = listError;
                return partidas;
            }
            catch (Exception ex)
            {
                throw new Exception($"El archivo es invalido, por favor revise la linea {counter}");
            }
        }

        public PartidasContent cargaMasiva<T>(T input, string userId,  int areaId)
        {
            int counter = 1;
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();
            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                //string dateFormat = "MMddyyyy";
                //Counting number of record already exist.
                DateTime today = DateTime.Now;
                string mensaje = string.Empty;
                //var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);
                int codAreaGenerica = Convert.ToInt16(WebConfigurationManager.AppSettings["areaOperativaGenerica"]);
                var areaGenerica = areaOperativaService.GetSingle(x => x.CA_COD_AREA == codAreaGenerica);
                var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
                var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
                var cuentas = contableService.GetAllFlatten<CuentaContableModel>();
                var empresa = empresaService.GetAllFlatten<EmpresaModel>();
                List<MonedaModel> lstMoneda = monedaService.GetAllFlatten<MonedaModel>();

                registroService = registroService ?? new RegistroControlService();

                var ds = input as DataSet;
                var cuenta = string.Empty;
                var finalList = FillDataToList(ds, userId, ref listError);

                var consolidatedReference = partidaService.getConsolidaReferencias(finalList);
                decimal montoConsolidado = 0;

                foreach (var iteminner in finalList)
                {
                    String PA_REFERENCIA = string.Empty;
                    CuentaContableModel singleCuenta = null;
                    try
                    {
                        var referenciaEmbedded = iteminner.PA_REFERENCIA;
                        if (string.IsNullOrEmpty(iteminner.PA_CTA_CONTABLE)) {
                            mensaje = $"La cuenta contable no puede estar en blanco";
                            throw new CuentaContableException();
                        }
                        cuenta = iteminner.PA_CTA_CONTABLE.Trim().ToUpper();
                        iteminner.PA_COD_EMPRESA = iteminner.PA_COD_EMPRESA == null ? string.Empty : iteminner.PA_COD_EMPRESA;
                        var importe = iteminner.PA_IMPORTE;
                        var empresaSingle = empresa.FirstOrDefault(x => x.CE_COD_EMPRESA.Trim() == iteminner.PA_COD_EMPRESA.Trim());
                        if (empresaSingle == null)
                        {
                            throw new EmpresaException($"La empresa {iteminner.PA_COD_EMPRESA} no existe en el sistema.");

                        }
                        singleCuenta = cuentas.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim().ToUpper() + c.CO_COD_AUXILIAR.Trim().ToUpper() + c.CO_NUM_AUXILIAR.Trim().ToUpper()) == cuenta && (c.CA_ID_AREA == areaId || c.CA_ID_AREA == areaGenerica.CA_ID_AREA) && c.CE_ID_EMPRESA == empresaSingle.CE_ID_EMPRESA);
                        if (singleCuenta == null)
                        {
                            throw new CuentaContableAreaException($"La cuenta contable{cuenta} no existe en el sistema. Favor verificar nombre de la cuenta, la empresa y el area seleccionada.");
                        }

                        var fechaCarga = iteminner.PA_FECHA_CARGA;
                        //if (fechaCarga == null)
                        //    throw new Exception("Debe contener una fecha de carga para las partidas.");
                        decimal monto = 0;
                        if (singleCuenta.CO_COD_CONCILIA.Equals("1"))
                        {
                            if (string.IsNullOrEmpty(singleCuenta.CO_COD_NATURALEZA))
                                throw new CodNaturalezaException("La cuenta contable no tiene definida naturaleza dentro del catalogo de cuentas.");
                            if (string.IsNullOrEmpty(singleCuenta.CO_COD_CONCILIA))
                                throw new CodNaturalezaException("La cuenta contable no tiene definida estatus de conciliación dentro del catalogo de cuentas.");
                            if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe > 0)
                            {
                                if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                                {
                                    mensaje = $"Cuenta de naturaleza debito con importe positivo, la referencia tiene que estar en blanco";
                                    throw new Exception();
                                }
                                //Colocar por asignar
                                iteminner.PA_REFERENCIA = "";
                                iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                                //iteminner.PA_REFERENCIA = fechaCarga.ToString(refFormat) + counter.ToString().PadLeft(5, '0');
                            }
                            else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                            {
                                if (String.IsNullOrEmpty(referenciaEmbedded))
                                {
                                    mensaje = $"La referencia es requerida , cuenta de naturaleza debito con importe negativo. {referenciaEmbedded}";
                                    throw new Exception();
                                }
                                var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                                montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                                var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), montoConsolidado, ref monto);
                                if (!(refval == "S"))
                                {
                                    mensaje = $"La referencia es invalida, cuenta de naturaleza debito con importe negativo. {referenciaEmbedded}";
                                    throw new Exception();
                                }
                                if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                                {
                                    mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}";
                                    throw new Exception();
                                }
                                iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                            }
                            else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                            {
                                if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                                {
                                    mensaje = $"Cuenta de naturaleza credito con importe negativo, la referencia tiene que estar en blanco";
                                    throw new Exception();
                                }
                                iteminner.PA_REFERENCIA = "";
                                iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                                //iteminner.PA_REFERENCIA = fechaCarga.Date.ToString(refFormat) + counter.ToString().PadLeft(5, '0');
                            }
                            else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe > 0)
                            {
                                if (String.IsNullOrEmpty(referenciaEmbedded))
                                {
                                    mensaje = $"La referencia es requerida , cuenta de naturaleza credito con importe positivo. {referenciaEmbedded}";
                                    throw new Exception();
                                }
                                var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                                montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                                var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), montoConsolidado, ref monto);
                                if (!(refval == "S"))
                                {
                                    mensaje = $"La referencia es invalida, cuenta de naturaleza credito con importe positivo. {referenciaEmbedded}";
                                    throw new Exception();
                                }
                                if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                                {
                                    mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}";
                                    throw new Exception();
                                }
                                iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
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
                            if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                            {
                                mensaje = $"la cuenta no es conciliable, por lo tanto no puede tener referencia ";
                                throw new Exception();
                            }
                            if (string.IsNullOrEmpty(referenciaEmbedded))
                                referenciaEmbedded = "NOCONCILIA";
                            PA_REFERENCIA = referenciaEmbedded;
                            iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                        }
                    }
                    catch (Exception e)
                    {
                        

                        if (e is CuentaContableException)
                        {
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "Cuenta Contable" });
                        }

                        if (e is CodNaturalezaException)
                        {
                            mensaje = $"Validar naturaleza de cuenta contable {cuenta}.";
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        }
                        if (e is CodConciliaException)
                        {
                            mensaje = $"Validar conciliación de cuenta contable {cuenta}.";
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        }
                        if (singleCuenta == null)
                        {
                            mensaje = $"No se puede encontrar la cuenta contable {cuenta} para calcular la referencia.";
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        }
                        if (e is CuentaContableAreaException)
                        {
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = " Cuenta Contable" });
                            mensaje = string.Empty;
                        }
                        if (e is EmpresaException)
                        {
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = " Empresa" });
                            mensaje = string.Empty;
                        }
                        else
                        {

                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        }
                    }
                    ValidaReglasCarga(counter, ref list, ref listError, iteminner, Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA), centroCostos, conceptoCostos, cuentas, empresa, finalList, lstMoneda);
                    counter += 1;
                    //counterRecords += 1;

                }
                //Validaciones globales por Saldos Balanceados por Moneda y Empresa
                var monedaError = new List<EmpresaMonedaValidationModel>();
                bool validaSaldoMoneda = partidaService.isSaldoValidoMonedaEmpresa(finalList, ref monedaError);
                if (validaSaldoMoneda)
                {
                    monedaError.ForEach(x =>
                    {
                        listError.Add(new MessageErrorPartida { Columna = "global", Linea = counter++, Mensaje = $"Partida desbalanceada en la empresa: {x.DescripcionEmpresa} y moneda {x.DescripcionMoneda}" });
                    });
                }

                partidas.ListPartidas = list;
                partidas.ListError = listError;
                return partidas;
            }
            catch (Exception ex)
            {
                throw new Exception($"El archivo es invalido, por favor revise la linea {counter}");
            }
        }

        private List<PartidasModel> FillDataToList(DataSet excelData, string userId, ref List<MessageErrorPartida> listError)
        {
            List<PartidasModel> listaPartidas = new List<PartidasModel>();
            int counter = 1;

            IFormatProvider culture = new CultureInfo("en-US", true);
            var ds = excelData as DataSet;
            int linea = 1;//Inicia en uno por la cabecera del excel.
            foreach (var item in ds.Tables[0].AsEnumerable().Skip(1).AsParallel())
            {
                linea++;
                var partidaModel = new PartidasModel();
                string mensaje = "No  cumple con el formato requerido";
                var referencia = string.Empty;
                try
                {
                    #region crear variables
                    string PA_COD_EMPRESA = string.Empty;
                    DateTime PA_FECHA_CARGA = DateTime.Now;
                    DateTime PA_FECHA_TRX = DateTime.Now;
                    String PA_CTA_CONTABLE = string.Empty;
                    String PA_CENTRO_COSTO = string.Empty;
                    String PA_COD_MONEDA = string.Empty;
                    decimal PA_IMPORTE = 0;
                    String PA_REFERENCIA = string.Empty;
                    String PA_EXPLICACION = string.Empty;
                    String PA_PLAN_ACCION = string.Empty;
                    String PA_CONCEPTO_COSTO = string.Empty;
                    String PA_CAMPO_1 = string.Empty;
                    String PA_CAMPO_2 = string.Empty;
                    String PA_CAMPO_3 = string.Empty;
                    String PA_CAMPO_4 = string.Empty;
                    String PA_CAMPO_5 = string.Empty;
                    String PA_CAMPO_6 = string.Empty;
                    String PA_CAMPO_7 = string.Empty;
                    String PA_CAMPO_8 = string.Empty;
                    String PA_CAMPO_9 = string.Empty;
                    String PA_CAMPO_10 = string.Empty;
                    String PA_CAMPO_11 = string.Empty;
                    String PA_CAMPO_12 = string.Empty;
                    String PA_CAMPO_13 = string.Empty;
                    String PA_CAMPO_14 = string.Empty;
                    String PA_CAMPO_15 = string.Empty;
                    String PA_CAMPO_16 = string.Empty;
                    String PA_CAMPO_17 = string.Empty;
                    String PA_CAMPO_18 = string.Empty;
                    String PA_CAMPO_19 = string.Empty;
                    String PA_CAMPO_20 = string.Empty;
                    String PA_CAMPO_21 = string.Empty;
                    String PA_CAMPO_22 = string.Empty;
                    String PA_CAMPO_23 = string.Empty;
                    String PA_CAMPO_24 = string.Empty;
                    String PA_CAMPO_25 = string.Empty;
                    String PA_CAMPO_26 = string.Empty;
                    String PA_CAMPO_27 = string.Empty;
                    String PA_CAMPO_28 = string.Empty;
                    String PA_CAMPO_29 = string.Empty;
                    String PA_CAMPO_30 = string.Empty;
                    String PA_CAMPO_31 = string.Empty;
                    String PA_CAMPO_32 = string.Empty;
                    String PA_CAMPO_33 = string.Empty;
                    String PA_CAMPO_34 = string.Empty;
                    String PA_CAMPO_35 = string.Empty;
                    String PA_CAMPO_36 = string.Empty;
                    String PA_CAMPO_37 = string.Empty;
                    String PA_CAMPO_38 = string.Empty;
                    String PA_CAMPO_39 = string.Empty;
                    String PA_CAMPO_40 = string.Empty;
                    String PA_CAMPO_41 = string.Empty;
                    String PA_CAMPO_42 = string.Empty;
                    String PA_CAMPO_43 = string.Empty;
                    String PA_CAMPO_44 = string.Empty;
                    String PA_CAMPO_45 = string.Empty;
                    String PA_CAMPO_46 = string.Empty;
                    String PA_CAMPO_47 = string.Empty;
                    String PA_CAMPO_48 = string.Empty;
                    String PA_CAMPO_49 = string.Empty;
                    String PA_CAMPO_50 = string.Empty;

                    #endregion

                    #region leer variables
                    try { PA_COD_EMPRESA = (String)item.Field<String>(0) == null ? "" : item.Field<String>(0); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_COD_EMPRESA" }); }
                    try { PA_FECHA_CARGA = DateTime.ParseExact(item.Field<String>(1), dateFormat, culture); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_FECHA_CARGA" }); }
                    try { PA_FECHA_TRX = DateTime.ParseExact(item.Field<String>(2), dateFormat, culture); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_FECHA_TRX" }); }
                    try { PA_CTA_CONTABLE = (String)item.Field<String>(3) == null ? "" : item.Field<String>(3); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CTA_CONTABLE" }); }
                    try { PA_CENTRO_COSTO = (String)item.Field<String>(4) == null ? "" : item.Field<String>(4); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CENTRO_COSTO" }); }
                    try { PA_COD_MONEDA = (String)item.Field<String>(5) == null ? "" : item.Field<String>(5); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_COD_MONEDA" }); }
                    try { PA_IMPORTE = (Decimal)item.Field<Double>(6); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_IMPORTE" }); }
                    try { PA_REFERENCIA = (String)item.Field<String>(7) == null ? "" : item.Field<String>(7); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_REFERENCIA" }); }
                    try { PA_EXPLICACION = (String)item.Field<String>(8) == null ? "" : item.Field<String>(8); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_EXPLICACION" }); }
                    try { PA_PLAN_ACCION = (String)item.Field<String>(9) == null ? "" : item.Field<String>(9).Truncate(699); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_PLAN_ACCION" }); }
                    try { PA_CONCEPTO_COSTO = (String)item.Field<String>(10) == null ? "" : item.Field<String>(10); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CONCEPTO_COSTO" }); }
                    try { PA_CAMPO_1 = (String)item.Field<String>(11) == null ? "" : item.Field<String>(11); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_1" }); }
                    try { PA_CAMPO_2 = (String)item.Field<String>(12) == null ? "" : item.Field<String>(12); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_2" }); }
                    try { PA_CAMPO_3 = (String)item.Field<String>(13) == null ? "" : item.Field<String>(13); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_3" }); }
                    try { PA_CAMPO_4 = (String)item.Field<String>(14) == null ? "" : item.Field<String>(14); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_4" }); }
                    try { PA_CAMPO_5 = (String)item.Field<String>(15) == null ? "" : item.Field<String>(15); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_5" }); }
                    try { PA_CAMPO_6 = (String)item.Field<String>(16) == null ? "" : item.Field<String>(16); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_6" }); }
                    try { PA_CAMPO_7 = (String)item.Field<String>(17) == null ? "" : item.Field<String>(17); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_7" }); }
                    try { PA_CAMPO_8 = (String)item.Field<String>(18) == null ? "" : item.Field<String>(18); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_8" }); }
                    try { PA_CAMPO_9 = (String)item.Field<String>(19) == null ? "" : item.Field<String>(19); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_9" }); }
                    try { PA_CAMPO_10 = (String)item.Field<String>(20) == null ? "" : item.Field<String>(20); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_10" }); }
                    try { PA_CAMPO_11 = (String)item.Field<String>(21) == null ? "" : item.Field<String>(21); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_11" }); }
                    try { PA_CAMPO_12 = (String)item.Field<String>(22) == null ? "" : item.Field<String>(22); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_12" }); }
                    try { PA_CAMPO_13 = (String)item.Field<String>(23) == null ? "" : item.Field<String>(23); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_13" }); }
                    try { PA_CAMPO_14 = (String)item.Field<String>(24) == null ? "" : item.Field<String>(24); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_14" }); }
                    try { PA_CAMPO_15 = (String)item.Field<String>(25) == null ? "" : item.Field<String>(25); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_15" }); }
                    try { PA_CAMPO_16 = (String)item.Field<String>(26) == null ? "" : item.Field<String>(26); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_16" }); }
                    try { PA_CAMPO_17 = (String)item.Field<String>(27) == null ? "" : item.Field<String>(27); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_17" }); }
                    try { PA_CAMPO_18 = (String)item.Field<String>(28) == null ? "" : item.Field<String>(28); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_18" }); }
                    try { PA_CAMPO_19 = (String)item.Field<String>(29) == null ? "" : item.Field<String>(29); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_19" }); }
                    try { PA_CAMPO_20 = (String)item.Field<String>(30) == null ? "" : item.Field<String>(30); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_20" }); }
                    try { PA_CAMPO_21 = (String)item.Field<String>(31) == null ? "" : item.Field<String>(31); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_21" }); }
                    try { PA_CAMPO_22 = (String)item.Field<String>(32) == null ? "" : item.Field<String>(32); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_22" }); }
                    try { PA_CAMPO_23 = (String)item.Field<String>(33) == null ? "" : item.Field<String>(33); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_23" }); }
                    try { PA_CAMPO_24 = (String)item.Field<String>(34) == null ? "" : item.Field<String>(34); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_24" }); }
                    try { PA_CAMPO_25 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(35); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_25" }); }
                    try { PA_CAMPO_26 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(36); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_26" }); }
                    try { PA_CAMPO_27 = (String)item.Field<String>(37) == null ? "" : item.Field<String>(37); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_27" }); }
                    try { PA_CAMPO_28 = (String)item.Field<String>(38) == null ? "" : item.Field<String>(38); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_28" }); }
                    try { PA_CAMPO_29 = (String)item.Field<String>(39) == null ? "" : item.Field<String>(39); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_29" }); }
                    try { PA_CAMPO_30 = (String)item.Field<String>(40) == null ? "" : item.Field<String>(40); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_30" }); }
                    try { PA_CAMPO_31 = (String)item.Field<String>(41) == null ? "" : item.Field<String>(41); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_31" }); }
                    try { PA_CAMPO_32 = (String)item.Field<String>(42) == null ? "" : item.Field<String>(42); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_32" }); }
                    try { PA_CAMPO_33 = (String)item.Field<String>(43) == null ? "" : item.Field<String>(43); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_33" }); }
                    try { PA_CAMPO_34 = (String)item.Field<String>(44) == null ? "" : item.Field<String>(44); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_34" }); }
                    try { PA_CAMPO_35 = (String)item.Field<String>(45) == null ? "" : item.Field<String>(45); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_35" }); }
                    try { PA_CAMPO_36 = (String)item.Field<String>(46) == null ? "" : item.Field<String>(46); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_36" }); }
                    try { PA_CAMPO_37 = (String)item.Field<String>(47) == null ? "" : item.Field<String>(47); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_37" }); }
                    try { PA_CAMPO_38 = (String)item.Field<String>(48) == null ? "" : item.Field<String>(48); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_38" }); }
                    try { PA_CAMPO_39 = (String)item.Field<String>(49) == null ? "" : item.Field<String>(49); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_39" }); }
                    try { PA_CAMPO_40 = (String)item.Field<String>(50) == null ? "" : item.Field<String>(50); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_40" }); }
                    try { PA_CAMPO_41 = (String)item.Field<String>(51) == null ? "" : item.Field<String>(51); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_41" }); }
                    try { PA_CAMPO_42 = (String)item.Field<String>(52) == null ? "" : item.Field<String>(52); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_42" }); }
                    try { PA_CAMPO_43 = (String)item.Field<String>(53) == null ? "" : item.Field<String>(53); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_43" }); }
                    try { PA_CAMPO_44 = (String)item.Field<String>(54) == null ? "" : item.Field<String>(54); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_44" }); }
                    try { PA_CAMPO_45 = (String)item.Field<String>(55) == null ? "" : item.Field<String>(55); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_45" }); }
                    try { PA_CAMPO_46 = (String)item.Field<String>(56) == null ? "" : item.Field<String>(56); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_46" }); }
                    try { PA_CAMPO_47 = (String)item.Field<String>(57) == null ? "" : item.Field<String>(57); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_47" }); }
                    try { PA_CAMPO_48 = (String)item.Field<String>(58) == null ? "" : item.Field<String>(58); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_48" }); }
                    try { PA_CAMPO_49 = (String)item.Field<String>(59) == null ? "" : item.Field<String>(59); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_49" }); }
                    try { PA_CAMPO_50 = (String)item.Field<String>(60) == null ? "" : item.Field<String>(60); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_50" }); }
                    #endregion

                    #region Set partida
                    partidaModel = new PartidasModel
                    {
                        PA_CONTADOR = counter,
                        RC_REGISTRO_CONTROL = 0,
                        PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR),
                        PA_COD_EMPRESA = PA_COD_EMPRESA,
                        PA_FECHA_CARGA = PA_FECHA_CARGA,
                        PA_FECHA_TRX = PA_FECHA_TRX,
                        PA_CTA_CONTABLE = PA_CTA_CONTABLE,
                        PA_CENTRO_COSTO = PA_CENTRO_COSTO,
                        PA_COD_MONEDA = PA_COD_MONEDA,
                        PA_IMPORTE = PA_IMPORTE,
                        PA_REFERENCIA = PA_REFERENCIA,
                        PA_EXPLICACION = PA_EXPLICACION,
                        PA_PLAN_ACCION = PA_PLAN_ACCION,
                        PA_CONCEPTO_COSTO = PA_CONCEPTO_COSTO,
                        PA_CAMPO_1 = PA_CAMPO_1,
                        PA_CAMPO_2 = PA_CAMPO_2,
                        PA_CAMPO_3 = PA_CAMPO_3,
                        PA_CAMPO_4 = PA_CAMPO_4,
                        PA_CAMPO_5 = PA_CAMPO_5,
                        PA_CAMPO_6 = PA_CAMPO_6,
                        PA_CAMPO_7 = PA_CAMPO_7,
                        PA_CAMPO_8 = PA_CAMPO_8,
                        PA_CAMPO_9 = PA_CAMPO_9,
                        PA_CAMPO_10 = PA_CAMPO_10,
                        PA_CAMPO_11 = PA_CAMPO_11,
                        PA_CAMPO_12 = PA_CAMPO_12,
                        PA_CAMPO_13 = PA_CAMPO_13,
                        PA_CAMPO_14 = PA_CAMPO_14,
                        PA_CAMPO_15 = PA_CAMPO_15,
                        PA_CAMPO_16 = PA_CAMPO_16,
                        PA_CAMPO_17 = PA_CAMPO_17,
                        PA_CAMPO_18 = PA_CAMPO_18,
                        PA_CAMPO_19 = PA_CAMPO_19,
                        PA_CAMPO_20 = PA_CAMPO_20,
                        PA_CAMPO_21 = PA_CAMPO_21,
                        PA_CAMPO_22 = PA_CAMPO_22,
                        PA_CAMPO_23 = PA_CAMPO_23,
                        PA_CAMPO_24 = PA_CAMPO_24,
                        PA_CAMPO_25 = PA_CAMPO_25,
                        PA_CAMPO_26 = PA_CAMPO_26,
                        PA_CAMPO_27 = PA_CAMPO_27,
                        PA_CAMPO_28 = PA_CAMPO_28,
                        PA_CAMPO_29 = PA_CAMPO_29,
                        PA_CAMPO_30 = PA_CAMPO_30,
                        PA_CAMPO_31 = PA_CAMPO_31,
                        PA_CAMPO_32 = PA_CAMPO_32,
                        PA_CAMPO_33 = PA_CAMPO_33,
                        PA_CAMPO_34 = PA_CAMPO_34,
                        PA_CAMPO_35 = PA_CAMPO_35,
                        PA_CAMPO_36 = PA_CAMPO_36,
                        PA_CAMPO_37 = PA_CAMPO_37,
                        PA_CAMPO_38 = PA_CAMPO_38,
                        PA_CAMPO_39 = PA_CAMPO_39,
                        PA_CAMPO_40 = PA_CAMPO_40,
                        PA_CAMPO_41 = PA_CAMPO_41,
                        PA_CAMPO_42 = PA_CAMPO_42,
                        PA_CAMPO_43 = PA_CAMPO_43,
                        PA_CAMPO_44 = PA_CAMPO_44,
                        PA_CAMPO_45 = PA_CAMPO_45,
                        PA_CAMPO_46 = PA_CAMPO_46,
                        PA_CAMPO_47 = PA_CAMPO_47,
                        PA_CAMPO_48 = PA_CAMPO_48,
                        PA_CAMPO_49 = PA_CAMPO_49,
                        PA_CAMPO_50 = PA_CAMPO_50,
                        PA_USUARIO_CREACION = userId,
                        PA_FECHA_CREACION = DateTime.Now
                    };
                    counter++;

                    listaPartidas.Add(partidaModel);
                    #endregion
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }
            }




            return listaPartidas;

        }

    }


}
