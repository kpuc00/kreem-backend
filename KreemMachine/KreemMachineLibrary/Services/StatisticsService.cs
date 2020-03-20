using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.DTO;

namespace KreemMachineLibrary.Services
{
    public class StatisticsService
    {
        DataBaseContext db = Globals.db;

        //Statistics per shift

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShift(DateTime displayMonth, string cbMornnig, string cbNoon, string cbEvening)
        {
            var nextMonth = displayMonth.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= displayMonth && ss.Date < nextMonth && (ss.Shift.Name == cbMornnig || ss.Shift.Name == cbNoon || ss.Shift.Name == cbEvening)
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new ResourcesPerShiftDTO
                         {
                             Date = g.Key.Date,
                             Shift = g.Key.Shift.Name,
                             Employees = g.Count(),
                             Cost = g.Sum(us => us.US.HourlyWage)
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShift(DateTime displayMonth)
        {
            var nextMonth = displayMonth.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= displayMonth && ss.Date < nextMonth
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new ResourcesPerShiftDTO
                         {
                             Date = g.Key.Date,
                             Shift = g.Key.Shift.Name,
                             Employees = g.Count(),
                             Cost = g.Sum(us => us.US.HourlyWage)
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        //Statistics per month

        public ObservableCollection<ResourcesPerMonthDTO> GetResourcesPerMonth(DateTime displayYear)
        {
            var nextYear = displayYear.AddYears(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= displayYear && ss.Date <= nextYear
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date.Month
                         } into g
                         select new ResourcesPerMonthDTO
                         {
                             //Month = g.Key.Month.ToString(),
                             EmployeeShifts = g.Count(),
                             Cost = g.Sum(us => us.US.HourlyWage)
                         };

            /*Console.WriteLine("result:");
            foreach (var x in result) { 
                Console.WriteLine(x.empCount + " " + x.empCost);
            }*/

            return new ObservableCollection<ResourcesPerMonthDTO>(result);
        }

        //Employee statistics

        public ObservableCollection<ResourcesPerEmployeeDTO> GetResourcesPerEmployee()
        {
            //var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         //where ss.Date >= dateTime && ss.Date < nextMonth
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             user = joined.US.User,
                         } into g
                         select new ResourcesPerEmployeeDTO
                         {
                             Employee = g.Key.user.FirstName + " " + g.Key.user.LastName,
                             Shifts = g.Count(),
                             HoursWorked = g.Sum(y => y.SS.Duration),
                             Cost = g.Sum(x => x.US.HourlyWage)
                         };

            Console.WriteLine("result:");
            foreach (var x in result)
            {
                Console.WriteLine(x.Employee + " " + x.Shifts + " " + x.HoursWorked + " " + x.Cost);
            }

            return new ObservableCollection<ResourcesPerEmployeeDTO>(result);
        }

        public ObservableCollection<ResourcesPerEmployeeDTO> GetResourcesPerEmployeeDate(DateTime fromDate, DateTime toDate)
        {
            //var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= fromDate && ss.Date < toDate
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             user = joined.US.User,
                         } into g
                         select new ResourcesPerEmployeeDTO
                         {
                             Employee = g.Key.user.FirstName + " " + g.Key.user.LastName,
                             Shifts = g.Count(),
                             HoursWorked = g.Sum(y => y.SS.Duration),
                             Cost = g.Sum(x => x.US.HourlyWage)
                         };

            Console.WriteLine("result:");
            foreach (var x in result)
            {
                Console.WriteLine(x.Employee + " " + x.Shifts + " " + x.HoursWorked + " " + x.Cost);
            }

            return new ObservableCollection<ResourcesPerEmployeeDTO>(result);
        }
    }
}
