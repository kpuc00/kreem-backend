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

        public double HoursScheduledThisMonth { get; }

        public bool IsScheduled { get; }

        public UserSchedulerViewModel(User user, ScheduledShift shift)
        {
            User = user;
            Shift = shift;
            
            IsScheduled = shift?.EmployeeScheduledShits?.Any(us => us.UserId == user.Id) ?? false;

            HoursScheduledThisMonth = user?.ScheduledShifts
                                            ?.Where(s => s.ScheduledShift.Date > shift.Date.ThisMonth() && s.ScheduledShift.Date < shift.Date.NextMonth())
                                            ?.Sum( s => s.ScheduledShift.Duration) ?? 0;

        }
    }
}
