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

        /// <summary>
        /// Returns all shifts scheduled between <code>start</code> and <code>end</code> dates
        /// Clients may depend on the fact that the result is sorted ascending by date
        /// </summary>
        /// <param name="start"> The first date to be included in the results (i.e. incluseive bound) </param>
        /// <param name="end"> The first date that cannot be included in the results (i.e. exclusive bound)</param>
        /// <returns>Returns all shifts scheduled between <code>start</code> and <code>end</code> dates</returns>
        public IEnumerable<ScheduledShift> GetScheduledShifts(DateTime start, DateTime end)
        {
            var shifts = db.ScheduledShifts
                            .Where(s => s.Date >= start && s.Date < end)
                            .OrderBy(s => s.Date);


            return shifts;
        }

    }
}
