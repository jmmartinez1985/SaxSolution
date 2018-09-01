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
using System.Web.Configuration;

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
        private IRegistroControlService registroService;
        private IParametroService paramService;
        private IAreaOperativaService areaOperativaService;
        private IEmpresaAreasCentroCostoService empresaAreaCentroCostoSrv;
        private IEmpresaCentroService empresaCentroSrv;

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
            paramService = paramService ?? new ParametroService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            empresaAreaCentroCostoSrv = empresaAreaCentroCostoSrv ?? new EmpresaAreasCentroCostoService();
            empresaCentroSrv = empresaCentroSrv ?? new EmpresaCentroService();
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
            paramService = paramService ?? new ParametroService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            empresaAreaCentroCostoSrv = empresaAreaCentroCostoSrv ?? new EmpresaAreasCentroCostoService();
            empresaCentroSrv = empresaCentroSrv ?? new EmpresaCentroService();
        }

        public RegistroControlContent CreateSinglePartidas(RegistroControlModel control, PartidaManualModel partida, int tipoOperacion)
        {
            int counter = 1;
            string referencia = partida.PA_REFERENCIA;
            string conceptoCosto = partida.PA_CONCEPTO_COSTO;
            partida.PA_CONCEPTO_COSTO = string.Empty;
            partida.PA_REFERENCIA = string.Empty;
            DateTime todays = DateTime.Now.Date;
            var counterRecord = base.Count(c => DbFunctions.TruncateTime(c.RC_FECHA_CREACION) == todays);
            string dateFormat = "yyyyMMdd";
            var model = new List<SAX_PARTIDAS>();
            var registroContext = new RegistroControlContent();
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();

            var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
            //var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
            List<ConceptoCostoModel> conceptoCostos = conceptoCostoService.Query(x => x.CC_ID_CONCEPTO_COSTO == x.CC_ID_CONCEPTO_COSTO).Select(y => new ConceptoCostoModel
            {
                CC_ID_CONCEPTO_COSTO= y.CC_ID_CONCEPTO_COSTO,
                CE_ID_EMPRESA = y.CE_ID_EMPRESA,
                CC_NUM_CONCEPTO = y.CC_NUM_CONCEPTO,
                CC_CUENTA_MAYOR = y.CC_CUENTA_MAYOR,
                CC_ESTATUS = y.CC_ESTATUS               
            }).ToList();
            //var cuentas = ctaService.GetAllFlatten<CuentaContableModel>();
            List<CuentaContableModel> cuentas = ctaService.Query(x => x.CO_ID_CUENTA_CONTABLE == x.CO_ID_CUENTA_CONTABLE).Select(y => new CuentaContableModel
            {
                CO_ID_CUENTA_CONTABLE = y.CO_ID_CUENTA_CONTABLE,
                CE_ID_EMPRESA= y.CE_ID_EMPRESA,
                CO_CUENTA_CONTABLE = y.CO_CUENTA_CONTABLE,
                CO_COD_AUXILIAR = y.CO_COD_AUXILIAR,
                CO_NUM_AUXILIAR = y.CO_NUM_AUXILIAR,
                CA_ID_AREA = y.ca_id_area,
                CO_COD_NATURALEZA=y.CO_COD_NATURALEZA,
                CO_COD_CONCILIA= y.CO_COD_CONCILIA,
                CO_NOM_CUENTA_CONTRA= y.CO_NOM_CUENTA_CONTRA,
                CO_CTA_CONTABLE_CONTRA= y.CO_CTA_CONTABLE_CONTRA,
                CO_ESTATUS= y.CO_ESTATUS
                
            }).ToList();
            var empresa = empresaService.GetAllFlatten<EmpresaModel>();
            var empresaAreaCentro = empresaAreaCentroCostoSrv.GetAll();
            var empresaCentro = empresaCentroSrv.GetAll();
            List<MonedaModel> lstMoneda = monedaService.GetAllFlatten<MonedaModel>();
            CuentaContableModel cuenta_debito = cuentas.Where(x => x.CO_ID_CUENTA_CONTABLE == partida.EV_CUENTA_DEBITO).FirstOrDefault();
            CuentaContableModel cuenta_credito = cuentas.Where(x => x.CO_ID_CUENTA_CONTABLE == partida.EV_CUENTA_CREDITO).FirstOrDefault();
            //fecha Operativa
            DateTime fechaOperativa;
            var param = paramService.GetSingle();
            if (param != null && param.PA_FECHA_PROCESO != null)
                fechaOperativa = param.PA_FECHA_PROCESO.Date;
            else
                fechaOperativa = DateTime.Now.Date;

            string codeOperacion = string.Empty;
            if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL))
                codeOperacion = "I";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA))
                codeOperacion = "D";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
                codeOperacion = "M";


            control.CA_ID_AREA = control.CA_ID_AREA;
            control.RC_COD_EVENTO = partida.PA_EVENTO;
            control.EV_COD_EVENTO = Convert.ToInt16(partida.PA_EVENTO);
            control.RC_COD_OPERACION = tipoOperacion;
            control.RC_COD_PARTIDA = System.DateTime.Now.Date.ToString(dateFormat) + codeOperacion + ((counterRecord + 1).ToString("00000"));
            control.RC_FECHA_APROBACION = null;
            control.RC_FECHA_MOD = null;
            control.RC_USUARIO_CREACION = control.RC_COD_USUARIO;
            control.RC_ESTATUS_LOTE = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            var partidaDebito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            if (cuenta_debito != null && !string.IsNullOrEmpty(cuenta_debito.CO_CUENTA_CONTABLE))
            {
                partidaDebito.PA_CTA_CONTABLE = cuenta_debito.CO_CUENTA_CONTABLE.Trim() + cuenta_debito.CO_COD_AUXILIAR.Trim() + cuenta_debito.CO_NUM_AUXILIAR.Trim();
                if (cuenta_debito.CO_COD_CONCILIA.Equals("1"))
                {
                    partidaDebito.PA_REFERENCIA = partida.REFERENCIA_CUENTA_DEBITO;
                }
            }

            if (cuenta_debito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || cuenta_debito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || cuenta_debito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || cuenta_debito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("32"))
            {
                partidaDebito.PA_CONCEPTO_COSTO = conceptoCosto;
            }
            partidaDebito.PA_CENTRO_COSTO = partida.PA_CENTRO_COSTO;
            partidaDebito.PA_USUARIO_MOD = null;
            partidaDebito.PA_USUARIO_APROB = null;
            partidaDebito.PA_FECHA_MOD = null;
            partidaDebito.PA_FECHA_APROB = null;
            partidaDebito.PA_FECHA_CREACION = DateTime.Now;
            partidaDebito.PA_FECHA_CONCILIA = null;
            partidaDebito.PA_FECHA_ANULACION = null;
            partidaDebito.PA_USUARIO_CREACION = control.RC_COD_USUARIO;
            var credito = (partida.PA_IMPORTE * -1);
            var debito = partida.PA_IMPORTE;

            partidaDebito.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            partidaDebito.PA_IMPORTE = debito;
            partidaDebito.PA_TIPO_CONCILIA = 0;
            partidaDebito.PA_CONTADOR = 1;
            list.Add(partidaDebito);

            var partidaCredito = partida.CustomMapIgnoreICollection<PartidaManualModel, PartidasModel>();
            if (cuenta_credito != null && !string.IsNullOrEmpty(cuenta_credito.CO_CUENTA_CONTABLE))
            {
                partidaCredito.PA_CTA_CONTABLE = cuenta_credito.CO_CUENTA_CONTABLE.Trim() + cuenta_credito.CO_COD_AUXILIAR.Trim() + cuenta_credito.CO_NUM_AUXILIAR.Trim();
                if (cuenta_credito.CO_COD_CONCILIA.Equals("1"))
                {
                    partidaCredito.PA_REFERENCIA = partida.REFERENCIA_CUENTA_CREDITO;
                }
            }
            if (cuenta_credito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("51") || cuenta_credito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("52") || cuenta_credito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("31") || cuenta_credito.CO_CUENTA_CONTABLE.Trim().Substring(0, 2).Equals("32"))
            {
                partidaCredito.PA_CONCEPTO_COSTO = conceptoCosto;
            }
            partidaCredito.PA_CENTRO_COSTO = partida.CENTRO_COSTO_CREDITO;
            partidaCredito.PA_FECHA_MOD = null;
            partidaCredito.PA_FECHA_APROB = null;
            partidaCredito.PA_FECHA_CONCILIA = null;
            partidaCredito.PA_FECHA_ANULACION = null;
            partidaCredito.PA_USUARIO_MOD = null;
            partidaCredito.PA_USUARIO_APROB = null;
            partidaCredito.PA_FECHA_CREACION = DateTime.Now;
            partidaCredito.PA_USUARIO_CREACION = control.RC_COD_USUARIO;
            partidaCredito.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            partidaCredito.PA_TIPO_CONCILIA = 0;
            partidaCredito.PA_IMPORTE = credito;
            partidaCredito.PA_CONTADOR = 2;

            list.Add(partidaCredito);

            DateTime today = DateTime.Now;
            var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);

            control.RC_TOTAL_REGISTRO = list.Count;
            control.RC_USUARIO_CREACION = control.RC_USUARIO_CREACION;

            control.RC_TOTAL_CREDITO = credito;
            control.RC_TOTAL_DEBITO = debito;
            control.RC_TOTAL = credito + debito;
           
            control.RC_FECHA_CREACION = DateTime.Now;
            
            control.RC_FECHA_PROCESO = partida.PA_FECHA_TRX;
            

            var mensaje = string.Empty;

            decimal montoConsolidado = 0;
            UsuarioAreaService usuarioAreaService = new UsuarioAreaService();
            var cuenta = string.Empty;
            registroService = registroService ?? new RegistroControlService();
            var consolidatedReference = partidaService.getConsolidaReferencias(list);
            int codAreaGenerica = Convert.ToInt16(WebConfigurationManager.AppSettings["areaOperativaGenerica"]);
            var areaGenerica = areaOperativaService.GetSingle(x => x.CA_COD_AREA == codAreaGenerica);
           
            foreach (var iteminner in list)
            {
                counter++;
                String PA_REFERENCIA = string.Empty;
                CuentaContableModel singleCuenta = null;
                try
                {
                    var referenciaEmbedded = iteminner.PA_REFERENCIA;
                    if (string.IsNullOrEmpty(iteminner.PA_CTA_CONTABLE))
                    {
                        mensaje = $"La cuenta contable no puede estar en blanco.";
                        throw new CuentaContableVaciaException();
                    }
                    else
                    {
                        iteminner.PA_CTA_CONTABLE = iteminner.PA_CTA_CONTABLE.Trim();
                        if (string.IsNullOrEmpty(iteminner.PA_CTA_CONTABLE))
                        {
                            mensaje = $"La cuenta contable no puede estar en blanco.";
                            throw new CuentaContableException();
                        }
                    }
                    cuenta = iteminner.PA_CTA_CONTABLE.Trim().ToUpper();
                    iteminner.PA_COD_EMPRESA = iteminner.PA_COD_EMPRESA == null ? string.Empty : iteminner.PA_COD_EMPRESA;
                    var importe = iteminner.PA_IMPORTE;
                    var empresaSingle = empresa.FirstOrDefault(x => x.CE_COD_EMPRESA.Trim() == iteminner.PA_COD_EMPRESA.Trim());
                    if (empresaSingle == null)
                    {
                        throw new EmpresaException($"La empresa {iteminner.PA_COD_EMPRESA} no existe en el sistema.");

                    }
                    singleCuenta = cuentas.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim().ToUpper() + c.CO_COD_AUXILIAR.Trim().ToUpper() + c.CO_NUM_AUXILIAR.Trim().ToUpper()) == cuenta && (c.CA_ID_AREA == partida.CA_ID_AREA || c.CA_ID_AREA == areaGenerica.CA_ID_AREA) && c.CE_ID_EMPRESA == empresaSingle.CE_ID_EMPRESA);
                    if (singleCuenta == null)
                    {
                        throw new CuentaContableAreaException($"La cuenta contable {cuenta} no existe en el sistema. Verificar cuenta contable para  empresa y el área indicada.");
                    }
                    var fechaCarga = iteminner.PA_FECHA_CARGA;
                    decimal monto = 0;
                    int tipo_error = 0;
                    if (singleCuenta.CO_COD_CONCILIA.Equals("1"))
                    {
                        if (string.IsNullOrEmpty(singleCuenta.CO_COD_NATURALEZA))
                            throw new CodNaturalezaException("La cuenta contable conciliable y no tiene definida naturaleza dentro del catálogo de cuentas.");
                        if (string.IsNullOrEmpty(singleCuenta.CO_COD_CONCILIA))
                            throw new CodNaturalezaException("La cuenta contable no tiene definida estatus de conciliación dentro del catálogo de cuentas.");
                        if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && !String.IsNullOrEmpty(referenciaEmbedded) /*&& importe > 0*/)
                        {

                            if (String.IsNullOrEmpty(referenciaEmbedded))
                            {
                                mensaje = $"La referencia es requerida, cuenta de naturaleza débito con importe negativo. {referenciaEmbedded}";
                                throw new Exception();
                            }
                            var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                            montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                            var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), iteminner.PA_CENTRO_COSTO, montoConsolidado, ref monto, ref tipo_error);
                            if (!(refval == "S"))
                            {
                                if (tipo_error == 1)
                                {
                                    mensaje = $"La referencia indicada ({referenciaEmbedded}) no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                                    throw new Exception();
                                }
                                else if (tipo_error == 2)
                                {
                                    //No es necesario validar si la referencia se excede porque en carga manual siempre se suma
                                    //mensaje = $"La referencia ({referenciaEmbedded}) excede el monto inicial {monto}.";
                                    //throw new Exception();
                                }
                                else if (tipo_error == 3)
                                {
                                    mensaje = $"La referencia indicada ({referenciaEmbedded}) no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                                    throw new Exception();
                                }
                                else
                                {
                                    mensaje = $"La referencia indicada ({referenciaEmbedded}) no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                                    throw new Exception();
                                }
                               
                            }
                            //if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                            //{
                            //    mensaje = $"El importe es mayor al saldo acumulado por referencia: {referenciaEmbedded}.";
                            //    throw new Exception();
                            //}
                            iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);

                            //if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                            //{
                            //    mensaje = $"Cuenta de naturaleza débito con importe positivo, la referencia tiene que estar en blanco.";
                            //    throw new Exception();
                            //}
                            ////Colocar por asignar
                            //iteminner.PA_REFERENCIA = "";
                            //iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                        }
                        //else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                        //{
                        //    if (String.IsNullOrEmpty(referenciaEmbedded))
                        //    {
                        //        mensaje = $"La referencia es requerida, cuenta de naturaleza débito con importe negativo. {referenciaEmbedded}";
                        //        throw new Exception();
                        //    }
                        //    var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                        //    montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                        //    var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), iteminner.PA_CENTRO_COSTO, montoConsolidado, ref monto, ref tipo_error);
                        //    if (!(refval == "S"))
                        //    {
                        //        if (tipo_error == 1)
                        //        {
                        //            mensaje = $"La referencia indicada no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                        //        }
                        //        else if (tipo_error == 2)
                        //        {
                        //            mensaje = $"La referencia {referenciaEmbedded} excede el monto inicial {monto}.";
                        //        }
                        //        else if (tipo_error == 3)
                        //        {
                        //            mensaje = $"La referencia indicada no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                        //        }
                        //        else
                        //        {
                        //            mensaje = $"La referencia indicada no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                        //        }
                        //        throw new Exception();
                        //    }
                        //    if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                        //    {
                        //        mensaje = $"El importe es mayor al saldo acumulado por referencia: {referenciaEmbedded}.";
                        //        throw new Exception();
                        //    }
                        //    iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                        //}
                        //else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                        //{
                        //    if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                        //    {
                        //        mensaje = $"Cuenta de naturaleza crédito con importe negativo, la referencia tiene que estar en blanco.";
                        //        throw new Exception();
                        //    }
                        //    iteminner.PA_REFERENCIA = "";
                        //    iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                        //}
                        else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && !String.IsNullOrEmpty(referenciaEmbedded)/*&& importe > 0 */)
                        {
                            if (String.IsNullOrEmpty(referenciaEmbedded))
                            {
                                mensaje = $"La referencia es requerida, cuenta de naturaleza crédito con importe positivo. {referenciaEmbedded}";
                                throw new Exception();
                            }
                            var refSummary = consolidatedReference.Where(c => c.Referencia == referenciaEmbedded).FirstOrDefault();
                            montoConsolidado = refSummary == null ? 0 : refSummary.Monto;
                            var refval = registroService.IsValidReferencia(referenciaEmbedded, iteminner.PA_COD_EMPRESA.Trim(), iteminner.PA_COD_MONEDA.Trim(), iteminner.PA_CTA_CONTABLE.Trim(), iteminner.PA_CENTRO_COSTO, montoConsolidado, ref monto, ref tipo_error);
                            if (!(refval == "S"))
                            {
                                if (tipo_error == 1)
                                {
                                    mensaje = $"La referencia indicada ({referenciaEmbedded}) no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                                    throw new Exception();
                                }
                                else if (tipo_error == 2)
                                {
                                   // mensaje = $"La referencia ({referenciaEmbedded}) excede el monto inicial {monto}.";
                                   // throw new Exception();
                                }
                                else if (tipo_error == 3)
                                {
                                    mensaje = $"La referencia indicada ({referenciaEmbedded}) no coincide en el sistema para la empresa, moneda, cuenta, centro de costo indicado en la partida a cargar.";
                                    throw new Exception();
                                }
                                else
                                {
                                    mensaje = $"La referencia ({referenciaEmbedded}) es invalida para los datos definidos en la partida {referenciaEmbedded}.";
                                    throw new Exception();
                                }
                               
                            }
                            //if (Math.Abs(montoConsolidado) > Math.Abs(monto))
                            //{
                            //    mensaje = $"El impote es mayor al saldo acumulado por referencia: {referenciaEmbedded}.";
                            //    throw new Exception();
                            //}
                            iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.MANUAL);
                        } else if ((singleCuenta.CO_COD_NATURALEZA.Equals("C") || singleCuenta.CO_COD_NATURALEZA.Equals("D")) && String.IsNullOrEmpty(referenciaEmbedded)) {

                            iteminner.PA_REFERENCIA = "";
                            iteminner.PA_ORIGEN_REFERENCIA = Convert.ToInt16(BusinessEnumerations.TipoReferencia.AUTOMATICO);
                        }
                        else
                        {
                            mensaje = "No se cumple con una referencia valida por naturaleza ni importe.";
                            throw new Exception();
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(iteminner.PA_REFERENCIA))
                        {
                            mensaje = $"La cuenta no es conciliable, por lo tanto no puede tener referencia.";
                            throw new Exception();
                        }
                        
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
                        mensaje = $"Validar naturaleza de cuenta contable {cuenta}.";
                        listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "Referencia" });
                        mensaje = string.Empty;
                    }
                    if (e is CodConciliaException)
                    {
                        mensaje = $"Validar conciliación de cuenta contable {cuenta}.";
                        listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "Referencia" });
                        mensaje = string.Empty;
                    }
                    if (e is CuentaContableVaciaException)
                    {
                        //Si la cuenta contable viene vacia o con formato incorrecto (Numero y caracteres especiales)
                        //El error se manejo en la lectura del campo
                    }
                    if (singleCuenta == null)
                    {
                        //mensaje = $"No se puede encontrar la cuenta contable {cuenta}.";
                        //listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        mensaje = string.Empty;
                    }
                    if (e is CuentaContableAreaException)
                    {
                        listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = "Cuenta contable" });
                        mensaje = string.Empty;
                    }
                    if (e is EmpresaException)
                    {
                        listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = e.Message, Columna = "Empresa" });
                        mensaje = string.Empty;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mensaje))
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "Referencia" });
                    }
                }
                fileProvider.ValidaReglasCarga(counter, ref list, ref listError, iteminner, Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL), centroCostos, conceptoCostos, cuentas, empresa, list, lstMoneda, fechaOperativa, empresaAreaCentro, partida.CA_ID_AREA, empresaCentro);
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
            DateTime fecha_proceso = this.GetFechaProceso();
            string codeOperacion = string.Empty;
            if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL))
                codeOperacion = "I";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA))
                codeOperacion = "D";
            else if (tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL))
                codeOperacion = "M";
            DateTime todays = DateTime.Now.Date;
            var counterRecord = base.Count(c => DbFunctions.TruncateTime(c.RC_FECHA_CREACION) == todays && c.RC_COD_OPERACION== tipoOperacion);
            string dateFormat = "yyyyMMdd";
            var model = Mapper.Map<List<PartidasModel>, List<SAX_PARTIDAS>>(excelData);
            var firstElement = model.FirstOrDefault();
            //var tipoCarga = firstElement.PA_FECHA_CARGA < System.DateTime.Now.Date ? Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_INICIAL).ToString() : Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA).ToString();
            control.CA_ID_AREA = control.CA_ID_AREA;
            control.RC_ARCHIVO = this.FileName;
            control.RC_COD_OPERACION = tipoOperacion;
            control.RC_COD_PARTIDA = fecha_proceso.Date.ToString(dateFormat) + codeOperacion + ((counterRecord + 1).ToString("00000"));
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
            //control.RC_FECHA_APROBACION = DateTime.Now;
            control.RC_FECHA_CREACION = DateTime.Now;
            //control.RC_FECHA_MOD = DateTime.Now;
            control.RC_FECHA_PROCESO = fecha_proceso.Date;

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

        public string IsValidReferencia(string referencia, string empresa, string moneda, string cuenta_contable,string centro_costo, decimal monto_saldo, ref decimal monto , ref int tipo_error)
        {
            return registroControl.IsValidReferencia(referencia, empresa, moneda, cuenta_contable,centro_costo, monto_saldo, ref monto , ref tipo_error);
        }

        public string FileName { get; set; }

        private DateTime GetFechaProceso()
        {
            ParametroService  paramService = new ParametroService();
            DateTime fechaProceso;
            try
            {
                var param = paramService.GetSingle();

                if (param != null)
                {
                    return fechaProceso = param.PA_FECHA_PROCESO;

                }
                else
                {

                    return DateTime.Now;
                }


            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
    }
}
