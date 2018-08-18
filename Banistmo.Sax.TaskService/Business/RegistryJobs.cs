using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.TaskService.Business
{
   public  class RegistryJobs : Registry
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RegistryJobs()
        {

            log.Info("Registering Services Task");
            //Schedule<FileJob>().ToRunNow().AndEvery(10).Seconds();
            Schedule<StartJob>().ToRunEvery(10).Seconds();
            //Schedule<StartJob>().ToRunEvery(1).Days().At(15, 40);
            Schedule<StopJob>().ToRunEvery(10).Seconds();

            log.Info("Ending Registering Services Task");

        }
    }
}
