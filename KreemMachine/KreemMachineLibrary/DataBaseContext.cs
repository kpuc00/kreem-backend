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
        public DataBaseContext()
            : base("name=DataBaseContext")
        {
            Console.WriteLine("initialized db context ");

        }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<User> Users { get; set; }
    }
}