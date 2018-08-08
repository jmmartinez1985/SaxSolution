using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.ManagementPool
{
    class Program
    {
        static void Main(string[] args)
        {
            RecycleApplicationPool("DESKTOP-T36SFN9", "SaxApiPool");
        }

        public static void RecycleApplicationPool(string serverName, string appPoolName)
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
                                if (isStopTime())
                                {
                                    while (appPool.State == ObjectState.Starting) { System.Threading.Thread.Sleep(3000); }
                                    if (appPool.State != ObjectState.Stopped)
                                    {
                                        appPool.Stop();
                                    }
                                    appPoolStopped = true;
                                }
                            }
                            if (appPoolStopped)
                            {
                                if (isStartTime())
                                {
                                    while (appPool.State == ObjectState.Stopping)
                                    {
                                        System.Threading.Thread.Sleep(3000);
                                    }
                                    appPool.Start();                          
                                }
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

        private static bool isStopTime() => System.DateTime.Now.TimeOfDay.Hours >= int.Parse(System.Configuration.ConfigurationManager.AppSettings["StopTime"].ToString().Split(':').FirstOrDefault());
        private static bool isStartTime() => System.DateTime.Now.TimeOfDay.Hours <= int.Parse(System.Configuration.ConfigurationManager.AppSettings["StartTime"].ToString().Split(':').FirstOrDefault());


    }
}
