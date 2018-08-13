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
            Console.WriteLine($"Iniciar tarea...{System.DateTime.Now}");
            RecycleApplicationPool(ServerName, Pool);
            Console.WriteLine($"Finalizar tarea...{System.DateTime.Now}");
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
                                    Console.WriteLine($"Apagando Pool...{System.DateTime.Now}");
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
                                    Console.WriteLine($"Iniciando Pool...{System.DateTime.Now}");
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
        private static string Pool => System.Configuration.ConfigurationManager.AppSettings["Pool"].ToString();
        private static string ServerName => System.Configuration.ConfigurationManager.AppSettings["ServerName"].ToString();

    }
}
