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

        #region Statistics per shift
        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShift(DateTime displayMonth, string cbMornnig, string cbNoon, string cbNight)
        {
            var nextMonth = displayMonth.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= displayMonth && ss.Date < nextMonth && (ss.Shift.Name == cbMornnig || ss.Shift.Name == cbNoon || ss.Shift.Name == cbNight)
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
                             NumberOfEmployees = g.Count(),
                             Cost = g.Sum(us => us.US.HourlyWage)
                         };

            return new ObservableCollection<ResourcesPerShiftDTO>(result);
        }

        public ObservableCollection<ResourcesPerShiftDTO> GetResourcesPerShift(DateTime displayMonth)
        {
            return GetResourcesPerShift(displayMonth, "Morning", "Noon", "Night");
        }
        #endregion

        #region Statistics per month
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
                             Month = g.FirstOrDefault().SS.Date,
                             NumberOfEmployees = g.Count(),
                             Cost = g.Sum(us => us.US.HourlyWage)
                         };


            return new ObservableCollection<ResourcesPerMonthDTO>(result);
        }
        #endregion

        #region Employee statistics
        public ObservableCollection<ResourcesPerEmployeeDTO> GetResourcesPerEmployeeDate(DateTime fromDate, DateTime toDate)
        {
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
                             EmployeeName = g.Key.user.FirstName + " " + g.Key.user.LastName,
                             NumberOfScheduledShifts = g.Count(),
                             HoursWorked = g.Sum(y => y.SS.Duration),
                             Cost = g.Sum(x => x.US.HourlyWage)
                         };

            return new ObservableCollection<ResourcesPerEmployeeDTO>(result);
        }
        #endregion

        #region Stock statistics
        public float CalculateProfit(Product product)
        {
            using (var db = new DataBaseContext())
            {
                return db.Products.Where(p => p.Id == product.Id)
                                  .Select(p => (p.SellPrice - p.BuyCost) * p.Quantity)
                                  .FirstOrDefault();
            }
        }



        // In order to sell a product we have to get the quantity of products sold and remove them from the current quantity of the product
        // 1 New form that accepts a number(integer) of how many products are sold
        // 2 On confirmation -> previous quantity - inputted quantity = get the profit

        #endregion
    }
}
