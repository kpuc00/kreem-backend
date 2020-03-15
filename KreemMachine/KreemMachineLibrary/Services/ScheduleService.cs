using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ScheduleService
    {
        private DataBaseContext db = Globals.db;

        /// <summary>
        /// Returns all shifts scheduled between <code>start</code> and <code>end</code> dates
        /// Clients may depend on the fact that the result is sorted ascending by date
        /// </summary>
        /// <param name="start"> The first date to be included in the results (i.e. incluseive bound) </param>
        /// <param name="end"> The first date that cannot be included in the results (i.e. exclusive bound)</param>
        /// <returns>Returns all shifts scheduled between <code>start</code> and <code>end</code> dates</returns>
        public IEnumerable<ScheduledShift> GetScheduledShifts(DateTime start, DateTime end)
        {
            var shifts = db.ScheduledShifts
                            .Where(s => s.Date >= start && s.Date < end)
                            .OrderBy(s => s.Date);


            return shifts;
        }

        public ScheduledShift GetScheduledShiftOrCreateNew(DateTime date, Shift shift)
        {

            var scheduleFromDb = db.ScheduledShifts.Where(s => s.ShiftId == shift.Id && s.Date == date).FirstOrDefault();
            return scheduleFromDb ?? Save(new ScheduledShift(date, shift));
        }

        internal ScheduledShift Save(ScheduledShift scheduledShift)
        {
            var shift = db.ScheduledShifts.Add(scheduledShift);
            db.SaveChanges();
            return shift;
        }

        public void AssignUserToShift(User user, ScheduledShift shift)
        {
            bool alreadyAssigned = db.UserScheduledShifts
                .Any(us => us.UserId == user.Id && us.ScheduledShiftId == shift.Id);

            if (!alreadyAssigned)
            {
                var userShiftAssignment = new UserScheduledShift(user, shift);
                db.UserScheduledShifts.Add(userShiftAssignment);
                db.SaveChanges();
            }
        }

        public void RemoveUserFromShift(User user, ScheduledShift shift)
        {

            var userShiftAssignment = db.UserScheduledShifts.Where(us => us.UserId == user.Id && us.ScheduledShiftId == shift.Id).FirstOrDefault();
            if(userShiftAssignment != null)
            {
                db.UserScheduledShifts.Remove(userShiftAssignment);
                db.SaveChanges();
            }

        }
    }
}
