using KreemMachineLibrary.Helpers;
using KreemMachineLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Services
{
    public class UserService
    {

        DataBaseContext db = Globals.db;
        
        internal User Create(string name, string email, Role role, string password) {   
            string hash = PasswordHashHelper.CreateHash(password);
            var user = new User(name, email, role, hash);
            return user;
        } 

        public User CreateAndSave(string name, string email, Role role, string password)
        {
            var user = Create(name, email, role, password);

            user = SaveToDatabase(user);
            
       
            db.SaveChanges();
            return user;
        }

        internal User SaveToDatabase (User user)
        {
            db.Roles.Attach(user.Role);
            return db.Users.Add(user);
        }

        /// <summary>
        /// Returns the user from the database if the password matches the hash in the database
        /// </summary>
        /// <param name="email">user's email</param>
        /// <param name="password">the password entered by the user</param>
        /// <returns> the user identified by the <code>email</code> and <code>password</code> 
        /// or <code>null</code> if credentials wrong</returns>
        public User getByCredentials (string email, string password)
        {
            var candidate = db.Users.SingleOrDefault(u => u.Email == email);
            if (candidate == null)
                return null;

            if (PasswordHashHelper.VerifyPassword(password, candidate.Hash))
                return candidate;

            return null;
        }


    }
}
