using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            return shift;
        }

        
    }
}
