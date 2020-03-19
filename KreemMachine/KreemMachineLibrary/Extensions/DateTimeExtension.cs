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

        public static DateTime ThisYear(this DateTime date) =>
            date.Date.AddDays(-DateTime.Today.Day + 1);

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime NextWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday) =>
            dt.StartOfWeek().AddDays(7);
    }
}
