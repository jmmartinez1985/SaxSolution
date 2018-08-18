using Banistmo.Sax.TaskService.Business;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.TaskService
{
    public partial class TaskService : ServiceBase
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TaskService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Iniciando los schedulers....");
            JobManager.Initialize(new RegistryJobs());
        }

        protected override void OnStop()
        {
            log.Info("Algo mal....");
        }
    }
}
