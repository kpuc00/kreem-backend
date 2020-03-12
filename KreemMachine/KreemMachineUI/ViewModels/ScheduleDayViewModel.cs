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

        public DateTime Day { get; set; }

        public IList<ScheduledShift> Shifts { get; set; } = new List<ScheduledShift>();

        public ScheduleDayViewModel(DateTime day)
        {
            this.Day = day;
        }

        public override string ToString()
        {
            return Day.Day.ToString();
        }

    }
}
