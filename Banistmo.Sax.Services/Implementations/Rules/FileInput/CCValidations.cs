using Banistmo.Sax.Common;
using Banistmo.Sax.Repository.Interfaces.Business;
using Banistmo.Sax.Repository.Model;
using Banistmo.Sax.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Rules.FileInput
{
    /// <summary>
    /// Validacion de centro de costo
    /// </summary>
    public class CCValidations : ValidationBase<PartidasModel>
    {
        private List<EmpresaAreasCentroCostoModel> ListaEmpreAreaCentroCosto;
        private List<EmpresaModel> listaEmpresa;
        private int idArea;
        string mensaje;
        public CCValidations(PartidasModel context, object objectData, object listaEmpresaAreaCentro, int area, List<EmpresaModel> listEmpresa) : base(context, objectData)
        {
            ListaEmpreAreaCentroCosto = (List<EmpresaAreasCentroCostoModel>)listaEmpresaAreaCentro;
            idArea = area;
            listaEmpresa = listEmpresa;
            mensaje = "El centro de costo no existe o está inactivo.";
        }
        public override string Message
        {
            get
            {
                return mensaje;
            }
        }

        public override string Columna
        {
            get
            {
                return "Centro de Costo";
            }
  
        }

        public override bool Requirement
        {
            get
            {
                var codEmpresa=Context.PA_COD_EMPRESA;
                if (string.IsNullOrEmpty(codEmpresa))
                    return true;
                int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                EmpresaModel resultEmpresa = listaEmpresa.FirstOrDefault(c => c.CE_COD_EMPRESA.Trim() == Context.PA_COD_EMPRESA.Trim() && c.CE_ESTATUS == activo.ToString());
                if (resultEmpresa == null)
                    return true;

                var centrocosto = (List<CentroCostoModel>)inputObject;
                var result = centrocosto.FirstOrDefault(c => c.CC_CENTRO_COSTO.Trim() == Context.PA_CENTRO_COSTO.Trim() && c.CC_ESTATUS== activo) ;
                if (result != null)
                {
                    var empresaAreaCentro = ListaEmpreAreaCentroCosto.Where(x => x.IdCentroCosto == result.CC_ID_CENTRO_COSTO && x.IdArea== idArea && x.IdEmpresa== resultEmpresa.CE_ID_EMPRESA);
                    if (empresaAreaCentro != null)
                    {
                        return true;
                    }
                    else {
                        mensaje = $"El centro de costo {Context.PA_CENTRO_COSTO} no existe para la empresa y area.";
                        return false;
                    }
                }
                else {
                    mensaje = $"El centro de costo {Context.PA_CENTRO_COSTO} no existe o esta inactivo";
                    return false;
                }
            }
        }
    }
}
