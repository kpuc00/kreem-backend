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
    public class UserService
    {

        DataBaseContext db = Globals.db;

        public UserService()
        {
            // Load all the roles into memory, 
            // since it's just a few of them that are probable to be used anyway
            // it's better to load them in memory in advance
            // than to have a sepparate query when we actually need them
            db.Roles.Load();
        }

        internal User Create(string name, string email, Role role, string password) {   
            string hash = PasswordHashHelper.CreateHash(password);
            var user = new User(name, email, role, hash);
            return user;
        } 

        public User CreateAndSave(string name, string email, Role role, string password)
        {
            var user = Create(name, email, role, password);
            user = SaveToDatabase(user);
            return user;
        }

        internal User SaveToDatabase (User user)
        {
            db.Roles.Attach(user.Role);

            var fromDb = db.Users.Add(user);
            db.SaveChanges();

            return fromDb;
        }

        /// <summary>
        /// Returns the user from the database if the password matches the hash in the database
        /// </summary>
        /// <param name="email">user's email</param>
        /// <param name="password">the password entered by the user (plain text) </param>
        /// <returns> the user identified by the <code>email</code> and <code>password</code> 
        /// or <code>null</code> if credentials wrong</returns>
        public User AuthenticateByCredentials (string email, string password)
        {
            var candidate = db.Users.SingleOrDefault(u => u.Email == email);
            if (candidate == null)
                return null;

            if (PasswordHashHelper.VerifyPassword(password, candidate.Hash))
            {
                SecurityContext.Authenticate(candidate);
                return candidate;
            }

            return null;
        }

        public ObservableCollection<User> GetAll()
        {
            db.Users.Load();
            return db.Users.Local;
        }

       

    }

}
