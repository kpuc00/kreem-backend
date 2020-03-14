using KreemMachineLibrary.Helpers;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class ShiftService
    {
        private DataBaseContext db = Globals.db;

        public ShiftService()
        {

        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        internal Shift GetShift(string name)
        {
            var shift = db.Shifts.Where(s => s.Name == name).FirstOrDefault();
            return shift;
        }

        public void ChangeShiftStart(string shiftName, TimeSpan time)
        {
            GetShift(shiftName).StartHour = time;
            db.SaveChanges();
        }

        public void ChangeShiftEnd(string shiftName, TimeSpan time)
        {
            GetShift(shiftName).EndHour = time;
            db.SaveChanges();
        }

        public void ChangeMinimumStaff(string shiftName, int number)
        {
            GetShift(shiftName).MinStaff = number;
            db.SaveChanges();
        }

        public void ChangeMaximumStaff(string shiftName, int number)
        {
            GetShift(shiftName).MaxStaff = number;
            db.SaveChanges();
        }

        public void ChangePreferredStaff(string shiftName, int number)
        {
            GetShift(shiftName).PreferredStaff = number;
            db.SaveChanges();
        }

        public IEnumerable<Shift> GetAllShifts() => db.Shifts.ToList();

    }
}
