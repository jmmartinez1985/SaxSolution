using Banistmo.Sax.Services.Interfaces.Business;
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

        public IHttpActionResult Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var files = Request.GetRequestContext();

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            var streamProvider = new MultipartMemoryStreamProvider();
            Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(streamProvider).ContinueWith((tsk) =>
            {
                foreach (HttpContent ctnt in streamProvider.Contents)
                {
                    Stream stream = ctnt.ReadAsStreamAsync().Result;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
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

                       var data= fileService.getDataFrom(result);
                    }

                }
            }).ContinueWith((secTsk)=>{
                
            });
            return Ok("The input file has been loaded into database.");

        }
    }
}
