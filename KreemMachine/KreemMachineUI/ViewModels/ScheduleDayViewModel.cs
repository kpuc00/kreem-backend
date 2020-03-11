using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachine.ViewModels
{
    public class ScheduleDayViewModel
    {

        public DateTime day { get; set; }

        public List<ScheduledShift> Shifts { get; set; }

        public ScheduleDayViewModel(DateTime day)
        {
            this.day = day;
        }

        public override string ToString()
        {
            return day.Day.ToString();
        }

    }
}
