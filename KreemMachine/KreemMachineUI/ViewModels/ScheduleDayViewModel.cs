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
                if (IsUnscheduled())
                    return Brushes.Transparent;

                else if (IsUnderstaffed())
                    return Brushes.Red;

                else if (IsOverstaffed())
                    return Brushes.Yellow;

                else
                    return Brushes.LawnGreen;

            }
        }

        private bool IsUnscheduled() =>
            Shifts.Count == 0;

        private bool IsUnderstaffed() => 
            Shifts.Any(s => s.isUnderstaffed);


        private bool IsOverstaffed() => 
             Shifts.Any(s => s.IsOverstaffed);


        public ScheduleDayViewModel(DateTime day)
        {
            this.Day = day;
        }

    }
}
