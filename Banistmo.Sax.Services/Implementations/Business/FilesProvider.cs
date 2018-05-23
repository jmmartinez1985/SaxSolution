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

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class FilesProvider : IFilesProvider
    {
        private readonly IPartidasService partidaService;
        private readonly ICentroCostoService centroCostoService;
        private readonly IEmpresaService empresaService;
        private readonly IConceptoCostoService conceptoCostoService;
        private readonly ICuentaContableService contableService;
        private IRegistroControlService registroService;




        public FilesProvider(
            IPartidasService partidaSvc,
            ICentroCostoService centroCostoSvc,
            IEmpresaService empresaSvc,
            IConceptoCostoService conceptoCostoSvc,
            ICuentaContableService contableSvc
            )
        {
            partidaService = partidaSvc;
            centroCostoService = centroCostoSvc;
            empresaService = empresaSvc;
            conceptoCostoService = conceptoCostoSvc;
            contableService = contableSvc;
        }

        public FilesProvider()
        {
            partidaService = partidaService ?? new PartidasService();
            centroCostoService = centroCostoService ?? new CentroCostoService();
            empresaService = empresaService ?? new EmpresaService();
            conceptoCostoService = conceptoCostoService ?? new ConceptoCostoService();
            contableService = contableService ?? new CuentaContableService();
            registroService = registroService ?? new RegistroControlService();

        }

        public PartidasContent cargaMasiva<T>(T input, string userId)
        {
            int counter = 1;
            List<PartidasModel> list = new List<PartidasModel>();
            PartidasContent partidas = new PartidasContent();
            List<MessageErrorPartida> listError = new List<MessageErrorPartida>();
            try
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                string dateFormat = "MMddyyyy";
                //Counting number of record already exist.
                DateTime today = DateTime.Now;
                //var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);

                var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
                var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
                var cuentas = contableService.GetAllFlatten<CuentaContableModel>();
                var empresa = empresaService.GetAllFlatten<EmpresaModel>();

                registroService = registroService ?? new RegistroControlService();

                var ds = input as DataSet;
                int linea = 2;//Inicia en uno por la cabecera del excel.
                foreach (var item in ds.Tables[0].AsEnumerable().Skip(1).AsParallel())
                {
                    linea++;
                    var partidaModel = new PartidasModel();
                    string mensaje = "No  cumple con el formato requerido";
                    var referencia = string.Empty;
                    try
                    {
                        try { string PA_COD_EMPRESA = (String)item.Field<String>(0) == null ? "" : item.Field<String>(0); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_COD_EMPRESA" }); }
                        try { DateTime PA_FECHA_CARGA = DateTime.ParseExact(item.Field<String>(1), dateFormat, culture); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_FECHA_CARGA" }); }
                        try { DateTime PA_FECHA_TRX = DateTime.ParseExact(item.Field<String>(2), dateFormat, culture); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_FECHA_TRX" }); }
                        try { String PA_CTA_CONTABLE = (String)item.Field<String>(3) == null ? "" : item.Field<String>(3); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CTA_CONTABLE" }); }
                        try { String PA_CENTRO_COSTO = (String)item.Field<String>(4) == null ? "" : item.Field<String>(4); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CENTRO_COSTO" }); }
                        try { String PA_COD_MONEDA = (String)item.Field<String>(5) == null ? "" : item.Field<String>(5); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_COD_MONEDA" }); }
                        try { Double PA_IMPORTE = (Double)item.Field<Double>(6); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_IMPORTE" }); }
                        String PA_REFERENCIA = string.Empty;
                        CuentaContableModel singleCuenta = null;
                        try
                        {
                            var referenciaEmbedded = (String)item.Field<String>(7);
                            var cuenta = (String)item.Field<String>(3) == null ? "" : item.Field<String>(3);
                            var importe = (Double)item.Field<Double>(6);
                            singleCuenta = cuentas.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE + c.CO_COD_AUXILIAR + c.CO_NUM_AUXILIAR) == cuenta);
                            if (singleCuenta.CO_COD_CONCILIA.Equals("S"))
                            {
                                if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe > 0)
                                {
                                    referencia = System.DateTime.Now.Date.ToString(dateFormat) + linea.ToString().PadLeft(5, '0');
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                                {
                                    if(registroService.IsValidReferencia(referenciaEmbedded) == "S")
                                        referencia = referenciaEmbedded;
                                    else
                                    {
                                        mensaje = "La referencia es invalida";
                                        throw new Exception();
                                    }
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                                {
                                    referencia = System.DateTime.Now.Date.ToString(dateFormat) + linea.ToString().PadLeft(5, '0');
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe > 0)
                                {
                                    if (registroService.IsValidReferencia(referenciaEmbedded) == "S")
                                        referencia = referenciaEmbedded;
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
                            }
                            else
                            {
                                PA_REFERENCIA = referenciaEmbedded;
                            }
                        }
                        catch (Exception e)
                        {
                            if (singleCuenta == null)
                                mensaje = "Cuenta contable para calculo de referencia no existe. Validar cuenta.";
                            listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        }
                        try { String PA_EXPLICACION = (String)item.Field<String>(8) == null ? "" : item.Field<String>(8); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_EXPLICACION" }); }
                        try { String PA_PLAN_ACCION = (String)item.Field<String>(9) == null ? "" : item.Field<String>(9).Truncate(699); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_PLAN_ACCION" }); }
                        try { String PA_CONCEPTO_COSTO = (String)item.Field<String>(10) == null ? "" : item.Field<String>(10); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CONCEPTO_COSTO" }); }
                        try { String PA_CAMPO_1 = (String)item.Field<String>(11) == null ? "" : item.Field<String>(11); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_1" }); }
                        try { String PA_CAMPO_2 = (String)item.Field<String>(12) == null ? "" : item.Field<String>(12); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_2" }); }
                        try { String PA_CAMPO_3 = (String)item.Field<String>(13) == null ? "" : item.Field<String>(13); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_3" }); }
                        try { String PA_CAMPO_4 = (String)item.Field<String>(14) == null ? "" : item.Field<String>(14); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_4" }); }
                        try { String PA_CAMPO_5 = (String)item.Field<String>(15) == null ? "" : item.Field<String>(15); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_5" }); }
                        try { String PA_CAMPO_6 = (String)item.Field<String>(16) == null ? "" : item.Field<String>(16); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_6" }); }
                        try { String PA_CAMPO_7 = (String)item.Field<String>(17) == null ? "" : item.Field<String>(17); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_7" }); }
                        try { String PA_CAMPO_8 = (String)item.Field<String>(18) == null ? "" : item.Field<String>(18); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_8" }); }
                        try { String PA_CAMPO_9 = (String)item.Field<String>(19) == null ? "" : item.Field<String>(19); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_9" }); }
                        try { String PA_CAMPO_10 = (String)item.Field<String>(20) == null ? "" : item.Field<String>(20); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_10" }); }
                        try { String PA_CAMPO_11 = (String)item.Field<String>(21) == null ? "" : item.Field<String>(21); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_11" }); }
                        try { String PA_CAMPO_12 = (String)item.Field<String>(22) == null ? "" : item.Field<String>(22); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_12" }); }
                        try { String PA_CAMPO_13 = (String)item.Field<String>(23) == null ? "" : item.Field<String>(23); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_13" }); }
                        try { String PA_CAMPO_14 = (String)item.Field<String>(24) == null ? "" : item.Field<String>(24); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_14" }); }
                        try { String PA_CAMPO_15 = (String)item.Field<String>(25) == null ? "" : item.Field<String>(25); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_15" }); }
                        try { String PA_CAMPO_16 = (String)item.Field<String>(26) == null ? "" : item.Field<String>(26); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_16" }); }
                        try { String PA_CAMPO_17 = (String)item.Field<String>(27) == null ? "" : item.Field<String>(27); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_17" }); }
                        try { String PA_CAMPO_18 = (String)item.Field<String>(28) == null ? "" : item.Field<String>(28); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_18" }); }
                        try { String PA_CAMPO_19 = (String)item.Field<String>(29) == null ? "" : item.Field<String>(29); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_19" }); }
                        try { String PA_CAMPO_20 = (String)item.Field<String>(30) == null ? "" : item.Field<String>(30); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_20" }); }
                        try { String PA_CAMPO_21 = (String)item.Field<String>(31) == null ? "" : item.Field<String>(31); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_21" }); }
                        try { String PA_CAMPO_22 = (String)item.Field<String>(32) == null ? "" : item.Field<String>(32); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_22" }); }
                        try { String PA_CAMPO_23 = (String)item.Field<String>(33) == null ? "" : item.Field<String>(33); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_23" }); }
                        try { String PA_CAMPO_24 = (String)item.Field<String>(34) == null ? "" : item.Field<String>(34); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_24" }); }
                        try { String PA_CAMPO_25 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(35); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_25" }); }
                        try { String PA_CAMPO_26 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(36); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_26" }); }
                        try { String PA_CAMPO_27 = (String)item.Field<String>(37) == null ? "" : item.Field<String>(37); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_27" }); }
                        try { String PA_CAMPO_28 = (String)item.Field<String>(38) == null ? "" : item.Field<String>(38); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_28" }); }
                        try { String PA_CAMPO_29 = (String)item.Field<String>(39) == null ? "" : item.Field<String>(39); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_29" }); }
                        try { String PA_CAMPO_30 = (String)item.Field<String>(40) == null ? "" : item.Field<String>(40); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_30" }); }
                        try { String PA_CAMPO_31 = (String)item.Field<String>(41) == null ? "" : item.Field<String>(41); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_31" }); }
                        try { String PA_CAMPO_32 = (String)item.Field<String>(42) == null ? "" : item.Field<String>(42); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_32" }); }
                        try { String PA_CAMPO_33 = (String)item.Field<String>(43) == null ? "" : item.Field<String>(43); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_33" }); }
                        try { String PA_CAMPO_34 = (String)item.Field<String>(44) == null ? "" : item.Field<String>(44); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_34" }); }
                        try { String PA_CAMPO_35 = (String)item.Field<String>(45) == null ? "" : item.Field<String>(45); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_35" }); }
                        try { String PA_CAMPO_36 = (String)item.Field<String>(46) == null ? "" : item.Field<String>(46); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_36" }); }
                        try { String PA_CAMPO_37 = (String)item.Field<String>(47) == null ? "" : item.Field<String>(47); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_37" }); }
                        try { String PA_CAMPO_38 = (String)item.Field<String>(48) == null ? "" : item.Field<String>(48); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_38" }); }
                        try { String PA_CAMPO_39 = (String)item.Field<String>(49) == null ? "" : item.Field<String>(49); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_39" }); }
                        try { String PA_CAMPO_40 = (String)item.Field<String>(50) == null ? "" : item.Field<String>(50); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_40" }); }
                        try { String PA_CAMPO_41 = (String)item.Field<String>(51) == null ? "" : item.Field<String>(51); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_41" }); }
                        try { String PA_CAMPO_42 = (String)item.Field<String>(52) == null ? "" : item.Field<String>(52); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_42" }); }
                        try { String PA_CAMPO_43 = (String)item.Field<String>(53) == null ? "" : item.Field<String>(53); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_43" }); }
                        try { String PA_CAMPO_44 = (String)item.Field<String>(54) == null ? "" : item.Field<String>(54); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_44" }); }
                        try { String PA_CAMPO_45 = (String)item.Field<String>(55) == null ? "" : item.Field<String>(55); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_45" }); }
                        try { String PA_CAMPO_46 = (String)item.Field<String>(56) == null ? "" : item.Field<String>(56); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_46" }); }
                        try { String PA_CAMPO_47 = (String)item.Field<String>(57) == null ? "" : item.Field<String>(57); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_47" }); }
                        try { String PA_CAMPO_48 = (String)item.Field<String>(58) == null ? "" : item.Field<String>(58); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_48" }); }
                        try { String PA_CAMPO_49 = (String)item.Field<String>(59) == null ? "" : item.Field<String>(59); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_49" }); }
                        try { String PA_CAMPO_50 = (String)item.Field<String>(60) == null ? "" : item.Field<String>(60); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_50" }); }

                        #region Set partida
                        partidaModel = new PartidasModel
                        {
                            PA_CONTADOR = counter,
                            RC_REGISTRO_CONTROL = 0,
                            PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CREADO),
                            PA_COD_EMPRESA = (String)item.Field<String>(0) == null ? "" : item.Field<String>(0),
                            PA_FECHA_CARGA = DateTime.ParseExact(item.Field<String>(1), dateFormat, culture),
                            PA_FECHA_TRX = DateTime.ParseExact(item.Field<String>(2), dateFormat, culture),
                            PA_CTA_CONTABLE = (String)item.Field<String>(3) == null ? "" : item.Field<String>(3),
                            PA_CENTRO_COSTO = (String)item.Field<String>(4) == null ? "" : item.Field<String>(4),
                            PA_COD_MONEDA = (String)item.Field<String>(5) == null ? "" : item.Field<String>(5),
                            PA_IMPORTE = decimal.Parse(item.Field<Double>(6).ToString()),
                            PA_REFERENCIA = referencia,
                            PA_EXPLICACION = (String)item.Field<String>(8) == null ? "" : item.Field<String>(8),
                            PA_PLAN_ACCION = (String)item.Field<String>(9) == null ? "" : item.Field<String>(9).Truncate(699),
                            PA_CONCEPTO_COSTO = (String)item.Field<String>(10) == null ? "" : item.Field<String>(10),
                            PA_CAMPO_1 = (String)item.Field<String>(11) == null ? "" : item.Field<String>(11),
                            PA_CAMPO_2 = (String)item.Field<String>(12) == null ? "" : item.Field<String>(12),
                            PA_CAMPO_3 = (String)item.Field<String>(13) == null ? "" : item.Field<String>(13),
                            PA_CAMPO_4 = (String)item.Field<String>(14) == null ? "" : item.Field<String>(14),
                            PA_CAMPO_5 = (String)item.Field<String>(15) == null ? "" : item.Field<String>(15),
                            PA_CAMPO_6 = (String)item.Field<String>(16) == null ? "" : item.Field<String>(16),
                            PA_CAMPO_7 = (String)item.Field<String>(17) == null ? "" : item.Field<String>(17),
                            PA_CAMPO_8 = (String)item.Field<String>(18) == null ? "" : item.Field<String>(18),
                            PA_CAMPO_9 = (String)item.Field<String>(19) == null ? "" : item.Field<String>(19),
                            PA_CAMPO_10 = (String)item.Field<String>(20) == null ? "" : item.Field<String>(20),
                            PA_CAMPO_11 = (String)item.Field<String>(21) == null ? "" : item.Field<String>(21),
                            PA_CAMPO_12 = (String)item.Field<String>(22) == null ? "" : item.Field<String>(22),
                            PA_CAMPO_13 = (String)item.Field<String>(23) == null ? "" : item.Field<String>(23),
                            PA_CAMPO_14 = (String)item.Field<String>(24) == null ? "" : item.Field<String>(24),
                            PA_CAMPO_15 = (String)item.Field<String>(25) == null ? "" : item.Field<String>(25),
                            PA_CAMPO_16 = (String)item.Field<String>(26) == null ? "" : item.Field<String>(26),
                            PA_CAMPO_17 = (String)item.Field<String>(27) == null ? "" : item.Field<String>(27),
                            PA_CAMPO_18 = (String)item.Field<String>(28) == null ? "" : item.Field<String>(28),
                            PA_CAMPO_19 = (String)item.Field<String>(29) == null ? "" : item.Field<String>(29),
                            PA_CAMPO_20 = (String)item.Field<String>(30) == null ? "" : item.Field<String>(30),
                            PA_CAMPO_21 = (String)item.Field<String>(31) == null ? "" : item.Field<String>(31),
                            PA_CAMPO_22 = (String)item.Field<String>(32) == null ? "" : item.Field<String>(32),
                            PA_CAMPO_23 = (String)item.Field<String>(33) == null ? "" : item.Field<String>(33),
                            PA_CAMPO_24 = (String)item.Field<String>(34) == null ? "" : item.Field<String>(34),
                            PA_CAMPO_25 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(35),
                            PA_CAMPO_26 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(36),
                            PA_CAMPO_27 = (String)item.Field<String>(37) == null ? "" : item.Field<String>(37),
                            PA_CAMPO_28 = (String)item.Field<String>(38) == null ? "" : item.Field<String>(38),
                            PA_CAMPO_29 = (String)item.Field<String>(39) == null ? "" : item.Field<String>(39),
                            PA_CAMPO_30 = (String)item.Field<String>(40) == null ? "" : item.Field<String>(40),
                            PA_CAMPO_31 = (String)item.Field<String>(41) == null ? "" : item.Field<String>(41),
                            PA_CAMPO_32 = (String)item.Field<String>(42) == null ? "" : item.Field<String>(42),
                            PA_CAMPO_33 = (String)item.Field<String>(43) == null ? "" : item.Field<String>(43),
                            PA_CAMPO_34 = (String)item.Field<String>(44) == null ? "" : item.Field<String>(44),
                            PA_CAMPO_35 = (String)item.Field<String>(45) == null ? "" : item.Field<String>(45),
                            PA_CAMPO_36 = (String)item.Field<String>(46) == null ? "" : item.Field<String>(46),
                            PA_CAMPO_37 = (String)item.Field<String>(47) == null ? "" : item.Field<String>(47),
                            PA_CAMPO_38 = (String)item.Field<String>(48) == null ? "" : item.Field<String>(48),
                            PA_CAMPO_39 = (String)item.Field<String>(49) == null ? "" : item.Field<String>(49),
                            PA_CAMPO_40 = (String)item.Field<String>(50) == null ? "" : item.Field<String>(50),
                            PA_CAMPO_41 = (String)item.Field<String>(51) == null ? "" : item.Field<String>(51),
                            PA_CAMPO_42 = (String)item.Field<String>(52) == null ? "" : item.Field<String>(52),
                            PA_CAMPO_43 = (String)item.Field<String>(53) == null ? "" : item.Field<String>(53),
                            PA_CAMPO_44 = (String)item.Field<String>(54) == null ? "" : item.Field<String>(54),
                            PA_CAMPO_45 = (String)item.Field<String>(55) == null ? "" : item.Field<String>(55),
                            PA_CAMPO_46 = (String)item.Field<String>(56) == null ? "" : item.Field<String>(56),
                            PA_CAMPO_47 = (String)item.Field<String>(57) == null ? "" : item.Field<String>(57),
                            PA_CAMPO_48 = (String)item.Field<String>(58) == null ? "" : item.Field<String>(58),
                            PA_CAMPO_49 = (String)item.Field<String>(59) == null ? "" : item.Field<String>(59),
                            PA_CAMPO_50 = (String)item.Field<String>(60) == null ? "" : item.Field<String>(60),
                            PA_USUARIO_CREACION = userId,
                            PA_FECHA_CREACION = DateTime.Now,
                        };
                        #endregion
                    }
                    catch (Exception e)
                    {
                        //List<String> mensajes = new List<String>();
                        //mensajes.Add($"La fila {counter} no cumple con el formato requerido"+e.Message);
                        //listError.Add(new MessageErrorPartida() { Linea = linea, Mensajes =mensajes});
                    }
                    ValidaReglasCarga(counter, ref list, ref listError, partidaModel, 1, centroCostos,conceptoCostos,cuentas,empresa);
                    counter += 1;
                    //counterRecords += 1;
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

        public void ValidaReglasCarga(int counter, ref List<PartidasModel> list, ref List<MessageErrorPartida> listError, PartidasModel partidaModel, int carga, List<CentroCostoModel> centroCostos, List<ConceptoCostoModel> conCostos,List<CuentaContableModel> ctaContables, List<EmpresaModel> empresa )
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
            ValidationList rules = new ValidationList();
            if (carga == 1)
            {
                
                rules.Add(new FTSFOValidation(partidaModel, null));
                rules.Add(new FTFCIFOValidation(partidaModel, null));
                rules.Add(new COValidation(partidaModel, ctaContables));
                rules.Add(new CEValidation(partidaModel, empresa));
                rules.Add(new CCValidations(partidaModel, centroCostos));
                rules.Add(new CONCEPCOSValidation(partidaModel, conCostos));
                rules.Add(new IImporteValidation(partidaModel, null));
            }
            else
            {
                //rules.Add(new FTSFOValidation(partidaModel, null));
                //rules.Add(new FTFCIFOValidation(partidaModel, null));
                rules.Add(new COValidation(partidaModel, ctaContables));
                rules.Add(new CEValidation(partidaModel, empresa));
                rules.Add(new CCValidations(partidaModel, centroCostos));
                rules.Add(new CONCEPCOSValidation(partidaModel, conCostos));
                rules.Add(new IImporteValidation(partidaModel, null));
            }
            if (rules.IsValid && isValid)
                list.Add(partidaModel);
            else if (!rules.IsValid)
            {
                foreach (var error in rules.Messages)
                {
                    listError.Add(new MessageErrorPartida() { Linea = counter + 1, Mensaje = error });
                }
            }
        }

        public PartidasContent cargaInicial<T>(T input, string userId)
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
                string dateFormat = "MMddyyyy";
                string mensaje = string.Empty;

                //var counterRecords = partidaService.Count(c => c.PA_FECHA_CARGA.Year == today.Year && c.PA_FECHA_CARGA.Month == today.Month && c.PA_FECHA_CARGA.Day == today.Day);
                var centroCostos = centroCostoService.GetAllFlatten<CentroCostoModel>();
                var conceptoCostos = conceptoCostoService.GetAllFlatten<ConceptoCostoModel>();
                var cuentas = contableService.GetAllFlatten<CuentaContableModel>();
                var empresa = empresaService.GetAllFlatten<EmpresaModel>();
                var ds = input as DataSet;

                var finalList = ReorderCargaInicial(ds, userId, ref listError);
                var reorder = finalList.OrderBy(c => c.PA_FECHA_TRX).GroupBy(c => c.PA_FECHA_TRX);
                foreach (var item in reorder)
                {
                    int internalcounter = 1;
                    foreach (var iteminner in item)
                    {
                        String PA_REFERENCIA = string.Empty;
                        CuentaContableModel singleCuenta = null;
                        try
                        {
                            var referenciaEmbedded = iteminner.PA_REFERENCIA;
                            var cuenta = iteminner.PA_CTA_CONTABLE;
                            var importe = iteminner.PA_IMPORTE;
                            singleCuenta = cuentas.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim() + c.CO_COD_AUXILIAR.Trim() + c.CO_NUM_AUXILIAR.Trim()) == cuenta);
                            if (singleCuenta.CO_COD_CONCILIA.Equals("1"))
                            {
                                if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe > 0)
                                {
                                    iteminner.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + internalcounter.ToString().PadLeft(5, '0');
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("D") && importe < 0)
                                {
                                    if (registroService.IsValidReferencia(referenciaEmbedded) == "S")
                                        continue;
                                    else
                                    {
                                        mensaje = "La referencia es invalida";
                                        throw new Exception();
                                    }
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe < 0)
                                {
                                    iteminner.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + internalcounter.ToString().PadLeft(5, '0');
                                }
                                else if (singleCuenta.CO_COD_NATURALEZA.Equals("C") && importe > 0)
                                {
                                    if (registroService.IsValidReferencia(referenciaEmbedded) == "S")
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
                            if (singleCuenta == null)
                                mensaje = "Cuenta contable para calculo de referencia no existe. Validar cuenta.";
                            listError.Add(new MessageErrorPartida() { Linea = counter, Mensaje = mensaje, Columna = "PA_REFERENCIA" });
                        }
                        ValidaReglasCarga(counter, ref list, ref listError, iteminner, 2, centroCostos,conceptoCostos,cuentas,empresa);
                        counter += 1;
                        //counterRecords += 1;
                        internalcounter += 1;
                    }
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

        private List<PartidasModel> ReorderCargaInicial(DataSet excelData, string userId, ref List<MessageErrorPartida> listError)
        {
            List<PartidasModel> listaPartidas = new List<PartidasModel>();
            int counter = 1;

            IFormatProvider culture = new CultureInfo("en-US", true);
            string dateFormat = "MMddyyyy";
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
                    try { string PA_COD_EMPRESA = (String)item.Field<String>(0) == null ? "" : item.Field<String>(0); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_COD_EMPRESA" }); }
                    try { DateTime PA_FECHA_CARGA = DateTime.ParseExact(item.Field<String>(1), dateFormat, culture); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_FECHA_CARGA" }); }
                    try { DateTime PA_FECHA_TRX = DateTime.ParseExact(item.Field<String>(2), dateFormat, culture); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_FECHA_TRX" }); }
                    try { String PA_CTA_CONTABLE = (String)item.Field<String>(3) == null ? "" : item.Field<String>(3); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CTA_CONTABLE" }); }
                    try { String PA_CENTRO_COSTO = (String)item.Field<String>(4) == null ? "" : item.Field<String>(4); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CENTRO_COSTO" }); }
                    try { String PA_COD_MONEDA = (String)item.Field<String>(5) == null ? "" : item.Field<String>(5); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_COD_MONEDA" }); }
                    try { Double PA_IMPORTE = (Double)item.Field<Double>(6); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_IMPORTE" }); }
                    try { String PA_REFERENCIA = (String)item.Field<String>(7) == null ? "" : item.Field<String>(7); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_REFERENCIA" }); }
                    try { String PA_EXPLICACION = (String)item.Field<String>(8) == null ? "" : item.Field<String>(8); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_EXPLICACION" }); }
                    try { String PA_PLAN_ACCION = (String)item.Field<String>(9) == null ? "" : item.Field<String>(9).Truncate(699); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_PLAN_ACCION" }); }
                    try { String PA_CONCEPTO_COSTO = (String)item.Field<String>(10) == null ? "" : item.Field<String>(10); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CONCEPTO_COSTO" }); }
                    try { String PA_CAMPO_1 = (String)item.Field<String>(11) == null ? "" : item.Field<String>(11); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_1" }); }
                    try { String PA_CAMPO_2 = (String)item.Field<String>(12) == null ? "" : item.Field<String>(12); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_2" }); }
                    try { String PA_CAMPO_3 = (String)item.Field<String>(13) == null ? "" : item.Field<String>(13); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_3" }); }
                    try { String PA_CAMPO_4 = (String)item.Field<String>(14) == null ? "" : item.Field<String>(14); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_4" }); }
                    try { String PA_CAMPO_5 = (String)item.Field<String>(15) == null ? "" : item.Field<String>(15); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_5" }); }
                    try { String PA_CAMPO_6 = (String)item.Field<String>(16) == null ? "" : item.Field<String>(16); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_6" }); }
                    try { String PA_CAMPO_7 = (String)item.Field<String>(17) == null ? "" : item.Field<String>(17); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_7" }); }
                    try { String PA_CAMPO_8 = (String)item.Field<String>(18) == null ? "" : item.Field<String>(18); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_8" }); }
                    try { String PA_CAMPO_9 = (String)item.Field<String>(19) == null ? "" : item.Field<String>(19); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_9" }); }
                    try { String PA_CAMPO_10 = (String)item.Field<String>(20) == null ? "" : item.Field<String>(20); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_10" }); }
                    try { String PA_CAMPO_11 = (String)item.Field<String>(21) == null ? "" : item.Field<String>(21); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_11" }); }
                    try { String PA_CAMPO_12 = (String)item.Field<String>(22) == null ? "" : item.Field<String>(22); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_12" }); }
                    try { String PA_CAMPO_13 = (String)item.Field<String>(23) == null ? "" : item.Field<String>(23); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_13" }); }
                    try { String PA_CAMPO_14 = (String)item.Field<String>(24) == null ? "" : item.Field<String>(24); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_14" }); }
                    try { String PA_CAMPO_15 = (String)item.Field<String>(25) == null ? "" : item.Field<String>(25); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_15" }); }
                    try { String PA_CAMPO_16 = (String)item.Field<String>(26) == null ? "" : item.Field<String>(26); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_16" }); }
                    try { String PA_CAMPO_17 = (String)item.Field<String>(27) == null ? "" : item.Field<String>(27); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_17" }); }
                    try { String PA_CAMPO_18 = (String)item.Field<String>(28) == null ? "" : item.Field<String>(28); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_18" }); }
                    try { String PA_CAMPO_19 = (String)item.Field<String>(29) == null ? "" : item.Field<String>(29); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_19" }); }
                    try { String PA_CAMPO_20 = (String)item.Field<String>(30) == null ? "" : item.Field<String>(30); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_20" }); }
                    try { String PA_CAMPO_21 = (String)item.Field<String>(31) == null ? "" : item.Field<String>(31); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_21" }); }
                    try { String PA_CAMPO_22 = (String)item.Field<String>(32) == null ? "" : item.Field<String>(32); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_22" }); }
                    try { String PA_CAMPO_23 = (String)item.Field<String>(33) == null ? "" : item.Field<String>(33); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_23" }); }
                    try { String PA_CAMPO_24 = (String)item.Field<String>(34) == null ? "" : item.Field<String>(34); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_24" }); }
                    try { String PA_CAMPO_25 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(35); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_25" }); }
                    try { String PA_CAMPO_26 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(36); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_26" }); }
                    try { String PA_CAMPO_27 = (String)item.Field<String>(37) == null ? "" : item.Field<String>(37); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_27" }); }
                    try { String PA_CAMPO_28 = (String)item.Field<String>(38) == null ? "" : item.Field<String>(38); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_28" }); }
                    try { String PA_CAMPO_29 = (String)item.Field<String>(39) == null ? "" : item.Field<String>(39); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_29" }); }
                    try { String PA_CAMPO_30 = (String)item.Field<String>(40) == null ? "" : item.Field<String>(40); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_30" }); }
                    try { String PA_CAMPO_31 = (String)item.Field<String>(41) == null ? "" : item.Field<String>(41); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_31" }); }
                    try { String PA_CAMPO_32 = (String)item.Field<String>(42) == null ? "" : item.Field<String>(42); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_32" }); }
                    try { String PA_CAMPO_33 = (String)item.Field<String>(43) == null ? "" : item.Field<String>(43); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_33" }); }
                    try { String PA_CAMPO_34 = (String)item.Field<String>(44) == null ? "" : item.Field<String>(44); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_34" }); }
                    try { String PA_CAMPO_35 = (String)item.Field<String>(45) == null ? "" : item.Field<String>(45); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_35" }); }
                    try { String PA_CAMPO_36 = (String)item.Field<String>(46) == null ? "" : item.Field<String>(46); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_36" }); }
                    try { String PA_CAMPO_37 = (String)item.Field<String>(47) == null ? "" : item.Field<String>(47); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_37" }); }
                    try { String PA_CAMPO_38 = (String)item.Field<String>(48) == null ? "" : item.Field<String>(48); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_38" }); }
                    try { String PA_CAMPO_39 = (String)item.Field<String>(49) == null ? "" : item.Field<String>(49); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_39" }); }
                    try { String PA_CAMPO_40 = (String)item.Field<String>(50) == null ? "" : item.Field<String>(50); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_40" }); }
                    try { String PA_CAMPO_41 = (String)item.Field<String>(51) == null ? "" : item.Field<String>(51); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_41" }); }
                    try { String PA_CAMPO_42 = (String)item.Field<String>(52) == null ? "" : item.Field<String>(52); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_42" }); }
                    try { String PA_CAMPO_43 = (String)item.Field<String>(53) == null ? "" : item.Field<String>(53); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_43" }); }
                    try { String PA_CAMPO_44 = (String)item.Field<String>(54) == null ? "" : item.Field<String>(54); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_44" }); }
                    try { String PA_CAMPO_45 = (String)item.Field<String>(55) == null ? "" : item.Field<String>(55); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_45" }); }
                    try { String PA_CAMPO_46 = (String)item.Field<String>(56) == null ? "" : item.Field<String>(56); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_46" }); }
                    try { String PA_CAMPO_47 = (String)item.Field<String>(57) == null ? "" : item.Field<String>(57); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_47" }); }
                    try { String PA_CAMPO_48 = (String)item.Field<String>(58) == null ? "" : item.Field<String>(58); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_48" }); }
                    try { String PA_CAMPO_49 = (String)item.Field<String>(59) == null ? "" : item.Field<String>(59); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_49" }); }
                    try { String PA_CAMPO_50 = (String)item.Field<String>(60) == null ? "" : item.Field<String>(60); } catch (Exception e) { listError.Add(new MessageErrorPartida() { Linea = linea, Mensaje = mensaje, Columna = "PA_CAMPO_50" }); }

                    #region Set partida
                    partidaModel = new PartidasModel
                    {
                        PA_CONTADOR = counter,
                        RC_REGISTRO_CONTROL = 0,
                        PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.CREADO),
                        PA_COD_EMPRESA = (String)item.Field<String>(0) == null ? "" : item.Field<String>(0),
                        PA_FECHA_CARGA = DateTime.ParseExact(item.Field<String>(1), dateFormat, culture),
                        PA_FECHA_TRX = DateTime.ParseExact(item.Field<String>(2), dateFormat, culture),
                        PA_CTA_CONTABLE = (String)item.Field<String>(3) == null ? "" : item.Field<String>(3),
                        PA_CENTRO_COSTO = (String)item.Field<String>(4) == null ? "" : item.Field<String>(4),
                        PA_COD_MONEDA = (String)item.Field<String>(5) == null ? "" : item.Field<String>(5),
                        PA_IMPORTE = decimal.Parse(item.Field<Double>(6).ToString()),
                        PA_REFERENCIA = referencia,
                        PA_EXPLICACION = (String)item.Field<String>(8) == null ? "" : item.Field<String>(8),
                        PA_PLAN_ACCION = (String)item.Field<String>(9) == null ? "" : item.Field<String>(9).Truncate(699),
                        PA_CONCEPTO_COSTO = (String)item.Field<String>(10) == null ? "" : item.Field<String>(10),
                        PA_CAMPO_1 = (String)item.Field<String>(11) == null ? "" : item.Field<String>(11),
                        PA_CAMPO_2 = (String)item.Field<String>(12) == null ? "" : item.Field<String>(12),
                        PA_CAMPO_3 = (String)item.Field<String>(13) == null ? "" : item.Field<String>(13),
                        PA_CAMPO_4 = (String)item.Field<String>(14) == null ? "" : item.Field<String>(14),
                        PA_CAMPO_5 = (String)item.Field<String>(15) == null ? "" : item.Field<String>(15),
                        PA_CAMPO_6 = (String)item.Field<String>(16) == null ? "" : item.Field<String>(16),
                        PA_CAMPO_7 = (String)item.Field<String>(17) == null ? "" : item.Field<String>(17),
                        PA_CAMPO_8 = (String)item.Field<String>(18) == null ? "" : item.Field<String>(18),
                        PA_CAMPO_9 = (String)item.Field<String>(19) == null ? "" : item.Field<String>(19),
                        PA_CAMPO_10 = (String)item.Field<String>(20) == null ? "" : item.Field<String>(20),
                        PA_CAMPO_11 = (String)item.Field<String>(21) == null ? "" : item.Field<String>(21),
                        PA_CAMPO_12 = (String)item.Field<String>(22) == null ? "" : item.Field<String>(22),
                        PA_CAMPO_13 = (String)item.Field<String>(23) == null ? "" : item.Field<String>(23),
                        PA_CAMPO_14 = (String)item.Field<String>(24) == null ? "" : item.Field<String>(24),
                        PA_CAMPO_15 = (String)item.Field<String>(25) == null ? "" : item.Field<String>(25),
                        PA_CAMPO_16 = (String)item.Field<String>(26) == null ? "" : item.Field<String>(26),
                        PA_CAMPO_17 = (String)item.Field<String>(27) == null ? "" : item.Field<String>(27),
                        PA_CAMPO_18 = (String)item.Field<String>(28) == null ? "" : item.Field<String>(28),
                        PA_CAMPO_19 = (String)item.Field<String>(29) == null ? "" : item.Field<String>(29),
                        PA_CAMPO_20 = (String)item.Field<String>(30) == null ? "" : item.Field<String>(30),
                        PA_CAMPO_21 = (String)item.Field<String>(31) == null ? "" : item.Field<String>(31),
                        PA_CAMPO_22 = (String)item.Field<String>(32) == null ? "" : item.Field<String>(32),
                        PA_CAMPO_23 = (String)item.Field<String>(33) == null ? "" : item.Field<String>(33),
                        PA_CAMPO_24 = (String)item.Field<String>(34) == null ? "" : item.Field<String>(34),
                        PA_CAMPO_25 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(35),
                        PA_CAMPO_26 = (String)item.Field<String>(35) == null ? "" : item.Field<String>(36),
                        PA_CAMPO_27 = (String)item.Field<String>(37) == null ? "" : item.Field<String>(37),
                        PA_CAMPO_28 = (String)item.Field<String>(38) == null ? "" : item.Field<String>(38),
                        PA_CAMPO_29 = (String)item.Field<String>(39) == null ? "" : item.Field<String>(39),
                        PA_CAMPO_30 = (String)item.Field<String>(40) == null ? "" : item.Field<String>(40),
                        PA_CAMPO_31 = (String)item.Field<String>(41) == null ? "" : item.Field<String>(41),
                        PA_CAMPO_32 = (String)item.Field<String>(42) == null ? "" : item.Field<String>(42),
                        PA_CAMPO_33 = (String)item.Field<String>(43) == null ? "" : item.Field<String>(43),
                        PA_CAMPO_34 = (String)item.Field<String>(44) == null ? "" : item.Field<String>(44),
                        PA_CAMPO_35 = (String)item.Field<String>(45) == null ? "" : item.Field<String>(45),
                        PA_CAMPO_36 = (String)item.Field<String>(46) == null ? "" : item.Field<String>(46),
                        PA_CAMPO_37 = (String)item.Field<String>(47) == null ? "" : item.Field<String>(47),
                        PA_CAMPO_38 = (String)item.Field<String>(48) == null ? "" : item.Field<String>(48),
                        PA_CAMPO_39 = (String)item.Field<String>(49) == null ? "" : item.Field<String>(49),
                        PA_CAMPO_40 = (String)item.Field<String>(50) == null ? "" : item.Field<String>(50),
                        PA_CAMPO_41 = (String)item.Field<String>(51) == null ? "" : item.Field<String>(51),
                        PA_CAMPO_42 = (String)item.Field<String>(52) == null ? "" : item.Field<String>(52),
                        PA_CAMPO_43 = (String)item.Field<String>(53) == null ? "" : item.Field<String>(53),
                        PA_CAMPO_44 = (String)item.Field<String>(54) == null ? "" : item.Field<String>(54),
                        PA_CAMPO_45 = (String)item.Field<String>(55) == null ? "" : item.Field<String>(55),
                        PA_CAMPO_46 = (String)item.Field<String>(56) == null ? "" : item.Field<String>(56),
                        PA_CAMPO_47 = (String)item.Field<String>(57) == null ? "" : item.Field<String>(57),
                        PA_CAMPO_48 = (String)item.Field<String>(58) == null ? "" : item.Field<String>(58),
                        PA_CAMPO_49 = (String)item.Field<String>(59) == null ? "" : item.Field<String>(59),
                        PA_CAMPO_50 = (String)item.Field<String>(60) == null ? "" : item.Field<String>(60),
                        PA_USUARIO_CREACION = userId,
                        PA_FECHA_CREACION = DateTime.Now,
                    };
                    listaPartidas.Add(partidaModel);
                    #endregion
                }
                catch (Exception e)
                {

                }
            }




            return listaPartidas;

        }

    }


}
