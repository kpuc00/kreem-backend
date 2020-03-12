using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachine.ViewModels
{
    public class ScheduleDayViewModel
    {

        public DateTime Day { get; set; }

        public IList<ScheduledShift> Shifts { get; set; } = new List<ScheduledShift>();

        public Brush StatusBackgroundColor
        {
            get
            {
                if (Shifts.Count == 0)
                    return Brushes.Transparent;
                else
                    return Brushes.LawnGreen;
            }
        }

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
