using FluentScheduler;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Banistmo.Sax.TaskManagement.Business
{
    public class StopJob : IJob, IRegisteredObject
    {
        private readonly object _lock = new object();

        private bool _shuttingDown;

        private string path;

        public StopJob()
        {
            HostingEnvironment.RegisterObject(this);
        }
        public void Execute()
        {
            try
            {
                lock (_lock)
                {
                    RecycleApplicationPool(ServerName, Pool);
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

        public void RecycleApplicationPool(string serverName, string appPoolName)
        {
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(appPoolName))
            {
                try
                {
                    using (ServerManager manager = ServerManager.OpenRemote(serverName))
                    {
                        ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == appPoolName);
                        if (appPool != null)
                        {
                            bool appPoolRunning = appPool.State == ObjectState.Started || appPool.State == ObjectState.Starting;
                            bool appPoolStopped = appPool.State == ObjectState.Stopped || appPool.State == ObjectState.Stopping;
                            if (appPoolRunning)
                            {

                                while (appPool.State == ObjectState.Starting) { System.Threading.Thread.Sleep(3000); }
                                if (appPool.State != ObjectState.Stopped)
                                {
                                    appPool.Stop();
                                }
                                appPoolStopped = true;

                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to restart the application pools for {0}.{1}", serverName, appPoolName), ex.InnerException);
                }
            }
        }

        private bool isStopTime() => System.DateTime.Now.TimeOfDay.Hours >= int.Parse(System.Configuration.ConfigurationManager.AppSettings["StopTime"].ToString().Split(':').FirstOrDefault());
        private bool isStartTime() => System.DateTime.Now.TimeOfDay.Hours <= int.Parse(System.Configuration.ConfigurationManager.AppSettings["StartTime"].ToString().Split(':').FirstOrDefault());
        private string Pool => System.Configuration.ConfigurationManager.AppSettings["Pool"].ToString();
        private string ServerName => System.Configuration.ConfigurationManager.AppSettings["ServerName"].ToString();
    }
}
