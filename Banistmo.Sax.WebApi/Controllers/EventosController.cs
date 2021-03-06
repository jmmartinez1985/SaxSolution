﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using Banistmo.Sax.Services.Implementations.Business;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Implementations;
using System.IO;
using System.Net.Http.Headers;

namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Eventos")]
    public class EventosController : ApiController
    {
        private readonly IEventosService eventoService;
        private readonly IEventosTempService eventoTempService;
        private ApplicationUserManager _userManager;
        private AreaOperativaService areaservice;
        private IUsuarioAreaService usuarioAreaService;
        private ICuentaContableService cuentaContableService;
        private IAreaOperativaService areaOperativaService;
        private IUsuarioEmpresaService empresaUsuarioService;
        public EventosController()
        {
            eventoService = new EventosService();
            eventoTempService = new EventosTemporalService();
            areaservice = new AreaOperativaService();
            usuarioAreaService = new UsuarioAreaService();
            cuentaContableService = new CuentaContableService();
            areaOperativaService = new AreaOperativaService();
            empresaUsuarioService = new UsuarioEmpresaService();
        }

        //public EventosController(IEventosService ev, IEventosTempService evt)
        //{
        //    eventoService = ev;
        //    eventoTempService = evt;
        //}

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

        [Route("ListarEventos"), HttpGet]
        public IHttpActionResult ListarEventos()
        {
            try
            {

                //
                var userId = User.Identity.GetUserId();
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();
                List<int> listaEmpresaUsuario = empresaUsuarioService.GetAll(x => x.US_ID_USUARIO == userId, null, includes: c => c.AspNetUsers).Select(y => y.CE_ID_EMPRESA).ToList(); 


               
                //
                int estado = Convert.ToInt16(RegistryState.Aprobado);
                int estadoInac = Convert.ToInt16(RegistryState.Inactivo);
                var evnt = eventoService.GetAll(c => c.EV_ESTATUS == estado || c.EV_ESTATUS == estadoInac, null, includes: c => c.AspNetUsers);

                if (evnt == null)
                {
                    return BadRequest("No se puedo listar los eventos.");
                }
                else
                {//
                    if (evnt.Count() > 0)
                        evnt = evnt.Where(c => listAreaUsuario.Contains(c.EV_ID_AREA) && listaEmpresaUsuario.Contains(c.CE_ID_EMPRESA)).OrderBy(c => c.EV_COD_EVENTO).ToList();

                    var eve = evnt.Select(ev => new
                    {
                        EV_COD_EVENTO = ev.EV_COD_EVENTO
                        ,
                        CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                        ,
                        NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                        ,

                        EV_ID_AREA = ev.EV_ID_AREA
                        ,
                        NOMBRE_AREA = Convert.ToString(ev.SAX_AREA_OPERATIVA.CA_COD_AREA) + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE,

                        EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                        ,
                        EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                        EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                        NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                        EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                        EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                        NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                        EV_REFERENCIA = ev.EV_REFERENCIA_DEBITO
                        ,
                        EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                        ,
                        EV_ESTATUS = ev.EV_ESTATUS
                        ,
                        EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                        ,
                        EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                        ,
                        NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                        ,
                        EV_FECHA_MOD = ev.EV_FECHA_MOD
                        ,
                        EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                        ,
                        NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                        ,
                        EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                        ,
                        EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                        ,
                        NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                    });
                    return Ok(eve);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


        }


        [Route("BuscarEventos"), HttpGet]
        public IHttpActionResult BuscarEventos([FromUri]ParametroEvento parmEvento)
        {
            try
            {

                var userId = User.Identity.GetUserId();
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();
                List<int> listaEmpresaUsuario = empresaUsuarioService.GetAll(x => x.US_ID_USUARIO == userId, null, includes: c => c.AspNetUsers).Select(y => y.CE_ID_EMPRESA).ToList();

                int activo = Convert.ToInt16(RegistryState.Aprobado);
                int incativo = Convert.ToInt16(RegistryState.Inactivo);
                List<int> cuentaDebito = new List<int>();
                List<int> cuentaCredito= new List<int>();
                if (!string.IsNullOrEmpty(parmEvento.cuentaDebito)) {
                    var cuenta = cuentaContableService.Query(x => x.CO_CUENTA_CONTABLE + x.CO_COD_AUXILIAR + x.CO_NUM_AUXILIAR.Trim() == parmEvento.cuentaDebito.Trim()).Select(y => y.CO_ID_CUENTA_CONTABLE);
                    if (cuenta != null && cuenta.Count() > 0) {
                        cuentaDebito = cuenta.ToList();
                    }
                }
                if (!string.IsNullOrEmpty(parmEvento.cuentaCredito))
                {
                    var cuenta = cuentaContableService.Query(x => x.CO_CUENTA_CONTABLE + x.CO_COD_AUXILIAR + x.CO_NUM_AUXILIAR.Trim() == parmEvento.cuentaCredito.Trim()).Select(y => y.CO_ID_CUENTA_CONTABLE);
                    if (cuenta != null && cuenta.Count() > 0)
                    {
                        cuentaCredito = cuenta.ToList();
                    }
                }
                var evnt = eventoService.GetAll(c => (c.EV_ESTATUS == activo || c.EV_ESTATUS == incativo)
                     && c.CE_ID_EMPRESA==(parmEvento.CE_ID_EMPRESA>0?parmEvento.CE_ID_EMPRESA:c.CE_ID_EMPRESA)
                     && c.EV_ID_AREA == (parmEvento.EV_ID_AREA > 0 ? parmEvento.EV_ID_AREA : c.EV_ID_AREA)
                     && c.EV_COD_EVENTO == (parmEvento.EV_COD_EVENTO > 0 ? parmEvento.EV_COD_EVENTO : c.EV_COD_EVENTO)
                     //&& c.EV_CUENTA_DEBITO ==(cuentaDebito>0? cuentaDebito:c.EV_CUENTA_DEBITO)
                     //&& c.EV_CUENTA_CREDITO == (cuentaCredito.Count() > 0 ? cuentaCredito : c.EV_CUENTA_CREDITO)
                     ,
                    null, includes: c => c.AspNetUsers);
                if (evnt != null && evnt.Count() > 0) {
                    if (cuentaCredito.Count() > 0 && !string.IsNullOrEmpty(parmEvento.cuentaCredito))
                        evnt = evnt.Where(x => cuentaCredito.Contains(x.EV_CUENTA_CREDITO)).ToList();
                    else if (cuentaCredito.Count() == 0 && !string.IsNullOrEmpty(parmEvento.cuentaCredito))
                        return Ok();
                    if (cuentaDebito.Count() > 0 && !string.IsNullOrEmpty(parmEvento.cuentaDebito))
                        evnt = evnt.Where(x => cuentaDebito.Contains(x.EV_CUENTA_DEBITO)).ToList();
                    else if (cuentaDebito.Count() == 0 && !string.IsNullOrEmpty(parmEvento.cuentaDebito))
                        return Ok();
                }

                if (evnt == null)
                {
                    return BadRequest("No se puedo listar los eventos.");
                }
                else
                {
                    if (evnt.Count() > 0)
                        evnt = evnt.Where(c => listAreaUsuario.Contains(c.EV_ID_AREA) && listaEmpresaUsuario.Contains(c.CE_ID_EMPRESA)).OrderBy(c => c.EV_COD_EVENTO).ToList();

                    var eve = evnt.Select(ev => new
                    {
                        EV_COD_EVENTO = ev.EV_COD_EVENTO
                        ,
                        CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                        ,
                        NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                        ,

                        EV_ID_AREA = ev.EV_ID_AREA
                        ,
                        NOMBRE_AREA = Convert.ToString(ev.SAX_AREA_OPERATIVA.CA_COD_AREA) + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE,

                        EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                        ,
                        EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                        EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                        NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                        EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                        EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                        NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                        EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                        ,
                        EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                        ,
                        EV_ESTATUS = ev.EV_ESTATUS
                        ,
                        EV_REFERENCIA_DEBITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_DEBITO)
                        ,
                        EV_REFERENCIA_CREDITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_CREDITO)
                        ,
                        ACCION = ev.EV_USUARIO_APROBADOR == null ? "Creación" : "Modificación"
                        
                        ,
                        EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                        ,
                        EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                        ,
                        NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                        ,
                        EV_FECHA_MOD = ev.EV_FECHA_MOD
                        ,
                        EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                        ,
                        NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                        ,
                        EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                        ,
                        EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                        ,
                        NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                    });
                    //eve = eve.Where(x => x.EV_CUENTA_DEBITO_NUM == (parmEvento.cuentaDebito == null ? x.EV_CUENTA_DEBITO_NUM : parmEvento.cuentaDebito)
                    //                 && x.EV_CUENTA_CREDITO_NUM == (parmEvento.cuentaCredito == null ? x.EV_CUENTA_CREDITO_NUM : parmEvento.cuentaCredito)).ToList();
                    return Ok(eve);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


        }

        [Route("EventosExcel"), HttpGet]
        public HttpResponseMessage DescargarExcel([FromUri]ParametroEvento parmEvento)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest);
            try
            {

                var userId = User.Identity.GetUserId();
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();
                List<int> listaEmpresaUsuario = empresaUsuarioService.GetAll(x => x.US_ID_USUARIO == userId, null, includes: c => c.AspNetUsers).Select(y => y.CE_ID_EMPRESA).ToList();

                int activo = Convert.ToInt16(RegistryState.Aprobado);
                int incativo = Convert.ToInt16(RegistryState.Inactivo);
                List<int> cuentaDebito = new List<int>();
                List<int> cuentaCredito = new List<int>();
                if (!string.IsNullOrEmpty(parmEvento.cuentaDebito))
                {
                    var cuenta = cuentaContableService.Query(x => x.CO_CUENTA_CONTABLE + x.CO_COD_AUXILIAR + x.CO_NUM_AUXILIAR.Trim() == parmEvento.cuentaDebito.Trim()).Select(y => y.CO_ID_CUENTA_CONTABLE);
                    if (cuenta != null && cuenta.Count() > 0)
                    {
                        cuentaDebito = cuenta.ToList();
                    }
                }
                if (!string.IsNullOrEmpty(parmEvento.cuentaCredito))
                {
                    var cuenta = cuentaContableService.Query(x => x.CO_CUENTA_CONTABLE + x.CO_COD_AUXILIAR + x.CO_NUM_AUXILIAR.Trim() == parmEvento.cuentaCredito.Trim()).Select(y => y.CO_ID_CUENTA_CONTABLE);
                    if (cuenta != null && cuenta.Count() > 0)
                    {
                        cuentaCredito = cuenta.ToList();
                    }
                }
                var evnt = eventoService.GetAll(c => (c.EV_ESTATUS == activo || c.EV_ESTATUS == incativo)
                     && c.CE_ID_EMPRESA == (parmEvento.CE_ID_EMPRESA > 0 ? parmEvento.CE_ID_EMPRESA : c.CE_ID_EMPRESA)
                     && c.EV_ID_AREA == (parmEvento.EV_ID_AREA > 0 ? parmEvento.EV_ID_AREA : c.EV_ID_AREA)
                     && c.EV_COD_EVENTO == (parmEvento.EV_COD_EVENTO > 0 ? parmEvento.EV_COD_EVENTO : c.EV_COD_EVENTO)
                     //&& c.EV_CUENTA_DEBITO ==(cuentaDebito>0? cuentaDebito:c.EV_CUENTA_DEBITO)
                     //&& c.EV_CUENTA_CREDITO == (cuentaCredito.Count() > 0 ? cuentaCredito : c.EV_CUENTA_CREDITO)
                     ,
                    null, includes: c => c.AspNetUsers);
                if (evnt != null && evnt.Count() > 0)
                {
                    if (cuentaCredito.Count() > 0 && !string.IsNullOrEmpty(parmEvento.cuentaCredito))
                        evnt = evnt.Where(x => cuentaCredito.Contains(x.EV_CUENTA_CREDITO)).ToList();
                    else if (cuentaCredito.Count() == 0 && !string.IsNullOrEmpty(parmEvento.cuentaCredito))
                        return response;
                    if (cuentaDebito.Count() > 0 && !string.IsNullOrEmpty(parmEvento.cuentaDebito))
                        evnt = evnt.Where(x => cuentaDebito.Contains(x.EV_CUENTA_DEBITO)).ToList();
                    else if (cuentaDebito.Count() == 0 && !string.IsNullOrEmpty(parmEvento.cuentaDebito))
                        return response;
                }

                if (evnt == null)
                {
                    return response;
                }
                else
                {
                    if (evnt.Count() > 0)
                        evnt = evnt.Where(c => listAreaUsuario.Contains(c.EV_ID_AREA) && listaEmpresaUsuario.Contains(c.CE_ID_EMPRESA)).OrderBy(c => c.EV_COD_EVENTO).ToList();

                    var eve = evnt.Select(ev => new
                    {

                        NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                        ,
                        NOMBRE_AREA = Convert.ToString(ev.SAX_AREA_OPERATIVA.CA_COD_AREA) + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                        ,
                        EV_COD_EVENTO = ev.EV_COD_EVENTO
                        ,
                        EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                        ,

                        EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                        NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,

                        EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                        NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                        EV_REFERENCIA_DEBITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_DEBITO)
                        ,
                        EV_REFERENCIA_CREDITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_CREDITO)
                        ,
                        Estado = this.getEstadoEvento((ev.EV_ESTATUS==null? 0:ev.EV_ESTATUS.Value))
                        ,
                 
                        NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                    });


                    var dt = eve.ToList().AnonymousToDataTable();
                    if (dt != null && dt.Columns.Count > 0)
                    {
                        dt.Columns[0].Caption = "Empresa";
                        dt.Columns[1].Caption = "Area Operativa";
                        dt.Columns[2].Caption = "Cod. Evento";
                        dt.Columns[3].Caption = "Descripción Evento";
                        dt.Columns[4].Caption = "Cuenta a Debitar";
                        dt.Columns[5].Caption = "Nombre de la Cuenta a Debitar";
                        dt.Columns[6].Caption = "Cuenta a Creditar";
                        dt.Columns[7].Caption = "Nombre de la Cuenta de Acreditar";
                        dt.Columns[8].Caption = "Referencia Cuenta a Debitar";
                        dt.Columns[9].Caption = "Referencia Cuenta a Acreditar";
                        dt.Columns[10].Caption = "Estado";
                        dt.Columns[11].Caption = "Aprobado por";

                    }
                    ReporterService reportExcelService = new ReporterService();
                    byte[] fileExcell = reportExcelService.CreateReportBinary(dt, "Hoja1");
                    var contentLength = fileExcell.Length;
                    //200
                    //successful
                    var statuscode = HttpStatusCode.OK;
                    response = Request.CreateResponse(statuscode);
                    response.Content = new StreamContent(new MemoryStream(fileExcell));
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentLength = contentLength;
                    ContentDispositionHeaderValue contentDisposition = null;
                    if (ContentDispositionHeaderValue.TryParse("inline; filename=" + "document" + ".xlsx", out contentDisposition))
                    {
                        response.Content.Headers.ContentDisposition = contentDisposition;
                    }
                    return response;
                }
            }
            catch (Exception ex)
            {
                return response;
            }


        }

        [Route("EventosForSelect"), HttpGet]
        public IHttpActionResult ListarForSeletect()
        {
            try
            {
                int activo = Convert.ToInt16(RegistryState.Aprobado);
                int incativo = Convert.ToInt16(RegistryState.Inactivo);
                var evnt = eventoService.Query(c => (c.EV_ESTATUS == activo || c.EV_ESTATUS == incativo)).Select(y=> new {
                    EV_COD_EVENTO = y.EV_COD_EVENTO,
                    EV_DESCRIPCION_EVENTO= y.EV_DESCRIPCION_EVENTO
                });

                if (evnt == null)
                {
                    return BadRequest("No se puedo listar los eventos.");
                }
                else
                {
                    var eve = evnt.Select(ev => new
                    {
                        id = ev.EV_COD_EVENTO
                        ,
                        disabled = false
                        ,
                        text = (ev.EV_COD_EVENTO + "-" + ev.EV_DESCRIPCION_EVENTO)
                        
                    });
                    return Ok(eve);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


        }

        [Route("{eventoId:int}"), HttpGet]
        public IHttpActionResult ListarEventosPorId(int eventoId)
        {
            var ev = eventoService.GetSingle(evn => evn.EV_COD_EVENTO == eventoId);

            if (ev == null)
            {
                return BadRequest("No se puedo listar los eventos.");
            }
            else
            {
                var eve = new
                {

                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                    EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                    EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                    ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                    EV_REFERENCIA_CREDITO = ev.EV_REFERENCIA_CREDITO
                    ,
                    EV_REFERENCIA_DEBITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_DEBITO )
                    ,
                    EV_REFERENCIA_CREDITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_CREDITO)
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = (ev.AspNetUsers ==null ? "": ev.AspNetUsers.FirstName)
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = (ev.AspNetUsers1 == null ? "" : ev.AspNetUsers1.FirstName)
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                };
                return Ok(eve);
            }
        }

        [Route("ReporteEventoOperaciones"), HttpGet]
        public async Task<IHttpActionResult> ListarEventosPorAreasYFiltro([FromUri] ReportFilter data)
        {
            Int32 aprobado = Convert.ToInt32(RegistryState.Aprobado);
            string statusaccion = "1";

            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                var userArea = usuarioAreaService.GetAll(d => d.US_ID_USUARIO == user.Id,   null, includes: d => d.AspNetUsers);
                   // usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);
                var userAreacod = new List<AreaOperativaModel>();
                //areaservice.GetSingle(d => d.CA_ID_AREA == userArea.CA_ID_AREA);

                foreach (var item in userArea)
                {
                    userAreacod.Add(areaOperativaService.GetSingle(d => d.CA_ID_AREA == item.CA_ID_AREA));
                }


                if (data == null)
                {
                    data = new ReportFilter();
                    data.EmpId = null;
                    data.EventoId = null;
                    data.IdAreaOpe = null;
                    data.IdCuentaCr = null;
                    data.IdCuentaDb = null;
                }

                var evnt = eventoService.GetAll(ev => ev.EV_COD_EVENTO == (data.EventoId == null ? ev.EV_COD_EVENTO : data.EventoId)
                                                //&& ev.EV_CUENTA_CREDITO == (data.IdCuentaDb == null ? ev.EV_CUENTA_CREDITO : data.IdCuentaDb)
                                                //&& ev.EV_CUENTA_DEBITO == (data.IdCuentaCr == null ? ev.EV_CUENTA_DEBITO : data.IdCuentaCr)
                                                && ev.CE_ID_EMPRESA == (data.EmpId == null ? ev.CE_ID_EMPRESA : data.EmpId)
                                                && ev.EV_ESTATUS_ACCION == (statusaccion) && ev.EV_ESTATUS == aprobado
                                              //  && ev.EV_ID_AREA == userArea.CA_ID_AREA
                                                , null, includes: c => c.AspNetUsers);
               
                var evnt_area = new List<EventosModel>();
                if (evnt.Count == 0)
                {
                    return Ok();
                }
                else
                {
                    if (data.IdAreaOpe == null)
                    {
                        foreach (var areaItem in userAreacod)
                        {
                            foreach (var item in evnt)
                            {
                                if (item.EV_ID_AREA == areaItem.CA_ID_AREA)
                                {
                                    evnt_area.Add(item);
                                }
                            }
                        }
                    }
                    else if(data.IdAreaOpe!=null)
                    {
                        evnt_area = evnt.Where(r => r.EV_ID_AREA == data.IdAreaOpe).ToList();
                    }
                    

                    var eve = evnt_area.Select(ev => new
                    {
                        EV_COD_EVENTO = ev.EV_COD_EVENTO
                        ,
                        CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                        ,
                        NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                        ,
                        EV_ID_AREA = ev.EV_ID_AREA
                        ,
                        NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                        ,
                        EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                        ,
                        EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                            ,
                        EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                                   ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                                   ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                            ,
                        NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                            ,
                        EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                            ,
                        EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                                   ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                                   ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                            ,
                        NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                            ,
                        EV_REFERENCIA_DEBITO = this.getReferenciaSiNo(ev.EV_REFERENCIA_DEBITO),


                        EV_REFERENCIA_CREDITO = this.getReferenciaSiNo(ev.EV_REFERENCIA_CREDITO)
                        ,
                        EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                        ,
                        EV_ESTATUS = ev.EV_ESTATUS
                        ,
                        EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                        ,
                        EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                        ,
                        NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                        ,
                        EV_FECHA_MOD = ev.EV_FECHA_MOD
                        ,
                        EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                        ,
                        NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                        ,
                        EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                        ,
                        EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                        ,
                        NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                        ,
                        ACCION = ev.EV_USUARIO_MOD == null ? "Creación" : "Modificado"
                    });
                    eve = eve.Where(t => t.EV_CUENTA_DEBITO_NUM.TrimEnd() == (data.IdCuentaDb == null ? t.EV_CUENTA_DEBITO_NUM.TrimEnd() : data.IdCuentaDb) &&
                                        t.EV_CUENTA_CREDITO_NUM.TrimEnd() == (data.IdCuentaCr == null ? t.EV_CUENTA_CREDITO_NUM.TrimEnd() : data.IdCuentaCr)).ToList();
                    return Ok(eve);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("FiltrarEventos"), HttpGet]
        public IHttpActionResult ListarEventosPorFiltros([FromUri] ParameterFilter data)
        {
            Int32 aprobado = Convert.ToInt32(RegistryState.Aprobado);
            string statusaccion = "1";

            try
            {
                var evnt = eventoService.GetAll(ev => ev.EV_COD_EVENTO == (data.EventoId == null ? ev.EV_COD_EVENTO : data.EventoId)
                                                //&& ev.EV_CUENTA_CREDITO == (data.IdCuentaDb == null ? ev.EV_CUENTA_CREDITO : data.IdCuentaDb)
                                                //&& ev.EV_CUENTA_DEBITO == (data.IdCuentaCr == null ? ev.EV_CUENTA_DEBITO : data.IdCuentaCr)
                                                && ev.EV_CUENTA_CREDITO == (data.IdCuentaCr == null ? ev.EV_CUENTA_CREDITO : data.IdCuentaCr)
                                                && ev.EV_CUENTA_DEBITO == (data.IdCuentaDb == null ? ev.EV_CUENTA_DEBITO : data.IdCuentaDb)
                                                && ev.CE_ID_EMPRESA == (data.EmpId == null ? ev.CE_ID_EMPRESA : data.EmpId)
                                                && ev.EV_ESTATUS_ACCION == (statusaccion) && ev.EV_ESTATUS == aprobado
                                                && ev.EV_ID_AREA == (data.IdAreaOpe == null ? ev.EV_ID_AREA : data.IdAreaOpe), null, includes: c => c.AspNetUsers);

                if (evnt.Count == 0)
                {
                    return BadRequest("No se puedo listar los eventos filtrados.");
                }
                else
                {
                    var eve = evnt.Select(ev => new
                    {
                        EV_COD_EVENTO = ev.EV_COD_EVENTO
                        ,
                        CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                        ,
                        NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                        ,
                        EV_ID_AREA = ev.EV_ID_AREA
                        ,
                        NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                        ,
                        EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                        ,
                        EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                            ,
                        EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                                   ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                                   ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                            ,
                        NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                            ,
                        EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                            ,
                        EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                                   ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                                   ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                            ,
                        NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                            ,
                        EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                        ,
                        EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                        ,
                        EV_ESTATUS = ev.EV_ESTATUS
                        ,
                        EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                        ,
                        EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                        ,
                        NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                        ,
                        EV_FECHA_MOD = ev.EV_FECHA_MOD
                        ,
                        EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                        ,
                        NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                        ,
                        EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                        ,
                        EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                        ,
                        NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                    });
                    return Ok(eve);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class ParameterFilter
        {

            public int? EventoId { get; set; }
            public int? IdCuentaDb { get; set; }
            public int? IdCuentaCr { get; set; }
            public int? EmpId { get; set; }
            public int? IdAreaOpe { get; set; }

        }

        public class ReportFilter
        {

            public int? EventoId { get; set; }
            public string IdCuentaDb { get; set; }
            public string IdCuentaCr { get; set; }
            public int? EmpId { get; set; }
            public int? IdAreaOpe { get; set; }

        }

        [Route("NuevoEvento"), HttpPost]
        public async Task<IHttpActionResult> NuevoEvento([FromBody] ParameterEventoModel evemodel)
        {
            string MensajeVal = "";
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                evemodel.EV_USUARIO_CREACION = user.Id;
                evemodel.EV_FECHA_CREACION = DateTime.Now.Date;
                evemodel.EV_ESTATUS = Convert.ToInt32(RegistryState.PorAprobar);
                int eventoId = eventoService.Insert_Eventos_EventosTempOperador(mapeoParametro_EventoModel(evemodel));
                if (eventoId <= 0)
                {
                    switch (eventoId)
                    {
                        case -2:
                            MensajeVal = "Evento existente o una de las cuentas no es conciliable.";
                            break;
                        case -3:
                            MensajeVal = "Evento Existente.";
                            break;
                        case -4:
                            MensajeVal = "El evento requiere una cuenta conciliable.";
                            break;

                    }
                    return BadRequest("No se pudo crear el evento: " + MensajeVal);
                }
                return Ok("El Evento " + eventoId.ToString() + " ha sido creado, correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo crear el evento. " + ex.Message);
            }
        }

        private EventosModel mapeoParametro_EventoModel(ParameterEventoModel evt)
        {
            var evtReturn = new EventosModel();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = evt.EV_ESTATUS;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            evtReturn.EV_REFERENCIA_DEBITO = evt.EV_REFERENCIA_DEBITO;
            evtReturn.EV_REFERENCIA_CREDITO = evt.EV_REFERENCIA_CREDITO;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            return evtReturn;
        }

        [Route("ActualizarEvento"), HttpPost]
        public async Task<IHttpActionResult> ActualizarEvento_EventoTemp([FromBody] ParameterEventoModel modelevtmp)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                //modelevtmp.EV_USUARIO_CREACION = user.Id;
                modelevtmp.EV_FECHA_CREACION = DateTime.Now.Date;
                modelevtmp.EV_USUARIO_MOD = user.Id;
                modelevtmp.EV_FECHA_MOD = DateTime.Now.Date;
                modelevtmp.EV_ESTATUS = Convert.ToInt32(RegistryState.PorAprobar);
                //modelevtmp.EV_FECHA_APROBACION = null;
                //modelevtmp.EV_USUARIO_APROBADOR = null;
                int actualizado = eventoService.Update_EventoTempOperador(mapeoParametro_EventosTempModel(modelevtmp));
                if (actualizado <= 0)
                {
                    return BadRequest("No se pudo actualizar el evento. ");
                }
                return Ok("El Evento con código " + actualizado.ToString() + " ha sido actualizado, correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar el evento. " + ex.Message);
            }
        }

        private EventosTempModel mapeoParametro_EventosTempModel(ParameterEventoModel evt)
        {
            var evtReturn = new EventosTempModel();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = evt.EV_ESTATUS;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            evtReturn.EV_REFERENCIA_DEBITO = evt.EV_REFERENCIA_DEBITO;
            evtReturn.EV_REFERENCIA_CREDITO = evt.EV_REFERENCIA_CREDITO;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            return evtReturn;
        }

        [Route("CancelarEvento"), HttpPost]
        public IHttpActionResult CancelarEvento([FromBody] int eventoid)
        {
            try
            {
                var evnt = eventoService.GetSingle(ev => ev.EV_COD_EVENTO == eventoid);

                if (evnt == null)
                {
                    return BadRequest("No se puedo obtener el evento a cancelar.");
                }
                else
                {
                    evnt.EV_ESTATUS = Convert.ToInt32(RegistryState.Aprobado);

                    bool Deshacer = eventoService.Deshacer_EventoTempOperador(eventoid);
                    //bool Deshacer = eventoService.Update_EventoTempOperador(mapeoEventoModel_EventosTempModel(evnt));
                    if (Deshacer == false)
                    {
                        return BadRequest("No se puede cancelar el cambio del evento. ");
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo cancelar el cambio del evento. " + ex.Message);
            }
        }

        private EventosTempModel mapeoEventoModel_EventosTempModel(EventosModel evt)
        {
            var evtReturn = new EventosTempModel();
            evtReturn.EV_ID_AREA = evt.EV_ID_AREA;
            evtReturn.CE_ID_EMPRESA = evt.CE_ID_EMPRESA;
            evtReturn.EV_COD_EVENTO = evt.EV_COD_EVENTO;
            //evtReturn.EV_COD_EVENTO_TEMP = evt.EV_COD_EVENTO;
            evtReturn.EV_CUENTA_CREDITO = evt.EV_CUENTA_CREDITO;
            evtReturn.EV_CUENTA_DEBITO = evt.EV_CUENTA_DEBITO;
            evtReturn.EV_DESCRIPCION_EVENTO = evt.EV_DESCRIPCION_EVENTO;
            evtReturn.EV_ESTATUS = evt.EV_ESTATUS;
            evtReturn.EV_ESTATUS_ACCION = evt.EV_ESTATUS_ACCION;
            evtReturn.EV_FECHA_APROBACION = evt.EV_FECHA_APROBACION;
            evtReturn.EV_FECHA_CREACION = evt.EV_FECHA_CREACION;
            evtReturn.EV_FECHA_MOD = evt.EV_FECHA_MOD;
            evtReturn.EV_REFERENCIA_DEBITO = evt.EV_REFERENCIA_DEBITO;
            evtReturn.EV_USUARIO_APROBADOR = evt.EV_USUARIO_APROBADOR;
            evtReturn.EV_USUARIO_CREACION = evt.EV_USUARIO_CREACION;
            evtReturn.EV_USUARIO_MOD = evt.EV_USUARIO_MOD;
            return evtReturn;
        }


        [Route("BuscarEventoPorEmpresaAreaLogin"), HttpGet]
        public async Task<IHttpActionResult> BuscarEventoPorEmpresaAreaLogin(int idEmpresa)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var userId = User.Identity.GetUserId();
                List<int> listUserArea = usuarioAreaService.Query(d => d.US_ID_USUARIO == userId).Select(y => y.CA_ID_AREA).ToList();
                List<AreaOperativaModel> listArea = areaOperativaService.GetAll().ToList();
                List<int> listAreaUsuario = listArea.Where(x => listUserArea.Contains(x.CA_ID_AREA)).Select(a => a.CA_ID_AREA).ToList();

                int activo=Convert.ToInt16(BusinessEnumerations.Estatus.ACTIVO);
                var evento = eventoService.Query(a => a.CE_ID_EMPRESA == idEmpresa && a.EV_ESTATUS==activo).Select(
                    x=> new {
                        EV_COD_EVENTO =x.EV_COD_EVENTO,
                        CE_ID_EMPRESA = x.CE_ID_EMPRESA,
                        EV_ID_AREA = x.EV_ID_AREA,
                        EV_DESCRIPCION_EVENTO= x.EV_DESCRIPCION_EVENTO,
                        EV_CUENTA_DEBITO=x.EV_CUENTA_DEBITO,
                        EV_CUENTA_CREDITO=x.EV_CUENTA_CREDITO,
                        EV_REFERENCIA_DEBITO = x.EV_REFERENCIA_DEBITO,
                        EV_ESTATUS_ACCION=x.EV_ESTATUS_ACCION,
                        EV_ESTATUS=x.EV_ESTATUS,
                        EV_FECHA_CREACION=x.EV_FECHA_CREACION,
                        EV_USUARIO_CREACION=x.EV_USUARIO_CREACION
                    }).ToList();
                //var listaCuenta = cuentaContableService.GetAllFlatten<CuentaContableModel>();
                List<CuentaContableModel> listaCtaContable = cuentaContableService.Query(x => x.CO_ID_CUENTA_CONTABLE== x.CO_ID_CUENTA_CONTABLE).Select(y => new CuentaContableModel
                {
                    CO_ID_CUENTA_CONTABLE= y.CO_ID_CUENTA_CONTABLE,
                    CO_CUENTA_CONTABLE= y.CO_CUENTA_CONTABLE,
                    CO_COD_AUXILIAR= y.CO_COD_AUXILIAR,
                    CO_NUM_AUXILIAR=y.CO_NUM_AUXILIAR
                }).ToList();
                if (evento.Count == 0)
                {
                    return BadRequest("El filtro no trajo eventos.");
                }else
                {
                    evento = evento.Where(e => listAreaUsuario.Contains(e.EV_ID_AREA)).ToList();
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                    EV_CUENTA_DEBITO_NUM = getNombreCuentaContable(ev.EV_CUENTA_DEBITO, ref listaCtaContable)
                        ,
                    NOMBRE_CTA_DEBITO = getNombreCuentaContableAUX(ev.EV_CUENTA_DEBITO, ref listaCtaContable)
                        ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                    EV_CUENTA_CREDITO_NUM = getNombreCuentaContable(ev.EV_CUENTA_CREDITO, ref listaCtaContable)
                        ,
                    NOMBRE_CTA_CREDITO = getNombreCuentaContableAUX(ev.EV_CUENTA_CREDITO, ref listaCtaContable)
                        ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION

                });

                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos por aprobar buscados. " + ex.Message);
            }
        }

        [Route("BuscarEventoPorArea"), HttpGet]
        public async Task<IHttpActionResult> BuscarEventoPorArea(int idEmpresa)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var area = usuarioAreaService.GetSingle(d => d.US_ID_USUARIO == user.Id);

                var evento = eventoService.GetAll(a => a.CE_ID_EMPRESA == idEmpresa
                                                       && a.EV_ID_AREA == area.CA_ID_AREA, null, includes: c => c.AspNetUsers);
                if (evento.Count == 0)
                {
                    return BadRequest("El filtro no trajo eventos. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                    EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                    EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });

                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos por aprobar buscados. " + ex.Message);
            }
        }

        [Route("BuscarEventoTempTodos"), HttpGet]
        public IHttpActionResult BuscarEventoTempTodos()
        {
            try
            {

                var evento = eventoTempService.GetAll();
                if (evento.Count == 0)
                {
                    return BadRequest("El filtro no trajo eventos. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                    EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                    EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });

                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos por aprobar buscados. " + ex.Message);
            }
        }

        [Route("BuscarEventoTempPorAprobar"), HttpGet]
        public IHttpActionResult BuscarEventoTempPorAprobar([FromUri] ParamtrosFiltroEvTemp pdata)
        {
            try
            {
                
                if (pdata == null)
            
                {
                    pdata = new ParamtrosFiltroEvTemp();
                    pdata.fechaCaptura = null;
                    pdata.status = null;
                 
                }

                var evento = eventoTempService.GetAll(c => 
                                                    (c.EV_FECHA_CREACION >= (pdata.fechaCaptura== null ? c.EV_FECHA_CREACION : pdata.fechaCaptura)
                                                    && c.EV_FECHA_CREACION <= (pdata.fechaCaptura == null ? c.EV_FECHA_CREACION : pdata.fechaCaptura.Value)
                                                    ||
                                                    (c.EV_FECHA_MOD >= (pdata.fechaCaptura == null ? c.EV_FECHA_MOD : pdata.fechaCaptura)
                                                    && c.EV_FECHA_MOD <= (pdata.fechaCaptura == null ? c.EV_FECHA_MOD : pdata.fechaCaptura.Value)
                                                    ))
                                                    && 
                                                    (c.EV_USUARIO_CREACION == (pdata.userCapturador == null ? c.EV_USUARIO_CREACION : pdata.userCapturador)
                                                    || c.EV_USUARIO_MOD == (pdata.userCapturador == null ? c.EV_USUARIO_MOD : pdata.userCapturador)
                                                    )
                                                    && c.EV_ESTATUS == 2, null, includes: c => c.AspNetUsers);
                if (evento.Count == 0)
                {
                    return Ok();
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                    EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                    EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                   
                    REFERENCIA_DEBITO_TXT = this.getReferenciaSiNo( ev.EV_REFERENCIA_DEBITO )
                    ,
                    REFERENCIA_CREDITO= ev.EV_REFERENCIA_CREDITO
                    ,
                    REFERENCIA_CREDITO_TXT = this.getReferenciaSiNo(ev.EV_REFERENCIA_CREDITO)
                    ,
                    ACCION = ev.EV_USUARIO_APROBADOR == null ? "Creación" : "Modificación"
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });

                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos por aprobar buscados. " + ex.Message);
            }
        }

        public class ParamtrosFiltroEvTemp
        {
            public DateTime? fechaCaptura { get; set; }
            public string userCapturador { get; set; }
            public int? status { get; set; }
        }

        [Route("BuscarEventoReporte"), HttpGet]
        public IHttpActionResult BuscarEventoReporte([FromUri] ParametroReporte data)
        {
            try
            {
                if (data == null)
                {
                    data = new ParametroReporte();
                    data.FechaAprobacion = null;
                    data.FechaCreacion = null;
                    data.Status = null;

                }
                IList<Sax.Services.Models.EventosModel> evento;

                evento = eventoService.GetAll(c => c.EV_FECHA_CREACION == (data.FechaCreacion == null ? c.EV_FECHA_CREACION : data.FechaCreacion)
                                                    && c.EV_FECHA_APROBACION == (data.FechaAprobacion == null ? c.EV_FECHA_APROBACION : data.FechaAprobacion)
                                                    && c.EV_ESTATUS == (data.Status == null ? c.EV_ESTATUS : data.Status),
                                                    null, includes: d => d.AspNetUsers);



                if (evento.Count == 0)
                {
                    return BadRequest("El filtro no trajo eventos. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = Convert.ToString(ev.SAX_AREA_OPERATIVA.CA_COD_AREA) + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE//ev.EV_ID_AREA
                    ,
                    NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                        ,
                    EV_CUENTA_DEBITO_NUM = ev.SAX_CUENTA_CONTABLE.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_AUXILIAR
                        ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                        ,
                    EV_CUENTA_CREDITO_NUM = ev.SAX_CUENTA_CONTABLE1.CO_CUENTA_CONTABLE +
                                               ev.SAX_CUENTA_CONTABLE1.CO_COD_AUXILIAR +
                                               ev.SAX_CUENTA_CONTABLE1.CO_NUM_AUXILIAR
                        ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_AUXILIAR
                        ,
                    EV_REFERENCIA_DEBITO = ObtenerRef(ev.EV_REFERENCIA_DEBITO)

                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ObtenerEstadoDes(ev.EV_ESTATUS.ToString())
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });

                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos buscados. " + ex.Message);
            }
        }

        public class ParametroReporte
        {
            public DateTime? FechaCreacion { get; set; }
            public DateTime? FechaAprobacion { get; set; }
            public int? Status { get; set; }
        }

        private string ObtenerRef(string valor)
        {
            string des = "Sí";
            if (valor == "N" || valor == "n")
                des = "No";
            return des;
        }

        private string ObtenerEstadoDes(string valor)
        {
            string des = "Aprobado";
            switch (valor)
            {
                case "0": des = "Por Aprobar"; break;
                case "2": des = "Por Aprobar"; break;
                case "3": des = "Eliminado"; break;
                case "4": des = "Rechazado"; break;

            }
            return des;
        }

        [Route("ListarEventosAprobados"), HttpGet]
        public IHttpActionResult ListarEventosAprobados()
        {
            try
            {
                var evento = eventoService.GetAll(c => c.EV_ESTATUS == 1);
                if (evento.Count == 0)
                {
                    return BadRequest("No existen eventos aprobados. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });
                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos aprobador. " + ex.Message);
            }
        }

        [Route("FiltroRptEventosAprobados"), HttpGet]
        public IHttpActionResult FiltroRptEventosAprobados()
        {
            try
            {
                var evento = eventoService.GetAll(c => c.EV_ESTATUS == 1);
                if (evento.Count == 0)
                {
                    return BadRequest("No existen eventos aprobados. ");
                }
                var even = evento.Select(ev => new
                {
                    EV_COD_EVENTO = ev.EV_COD_EVENTO
                    ,
                    CE_ID_EMPRESA = ev.CE_ID_EMPRESA
                    ,
                    NOMBRE_EMPRESA = ev.SAX_EMPRESA.CE_COD_EMPRESA + '-' + ev.SAX_EMPRESA.CE_NOMBRE
                    ,
                    EV_ID_AREA = ev.EV_ID_AREA
                    ,
                    NOMBRE_AREA = ev.SAX_AREA_OPERATIVA.CA_COD_AREA.ToString() + '-' + ev.SAX_AREA_OPERATIVA.CA_NOMBRE
                    ,
                    EV_DESCRIPCION_EVENTO = ev.EV_DESCRIPCION_EVENTO
                    ,
                    EV_CUENTA_DEBITO = ev.EV_CUENTA_DEBITO
                    ,
                    NOMBRE_CTA_DEBITO = ev.SAX_CUENTA_CONTABLE.CO_NOM_CUENTA
                    ,
                    EV_CUENTA_CREDITO = ev.EV_CUENTA_CREDITO
                    ,
                    NOMBRE_CTA_CREDITO = ev.SAX_CUENTA_CONTABLE1.CO_NOM_CUENTA
                    ,
                    EV_REFERENCIA_DEBITO = ev.EV_REFERENCIA_DEBITO
                    ,
                    EV_ESTATUS_ACCION = ev.EV_ESTATUS_ACCION
                    ,
                    EV_ESTATUS = ev.EV_ESTATUS
                    ,
                    EV_FECHA_CREACION = ev.EV_FECHA_CREACION
                    ,
                    EV_USUARIO_CREACION = ev.EV_USUARIO_CREACION
                    ,
                    NOMBRE_USER_CREA = ev.AspNetUsers.FirstName
                    ,
                    EV_FECHA_MOD = ev.EV_FECHA_MOD
                    ,
                    EV_USUARIO_MOD = ev.EV_USUARIO_MOD
                    ,
                    NOMBRE_USER_MOD = ev.AspNetUsers1 != null ? ev.AspNetUsers1.FirstName : string.Empty
                    ,
                    EV_FECHA_APROBACION = (ev.EV_FECHA_APROBACION == null ? (DateTime?)null : ev.EV_FECHA_APROBACION)
                    ,
                    EV_USUARIO_APROBADOR = (ev.EV_USUARIO_APROBADOR == null ? "" : ev.EV_USUARIO_APROBADOR)
                    ,
                    NOMBRE_USER_APROB = (ev.AspNetUsers2 == null ? "" : ev.AspNetUsers2.FirstName)
                });
                return Ok(even);
            }
            catch (Exception ex)
            {
                return BadRequest("No se pudo obtener los eventos aprobador. " + ex.Message);
            }
        }


        [Route("AprobarEvento"), HttpPost]
        public async Task<IHttpActionResult> ApruebaEvento([FromBody] int eventoidAprobado)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                int aprobado = eventoService.SupervidorAprueba_Evento(eventoidAprobado, user.Id);

                if (aprobado == -11)
                {
                    return BadRequest("Evento duplicado, favor rechazar. ");
                }
                else if (aprobado <= 0 && aprobado != -11)
                {
                    return BadRequest("Error al aprobar el Evento. ");
                }

                return Ok("El Evento " + aprobado.ToString() + " ha sido aprobado, correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al aprobar el Evento. " + ex.Message);
            }
        }

        [Route("RechazarEvento"), HttpPost]
        public async Task<IHttpActionResult> RechazaEvento([FromBody] int eventoidRechazado)
        {
            try
            {
                IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                int rechazado = eventoService.SupervidorRechaza_Evento(eventoidRechazado, user.Id);
                if (rechazado <= 0)
                {
                    return BadRequest("Error rechazando Evento, No se pudo declinar el Evento");
                }
                return Ok("El Evento " + rechazado.ToString() + " ha sido rechazado, correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al rechazar el Evento. " + ex.Message);
            }
        }

        public enum RegistryState
        {
            Inactivo = 0,
            Aprobado = 1,
            PorAprobar = 2,
            Eliminado = 3,
            Rechazado = 4
        }

        private string getNombreCuentaContable(int idCuenta, ref List<CuentaContableModel> listaCuenta)
        {
            string result = string.Empty;
            var cuenta = listaCuenta.Where(x => x.CO_ID_CUENTA_CONTABLE == idCuenta).FirstOrDefault();
            if (listaCuenta != null)
            {
                result = cuenta.CO_CUENTA_CONTABLE + cuenta.CO_COD_AUXILIAR + cuenta.CO_NUM_AUXILIAR;
            }
            return result;
        }

        private string getNombreCuentaContableAUX(int idCuenta, ref List<CuentaContableModel> listaCuenta)
        {
            string result = string.Empty;
            var cuenta = listaCuenta.Where(x => x.CO_ID_CUENTA_CONTABLE == idCuenta).FirstOrDefault();
            if (listaCuenta != null)
            {
                result = cuenta.CO_NOM_AUXILIAR;
            }
            return result;
        }

        private string getReferenciaSiNo(string referencia) {
            string result= string.Empty;
            if (!string.IsNullOrEmpty(referencia)) {
                switch (referencia){
                    case "1":
                        result = "Automática";
                        break;
                    case "0":
                        result = "Manual";
                        break;
                }
            }
            return result;
        }

        private string getEstadoEvento(int estado) {
            string result = string.Empty;
            switch (estado) {
                case 0:
                    result = "Inactivo";
                    break;
                case 1:
                    result = "Activo";
                    break;
                case 2:
                    result = "Eliminado";
                    break;
            }
            return result;
        }
    }
}
