using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Extensions.Date
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Given a datetime will return the first day of the next Month
        /// </summary>
        /// <returns> A datetime for the first day of the next Month</returns>
        public static DateTime NextMonth(this DateTime date) =>
            date.Date.AddDays(-date.Day + 1).AddMonths(1);

        public static DateTime ThisMonth(this DateTime date) =>
            date.Date.AddDays(-DateTime.Today.Day + 1);
    }
}
