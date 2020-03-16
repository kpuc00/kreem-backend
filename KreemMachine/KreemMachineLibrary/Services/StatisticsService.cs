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
            var shift = db.ScheduledShifts.Where(s => s.Date == displayMonth).FirstOrDefault();

            var result = from ss in db.ScheduledShifts
                         join us in db.UserScheduledShifts on ss.Id equals us.ScheduledShiftId
                         where SqlMethods.Like(ss.Date.ToString(), "2020/01/__")
                         group ss by new {
                             ss.Date,
                             ss.Shift
                             
                         } into g
                         select new {
                             g.Key.Date,
                             g.Key.Shift,
                             count = us.UserId.Count()
                         };


            

            var results = from s in db.UserScheduledShifts
                          join u in db.Users on s.UserId equals u.Id
                          where s.
                          group s.UserId  by p.PersonId into g
                          select new { PersonId = g.Key, Cars = g.ToList() };
            return shift;
        }

        
    }
}
