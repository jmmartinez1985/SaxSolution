using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.TaskManagement.Business
{
    public class RegistryJobs : Registry
    {
        public RegistryJobs()
        {

            //Schedule<FileJob>().ToRunNow().AndEvery(10).Seconds();
            Schedule<StartJob>().ToRunOnceIn(5).Seconds();
            //Schedule<StartJob>().ToRunEvery(1).Days().At(15, 40);
            Schedule<StopJob>().ToRunEvery(1).Days().At(19, 15);

        }
    }
}
