using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Banistmo.Sax.Services.Implementations.Business;

namespace Banistmo.Sax.WebApi.Controllers
{
    [RoutePrefix("api/Parametro")]
    public class ParametroController : ApiController
    {
        //Variables
        private readonly IParametroService paramService;
        private readonly IParametroTempService paramTempService;
        private readonly ICatalogoService catalagoService;
        private readonly IEventosTempService eventService;
        private readonly ISupervisorTempService supervisorService;
        private ApplicationUserManager _userManager;

        //Constructores
        public ParametroController()
        {
            paramService = paramService ?? new ParametroService();
            paramTempService = paramTempService ?? new ParametroTempService();
            catalagoService = catalagoService ?? new CatalogoService();
            eventService = eventService ?? new EventosTemporalService();
            supervisorService = supervisorService ?? new SupervisorTempService();
        }
        public ParametroController(IParametroService objParamService, IParametroTempService objParamTempService, ICatalogoService objCatalogoService, IEventosTempService objEventService, ISupervisorTempService objSupervisorService)
        {
            paramService = objParamService;
            paramTempService = objParamTempService;
            catalagoService = objCatalogoService;
            eventService = objEventService;
            supervisorService = objSupervisorService;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //Metodos
        public IHttpActionResult Get([FromUri]AprobacionParametrosModel model)
        {

            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_frecuencia", null, c => c.SAX_CATALOGO_DETALLE);


            if (model == null)
            {
                model = new AprobacionParametrosModel();
                model.FechaCreacion = null;
                model.UsuarioCreacion = null;
            }
            int yyyy = 0;
            int mm = 0;
            int dd = 0;
            DateTime dt = DateTime.Today;
            if (model.FechaCreacion != null)
            {
                mm = Convert.ToInt32(model.FechaCreacion.ToString().Substring(0, 2));
                dd = Convert.ToInt32(model.FechaCreacion.ToString().Substring(3, 2));
                yyyy = Convert.ToInt32(model.FechaCreacion.ToString().Substring(6, 4));
                dt = new DateTime(yyyy, mm, dd);
                dt = dt.AddDays(1);
            }

            var objParamService = paramService.GetAll(c =>
            c.PA_FECHA_CREACION >= (model.FechaCreacion == null ? c.PA_FECHA_CREACION : model.FechaCreacion)
            && c.PA_FECHA_CREACION <= (model.FechaCreacion == null ? c.PA_FECHA_CREACION : dt)
            && c.PA_USUARIO_CREACION == (model.UsuarioCreacion == null ? c.PA_USUARIO_CREACION : model.UsuarioCreacion), null, includes: c => c.AspNetUsers);

            if (objParamService == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }
            return Ok(objParamService.Select(c => new
            {
                PA_ID_PARAMETRO = c.PA_ID_PARAMETRO,
                PA_FECHA_PROCESO = c.PA_FECHA_PROCESO,
                PA_FRECUENCIA = c.PA_FRECUENCIA,
                PA_FRECUENCIA_DESC = c.PA_FRECUENCIA != 0 ? estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ID_CATALOGO_DETALLE == c.PA_FRECUENCIA).CD_VALOR : null,
                PA_HORA_EJECUCION = c.PA_HORA_EJECUCION,
                PA_RUTA_CONTABLE = c.PA_RUTA_CONTABLE,
                PA_RUTA_TEMPORAL = c.PA_RUTA_TEMPORAL,
                PA_FRECUENCIA_LIMPIEZA = c.PA_FRECUENCIA_LIMPIEZA,
                PA_FRECUENCIA_LIMPIEZA_DESC = c.PA_FRECUENCIA_LIMPIEZA != 0 ? estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ID_CATALOGO_DETALLE == c.PA_FRECUENCIA_LIMPIEZA).CD_VALOR : null,
                PA_ESTATUS = c.PA_ESTATUS,
                PA_FECHA_CREACION = c.PA_FECHA_CREACION,
                PA_USUARIO_CREACION = c.PA_USUARIO_CREACION,
                PA_USUARIO_CREACION_NOMBRE = c.AspNetUsers.FirstName,
                PA_FECHA_MOD = c.PA_FECHA_MOD,
                PA_USUARIO_MOD = c.PA_USUARIO_MOD,
                PA_USUARIO_MOD_NOMBRE = c.AspNetUsers2 != null ? c.AspNetUsers2.FirstName : null,
                PA_FECHA_APROBACION = c.PA_FECHA_APROBACION,
                PA_USUARIO_APROBADOR = c.PA_USUARIO_APROBADOR,
                PA_USUARIO_APROBADOR_NOMBRE = c.AspNetUsers1 != null ? c.AspNetUsers1.FirstName : null
            }));
        }
        public IHttpActionResult Get(int id)
        {
            var param = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (param != null)
            {
                return Ok(new
                {
                    PA_ID_PARAMETRO = param.PA_ID_PARAMETRO,
                    PA_FECHA_PROCESO = param.PA_FECHA_PROCESO,
                    PA_FRECUENCIA = param.PA_FRECUENCIA,
                    PA_HORA_EJECUCION = param.PA_HORA_EJECUCION,
                    PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE,
                    PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL,
                    PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA,
                    PA_ESTATUS = param.PA_ESTATUS,
                    PA_FECHA_CREACION = param.PA_FECHA_CREACION,
                    PA_USUARIO_CREACION = param.PA_USUARIO_CREACION,
                    PA_USUARIO_CREACION_NOMBRE = param.AspNetUsers.FirstName,
                    PA_FECHA_MOD = param.PA_FECHA_MOD,
                    PA_USUARIO_MOD = param.PA_USUARIO_MOD,
                    PA_USUARIO_MOD_NOMBRE = param.AspNetUsers2 != null ? param.AspNetUsers2.FirstName : null,
                    PA_FECHA_APROBACION = param.PA_FECHA_APROBACION,
                    PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR,
                    PA_USUARIO_APROBADOR_NOMBRE = param.AspNetUsers1 != null ? param.AspNetUsers1.FirstName : null

                });
            }
            return BadRequest("No se encontraron registros para la consulta realizada.");
        }
        public async Task<IHttpActionResult> Post([FromBody] ParametroModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            model.PA_USUARIO_CREACION = user.Id;
            model.PA_FECHA_CREACION = DateTime.Now;
            var parametro = paramService.InsertParametro(model);
            return Ok(parametro);
        }
        [Route("UpdateParametro"), HttpPost]
        public async Task<IHttpActionResult> Put([FromBody] ParametroModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            // Se obtiene el parametro y se actualiza la fecha de modificación y el estatus
            var param = paramService.GetSingle(c => c.PA_ID_PARAMETRO == model.PA_ID_PARAMETRO);
            param.PA_USUARIO_MOD = user.Id;
            param.PA_FECHA_MOD = DateTime.Now;
            param.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Pendiente);
            param.PA_FECHA_PROCESO = model.PA_FECHA_PROCESO;
            param.PA_FRECUENCIA = model.PA_FRECUENCIA;
            param.PA_HORA_EJECUCION = model.PA_HORA_EJECUCION;
            param.PA_RUTA_CONTABLE = model.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = model.PA_RUTA_TEMPORAL;
            param.PA_FRECUENCIA_LIMPIEZA = model.PA_FRECUENCIA_LIMPIEZA;

