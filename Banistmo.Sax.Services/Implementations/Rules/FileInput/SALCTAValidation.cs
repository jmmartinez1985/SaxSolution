using Banistmo.Sax.Common;
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
                return string.Format($"No existe balance  entre partidas y contrapartidas en cuentas de orden  {Context.PA_CTA_CONTABLE}.  Partida {Context.PA_CTA_CONTABLE} total importe: {ImporteCuenta} Contrapartida {this.CuentaContra} total de importe: {this.ImporteCuentaContra} .");
            }
        }

        public override string Columna
        {
            get
            {
                return "Cuenta Contable";
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
                if (string.IsNullOrEmpty(Context.PA_CTA_CONTABLE)) {
                    return true;
                }
                if (Context.PA_CTA_CONTABLE.Length < 3)
                {
                    return true;
                }
                if (ValidCuentas.Any(c => c.Equals(Context.PA_CTA_CONTABLE.Trim().Substring(0, 2))))
                {
                    int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                    var cuentaContable = saldo.CuentasList.FirstOrDefault(c => (c.CO_CUENTA_CONTABLE.Trim() + c.CO_COD_AUXILIAR.Trim() + c.CO_NUM_AUXILIAR.Trim()) == Context.PA_CTA_CONTABLE.Trim() && c.CO_ESTATUS == activo);
                    if (cuentaContable == null)
                        return false;
                    else
                    {
                        this.CuentaContra = GetNameCuentaContra(cuentaContable);
                        if (string.IsNullOrEmpty(this.CuentaContra))
                            return false;
                        else
                        {
                            this.ImporteCuenta = 0;
                            this.ImporteCuentaContra = 0;
                            //Queda pendiente para sumarizar el total de cuentas contables\
                            var listCuentaContraTmp = ListRaw.Where(x => x.PA_CTA_CONTABLE.Trim() == this.CuentaContra && x.PA_FECHA_TRX.ToString("MMddyyyy") == Context.PA_FECHA_TRX.ToString("MMddyyyy")).ToList();
                            if (listCuentaContraTmp != null && listCuentaContraTmp.Count > 0)
                            {
                                ImporteCuentaContra = listCuentaContraTmp.Sum(x => x.PA_IMPORTE);
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

        private string GetNameCuentaContra(CuentaContableModel cuenta)
        {
            string nameCuentaContra = string.Empty;
            if (cuenta != null)
            {
                nameCuentaContra = ((cuenta.CO_CTA_CONTABLE_CONTRA == null ? string.Empty : cuenta.CO_CTA_CONTABLE_CONTRA.Trim()) + (cuenta.CO_COD_AUXILIAR_CONTRA == null ? string.Empty : cuenta.CO_COD_AUXILIAR_CONTRA.Trim()) + (cuenta.CO_NUM_AUXILIAR_CONTRA == null ? string.Empty : cuenta.CO_NUM_AUXILIAR_CONTRA.Trim()));
            }
            return nameCuentaContra;
        }
    }
}
