using KreemMachineLibrary.Extensions.Date;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Models.Statics;
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
        public Task<List<ScheduledShift>> GetScheduledShiftsAsync(DateTime start, DateTime end)
        {
            using (var db = new DataBaseContext())
                return db.ScheduledShifts
                    .Where(ss => ss.Date >= start && ss.Date < end)
                    .Include(ss => ss.EmployeeScheduledShits.Select(es => es.User))
                    .Include(ss => ss.Shift)
                    .OrderBy(ss => ss.Date)
                    .ToListAsync();

        }


        public List<ScheduledShift> GetOrCreateScheduledShifts(DateTime start, DateTime end) 
        {
            Console.WriteLine($"Try to get data {start} – {end}");
            using (var db = new DataBaseContext())
            {
                var shiftTypes = db.Shifts.ToList();
                var shifts = new List<ScheduledShift>();

                var existing = GetScheduledShiftsAsync(start, end).Result;

                while(start < end)
                {
                    foreach (var shift in shiftTypes)
                        if(!existing.Any(s=>s.Date == start && s.ShiftId == shift.Id))
                        {
                            var newShift = new ScheduledShift(start, shift);
                            newShift.EmployeeScheduledShits = new List<UserScheduledShift>();
                            shifts.Add(newShift);
                        }
                    start = start.AddDays(1);
                }
                db.ScheduledShifts.AddRange(shifts);

                db.SaveChanges();
                Console.WriteLine($"Finished to get data {start} – {end}");


                return shifts.Union(existing).ToList();
            }
            
        }

        public async Task<ScheduledShift> GetScheduledShiftOrCreateNewAsync(DateTime date, Shift shift)
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


        public async Task AssignUserToShiftAsync(User user, ScheduledShift shift)
        {
            using (var db = new DataBaseContext())
            {
                bool alreadyAssigned = await db.UserScheduledShifts.AnyAsync(us => us.UserId == user.Id && us.ScheduledShiftId == shift.Id);
                if (!alreadyAssigned)
                {
                    var userShiftAssignment = new UserScheduledShift(user, shift);
                    db.UserScheduledShifts.Add(userShiftAssignment);
                    await db.SaveChangesAsync();
                }
            }
        }


        public Task<int> RemoveUserFromShiftAsync(User user, ScheduledShift shift)
        {
            using (var db = new DataBaseContext())
            {
                var userScheduledShift = db.UserScheduledShifts.Where(us => us.UserId == user.Id && us.ScheduledShiftId == shift.Id);
                db.UserScheduledShifts.RemoveRange(userScheduledShift);
                return db.SaveChangesAsync();
            }
        }
        public IEnumerable<User> GetSuggestedEmployees(ScheduledShift shift)
        {
            Task<List<User>> getEmployees;
            using (var db = new DataBaseContext())
                getEmployees = db.Users.Where(u => u.RoleStr == Role.Employee.ToString())
               .Where(u => !u.BlockOffs.Any(b => b.ScheduledShiftId == shift.Id ))
               .Include(u => u.ScheduledShifts)
               .Include(u => u.ScheduledShifts.Select(us => us.ScheduledShift))
               .ToListAsync();

            Task getShifts = ShiftService.CacheShiftsAsync();

            getEmployees.Wait();
            getShifts.Wait();

            var sugested = getEmployees.Result
                .Where(user => CanUserWorkShift(user, shift))
                .OrderBy(u => u.GetHoursWorkedInWeek(shift.Date))
                .ToList();
           
            return sugested;
        }

        internal bool CanUserWorkShift(User user, ScheduledShift scheduledShift)
        {
            if (CurrentUserHasAuthorityOverEmployee(user) == false)
                return false;

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

        private bool CurrentUserHasAuthorityOverEmployee(User user)
        {
            if (SecurityContext.HasPermissions(Permission.ScheduleAnyEmployee))
                return true;
            if (SecurityContext.HasPermissions(Permission.ScheduleOwnEmployee) && user.DepartmentId == SecurityContext.CurrentUser.DepartmentId)
                return true;
            return false;
        }

        internal bool IsUserAssignedToShift(User user, ScheduledShift scheduledShift)
        {
            return user.ScheduledShifts.Any(us => us.ScheduledShift.Id == scheduledShift.Id);
        }
        internal bool HasReachedWorkingHoursLimit(User user, DateTime shiftDate)
        {
            DateTime startOfWeek = shiftDate.Date.StartOfWeek();
            DateTime nextWeek = shiftDate.Date.NextWeek();

            double workedHoursInTheWeek = user.ScheduledShifts
                .Where(us => us.ScheduledShift.Date >= startOfWeek && us.ScheduledShift.Date < nextWeek)
                .Sum(us => us.ScheduledShift.Duration);
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

            if (scheduledShift.ShiftId != morningShift.Id)
                return false;

            DateTime previousDay = scheduledShift.Date.AddDays(-1);

            bool hasWorkedPreviousNight = user.ScheduledShifts.Any(us => us.ScheduledShift.Date == previousDay && us.ScheduledShift.ShiftId == nightShift.Id);

            return hasWorkedPreviousNight;
        }
        internal bool HasUserWorkedNextMorningShift(User user, ScheduledShift scheduledShift)
        {
            Shift morningShift = ShiftService.CahchedShifts.First();
            Shift nightShift = ShiftService.CahchedShifts.Last();

            if (scheduledShift.ShiftId != nightShift.Id)
                return false;

            DateTime nextDay = scheduledShift.Date.AddDays(1);

            bool hasWorkedNextMorning = user.ScheduledShifts.Any(us => us.ScheduledShift.Date == nextDay && us.ScheduledShift.ShiftId == morningShift.Id);

            return hasWorkedNextMorning;
        }

        public void AutogenerateSchedule(List<ScheduledShift> shifts)
        {
            using (var db = new DataBaseContext())
                foreach (var shift in shifts)
                {
                    var employees = GetSuggestedEmployees(shift).ToList();
                    foreach(var employee in employees)
                    {
                        if (shift.EmployeeScheduledShits.Count >= shift.Shift.MinStaff)
                            break;
                        if (shift.EmployeeScheduledShits.Any(e => e.Id == employee.Id))
                            continue;
                        var assignment = new UserScheduledShift(employee, shift);
                        db.UserScheduledShifts.Add(assignment);
                        shift.EmployeeScheduledShits.Add(assignment);
                    }
                    db.SaveChanges();
                }
        }

    }
}
