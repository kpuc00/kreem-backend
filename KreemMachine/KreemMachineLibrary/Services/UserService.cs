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
using BCrypt.Net;

namespace KreemMachineLibrary.Services
{
    public class UserService
    {
        
        public User Save(string firstName, string lastName, string email, Role role, string hourlyWage, DateTime birthDate, string address, string phoneNumber)
        {
            User user = this.VerifyInputDataUser(firstName, lastName, email, role, hourlyWage, birthDate, address, phoneNumber);
            HashPassword(user);
            user = SaveToDatabase(user);
            return user;
        }



        internal void HashPassword(User user)
        {
            Random random = new Random();
            var randomPassword = "";
            for (var i = 0; i < 10; i++)
            {
                randomPassword += ((char)(random.Next(1, 26) + 64)).ToString();
            }
            user.Password = randomPassword;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);
        }

        //This method checks the input values of a user
        private User VerifyInputDataUser(string firstName, string lastName, string email, Role role, string hourlyWage, DateTime birthDate, string address, string phoneNumber)
        {
            if (String.IsNullOrWhiteSpace(firstName))
            {
                throw new RequiredFieldsEmpty("You need to fill in all required fields");
            }
            if (String.IsNullOrWhiteSpace(lastName))
            {
                throw new RequiredFieldsEmpty("You need to fill in all required fields");
            }
            if (String.IsNullOrWhiteSpace(hourlyWage))
            {
                throw new RequiredFieldsEmpty("You need to fill in all required fields");
            }
            if (String.IsNullOrWhiteSpace(birthDate.ToString())) {
                throw new RequiredFieldsEmpty("You need to fill in all required fields");
            }

            //Checks if a string is a number
            int dotCount = 0;
            bool valH = true;
            foreach (Char c in hourlyWage) {
                if (c == '.') {
                    if (++dotCount > 1) {
                        valH = false;
                        break;
                    }
                }
                else {
                    if (c < '0' || c > '9') {
                        valH = false;
                        break;
                    }
                }
            }
            if (!valH)
            {
                throw new HourlyWageMustComtainOnlyNumbers("The hourly wage can only be a number");
            }

            foreach (Char c in phoneNumber) {
                if (c < '0' || c > '9') {
                    throw new PhoneNumberException("Phone number can only be numbers");
                }
            }

            return new User(firstName, lastName, email, role, float.Parse(hourlyWage), birthDate, address, phoneNumber);
        }

        internal User SaveToDatabase (User user)
        {
            string mail = user.Email;
            using (var db = new DataBaseContext())
            {
                var temp = from u in db.Users
                           select u.Email;
                var allMail = temp.ToList();

                int mailCount = 0;
                if (allMail.Contains(mail))
                {
                    foreach (string e in allMail)
                    {
                        if (e == mail)
                        {
                            mailCount++;
                        }
                    }
                    string newMail = mail[0] + mailCount.ToString() + mail.Substring(1);
                    user.Email = newMail;
                }


                var fromDb = db.Users.Add(user);
                db.SaveChanges();
                return fromDb;
            }

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

            User candidate;
            using (var db = new DataBaseContext())
            {
                candidate = db.Users.SingleOrDefault(u => u.Email == email);
            }

            if (candidate == null)
                return null;

            if (BCrypt. Net.BCrypt.Verify(password, candidate.PasswordHash))
            {
                SecurityContext.Authenticate(candidate);
                return candidate;
            }

            return null;
        }

        public List<User> GetAll()
        {
            using (var db = new DataBaseContext())
                return db.Users.ToList();
        }

        public void UpdateEmployee(User user, string firstName, string lastName, string email, Role role, string hourlyWage, DateTime birthDate, string address, string phoneNumber) {
            this.VerifyInputDataUser(firstName, lastName, email, role, hourlyWage, birthDate, address, phoneNumber);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Role = role;
            user.HourlyWage = float.Parse(hourlyWage);
            user.Birthdate = birthDate;
            user.Address = address;
            user.PhoneNumber = phoneNumber;

            using (var db = new DataBaseContext())
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public int DeleteEmployee(User user, User logedUser) {
            Role?[] notDeletableEmployeeRoles = new Role?[] { Role.Administrator, Role.Manager };
           
            if (notDeletableEmployeeRoles.Contains(user.Role) )
            {
                throw new DeletAdminAccountException("You are not allow to delet an employee with power");
            } 
            else 
            {
                using (var db = new DataBaseContext())
                {
                    if (!db.Users.Local.Contains(user))
                        db.Users.Attach(user);
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
                               
            }
            return 1;

        }

        public string GenerateEmployeeEmail(string first, string last) {
            string firstLetter = first[0].ToString().ToLower();
            string lastLower = last.ToLower();

            string employeeEmail = firstLetter + "." + lastLower + "@mediabazaar.nl";
            
            return employeeEmail;
        }


        public IList<User> GetAllByRole(Role role)
        {
            using (var db = new DataBaseContext())
                return db.Users.Where(u => u.RoleStr == Role.Employee.ToString()).ToList();

        }


        public ObservableCollection<User> FilterEmployees(string p)
        {
            using (var db = new DataBaseContext())
            {
                db.Users.Load();
                if (!String.IsNullOrWhiteSpace(p))
                {
                    return new ObservableCollection<User>(db.Users.Local.Where(u => u.FirstName.ToLower().Contains(p.ToLower())));
                }
                return db.Users.Local;
            }
        }

    }

}