            paramService.Update(param);
            // Se obtiene el parametro temporal para luego actualizarlo con el parametro
            var paramTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == model.PA_ID_PARAMETRO);
            paramTemp = MappingTempFromParam(paramTemp, param);
            paramTemp.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.PorAprobar);
            paramTempService.Update(paramTemp);
            return Ok();
        }
        public IHttpActionResult Delete(int id)
        {
            var diaFeriado = paramService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (diaFeriado == null)
            {
                return NotFound();
            }

            diaFeriado.PA_FECHA_MOD = DateTime.Now;
            diaFeriado.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Eliminado);
            paramService.Update(diaFeriado);
            return Ok();

        }
        [Route("AprobarParametro"), HttpPost]
        public async Task<IHttpActionResult> PutAprobarParametro([FromBody] AprobacionModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var tempModel = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == model.id);
            if (tempModel != null)
            {
                tempModel.PA_FECHA_APROBACION = DateTime.Now;
                tempModel.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                tempModel.PA_USUARIO_APROBADOR = user.Id;
                paramTempService.Update(tempModel);
                ParametroModel param = new ParametroModel();
                param = MappingParamFromTemp(param, tempModel);

                paramService.Update(param);
                return Ok("El parámetro ha sido aprobado.");
            }
            return BadRequest("No se encontraron datos para actualizar.");
        }
        [Route("RechazarParametro"), HttpPost]
        public async Task<IHttpActionResult> PutRechazarParametro([FromBody] AprobacionModel model)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var paramModel = paramService.GetSingle(c => c.PA_ID_PARAMETRO == model.id);
            if (paramModel != null)
            {
                paramModel.PA_USUARIO_MOD = user.Id;
                paramModel.PA_FECHA_MOD = DateTime.Now;
                paramModel.PA_ESTATUS = Convert.ToInt16(RegistryStateModel.RegistryState.Aprobado);
                paramService.Update(paramModel);
                var paramTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == model.id);
                paramTemp = MappingTempFromParam(paramTemp, paramModel);

                paramTempService.Update(paramTemp);
                return Ok("El parámetro ha sido rechazado.");
            }
            return BadRequest("No se encontraron datos para actualizar.");
        }
        [Route("GetTemp"), HttpGet]
        public IHttpActionResult GetTemp([FromUri]AprobacionParametrosModel model)
        {
            var estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_frecuencia", null, c => c.SAX_CATALOGO_DETALLE);


            if (model == null)
            {
                model = new AprobacionParametrosModel();
                model.FechaCreacion = null;
                model.UsuarioCreacion = null;
            }
            int yyyy = 0;
            int mm = 0;
            int dd = 0;
            DateTime dt = DateTime.Today;
            if (model.FechaCreacion != null)
            {
                mm = Convert.ToInt32(model.FechaCreacion.ToString().Substring(0, 2));
                dd = Convert.ToInt32(model.FechaCreacion.ToString().Substring(3, 2));
                yyyy = Convert.ToInt32(model.FechaCreacion.ToString().Substring(6, 4));
                dt = new DateTime(yyyy, mm, dd);
                dt = dt.AddDays(1);
            }

            var objParametroTempService = paramTempService.GetAll(c => c.PA_ESTATUS == 2
            && c.PA_FECHA_CREACION >= (model.FechaCreacion == null ? c.PA_FECHA_CREACION : model.FechaCreacion)
            && c.PA_FECHA_CREACION <= (model.FechaCreacion == null ? c.PA_FECHA_CREACION : dt)
            && c.PA_USUARIO_CREACION == (model.UsuarioCreacion == null ? c.PA_USUARIO_CREACION : model.UsuarioCreacion), null, includes: c => c.AspNetUsers);
            if (objParametroTempService == null)
            {
                return NotFound();
            }
            return Ok(objParametroTempService.Select(c => new
            {
                PA_ID_PARAMETRO = c.PA_ID_PARAMETRO,
                PA_FECHA_PROCESO = c.PA_FECHA_PROCESO,
                PA_FRECUENCIA = c.PA_FRECUENCIA,
                PA_FRECUENCIA_DESC = c.PA_FRECUENCIA != 0 ? estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ID_CATALOGO_DETALLE == c.PA_FRECUENCIA).CD_VALOR : null,
                PA_HORA_EJECUCION = c.PA_HORA_EJECUCION,
                PA_RUTA_CONTABLE = c.PA_RUTA_CONTABLE,
                PA_RUTA_TEMPORAL = c.PA_RUTA_TEMPORAL,
                PA_FRECUENCIA_LIMPIEZA = c.PA_FRECUENCIA_LIMPIEZA,
                //PA_FRECUENCIA_LIMPIEZA_DESC = c.PA_FRECUENCIA_LIMPIEZA != 0 ? estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ID_CATALOGO_DETALLE == c.PA_FRECUENCIA_LIMPIEZA).CD_VALOR : null,
                PA_ESTATUS = c.PA_ESTATUS,
                PA_FECHA_CREACION = c.PA_FECHA_CREACION,
                PA_USUARIO_CREACION = c.PA_USUARIO_CREACION,
                PA_USUARIO_CREACION_NOMBRE = c.AspNetUsers.FirstName,
                PA_FECHA_MOD = c.PA_FECHA_MOD,
                PA_USUARIO_MOD = c.PA_USUARIO_MOD,
                PA_USUARIO_MOD_NOMBRE = c.AspNetUsers2 != null ? c.AspNetUsers2.FirstName : null,
                PA_FECHA_APROBACION = c.PA_FECHA_APROBACION,
                PA_USUARIO_APROBADOR = c.PA_USUARIO_APROBADOR,
                PA_USUARIO_APROBADOR_NOMBRE = c.AspNetUsers1 != null ? c.AspNetUsers1.FirstName : null
            }));
        }
        [Route("GetTempById")]
        public IHttpActionResult GetTemp(int id)
        {
            var parametroTemp = paramTempService.GetSingle(c => c.PA_ID_PARAMETRO == id);

            if (parametroTemp != null)
            {
                return Ok(new
                {
                    PA_ID_PARAMETRO = parametroTemp.PA_ID_PARAMETRO,
                    PA_FECHA_PROCESO = parametroTemp.PA_FECHA_PROCESO,
                    PA_FRECUENCIA = parametroTemp.PA_FRECUENCIA,
                    PA_HORA_EJECUCION = parametroTemp.PA_HORA_EJECUCION,
                    PA_RUTA_CONTABLE = parametroTemp.PA_RUTA_CONTABLE,
                    PA_RUTA_TEMPORAL = parametroTemp.PA_RUTA_TEMPORAL,
                    PA_FRECUENCIA_LIMPIEZA = parametroTemp.PA_FRECUENCIA_LIMPIEZA,
                    PA_ESTATUS = parametroTemp.PA_ESTATUS,
                    PA_FECHA_CREACION = parametroTemp.PA_FECHA_CREACION,
                    PA_USUARIO_CREACION = parametroTemp.PA_USUARIO_CREACION,
                    PA_USUARIO_CREACION_NOMBRE = parametroTemp.AspNetUsers.FirstName,
                    PA_FECHA_MOD = parametroTemp.PA_FECHA_MOD,
                    PA_USUARIO_MOD = parametroTemp.PA_USUARIO_MOD,
                    PA_USUARIO_MOD_NOMBRE = parametroTemp.AspNetUsers2 != null ? parametroTemp.AspNetUsers2.FirstName : null,
                    PA_FECHA_APROBACION = parametroTemp.PA_FECHA_APROBACION,
                    PA_USUARIO_APROBADOR = parametroTemp.PA_USUARIO_APROBADOR,
                    PA_USUARIO_APROBADOR_NOMBRE = parametroTemp.AspNetUsers1 != null ? parametroTemp.AspNetUsers1.FirstName : null

                });
            }
            return BadRequest("No se encontraron registros para la consulta realizada.");
        }
        [Route("ReporteParametro"), HttpGet]
        public IHttpActionResult GetReporte([FromUri] ReporteParametroModel model)
        {
            if (model == null)
            {
                model = new ReporteParametroModel();
                model.FechaProceso = null;
                model.Frecuencia = null;
                model.FrecuenciaLimpieza = null;
                model.HoraEjecucion = null;
                model.RutaContable = null;
                model.RutaTemporal = null;
            }
            int estado = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            IList<ParametroModel> objParametroList
                = paramService.GetAll(f => f.PA_ESTATUS == estado
                && f.PA_FECHA_PROCESO == (model.FechaProceso == null ? f.PA_FECHA_PROCESO : model.FechaProceso)
                && f.PA_FRECUENCIA == (model.Frecuencia == null ? f.PA_FRECUENCIA : model.Frecuencia)
                && f.PA_HORA_EJECUCION == (model.HoraEjecucion == null ? f.PA_HORA_EJECUCION : model.HoraEjecucion)
                && f.PA_RUTA_CONTABLE == (model.RutaContable == null ? f.PA_RUTA_CONTABLE : model.RutaContable)
                && f.PA_RUTA_TEMPORAL == (model.RutaTemporal == null ? f.PA_RUTA_TEMPORAL : model.RutaTemporal)
                && f.PA_FRECUENCIA_LIMPIEZA == (model.FrecuenciaLimpieza == null ? f.PA_FRECUENCIA_LIMPIEZA : model.FrecuenciaLimpieza),
                null, includes: c => c.AspNetUsers);
            if (objParametroList == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(objParametroList.Select(c => new
            {
                PA_ID_PARAMETRO = c.PA_ID_PARAMETRO,
                PA_FECHA_PROCESO = c.PA_FECHA_PROCESO,
                PA_FRECUENCIA = c.PA_FRECUENCIA,
                PA_HORA_EJECUCION = c.PA_HORA_EJECUCION,
                PA_RUTA_CONTABLE = c.PA_RUTA_CONTABLE,
                PA_RUTA_TEMPORAL = c.PA_RUTA_TEMPORAL,
                PA_FRECUENCIA_LIMPIEZA = c.PA_FRECUENCIA_LIMPIEZA,
                PA_ESTATUS = c.PA_ESTATUS,
                PA_FECHA_CREACION = c.PA_FECHA_CREACION,
                PA_USUARIO_CREACION = c.PA_USUARIO_CREACION,
                PA_USUARIO_CREACION_NOMBRE = c.AspNetUsers != null ? c.AspNetUsers.FirstName : null,
                PA_FECHA_MOD = c.PA_FECHA_MOD,
                PA_USUARIO_MOD = c.PA_USUARIO_MOD,
                PA_USUARIO_MOD_NOMBRE = c.AspNetUsers2 != null ? c.AspNetUsers2.FirstName : null,
                PA_FECHA_APROBACION = c.PA_FECHA_APROBACION,
                PA_USUARIO_APROBADOR = c.PA_USUARIO_APROBADOR,
                PA_USUARIO_APROBADOR_NOMBRE = c.AspNetUsers1 != null ? c.AspNetUsers1.FirstName : null
            }));
        }
        [Route("ReporteAprobaciones"), HttpGet]
        public IHttpActionResult GetReporteAprobaciones([FromUri] AprobacionParametrosModel model)
        {
            List<CatalogoModel> estatusList = catalagoService.GetAll(c => c.CA_TABLA == "sax_estatus", null, c => c.SAX_CATALOGO_DETALLE).ToList();

            if (model == null)
            {
                model = new AprobacionParametrosModel();
                model.FechaCreacion = null;
                model.UsuarioCreacion = null;
            }

            int yyyy = 0;
            int mm = 0;
            int dd = 0;
            DateTime dt = DateTime.Today;
            if (model.FechaCreacion != null)
            {
                mm = Convert.ToInt32(model.FechaCreacion.ToString().Substring(0, 2));
                dd = Convert.ToInt32(model.FechaCreacion.ToString().Substring(3, 2));
                yyyy = Convert.ToInt32(model.FechaCreacion.ToString().Substring(6, 4));
                dt = new DateTime(yyyy, mm, dd);
                dt = dt.AddDays(1);
            }

            int estado = Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
            IList<ParametroTempModel> objParametroList 
                = paramTempService.GetAll(f => // f.PA_ESTATUS == estado && 
                 f.PA_FECHA_CREACION >= (model.FechaCreacion == null ? f.PA_FECHA_CREACION : model.FechaCreacion)
                 && f.PA_FECHA_CREACION <= (model.FechaCreacion == null ? f.PA_FECHA_CREACION : dt)
                 && f.PA_USUARIO_CREACION == (model.UsuarioCreacion == null ? f.PA_USUARIO_CREACION : model.UsuarioCreacion),
                 null, includes: c => c.AspNetUsers);

            var listParam = objParametroList.Select(c => new
            {
                Tipo = "Parametro Sistema",
                Descripcion = "Parametro No. " + c.PA_ID_PARAMETRO,
                Estado = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.PA_ESTATUS).CD_VALOR,
                FechaCreacion = c.PA_FECHA_CREACION,
                UsuarioCreacion = c.PA_USUARIO_CREACION,
                UsuarioCreacion_Nombre= c.AspNetUsers.FirstName,
                FechaAprobacion = c.PA_FECHA_APROBACION,
                UsuarioAprobador = c.PA_USUARIO_APROBADOR,
                UsuarioAprobador_Nombre = c.AspNetUsers1 != null ? c.AspNetUsers1.FirstName : null
            });


            IList<EventosTempModel> objEventosList
                = eventService.GetAll(f => //f.EV_ESTATUS == estado && 
                 f.EV_FECHA_CREACION >= (model.FechaCreacion == null ? f.EV_FECHA_CREACION : model.FechaCreacion)
                 && f.EV_FECHA_CREACION <= (model.FechaCreacion == null ? f.EV_FECHA_CREACION : dt)
                 && f.EV_USUARIO_CREACION == (model.UsuarioCreacion == null ? f.EV_USUARIO_CREACION : model.UsuarioCreacion),
                 null, includes: c => c.AspNetUsers);

            var listEvento = objEventosList.Select(c => new
            {
                Tipo = "Evento",
                Descripcion = "Evento No. " + c.EV_COD_EVENTO,
                Estado = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.EV_ESTATUS).CD_VALOR,
                FechaCreacion = Convert.ToDateTime( c.EV_FECHA_CREACION),
                UsuarioCreacion = c.EV_USUARIO_CREACION,
                UsuarioCreacion_Nombre = c.AspNetUsers.FirstName,
                FechaAprobacion = c.EV_FECHA_APROBACION,
                UsuarioAprobador = c.EV_USUARIO_APROBADOR,
                UsuarioAprobador_Nombre = c.AspNetUsers2 != null ? c.AspNetUsers2.FirstName : null
            });


            IList<SupervisorTempModel> objSupervisorList
                = supervisorService.GetAll(f => //f.SV_ESTATUS == estadov&& 
                 f.SV_FECHA_CREACION >= (model.FechaCreacion == null ? f.SV_FECHA_CREACION : model.FechaCreacion)
                 && f.SV_FECHA_CREACION <= (model.FechaCreacion == null ? f.SV_FECHA_CREACION : dt)
                 && f.SV_USUARIO_CREACION == (model.UsuarioCreacion == null ? f.SV_USUARIO_CREACION : model.UsuarioCreacion),
                 null, includes: c => c.AspNetUsers);

            var listSupervisor = objSupervisorList.Select(c => new
            {
                Tipo = "Supervisor",
                Descripcion = "Supervisor No. " + c.SV_ID_SUPERVISOR,
                Estado = estatusList.FirstOrDefault().SAX_CATALOGO_DETALLE.FirstOrDefault(k => k.CD_ESTATUS == c.SV_ESTATUS).CD_VALOR,
                FechaCreacion = c.SV_FECHA_CREACION,
                UsuarioCreacion = c.SV_USUARIO_CREACION,
                UsuarioCreacion_Nombre = c.AspNetUsers1.FirstName,
                FechaAprobacion = c.SV_FECHA_APROBACION,
                UsuarioAprobador = c.SV_USUARIO_APROBADOR,
                UsuarioAprobador_Nombre = c.AspNetUsers != null ? c.AspNetUsers.FirstName : null
            });
            
            var listFinal = listParam.Concat(listEvento).Concat(listSupervisor);
            

            if (listFinal == null)
            {
                return BadRequest("No se encontraron registros para la consulta realizada.");
            }

            return Ok(listFinal.Select(c => new
            {
                Tipo = c.Tipo,
                Descripcion = c.Descripcion,
                Estado = c.Estado,
                FechaCreacion = c.FechaCreacion,
                UsuarioCreacion = c.UsuarioCreacion,
                UsuarioCreacion_Nombre = c.UsuarioCreacion_Nombre,
                FechaAprobacion = c.FechaAprobacion,
                UsuarioAprobador = c.UsuarioAprobador,
                UsuarioAprobador_Nombre = c.UsuarioAprobador_Nombre
            }));
        }

        //Mapping
        private ParametroModel MappingParamFromTemp(ParametroTempModel paramTemp)
        {
            var param = new ParametroModel();

            param.PA_ESTATUS = 1;
            param.PA_FECHA_APROBACION = paramTemp.PA_FECHA_APROBACION;
            param.PA_FECHA_CREACION = paramTemp.PA_FECHA_CREACION;
            param.PA_FECHA_MOD = paramTemp.PA_FECHA_MOD;
            param.PA_FECHA_PROCESO = paramTemp.PA_FECHA_PROCESO;
            param.PA_HORA_EJECUCION = paramTemp.PA_HORA_EJECUCION;
            param.PA_ID_PARAMETRO = paramTemp.PA_ID_PARAMETRO;
            param.PA_RUTA_CONTABLE = paramTemp.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = paramTemp.PA_RUTA_TEMPORAL;
            param.PA_USUARIO_APROBADOR = paramTemp.PA_USUARIO_APROBADOR;
            param.PA_USUARIO_CREACION = paramTemp.PA_USUARIO_CREACION;
            param.PA_USUARIO_MOD = paramTemp.PA_USUARIO_MOD;

            param.PA_FRECUENCIA = paramTemp.PA_FRECUENCIA;
            param.PA_FRECUENCIA_LIMPIEZA = paramTemp.PA_FRECUENCIA_LIMPIEZA;
            return param;
        }
        private ParametroModel MappingParamFromTemp(ParametroModel param, ParametroTempModel paramTemp)
        {
            param.PA_ESTATUS = 1;
            param.PA_FECHA_APROBACION = paramTemp.PA_FECHA_APROBACION;
            param.PA_FECHA_CREACION = paramTemp.PA_FECHA_CREACION;
            param.PA_FECHA_MOD = paramTemp.PA_FECHA_MOD;
            param.PA_FECHA_PROCESO = paramTemp.PA_FECHA_PROCESO;
            param.PA_HORA_EJECUCION = paramTemp.PA_HORA_EJECUCION;
            param.PA_ID_PARAMETRO = paramTemp.PA_ID_PARAMETRO;
            param.PA_RUTA_CONTABLE = paramTemp.PA_RUTA_CONTABLE;
            param.PA_RUTA_TEMPORAL = paramTemp.PA_RUTA_TEMPORAL;
            param.PA_USUARIO_APROBADOR = paramTemp.PA_USUARIO_APROBADOR;
            param.PA_USUARIO_CREACION = paramTemp.PA_USUARIO_CREACION;
            param.PA_USUARIO_MOD = paramTemp.PA_USUARIO_MOD;

            param.PA_FRECUENCIA = paramTemp.PA_FRECUENCIA;
            param.PA_FRECUENCIA_LIMPIEZA = paramTemp.PA_FRECUENCIA_LIMPIEZA;

            return param;
        }
        private ParametroTempModel MappingTempFromParam(ParametroModel param)
        {
            var paramT = new ParametroTempModel();

            paramT.PA_ESTATUS = 1;
            paramT.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            paramT.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            paramT.PA_FECHA_MOD = DateTime.Today;
            paramT.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            paramT.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            paramT.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            paramT.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            paramT.PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL;
            paramT.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            paramT.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            paramT.PA_USUARIO_MOD = param.PA_USUARIO_MOD;

            paramT.PA_FRECUENCIA = param.PA_FRECUENCIA;
            paramT.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            return paramT;
        }
        private ParametroTempModel MappingTempFromParam(ParametroTempModel paramT, ParametroModel param)
        {
            paramT.PA_ESTATUS = 1;
            paramT.PA_FECHA_APROBACION = param.PA_FECHA_APROBACION;
            paramT.PA_FECHA_CREACION = param.PA_FECHA_CREACION;
            paramT.PA_FECHA_MOD = DateTime.Today;
            paramT.PA_FECHA_PROCESO = param.PA_FECHA_PROCESO;
            paramT.PA_HORA_EJECUCION = param.PA_HORA_EJECUCION;
            paramT.PA_ID_PARAMETRO = param.PA_ID_PARAMETRO;
            paramT.PA_RUTA_CONTABLE = param.PA_RUTA_CONTABLE;
            paramT.PA_RUTA_TEMPORAL = param.PA_RUTA_TEMPORAL;
            paramT.PA_USUARIO_APROBADOR = param.PA_USUARIO_APROBADOR;
            paramT.PA_USUARIO_CREACION = param.PA_USUARIO_CREACION;
            paramT.PA_USUARIO_MOD = param.PA_USUARIO_MOD;

            paramT.PA_FRECUENCIA = param.PA_FRECUENCIA;
            paramT.PA_FRECUENCIA_LIMPIEZA = param.PA_FRECUENCIA_LIMPIEZA;
            return paramT;
        }
    }
}