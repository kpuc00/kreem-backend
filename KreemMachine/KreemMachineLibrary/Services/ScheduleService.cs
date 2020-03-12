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

        public IEnumerable<ScheduledShift> GetScheduledShifts(DateTime start, DateTime end)
        {
            var shifts = db.ScheduledShifts
                            .Where(s => s.Date > start && s.Date < end)
                            .OrderBy(s => s.Date);


            return shifts;
        }

    }
}
