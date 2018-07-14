using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Banistmo.Sax.Services.Implementations.Jobs
{
    public class FileJob : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        private string path;

        public FileJob()
        {
            HostingEnvironment.RegisterObject(this);
            path = ConfigurationManager.AppSettings["filePath"].ToString();
        }
        public void Execute()
        {
            try
            {
                lock (_lock)
                {
                    if (_shuttingDown)
                        return;
                    string[] Files = Directory.GetFiles(path);
                    foreach (string file in Files)
                    {
                        File.Delete(file);
                    }
                    Console.WriteLine($"Joey son las {DateTime.Now}");
                }
            }
            finally
            {
                HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }
}
