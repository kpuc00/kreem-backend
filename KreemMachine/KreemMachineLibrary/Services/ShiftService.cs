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
        DataBaseContext db = Globals.db;

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


        public IList<Shift> GetAllShifts() => db.Shifts.OrderBy(s => s.StartHour).ToList();

    }
}
