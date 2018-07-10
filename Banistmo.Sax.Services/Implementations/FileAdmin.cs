using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banistmo.Sax.Services.Implementations
{
    public class FileAdmin:Registry

    {
        public FileAdmin() {
            Action someMethod = new Action(() =>
            {
                Console.WriteLine("Timed Task - Will run now");
            });

            // Schedule schedule = new Schedule(someMethod);
            // schedule.ToRunNow();

            this.Schedule(someMethod).ToRunNow();
        }
        public static void  PurgarExcel() {

        }
    }
}
