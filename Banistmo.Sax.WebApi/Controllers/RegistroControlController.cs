﻿using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Common;
using System.Data.Entity;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Registro")]
    public class RegistroControlController : ApiController
    {
        private  IRegistroControlService service;
        private IUsuarioEmpresaService usuarioEmpresaService;
        private IEmpresaService empresaService;
        private  IOnlyRegistroControlService srvOnlyRegistroControl;
        private  IUserService userService;
        private  ICatalogoService catalagoService;
        private IUsuarioAreaService usuarioAreaService;
        private IAreaOperativaService areaOperativaService;
        private IParametroService paramService;
        private IEventosService eventoService;

        public RegistroControlController()
        {
            service = service ?? new RegistroControlService();
            srvOnlyRegistroControl = srvOnlyRegistroControl ?? new OnlyRegistroControlService();
            userService = new UserService();
            catalagoService = new CatalogoService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            usuarioAreaService = usuarioAreaService ?? new UsuarioAreaService();
            paramService = paramService ?? new ParametroService();
            eventoService = eventoService ?? new EventosService();
            usuarioEmpresaService = usuarioEmpresaService ?? new UsuarioEmpresaService();
            empresaService = empresaService ?? new EmpresaService();

        }

        //public RegistroControlController(IRegistroControlService rc, IOnlyRegistroControlService rcOnlyRegistro)
        //{
        //    service = rc;
        //    srvOnlyRegistroControl = rcOnlyRegistro;
        //    userService = new UserService();
        //}

        [Route("GetAllRegistro")]
        public IHttpActionResult GetAll()
        {
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var mdl = service.GetAll(c=>c.RC_ESTATUS_LOTE == activo).Select( c=> new {
                Registro = c.RC_REGISTRO_CONTROL,
                Area = c.CA_ID_AREA
            });
            if (mdl == null)
            {
                return NotFound();
            }
            return Ok(mdl);
        }

        [Route("GetRegistroByUser")]
        public IHttpActionResult GetRegistroByUser()
        {
            var userId = User.Identity.GetUserId();
            var mdl = service.GetAll(c=> c.RC_COD_USUARIO == userId,null,null);
            if (mdl == null)
            {
                return NotFound();
            }

            return Ok(mdl);
        }

        [Route("GetRegistroControlByUser")]
        public IHttpActionResult GetRegistroControlByUser()
        {
            var userId = User.Identity.GetUserId();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<OnlyRegistroControlModel> mdl = srvOnlyRegistroControl.GetAll(c => c.RC_COD_USUARIO == userId && c.RC_ESTATUS_LOTE== activo);
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE);
            if (mdl == null)
            {
                return NotFound();
            }

            return Ok(mdl.Select( x=> new  {
                RC_COD_OPERACION = x.RC_COD_OPERACION,
                RC_COD_PARTIDA = x.RC_COD_PARTIDA,
                RC_ARCHIVO = x.RC_ARCHIVO,
                RC_TOTAL_REGISTRO= x.RC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.RC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.RC_TOTAL_CREDITO,
                RC_TOTAL = x.RC_TOTAL,
                RC_ESTATUS_LOTE = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(e=>e.CD_TABLA.ToString()==x.RC_ESTATUS_LOTE),
                RC_FECHA_CREACION = x.RC_FECHA_CREACION!=null? x.RC_FECHA_CREACION.ToString("d/M/yyyy"): string.Empty,
                RC_HORA_CREACION = x.RC_FECHA_CREACION != null?  x.RC_FECHA_CREACION.ToString("hh:mm:tt"): string.Empty,
                RC_COD_USUARIO = UserName(x.RC_COD_USUARIO)
            }));
        }

        private string GetNameTipoOperacion(int id,  ref CatalogoModel model) {
            string name = string.Empty;
            if (model != null) {
                CatalogoDetalleModel cataloDetalle=model.SAX_CATALOGO_DETALLE.Where(x => x.CD_ESTATUS == id).FirstOrDefault();
                if (cataloDetalle != null)
                    name = cataloDetalle.CD_VALOR;
            }
            return name;
        }
        [Route("GetRegistroByUserPag")]
        public IHttpActionResult GetRegistroByUserPag([FromUri]PagingParameterModel pagingparametermodel, int tipoOperacion)
        {
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var ltsTipoOperacion = catalagoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var listAreaOperativa = areaOperativaService.GetAll();
            List<EventosModel> listaEvento = eventoService.Query(x => x.EV_COD_EVENTO == x.EV_COD_EVENTO).Select(y => new EventosModel() { EV_COD_EVENTO = y.EV_COD_EVENTO, EV_DESCRIPCION_EVENTO = y.EV_DESCRIPCION_EVENTO }).ToList();

            var fechaOperacion = DateTime.Now;
            if (pagingparametermodel.fechaCreacion == null)
            {
                var param = paramService.GetSingle();
                if (param != null)
                {
                    fechaOperacion = param.PA_FECHA_PROCESO;
                }
            }
            else {
                fechaOperacion = pagingparametermodel.fechaCreacion.Value;
            }
           
            var userId = User.Identity.GetUserId();
            var source = service.Query(c => c.RC_COD_USUARIO == userId 
                                        && c.RC_COD_OPERACION== tipoOperacion
                                        && DbFunctions.TruncateTime(c.RC_FECHA_PROCESO) 
                                        == DbFunctions.TruncateTime(fechaOperacion)).OrderBy(c=>c.RC_REGISTRO_CONTROL);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var listItem = items.Select((x, index) => new
            {
                RC_REGISTRO_CONTROL = x.RC_REGISTRO_CONTROL,
                RC_COD_OPERACION = GetNameTipoOperacion(x.RC_COD_OPERACION, ref ltsTipoOperacion),
                RC_COD_PARTIDA = x.RC_COD_PARTIDA,
                RC_ARCHIVO = x.RC_ARCHIVO,
                RC_TOTAL_REGISTRO = x.RC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.RC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.RC_TOTAL_CREDITO,
                RC_TOTAL = x.RC_TOTAL,
                COD_ESTATUS_LOTE = x.RC_ESTATUS_LOTE,
                RC_ESTATUS_LOTE = GetStatusRegistroControl(x.RC_ESTATUS_LOTE, estatusList),
                RC_FECHA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("M/d/yyyy") : string.Empty,
                RC_HORA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("hh:mm:tt") : string.Empty,
                RC_COD_USUARIO = UserName(x.RC_COD_USUARIO),
                AREA = GetNameAreaOperativa(x.CA_ID_AREA,ref listAreaOperativa),
                EVENTO= GetNameEvento(x.EV_COD_EVENTO, ref listaEvento),
                SELETED = false

            });
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage,
                data= listItem
            };
            
            return Ok(paginationMetadata);
        }


        [Route("GetRegistroByUserPagPorAprobar")]
        public IHttpActionResult GetRegistroByUserPagPorAprobar([FromUri]PagingParameterModel pagingparametermodel)
        {
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var ltsTipoOperacion = catalagoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            int porAprobar=  Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_APROBAR);
            int? masiva = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA);
            int? manual = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL);
            int activo= Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            List<EventosModel> listaEvento = eventoService.Query(x => x.EV_COD_EVENTO == x.EV_COD_EVENTO).Select(y => new EventosModel() { EV_COD_EVENTO = y.EV_COD_EVENTO, EV_DESCRIPCION_EVENTO = y.EV_DESCRIPCION_EVENTO }).ToList();
            if (pagingparametermodel.tipoOperacion != null)
            {
                masiva = pagingparametermodel.tipoOperacion;
                manual= pagingparametermodel.tipoOperacion;
            }
            var userId = User.Identity.GetUserId();

            List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
            List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
            List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();
            var source = service.Query(c => c.RC_ESTATUS_LOTE == porAprobar
                                                                        && (c.RC_COD_OPERACION == masiva || c.RC_COD_OPERACION == manual)
                                                                        && (pagingparametermodel.lote == null ? c.RC_COD_PARTIDA == c.RC_COD_PARTIDA : c.RC_COD_PARTIDA == pagingparametermodel.lote)
                                                                        && (pagingparametermodel.idCapturador == null ? c.RC_USUARIO_CREACION == c.RC_USUARIO_CREACION : c.RC_USUARIO_CREACION == pagingparametermodel.idCapturador)
                                                                        ).OrderBy(c => c.RC_REGISTRO_CONTROL);

            List<int> list_CE_ID_EMPRESA = usuarioEmpresaService.Query(x => x.US_ID_USUARIO == userId).Select(y => y.CE_ID_EMPRESA).ToList();
            if (list_CE_ID_EMPRESA != null && list_CE_ID_EMPRESA.Count() == 0)
                new Exception("El usuario actualmente no tiene empresas asignadas. Es necesario tener por lo menos una empresa asignada para poder aprobar el registro.");
            List<string> empresas = empresaService.Query(x => list_CE_ID_EMPRESA.Contains(x.CE_ID_EMPRESA) && x.CE_ESTATUS == activo.ToString()).Select(y => y.CE_COD_EMPRESA).ToList();
            if (empresas != null && empresas.Count() == 0)
                new Exception("No se encontraron empresas para su usuario.");

            if (source.Count() > 0)
               source= source.Where(c => listAreaUsuario.Contains(c.CA_ID_AREA.HasValue ? c.CA_ID_AREA.Value : 0)).OrderBy(c => c.RC_REGISTRO_CONTROL);
            if (source.Count() > 0)
                source = source.Where(c => c.SAX_PARTIDAS.All(p=> empresas.Contains(p.PA_COD_EMPRESA))).OrderBy(c => c.RC_REGISTRO_CONTROL);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var listItem = items.Select(x => new
            {
                RC_REGISTRO_CONTROL = x.RC_REGISTRO_CONTROL,
                RC_COD_OPERACION = GetNameTipoOperacion(x.RC_COD_OPERACION, ref ltsTipoOperacion),
                RC_COD_PARTIDA = x.RC_COD_PARTIDA,
                RC_ARCHIVO = x.RC_ARCHIVO,
                RC_TOTAL_REGISTRO = x.RC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.RC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.RC_TOTAL_CREDITO,
                RC_TOTAL = x.RC_TOTAL,
                COD_ESTATUS_LOTE = x.RC_ESTATUS_LOTE,
                RC_ESTATUS_LOTE = GetStatusRegistroControl(x.RC_ESTATUS_LOTE, estatusList),
                RC_FECHA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("M/d/yyyy") : string.Empty,
                RC_HORA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("hh:mm:tt") : string.Empty,
                RC_COD_USUARIO = UserName(x.RC_COD_USUARIO),
                AREA= x.CA_ID_AREA,
                EVENTO = GetNameEvento(x.EV_COD_EVENTO, ref listaEvento),
                SELETED =false
            });
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage,
                data = listItem
            };
            return Ok(paginationMetadata);
        }
        //Eliminar este metodo despues de validar que no se use 
        [Route("GetRegistroControlPorConciliar")]
        public IHttpActionResult GetRegistroControlPorConciliar([FromUri]PagingRegistroControlModel pagingparametermodel)
        {
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus_carga", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            var ltsTipoOperacion = catalagoService.GetAll(c => c.CA_TABLA == "sax_tipo_operacion", null, c => c.SAX_CATALOGO_DETALLE).FirstOrDefault();
            int porConciliar = Convert.ToInt16(BusinessEnumerations.EstatusCarga.POR_CONCILIAR);
            int manual = Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL);
            var userId = User.Identity.GetUserId();
            var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == userId);
            var userAreacod = areaOperativaService.GetSingle(d => d.CA_ID_AREA == userArea.CA_ID_AREA);
            var source = service.Query(c => c.RC_ESTATUS_LOTE == porConciliar
                                       && (pagingparametermodel.lote==null? c.RC_COD_PARTIDA== c.RC_COD_PARTIDA: c.RC_COD_PARTIDA== (pagingparametermodel.lote.Trim()))
                                       && (pagingparametermodel.idCapturador == null ? c.RC_USUARIO_CREACION == c.RC_USUARIO_CREACION : c.RC_USUARIO_CREACION == pagingparametermodel.idCapturador)).OrderBy(c=>c.RC_REGISTRO_CONTROL);
            int count = source.Count();
            int CurrentPage = pagingparametermodel.pageNumber;
            int PageSize = pagingparametermodel.pageSize;
            int TotalCount = count;
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            var items = source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var previousPage = CurrentPage > 1 ? "Yes" : "No";
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";
            var listItem = items.Select(x => new
            {
                RC_REGISTRO_CONTROL = x.RC_REGISTRO_CONTROL,
                RC_COD_OPERACION = GetNameTipoOperacion(x.RC_COD_OPERACION, ref ltsTipoOperacion),
                RC_COD_PARTIDA = x.RC_COD_PARTIDA,
                RC_ARCHIVO = x.RC_ARCHIVO,
                RC_TOTAL_REGISTRO = x.RC_TOTAL_REGISTRO,
                RC_TOTAL_DEBITO = x.RC_TOTAL_DEBITO,
                RC_TOTAL_CREDITO = x.RC_TOTAL_CREDITO,
                RC_TOTAL = x.RC_TOTAL,
                COD_ESTATUS_LOTE = x.RC_ESTATUS_LOTE,
                RC_ESTATUS_LOTE = GetStatusRegistroControl(x.RC_ESTATUS_LOTE, estatusList),
                RC_FECHA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("d/M/yyyy") : string.Empty,
                RC_HORA_CREACION = x.RC_FECHA_CREACION != null ? x.RC_FECHA_CREACION.ToString("hh:mm:tt") : string.Empty,
                RC_COD_USUARIO = UserName(x.RC_USUARIO_MOD)
            });
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage,
                data = listItem
            };
            return Ok(paginationMetadata);
        }

        [Route("GetRegistroById")]
        public IHttpActionResult Get(int id)
        {
            var model = service.GetSingle(c => c.RC_REGISTRO_CONTROL == id);

            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        public IHttpActionResult Post([FromBody] RegistroControlModel model)
        {
            model.RC_USUARIO_CREACION = User.Identity.GetUserId();
            model.RC_FECHA_CREACION = DateTime.Now;
            return Ok(service.Insert(model, true));
        }

        [Route("UpdateRegistro"), HttpPost]
        public IHttpActionResult Put([FromBody]RegistroControlModel model)
        {
            model.RC_USUARIO_MOD = User.Identity.GetUserId();
            model.RC_FECHA_MOD = DateTime.Now;
            service.Update(model);
            return Ok();
        }


        [Route("UpdateEstatoRegistro"), HttpPost]
        public IHttpActionResult updateStatus([FromBody]UpdateStatusRegistroControl model)
        {
            if (model == null) {
                return BadRequest("El registro control esta vacio");
            }
            RegistroControlModel registroControl = service.GetSingle(r=>r.RC_REGISTRO_CONTROL== model.RC_REGISTRO_CONTROL);
            if (registroControl != null)
            {
                registroControl.RC_ESTATUS_LOTE = model.RC_ESTATUS_LOTE;
                registroControl.RC_USUARIO_MOD = User.Identity.GetUserId();
                registroControl.RC_FECHA_MOD = DateTime.Now;
                service.Update(registroControl);
                return Ok();
            }
            else {
                return BadRequest("No se  encontro el registro control con el ID:"+model.RC_REGISTRO_CONTROL);
            }
        
        }

        [Route("AprobarRegistro"), HttpPost]
        public IHttpActionResult AprobarRegistro([FromUri]int id)
        {
            string idUser = User.Identity.GetUserId();
            int activo = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            var control = service.Count(c => c.RC_REGISTRO_CONTROL == id);
            List<int> list_CE_ID_EMPRESA= usuarioEmpresaService.Query(x => x.US_ID_USUARIO == idUser).Select(y=>y.CE_ID_EMPRESA).ToList();
            if (list_CE_ID_EMPRESA != null && list_CE_ID_EMPRESA.Count() == 0)
                return BadRequest("El usuario actualmente no tiene empresas asignadas. Es necesario tener por lo menos una empresa asignada para poder aprobar el registro.");
            List<string> empresas = empresaService.Query(x => list_CE_ID_EMPRESA.Contains(x.CE_ID_EMPRESA) && x.CE_ESTATUS== activo.ToString()).Select(y => y.CE_COD_EMPRESA).ToList();
            if (empresas != null && empresas.Count() == 0)
                return BadRequest("No se encontraron empresas para su usuario.");

            if (control > 0)
            {
                try
                {
                    service.AprobarRegistro(id, User.Identity.GetUserId(), empresas);
                    return Ok();
                }catch(Exception e){
                    return BadRequest(e.Message);
                }
            }
            else
                return NotFound();
        }


        [Route("RechazarRegistro"), HttpPost]
        public IHttpActionResult RechazarRegistro([FromUri]int id)
        {
            var control = service.Count(c => c.RC_REGISTRO_CONTROL == id);
            if (control > 0)
            {
                service.RechazarRegistro(id, User.Identity.GetUserId());
                return Ok();
            }
            else
                return NotFound();
        }

        [Route("RegistrarCargaManual"), HttpPost]
        public IHttpActionResult CargaManual(PartidaManualModel model)
        {
            var registroControl = new RegistroControlModel();
            var userId = User.Identity.GetUserId();
            var userArea = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == userId);
            //
            model.CA_ID_AREA = userArea.CA_ID_AREA;
            registroControl.CA_ID_AREA = model.RC_COD_AREA;
            registroControl.RC_COD_EVENTO = model.PA_EVENTO;
            registroControl.RC_COD_USUARIO = User.Identity.GetUserId();
            var result=service.CreateSinglePartidas(registroControl, model, Convert.ToInt16(BusinessEnumerations.TipoOperacion.CAPTURA_MANUAL));
            return Ok(result);
        }

        [Route("RemoverRegistro"), HttpPost]
        public IHttpActionResult RemoverRegistro(int id)
        {
            var control = service.Count(c => c.RC_REGISTRO_CONTROL == id);
            if (control != 0)
            {
                service.removeRegistro(id);
                return Ok();
            }
            else
                return NotFound();
        }

        private string UserName(string id) {
            string result = string.Empty;
            AspNetUserModel usuario = userService.GetSingle(u => u.Id == id);
            if(usuario!=null)
                result = usuario.FirstName;
            return result;

        }

        private string GetStatusRegistroControl(int idStatus, CatalogoModel model) {
            string result = string.Empty;
            if (model != null) {
                var modelCatalogoDetalle=model.SAX_CATALOGO_DETALLE.Where(x=>x.CD_ESTATUS== idStatus).FirstOrDefault();
                if (modelCatalogoDetalle != null)
                    result= modelCatalogoDetalle.CD_VALOR;
            }
            return result;
        }

        private string GetNameAreaOperativa(int? idArea, ref List<AreaOperativaModel> model)
        {
            string result = string.Empty;
            if (idArea == null)
                return result;
            if (model != null)
            {
                var modeloResult = model.FirstOrDefault(x=>x.CA_ID_AREA == idArea);
                if (modeloResult != null)
                    result = modeloResult.CA_COD_AREA+"-"+modeloResult.CA_NOMBRE;
            }
            return result;
        }

        private string GetNameEvento(int? codEvento, ref List<EventosModel> model)
        {
            string result = string.Empty;
            if (codEvento == null)
                return result;
            if (model != null)
            {
                var modeloResult = model.FirstOrDefault(x => x.EV_COD_EVENTO == codEvento);
                if (modeloResult != null)
                    result = modeloResult.EV_COD_EVENTO + "-" + modeloResult.EV_DESCRIPCION_EVENTO;
            }
            return result;
        }

    }
}
