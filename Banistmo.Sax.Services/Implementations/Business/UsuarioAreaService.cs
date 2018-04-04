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

namespace Banistmo.Sax.Services.Implementations.Business
{
    [Injectable]
    public class UsuarioAreaService : ServiceBase<UsuarioAreaModel, SAX_USUARIO_AREA, UsuarioArea>, IUsuarioAreaService
    {

        private IUsuarioArea areService;

        public UsuarioAreaService()
            : this(new UsuarioArea())
        {

        }
        public UsuarioAreaService(UsuarioArea ua)
            : base(ua)
        { }

        public UsuarioAreaService(IUsuarioArea service)
            : this(new UsuarioArea())
        {
            areService = service;
        }
        /*public void Remove(UsuarioAreaModel remove)
        {
            areService = areService == null ? areService : new UsuarioArea();
            SAX_USUARIO_AREA modelA = Mapper.Map<UsuarioAreaModel, SAX_USUARIO_AREA>(remove);
            areService.Remove(modelA);

        }*/

        public void CreateAndRemove(List<UsuarioAreaModel> create, List<int> remove)
        {
            areService = areService==null ? areService : new UsuarioArea();
            List<SAX_USUARIO_AREA> modelA = Mapper.Map<List<UsuarioAreaModel>, List<SAX_USUARIO_AREA>>(create);
            areService.CreateAndRemove(modelA, remove);
        }
    }
}
