using KreemMachineLibrary.Extensions.Date;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ScheduleService
    {
        private DataBaseContext db = Globals.db;
        ShiftService ShiftService = new ShiftService();


        /// <summary>
        /// Returns all shifts scheduled between <b>start</b> and <b>end</b> dates
        /// Clients may depend on the fact that the result is sorted ascending by date
        /// </summary>
        /// <param name="start"> The first date to be included in the results (i.e. incluseive bound) </param>
        /// <param name="end"> The first date that cannot be included in the results (i.e. exclusive bound)</param>
        /// <returns>Returns all shifts scheduled between <code>start</code> and <code>end</code> dates</returns>
        public IEnumerable<ScheduledShift> GetScheduledShifts(DateTime start, DateTime end)
        {
            var shifts = db.ScheduledShifts
                            .Where(ss => ss.Date >= start && ss.Date < end)
                            .Include(ss => ss.EmployeeScheduledShits)
                            .Include(ss => ss.Shift)
                            .OrderBy(ss => ss.Date)
                            .ToList();


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
        public IEnumerable<User> GetSuggestedEmployees(ScheduledShift shift)
        {
            var employees = db.Users.Where(u => u.RoleStr == Role.Employee.ToString())
                .Include(u => u.ScheduledShifts.Select(us => us.ScheduledShift))
                .ToList();

            ShiftService.CacheShifts();

            foreach (User employee in employees)
            {
                if (CanUserWorkShift(employee, shift))
                {
                    yield return employee;
                }
            }
            ShiftService.ClearShiftCache();
        }

        internal bool CanUserWorkShift(User user, ScheduledShift scheduledShift)
        {
            if (IsUserAssignedToShift(user, scheduledShift))
                return true;

            if (HasReachedWorkingHoursLimit(user, scheduledShift.Date))
                return false;

            if (IsUserScheduledForDay(user, scheduledShift.Date))
                return false;

            if (HasUserWorkedPreviousNightShift(user, scheduledShift))
                return false;

            if (HasUserWorkedNextMorningShift(user, scheduledShift))
                return false;

            return true;
        }

        internal bool IsUserAssignedToShift(User user, ScheduledShift scheduledShift)
        {
            return user.ScheduledShifts.Select(us => us.ScheduledShift).Contains(scheduledShift);
        }
        internal bool HasReachedWorkingHoursLimit(User user, DateTime shiftDate)
        {
            DateTime startOfWeek = shiftDate.Date.StartOfWeek();
            DateTime nextWeek = shiftDate.Date.NextWeek();

            double workedHoursInTheWeek = user.ScheduledShifts
                .Where(us => us.ScheduledShift.Date >= startOfWeek && us.ScheduledShift.Date < nextWeek)
                .Sum( us => us.ScheduledShift.Duration);
            return workedHoursInTheWeek >= user.MaxMonthlyHoours;
        }
        internal bool IsUserScheduledForDay(User user, DateTime day)
        {
            return user.ScheduledShifts.Any(us => us.ScheduledShift.Date == day);
        }
        internal bool HasUserWorkedPreviousNightShift(User user, ScheduledShift scheduledShift)
        {
            Shift morningShift = ShiftService.CahchedShifts.First();
            Shift nightShift = ShiftService.CahchedShifts.Last();

            if (scheduledShift.Shift != morningShift)
                return false;

            DateTime previousDay = scheduledShift.Date.AddDays(-1);

            bool hasWorkedPreviousNight = user.ScheduledShifts.Where(us => us.ScheduledShift.Date == previousDay)
                .Select( us => us.ScheduledShift.Shift)
                .Contains(nightShift);

            return hasWorkedPreviousNight;
        }
        internal bool HasUserWorkedNextMorningShift(User user, ScheduledShift scheduledShift)
        {
            Shift morningShift = ShiftService.CahchedShifts.First();
            Shift nightShift = ShiftService.CahchedShifts.Last();

            if (scheduledShift.Shift != nightShift)
                return false;

            DateTime nextDay = scheduledShift.Date.AddDays(1);

            bool hasWorkedNextMorning = user.ScheduledShifts.Where(us => us.ScheduledShift.Date == nextDay)
                .Select(us => us.ScheduledShift.Shift)
                .Contains(morningShift);

            return hasWorkedNextMorning;
        }
    }
}
