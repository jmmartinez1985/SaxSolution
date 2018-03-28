using Banistmo.Sax.Services.Helpers;
using Banistmo.Sax.Services.Interfaces.Business;
using Banistmo.Sax.Services.Models;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;



namespace Banistmo.Sax.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/File")]
    public class FileController : ApiController
    {
        private readonly IFilesProvider fileService;
        private readonly IRegistroControlService registroService;
        private ApplicationUserManager _userManager;

        public FileController() { }

        public FileController(IFilesProvider file, IRegistroControlService registro)
        {
            fileService = file;
            registroService = registro;
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


        [HttpPost]
        public IHttpActionResult Upload()
        {
            RegistroControlModel recordCreated = null;
            try
            {
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
                    file.SaveAs(path);
                }
                if (File.Exists(path))
                {
                    MemoryStream ms = new MemoryStream();
                    FileStream xfile = new FileStream(path, FileMode.Open, FileAccess.Read);
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
                        PartidasContent data = fileService.getDataFrom(result, userId);
                        var registroModel = new RegistroControlModel()
                        {
                            RC_USUARIO_CREACION = userId
                        };
                        if (data.ListError.Count == 0)
                        {
                            recordCreated = registroService.LoadFileData(registroModel, data.ListPartidas);
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
                throw ex;
            }
            return Ok(new { Message = "The file has been loaded into database.Please check contents.", RegistroControl = recordCreated.RC_REGISTRO_CONTROL });

        }
    }
}
