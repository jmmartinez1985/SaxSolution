using FluentScheduler;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.TaskService.Business
{
    public class StopJob : IJob
    {
        private readonly object _lock = new object();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StopJob()
        {

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
            catch (Exception ex)
            {
                log.Error(ex);
            }
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
                            log.Info(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                            throw new Exception(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    throw new Exception(string.Format("Unable to restart the application pools for {0}.{1}", serverName, appPoolName), ex.InnerException);
                }
            }
        }

        private string Pool => System.Configuration.ConfigurationManager.AppSettings["Pool"].ToString();
        private string ServerName => System.Configuration.ConfigurationManager.AppSettings["ServerName"].ToString();
    }
}
