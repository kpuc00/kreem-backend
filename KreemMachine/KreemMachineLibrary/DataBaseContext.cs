namespace KreemMachineLibrary
{
    using KreemMachineLibrary.Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    class DataBaseContext : DbContext
    {
        // Your context has been configured to use a 'DBModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'KreemMachineLibrary.DataBaseContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DBModel' 
        // connection string in the application configuration file.
        public DataBaseContext(): base("name=DataBaseContext")
        {

            //Database.Log = (s) => Console.WriteLine(s);
            Database.SetInitializer<DataBaseContext>(null);
        }


        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Shift> Shifts { get; set; }

        public virtual DbSet<ScheduledShift> ScheduledShifts { get; set; }

        public virtual DbSet<UserScheduledShift> UserScheduledShifts { get; set; }

        public virtual DbSet<Department> Departments { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<RestockRequest> RestockRequests { get; set; }

        public virtual DbSet<RestockStage> RestockStages { get; set; }

        public virtual DbSet<ProductSale> ProductSales { get; set; }

    }
}