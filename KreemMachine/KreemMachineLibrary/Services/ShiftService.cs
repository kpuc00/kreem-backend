using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
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

        public IList<Shift> getAllShifts()
        {
            return db.Shifts.OrderBy(s => s.StartHour).ToList();   
        }
    }
}
