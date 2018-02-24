﻿using Banistmo.Sax.Services.Interfaces.Business;
using ExcelDataReader;
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
    public class FileController : ApiController
    {
        private readonly IFilesProvider fileService;

        public FileController(IFilesProvider file)
        {
            fileService = file;
        }

        [HttpPost]
        public IHttpActionResult Upload()
        {
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
                xfile.CopyToAsync(ms).ContinueWith((tsk) => {

                    using (var reader = ExcelReaderFactory.CreateReader(ms))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = true,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                EmptyColumnNamePrefix = "Column",
                                UseHeaderRow = false,
                                ReadHeaderRow = (rowReader) => {
                                    rowReader.Read();
                                },
                                FilterRow = (rowReader) => {
                                    return true;
                                },
                                FilterColumn = (rowReader, columnIndex) => {
                                    return true;
                                }
                            }
                        });
                        var data = fileService.getDataFrom(result);
                    }
                }).ContinueWith((secTsk) => {
                    System.IO.File.Delete(path);
                }); 

            }
            return Ok("The file has been loaded into database. Please check contents.");

        }
    }
}
