namespace KreemMachineLibrary.Migrations
{
    using KreemMachineLibrary.Models;
    using KreemMachineLibrary.Services;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<KreemMachineLibrary.DataBaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        /// <summary>
        /// Is called after every migration.
        /// In case there is no data in the database it will create some bare-minimum entries
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(DataBaseContext context)
        {
            context.Roles.AddOrUpdate(
                new Role(1, nameof(RolesService.Administrator)),
                new Role(2, nameof(RolesService.Manager)),
                new Role(3, nameof(RolesService.Depot)),
                new Role(4, nameof(RolesService.Employee))
            );
            var userService = new UserService();
            var user = userService.Create("Admin Adminov", "admin@mail.com", null, "qweqwe");
            user.role = 1;
            user.Id = 1;
            context.Users.AddOrUpdate(user);

        }
    }
}
