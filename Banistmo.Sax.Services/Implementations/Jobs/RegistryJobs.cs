using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations.Jobs
{
    public class RegistryJobs : Registry
    {
        public RegistryJobs()
        {

            //Schedule<FileJob>().ToRunNow().AndEvery(10).Seconds();
            //Schedule<FileJob>().ToRunOnceIn(5).Seconds();
            Schedule<FileJob>().ToRunEvery(1).Days().At(21, 15);

        }
    }
}
