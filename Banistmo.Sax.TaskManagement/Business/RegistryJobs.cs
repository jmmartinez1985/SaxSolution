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

            Schedule<StartJob>().ToRunNow().AndEvery(10).Seconds();
            Schedule<StopJob>().ToRunNow().AndEvery(10).Seconds();

        }
    }
}
