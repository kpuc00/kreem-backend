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

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShiftAll(DateTime dateTime)
        {
            var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= dateTime && ss.Date < nextMonth 
                         select new { SS = ss, US = us } into joined
                         group joined by new {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new ResourcesPerShiftDTO {
                             empCount = g.Count(),
                             empCost = g.Sum(us => us.US.HourlyWage),
                             date = g.Key.Date,
                             shift = g.Key.Shift.Name
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShiftMorning(DateTime dateTime)
        {
            var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= dateTime && ss.Date < nextMonth && ss.Shift.ToString() == "Morning"
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new ResourcesPerShiftDTO
                         {
                             empCount = g.Count(),
                             empCost = g.Sum(us => us.US.HourlyWage),
                             date = g.Key.Date,
                             shift = g.Key.Shift.Name
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShiftNoon(DateTime dateTime)
        {
            var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= dateTime && ss.Date < nextMonth && ss.Shift.ToString() == "Noon"
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new ResourcesPerShiftDTO
                         {
                             empCount = g.Count(),
                             empCost = g.Sum(us => us.US.HourlyWage),
                             date = g.Key.Date,
                             shift = g.Key.Shift.Name
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShiftEvening(DateTime dateTime)
        {
            var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= dateTime && ss.Date < nextMonth && ss.Shift.ToString() == "Evening"
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new ResourcesPerShiftDTO
                         {
                             empCount = g.Count(),
                             empCost = g.Sum(us => us.US.HourlyWage),
                             date = g.Key.Date,
                             shift = g.Key.Shift.Name
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        public ObservableCollection<ResourcesPerMonthDTO> GetResourcesPerMonth()
        {
            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= new DateTime(2020, 03, 01) && ss.Date <= new DateTime(2020, 03, 31)
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             joined.SS.Date.Month
                         } into g
                         select new ResourcesPerMonthDTO
                         {
                             empCount = g.Count(),
                             empCost = g.Sum(us => us.US.HourlyWage)
                         };

            /*Console.WriteLine("result:");
            foreach (var x in result) { 
                Console.WriteLine(x.empCount + " " + x.empCost);
            }*/

            return new ObservableCollection<ResourcesPerMonthDTO>(result);
        }

        public ObservableCollection<ResourcesPerEmployeeDTO> GetResourcesPerEmployee(DateTime dateTime) 
        {
            var nextMonth = dateTime.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= dateTime && ss.Date < nextMonth
                         select new { SS = ss, US = us } into joined
                         group joined by new
                         {
                             user = joined.US.User,
                         } into g
                         select new ResourcesPerEmployeeDTO
                         {
                             name = g.Key.user.FirstName + " " +g.Key.user.LastName,
                             shiftCount = g.Count(),
                             hourSum = g.Sum(y => y.SS.Duration),
                             empCost = g.Sum(x => x.US.HourlyWage)
                         };

            Console.WriteLine("result:");
            foreach (var x in result)
            {
                Console.WriteLine(x.name + " " + x.shiftCount + " " + x.hourSum + " " + x.empCost);
            }

            return new ObservableCollection<ResourcesPerEmployeeDTO>(result);
        }
    }
}
