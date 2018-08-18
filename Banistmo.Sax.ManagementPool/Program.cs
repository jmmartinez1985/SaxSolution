using Banistmo.Sax.Common;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.ManagementPool
{
    public class Program
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            //CreateSource.CreateSourceMain();
            Console.WriteLine($"Iniciar tarea...{System.DateTime.Now}");
            log.Info($"Iniciar tarea...{System.DateTime.Now}");
            RecycleApplicationPool(ServerName, Pool);
            Console.WriteLine($"Finalizar tarea...{System.DateTime.Now}");
            log.Info($"Finalizar tarea...{System.DateTime.Now}");
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
                                    log.Info($"Apagando Pool...{System.DateTime.Now}");

                                    while (appPool.State == ObjectState.Starting) { System.Threading.Thread.Sleep(3000); }
                                    if (appPool.State != ObjectState.Stopped)
                                    {
                                        appPool.Stop();
                                    }
                                    //appPoolStopped = true;
                                }
                            }
                            if (appPoolStopped)
                            {
                                if (isStartTime())
                                {
                                    Console.WriteLine($"Iniciando Pool...{System.DateTime.Now}");
                                    log.Info($"Iniciando Pool...{System.DateTime.Now}");
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
                            log.Error(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                            //throw new Exception(string.Format("An Application Pool does not exist with the name {0}.{1}", serverName, appPoolName));
                        }
                    }
                }
                catch (Exception ex) when (ex.Message == "Invalid Stop Runner")
                {
                    log.Error(ex);
                    Console.WriteLine($"Unabled to run stop runner at this...{System.DateTime.Now}");
                }
                catch (Exception ex) when (ex.Message == "Invalid Start Runner")
                {
                    log.Error(ex);
                    Console.WriteLine($"Unabled to run starting runner at this...{System.DateTime.Now}");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    //throw new Exception(string.Format("Unable to restart the application pools for {0}.{1}", serverName, appPoolName), ex.InnerException);
                }
            }
        }

        private static bool isStopTime()
        {
            try
            {
                TimeRange timeRange = new TimeRange();
                var splitedTime = System.Configuration.ConfigurationManager.AppSettings["StopTime"].ToString().Split(';');
                bool IsNowInTheRange = false;
                foreach (var item in splitedTime)
                {
                    timeRange = TimeRange.Parse(item);
                    IsNowInTheRange = timeRange.IsIn(DateTime.Now.TimeOfDay);
                    if (IsNowInTheRange)
                        break;
                }
                return IsNowInTheRange;
            }
            catch (Exception ex)
            {
                log.Error("Invalid Stop Runner");
                log.Error(ex);
                //throw new Exception("Invalid Stop Runner");
            }
            return false;
        }


        private static bool isStartTime()
        {
            try
            {
                TimeRange timeRange = new TimeRange();
                timeRange = TimeRange.Parse(System.Configuration.ConfigurationManager.AppSettings["StartTime"].ToString());
                bool IsNowInTheRange = timeRange.IsIn(DateTime.Now.TimeOfDay);
                return IsNowInTheRange;
            }
            catch (Exception ex)
            {
                log.Error("Invalid Stop Runner");
                log.Error(ex);
                //throw new Exception("Invalid Stop Runner");
            }
            return false;

        }

        private static string Pool => System.Configuration.ConfigurationManager.AppSettings["Pool"].ToString();
        private static string ServerName => System.Configuration.ConfigurationManager.AppSettings["ServerName"].ToString();

    }
}
