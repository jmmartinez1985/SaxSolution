using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de saldo de cuenta 61 y 64
    /// </summary>
    public class SALCTAValidation : ValidationBase<PartidasModel>
    {

        public SALCTAValidation(PartidasModel context, object objectData, List<PartidasModel> listPartidas) : base(context, objectData, listPartidas)
        {

        }

        public override string Message
        {
            get
            {
                return string.Format(@"La cuenta contable ""{0}"" no es válida  ya que cumple con la cta contra y/o importe no es $0", Context.PA_CTA_CONTABLE);
            }
        }

        public List<string> ValidCuentas
        {
            get
            {
                return ConfigurationManager.AppSettings["cuentaContra"].Split(';').ToList();
            }
        }

        public override bool Requirement
        {
            get
            {
                var saldo = (SaldoCuentaValidationModel)inputObject;
                string cuentraContra = string.Empty;
                if (ValidCuentas.Any(c => c.Equals(Context.PA_CTA_CONTABLE.Trim().Substring(0, 2))))
                {
                    var cuentaContable = saldo.CuentasList.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim() + c.CO_COD_AUXILIAR.Trim() + c.CO_NUM_AUXILIAR.Trim()) == Context.PA_CTA_CONTABLE.Trim());
                    if (cuentaContable == null)
                        return false;
                    else
                    {
                        cuentraContra = (cuentaContable.CO_CTA_CONTABLE_CONTRA.Trim() + cuentaContable.CO_COD_AUXILIAR_CONTRA.Trim() + cuentaContable.CO_NUM_AUXILIAR_CONTRA.Trim());
                        if (string.IsNullOrEmpty(cuentraContra))
                            return false;
                        else
                        {
                            //listPartidas.
                            var listCuentaTmp=ListRaw.Where(x => x.PA_CTA_CONTABLE.Trim() == cuentraContra && x.PA_FECHA_TRX == Context.PA_FECHA_TRX).ToList();
                            if (listCuentaTmp != null && listCuentaTmp.Count > 0) {
                                listCuentaTmp.Sum(x=>x.PA_IMPORTE);
                            }
                                                        saldo.CuentasList.Where(c => (c.CO_CTA_CONTABLE_CONTRA.Trim() + c.CO_COD_AUXILIAR_CONTRA.Trim() + c.CO_NUM_AUXILIAR_CONTRA.Trim()) == cuentraContra );
                            var importeCuenta = saldo.PartidasList.FirstOrDefault(c => c.PA_CTA_CONTABLE.Trim() == cuentraContra.Trim());
                            if (importeCuenta == null)
                                return false;
                            else
                            {
                                if ((importeCuenta.PA_IMPORTE + Context.PA_IMPORTE) == 0)
                                    return true;
                                else
                                    return false;
                            }
                        }
                    }
                }
                else
                    return true;

            }
        }
    }
}
