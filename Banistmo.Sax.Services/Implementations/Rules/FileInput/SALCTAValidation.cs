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
                return string.Format($"La cuenta contable {Context.PA_CTA_CONTABLE} no es válida ya que no cumple con la cta contra y/o importe no es $0.  Cuenta {Context.PA_CTA_CONTABLE} total importe: {ImporteCuenta} Cuenta contra {this.CuentaContra} total de importe: {this.ImporteCuentaContra}");
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
                if (ValidCuentas.Any(c => c.Equals(Context.PA_CTA_CONTABLE.Trim().Substring(0, 2))))
                {
                    var cuentaContable = saldo.CuentasList.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim() + c.CO_COD_AUXILIAR.Trim() + c.CO_NUM_AUXILIAR.Trim()) == Context.PA_CTA_CONTABLE.Trim());
                    if (cuentaContable == null)
                        return false;
                    else
                    {
                        this.CuentaContra = (cuentaContable.CO_CTA_CONTABLE_CONTRA.Trim() + cuentaContable.CO_COD_AUXILIAR_CONTRA.Trim() + cuentaContable.CO_NUM_AUXILIAR_CONTRA.Trim());
                        if (string.IsNullOrEmpty(this.CuentaContra))
                            return false;
                        else
                        {
                            this.ImporteCuenta = 0;
                            this.ImporteCuentaContra = 0;
                            //Queda pendiente para sumarizar el total de cuentas contables\
                            var listCuentaContraTmp=ListRaw.Where(x => x.PA_CTA_CONTABLE.Trim() == this.CuentaContra && x.PA_FECHA_TRX.ToString("MMddyyyy") == Context.PA_FECHA_TRX.ToString("MMddyyyy")).ToList();
                            if (listCuentaContraTmp != null && listCuentaContraTmp.Count > 0) {
                                ImporteCuentaContra = listCuentaContraTmp.Sum(x=>x.PA_IMPORTE);
                            }

                            var listCuentaConciliaTmp = ListRaw.Where(x => x.PA_CTA_CONTABLE.Trim() == Context.PA_CTA_CONTABLE.Trim() && x.PA_FECHA_TRX.ToString("MMddyyyy") == Context.PA_FECHA_TRX.ToString("MMddyyyy")).ToList();
                            if (listCuentaConciliaTmp != null && listCuentaConciliaTmp.Count > 0)
                            {
                                ImporteCuenta = listCuentaConciliaTmp.Sum(x => x.PA_IMPORTE);
                            }
                            saldo.CuentasList.Where(c => (c.CO_CTA_CONTABLE_CONTRA.Trim() + c.CO_COD_AUXILIAR_CONTRA.Trim() + c.CO_NUM_AUXILIAR_CONTRA.Trim()) == this.CuentaContra);
                            //var importeCuenta = saldo.PartidasList.FirstOrDefault(c => c.PA_CTA_CONTABLE.Trim() == cuentraContra.Trim());
                            //if (importeCuenta == null)
                            //    return false;
                            //else
                            //{
                                if ((ImporteCuentaContra + ImporteCuenta) == 0)
                                    return true;
                                else
                                    return false;
                           // }
                        }
                    }
                }
                else
                    return true;

            }
        }
    }
}
