using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class RolesService
    {
        static DataBaseContext db = Globals.db;

        public static Role Administrator => db.Roles.SingleOrDefault(r => r.Name == nameof(Administrator));
        public static Role Manager => db.Roles.SingleOrDefault(r => r.Name == nameof(Manager));
        public static Role Depot => db.Roles.SingleOrDefault(r => r.Name == nameof(Depot));
        public static Role Employee => db.Roles.SingleOrDefault(r => r.Name == nameof(Employee));
        

        
    }
}
