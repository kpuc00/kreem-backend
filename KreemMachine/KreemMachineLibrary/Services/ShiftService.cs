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

        public ReadOnlyCollection<Shift> CahchedShifts { get; private set; }

        public ShiftService()
        {
        }

        public void Save(Shift shift)
        {
            using (var db = new DataBaseContext())
            {
                db.Entry(shift).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        internal Shift GetShift(string name)
        {
            using (var db = new DataBaseContext() )
                return db.Shifts.Where(s => s.Name == name).FirstOrDefault();
        }


        public List<Shift> GetAllShifts()
        {
            using (var db = new DataBaseContext() )
                return db.Shifts.OrderBy(s => s.StartHour).ToList();
        }

        public async Task CacheShiftsAsync()
        {
            using (var db = new DataBaseContext())
            {
                List<Shift> shifts = await db.Shifts.OrderBy(s => s.StartHour).ToListAsync();
                CahchedShifts = shifts.AsReadOnly();
            }

        }

        public void ClearShiftCache()
        {
            CahchedShifts = null;
        }

        

    }
}
