using Banistmo.Sax.Common;
using Banistmo.Sax.Services.Helpers;
using Banistmo.Sax.Services.Implementations.Business;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using Banistmo.Sax.WebApi.Models;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;



namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/File")]
    public class FileController : ApiController
    {
        private IFilesProvider fileService;
        private IRegistroControlService registroService;
        private ApplicationUserManager _userManager;

        private readonly IPartidasService partService;
        private readonly ICentroCostoService centService;
        private readonly IEmpresaService empService;

        private readonly IConceptoCostoService cncService;
        private readonly ICuentaContableService ctaService;
        private readonly IMonedaService monedaSrv;
        private IAreaOperativaService areaOperativaService;


        public FileController()
        {

            //registroService = registroService ?? new RegistroControlService();
            partService = partService ?? new PartidasService();
            centService = centService ?? new CentroCostoService();
            empService = empService ?? new EmpresaService();
            ctaService = ctaService ?? new CuentaContableService();
            cncService = cncService ?? new ConceptoCostoService();
            registroService = registroService ?? new RegistroControlService();
            monedaSrv = monedaSrv ?? new MonedaService();
            areaOperativaService = areaOperativaService ?? new AreaOperativaService();
            fileService = fileService ?? new FilesProvider(partService, centService, empService, cncService, ctaService, monedaSrv, areaOperativaService);

        }

        //public FileController(IFilesProvider file, IRegistroControlService registro)
        //{
        //    fileService = file;
        //    registroService = registro;
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


        [HttpPost]
        public IHttpActionResult Upload([FromUri] LoadModel parametros)
        {
            RegistroControlModel recordCreated = null;
            FileStream xfile = null;
            try
            {

                //var value = registroService.IsValidLoad(DateTime.Now);
                //if (!value)
                //    return BadRequest("Fecha de carga no permitida");

                var userId = User.Identity.GetUserId();
                if (!Request.Content.IsMimeMultipartContent())
                {
                    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }
                var path = string.Empty;
                var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(
                        HttpContext.Current.Server.MapPath("~/App_Data"),
                        fileName
                    );
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    file.SaveAs(path);
                }
                else
                {
                    return BadRequest("Debe proveer un archivo de carga contable.");
                }
                if (File.Exists(path))
                {
                    MemoryStream ms = new MemoryStream();
                    xfile = new FileStream(path, FileMode.Open, FileAccess.Read);

                    xfile.CopyTo(ms);
                    using (var reader = ExcelReaderFactory.CreateReader(ms))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = true,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = false,
                                ReadHeaderRow = (rowReader) =>
                                {
                                    rowReader.Read();
                                },
                                FilterRow = (rowReader) =>
                                {
                                    return true;
                                },
                                FilterColumn = (rowReader, columnIndex) =>
                                {
                                    return true;
                                }
                            }
                        });
                        PartidasContent data = new PartidasContent();
                        if (parametros.tipoOperacion == Convert.ToInt16(BusinessEnumerations.TipoOperacion.CARGA_MASIVA))
                            data = fileService.cargaMasiva(result, userId,parametros.area);
                        else
                            data = fileService.cargaInicial(result, userId, parametros.area);

                        var registroModel = new RegistroControlModel()
                        {
                            RC_USUARIO_CREACION = userId,
                            CA_ID_AREA = parametros.area,
                            EV_COD_EVENTO = parametros.cod_event
                        };
                        if (data.ListError.Count == 0)
                        {
                            registroService.FileName = file.FileName;
                            recordCreated = registroService.LoadFileData(registroModel, data.ListPartidas, parametros.tipoOperacion);
                            reader.Close();
                        }

                        else
                        {
                            reader.Close();
                            return Ok(new { Messaje = "El contenido del archivo no cumple con el formato requerido.", Listerror = data.ListError });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la carga de archivo. {ex.Message}");
            }
            finally
            {
                if (xfile != null)
                    xfile.Close();
            }

            return Ok(new { Message = "The file has been loaded into database.Please check contents.", RegistroControl = recordCreated.RC_REGISTRO_CONTROL });

        }

       
        private  DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
