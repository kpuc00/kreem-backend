using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreemMachineLibrary.Models;

namespace KreemMachineLibrary.Services
{
    public class StatisticsService
    {
        DataBaseContext db = Globals.db;

        public ScheduledShift GetShiftPerMonth(DateTime displayMonth)
        {
            //var shift = db.ScheduledShifts.Where(s => s.Date == displayMonth).FirstOrDefault();

            var nextMonth = displayMonth.AddMonths(1);

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where ss.Date >= displayMonth && ss.Date < nextMonth //SqlMethods.Like(ss.Date.ToString(), "2020/01/__")
                         select new { SS = ss, US = us } into joined
                         group joined by new {
                             joined.SS.Date,
                             joined.SS.Shift
                         } into g
                         select new {
                             empCount = g.Count(),
                             empCost = g.Sum(us => us.US.HourlyWage),
                             date = g.Key.Date,
                             shift = g.Key.Shift
                         };

            Console.WriteLine("results:");
            foreach (var x in result) {
                Console.WriteLine(x.empCost + " " + x.empCount + " " + x.date + " " + x.shift.Name);
            }


            

            /*var results = from s in db.UserScheduledShifts
                          join u in db.Users on s.UserId equals u.Id
                          where s.
                          group s.UserId  by p.PersonId into g
                          select new { PersonId = g.Key, Cars = g.ToList() };*/
            return null;
        }

        
    }
}
