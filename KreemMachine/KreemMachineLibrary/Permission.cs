using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary
{
    public enum Permission
    {
        ViewUsers,
        CreateUsers,
        EditUsers,
        DeleteUsers,
        ViewSchedule,
        EditSchedule,
        AutogenerateSchedule,
        ViewStatistics,
        EditShifts,
        ViewAllProducts,
        ViewOwnProducts,
        RequestRestockForAnyProduct,
        RequestRestockForOwnProduct,
        ViewRestockRequests,
        ChangeRestockRequests,
    }
}
