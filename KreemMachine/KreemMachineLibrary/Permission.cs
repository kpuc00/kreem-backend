using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary
{
    public enum Permission
    {
        // CRUD Users
        ViewAllUsers,
        ViewOwnUsers,
        CreateUsers,
        EditUsers,
        DeleteUsers,
        // Schedule
        ViewSchedule,
        EditSchedule,
        AutogenerateSchedule,
        // Statistics
        ViewStatistics,
        EditShifts,
        // Products
        CreateProducts,
        DeleteProducts,
        EditOwnProducts,
        ViewAllProducts,
        ViewOwnProducts,
        SellOwnProducts,
        // Restock Requests
        RequestRestockForAnyProduct,
        RequestRestockForOwnProduct,
        ViewRestockRequests,
        ChangeRestockRequests,
        // Departments
        CreateDepartments,
        ViewDeparments,
        EditDepartments,
        DeleteDepartments,
        // Settings
        ViewSettings,
        ChangeSettings,
    }
}
