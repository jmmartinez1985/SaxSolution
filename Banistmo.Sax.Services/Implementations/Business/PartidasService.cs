using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banistmo.Sax.Repository.Implementations.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.Common;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Banistmo.Sax.Services.Implementations.Rules;
using Banistmo.Sax.Services.Implementations.Rules.FileInput;
using System.Globalization;

namespace Banistmo.Sax.Services.Implementations.Business
{

    [Injectable]
    public class PartidasService : ServiceBase<PartidasModel, SAX_PARTIDAS, Partidas>, IPartidasService
    {

        private IPartidasService partidaService;
        private ICentroCostoService centroCostoService;
        private IEmpresaService empresaService;
        private IConceptoCostoService conceptoCostoService;
        private ICuentaContableService contableService;

        private IMonedaService monedaService;

        public PartidasService()
            : this(new Partidas())
        {
           
        }
        public PartidasService(Partidas ao)
            : base(ao)
        {

            
        }

        public PartidasService(Partidas ao, ICentroCostoService centroCostoSvc,
            IEmpresaService empresaSvc,
            IConceptoCostoService conceptoCostoSvc,
            ICuentaContableService contableSvc, IMonedaService modSvc)
           : base(ao)
        {
            centroCostoService = centroCostoSvc;
            empresaService = empresaSvc;
            conceptoCostoService = conceptoCostoSvc;
            contableService = contableSvc;
            monedaService = modSvc;
        }

