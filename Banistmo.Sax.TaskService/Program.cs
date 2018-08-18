﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.TaskService
{
    static class Program
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            log.Info("Starting Task Runner");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TaskService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
