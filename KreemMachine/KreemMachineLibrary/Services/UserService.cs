using KreemMachineLibrary.Helpers;
using KreemMachineLibrary.Models;
using KreemMachineLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows;

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
            // db.Roles.Load();
        }

        public User Save(User user)
        {
            HashPassword(user);
            user = SaveToDatabase(user);
            return user;
        }

        internal void HashPassword(User user)
        {
            // TODO: Use randomly generated passwords later
            Random random = new Random();
            int length = 10;
            var randomPassword = "";
            for (var i = 0; i < length; i++)
            {
                randomPassword += ((char)(random.Next(1, 26) + 64)).ToString();
            }
            user.Password = randomPassword;
            user.PasswordHash = PasswordHashHelper.CreateHash(randomPassword);
        }

        internal User SaveToDatabase (User user)
        {

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

            if (PasswordHashHelper.VerifyPassword(password, candidate.PasswordHash))
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

        public void UpdateEmployee(User user) {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int DeleteEmployee(User user, User logedUser) {
            Role[] notDeletableEmployeeRoles = new Role[] { Role.Administrator, Role.Manager };
            if (user == logedUser) {
                DialogResult result = MessageBox.Show("Are you sure you want to delet you own accout? You will be logged out!", MessageBoxButtons.YesNoCancel.ToString());
                if(result == DialogResult.OK)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return -1;
                }
                
            }
            else if (notDeletableEmployeeRoles.Contains(user.Role)) {
                throw new DeletAdminAccountException("You are not allow to delet an employee with power");
            } else {
                db.Users.Remove(user);
                db.SaveChanges();               
            }
            return 1;

        }

        public string GenerateEmployeeEmail(string first, string last) {
            string firstLetter = first[0].ToString().ToLower();
            string lastLower = last.ToLower();

            string employeeEmail = firstLetter + "." + lastLower + "@mediabazaar.nl";
            
            return employeeEmail;
        }

        public ObservableCollection<User> FilterEmployees(string p) {
            db.Users.Load();
            if (!String.IsNullOrWhiteSpace(p)) {
                return new ObservableCollection<User>( db.Users.Local.Where(u => u.FirstName.ToLower().Contains(p.ToLower())));
            }
            return db.Users.Local;
        }



    }

}