        public PartidasModel CreateSinglePartida(PartidasModel par)
        {

            centroCostoService = centroCostoService ?? new CentroCostoService();
            partidaService = partidaService ?? new PartidasService();
            conceptoCostoService = conceptoCostoService ?? new ConceptoCostoService();
            contableService = contableService ?? new CuentaContableService();
            empresaService = empresaService ?? new EmpresaService();
            monedaService = monedaService??new  MonedaService();

            IFormatProvider culture = new CultureInfo("en-US", true);
            string dateFormat = "MMddyyyy";
            //Counting number of record already exist.
            var counterRecords = partidaService.Count();

            var centroCostos = centroCostoService.GetAll();
            var conceptoCostos = conceptoCostoService.GetAll();
            var cuentas = contableService.GetAll();
            var empresa = empresaService.GetAll();

            var context = new System.ComponentModel.DataAnnotations.ValidationContext(par, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(par, context, validationResults, true);
            ValidationList rules = new ValidationList();
            rules.Add(new FTSFOValidation(par, null));
            rules.Add(new FTFCIFOValidation(par, null));
            rules.Add(new COValidation(par, cuentas));
            rules.Add(new CEValidation(par, empresa));
            rules.Add(new CONCEPCOSValidation(par, conceptoCostos));
            rules.Add(new IImporteValidation(par, null));
            if (!rules.IsValid)
                throw new Exception("No se cumple con la entrada de datos y las reglas de negocios");
            par.PA_STATUS_PARTIDA = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            par.PA_REFERENCIA = System.DateTime.Now.Date.ToString(dateFormat) + counterRecords;
            return base.Insert(par, true);
        }

        public bool isSaldoValidoMoneda(List<PartidasModel> partidas, ref List<MonedaValidationModel> monedasValid)
        {
            int counter = 0;
            var gruopedBy = partidas.GroupBy(C => C.PA_COD_MONEDA);
            monedaService = monedaService ?? new MonedaService();
            var monedaList = monedaService.GetAllFlatten<MonedaModel>();
            foreach (var item in gruopedBy)
            {
                var moneda = item.Key;

                var credito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
                var debito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
                if ((credito + debito) == 0)
                {
                    continue;
                }
                else
                {
                    var itemdesc = monedaList.FirstOrDefault(c => c.CC_NUM_MONEDA.Trim() == moneda.Trim());
                    monedasValid.Add(new MonedaValidationModel { Codigo = moneda,  Descripcion = itemdesc.CC_COD_CURRENCY });
                    counter++;
                }
            }
            return (counter > 0);
        }

        public bool isSaldoValidoEmpresa(List<PartidasModel> partidas, ref List<EmpresaValidationModel> monedasValid)
        {
            int counter = 0;
            var gruopedBy = partidas.GroupBy(C => C.PA_COD_EMPRESA);
            empresaService = empresaService ?? new EmpresaService();
            var empresaList = empresaService.GetAllFlatten<EmpresaModel>();
            foreach (var item in gruopedBy)
            {
                var emp = item.Key;

                var credito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
                var debito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
                if ((credito + debito) == 0)
                {
                    continue;
                }
                else
                {
                    var itemdesc = empresaList.FirstOrDefault(c => c.CE_COD_EMPRESA.Trim() == emp.Trim());
                    monedasValid.Add(new EmpresaValidationModel { Codigo = emp, Descripcion = itemdesc.CE_NOMBRE });
                    counter++;
                }
            }
            return (counter > 0);
        }

        public bool isSaldoValidoMonedaEmpresa(List<PartidasModel> partidas, ref List<EmpresaMonedaValidationModel> monedasValid)
        {
            empresaService = empresaService ?? new EmpresaService();
            monedaService = monedaService ?? new MonedaService();
            int counter = 0;
            var gruopedByEmpresa = partidas.GroupBy(c=> new { c.PA_COD_EMPRESA });
            var gruopedByMoneda = partidas.GroupBy(c => new { c.PA_COD_MONEDA });

            bool balanceMoneda = false;
            bool balanceEmpresa = false;
            var empresaList = empresaService.GetAllFlatten<EmpresaModel>();
            var monedaList = monedaService.GetAllFlatten<MonedaModel>();

            foreach (var item in gruopedByEmpresa)
            {
                var emp = item.Key.PA_COD_EMPRESA;

                var credito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
                var debito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
                if ((credito + debito) == 0)
                {
                    continue;
                }
                else
                {
                    balanceEmpresa = true;
                    var emprDes = empresaList.FirstOrDefault(c => c.CE_COD_EMPRESA.Trim() == emp.Trim());
                    monedasValid.Add(new EmpresaMonedaValidationModel { CodigoEmpresa = emp, DescripcionEmpresa = emprDes.CE_NOMBRE});
                    counter++;
                }
            }

            foreach (var item in gruopedByMoneda)
            {
                var moneda = item.Key.PA_COD_MONEDA;

                var credito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? element : 0));
                var debito = item.Select(c => c.PA_IMPORTE).Sum(element => (element < 0 ? 0 : element));
                if ((credito + debito) == 0)
                {
                    continue;
                }
                else
                {
                    balanceMoneda = true;
                    var monDesc = monedaList.FirstOrDefault(c => c.CC_NUM_MONEDA.Trim() == moneda.Trim());
                    monedasValid.Add(new EmpresaMonedaValidationModel { CodigoMoneda = moneda, DescripcionMoneda = monDesc.CC_DESC_MONEDA });
                    counter++;
                }
            }
            return (balanceEmpresa || balanceMoneda);
        }

        public List<ReferenceGroupingModel> getConsolidaReferencias(List<PartidasModel> partidas)
        {
            var result = new List<ReferenceGroupingModel>();
            var groupingReference = partidas.Where(k => !string.IsNullOrEmpty(k.PA_REFERENCIA)).GroupBy(c => c.PA_REFERENCIA);
            foreach (var item in groupingReference)
            {
                result.Add(new ReferenceGroupingModel { Referencia = item.Key, Monto = item.Sum(c => c.PA_IMPORTE) });
            }
            return result;
        }


        private class MonedaGrouping
        {
            public MonedaGrouping(String empresa, string moneda)
            {
                Empresa = empresa;
                Moneda = moneda;
            }

            public String Empresa { get; set; }
            public String Moneda { get; set; }
            
        }
    }
}

