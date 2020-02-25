using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KreemMachineLibrary
{
    static class Globals
    {
        internal static DataBaseContext db = new DataBaseContext();

        
        static Globals()
        {
            // Force creating 'the model' so the app doesn't lag on first querry
            _ = db.Users.FirstOrDefault(); 
        }

    }
}
