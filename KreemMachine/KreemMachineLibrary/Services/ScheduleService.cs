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
            using (var db = new DataBaseContext())
                return db.ScheduledShifts
                    .Where(ss => ss.Date >= start && ss.Date < end)
                    .Include(ss => ss.EmployeeScheduledShits)
                    .Include(ss => ss.Shift)
                    .OrderBy(ss => ss.Date)
                    .ToList();

        }

        public async Task<ScheduledShift> GetScheduledShiftOrCreateNew(DateTime date, Shift shift)
        {
            using (var db = new DataBaseContext())
            {
                var scheduleFromDb = await db.ScheduledShifts
                                        .Where(s => s.ShiftId == shift.Id && s.Date == date)
                                        .Include(s => s.EmployeeScheduledShits)
                                        .Include(s => s.Shift)
                                        .FirstOrDefaultAsync();

                if (scheduleFromDb == null)
                {
                    scheduleFromDb = db.ScheduledShifts.Add(new ScheduledShift(date, shift));
                    await db.SaveChangesAsync();
                    db.Entry(scheduleFromDb).Reference(s => s.Shift).Load();
                }
                return scheduleFromDb;

            }


        }


        public void AssignUserToShift(User user, ScheduledShift shift)
        {
            using (var db = new DataBaseContext())
            {
                bool alreadyAssigned = db.UserScheduledShifts.Any(us => us.UserId == user.Id && us.ScheduledShiftId == shift.Id);
                if (!alreadyAssigned)
                {
                    var userShiftAssignment = new UserScheduledShift(user, shift);
                    db.UserScheduledShifts.Add(userShiftAssignment);
                    db.SaveChanges();
                }
            }
        }          


        public void RemoveUserFromShift(User user, ScheduledShift shift) 
        {
            using (var db = new DataBaseContext())
            {
                var userScheduledShift = db.UserScheduledShifts.Where(us => us.UserId == user.Id && us.ScheduledShiftId == shift.Id);
                db.UserScheduledShifts.RemoveRange(userScheduledShift);
                db.SaveChanges();
            }
        }
        public IEnumerable<User> GetSuggestedEmployees(ScheduledShift shift)
        {
            Task<List<User>> getEmployees;
            using (var db = new DataBaseContext())
                 getEmployees = db.Users.Where(u => u.RoleStr == Role.Employee.ToString())
                .Include(u => u.ScheduledShifts)
                .Include(u => u.ScheduledShifts.Select(us => us.ScheduledShift))
                .ToListAsync();

            Task getShifts = ShiftService.CacheShiftsAsync();

            getEmployees.Wait();
            getShifts.Wait();

            foreach (User employee in getEmployees.Result)
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
            return user.ScheduledShifts.Any( us => us.ScheduledShift.Id == scheduledShift.Id);
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
