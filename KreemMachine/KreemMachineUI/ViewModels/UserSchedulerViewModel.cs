using KreemMachineLibrary.Extensions.Date;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachine.ViewModels
{
    /// <summary>
    /// Contains data and interface logic for each employee 
    /// in the conext of the manual scheduler
    /// </summary>
    class UserSchedulerViewModel
    {
        public User User { get; set; }

        public ScheduledShift Shift {get; set;}

        public double HoursScheduledThisWeek { get; }

        public bool IsScheduled { get; }

        public UserSchedulerViewModel(User user, ScheduledShift shift)
        {
            User = user;
            Shift = shift;
            
            IsScheduled = shift?.EmployeeScheduledShits?.Any(us => us.UserId == user.Id) ?? false;

            HoursScheduledThisWeek = user?.ScheduledShifts
                                            ?.Where(s => s.ScheduledShift.Date >= shift.Date.StartOfWeek() && s.ScheduledShift.Date < shift.Date.NextWeek())
                                            ?.Sum( s => s.ScheduledShift.Duration) ?? 0;

        }
    }
}
