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
using Banistmo.Sax.Repository.Interfaces.Business;
using AutoMapper;

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class UsuarioEmpresaService : ServiceBase<UsuarioEmpresaModel, SAX_USUARIO_EMPRESA, UsuarioEmpresa>, IUsuarioEmpresaService
    {
        private readonly IUsuarioEmpresa areService;
        public UsuarioEmpresaService()
            : this(new UsuarioEmpresa())
        {

        }
        public UsuarioEmpresaService(UsuarioEmpresa ue)
            : base(ue)
        { }

        public UsuarioEmpresaService(IUsuarioEmpresa service)
        : this(new UsuarioEmpresa())
        {
            areService = service;
        }

        public void CreateAndRemove(List<UsuarioEmpresaModel> create, List<int> remove)
        {
            List<SAX_USUARIO_EMPRESA> modelA = Mapper.Map<List<UsuarioEmpresaModel>, List<SAX_USUARIO_EMPRESA>>(create);
            areService.CreateAndRemove(modelA, remove);
        }
    }
}
