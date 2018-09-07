using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    public class UsuarioEmpresaValidation : ValidationBase<Models.PartidasModel>
    {
        private string columna;
        private string mensaje;
        private List<EmpresaModel> listaEmpresa;
        private List<UsuarioEmpresaModel> listaUsuarioEmpresa;
        public UsuarioEmpresaValidation(PartidasModel context, object objectData,List<UsuarioEmpresaModel> listaUsuarioEmpresa_) : base(context, objectData)
        {

            mensaje = string.Empty;
            listaUsuarioEmpresa = listaUsuarioEmpresa_;
            this.columna = "Empresa";
        }


        public override string Columna
        {
            get
            {
                return this.columna;
            }
        }
        public override string Message
        {
            get
            {
                return mensaje;
            }
        }

        public override bool Requirement
        {
            get
            {
                string cod_empresa = this.Context.PA_COD_EMPRESA;
                if (cod_empresa == null || string.IsNullOrEmpty(cod_empresa)) {
                    return true;
                }
                cod_empresa = cod_empresa.Trim();
                var listaEmpresa= (List<EmpresaModel>)inputObject;
                EmpresaModel objEmpresa = listaEmpresa.FirstOrDefault(emp => emp.CE_COD_EMPRESA == cod_empresa);
                if (objEmpresa == null)
                    return true;
                var result = this.listaUsuarioEmpresa.Where(y => y.CE_ID_EMPRESA == objEmpresa.CE_ID_EMPRESA).ToList();
                if (result != null && result.Count>0)
                {
                    return true;
                }
                else {
                    this.mensaje = $"El código de empresa {cod_empresa} no está vinculado a su usuario";
                    return false;
                } 
            }
        }
    }
}
