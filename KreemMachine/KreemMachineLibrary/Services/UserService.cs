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
using KreemMachineLibrary.Models.Statics;

namespace KreemMachineLibrary.Services
{
    public class UserService
    {
        
        public User Save(string firstName, string lastName, string email, Role role, Department department, string hourlyWage, DateTime birthDate, string address, string phoneNumber)
        {
            User user = this.VerifyInputDataUser(firstName, lastName, email, role, department, hourlyWage, birthDate, address, phoneNumber);
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
        private User VerifyInputDataUser(string firstName, string lastName, string email, Role role, Department department, string hourlyWage, DateTime birthDate, string address, string phoneNumber)
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
                if (c == ',') {
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

            return new User(firstName, lastName, email, role, department, float.Parse(hourlyWage), birthDate, address, phoneNumber);
        }

        internal User SaveToDatabase (User user)
        {
            using (var db = new DataBaseContext())
            {

                int similarEmailsNumber = db.Users.Where(u => u.LastName.ToLower() == user.LastName.ToLower() && u.Email.Substring(0,1).ToLower() == user.Email.Substring(0,1).ToLower()).Count();

                user.Email = GenerateEmployeeEmail(user.FirstName, user.LastName, similarEmailsNumber.ToString());
                
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

        public List<User> GetUsers()
        {
            
            if (SecurityContext.HasPermissions(Permission.ViewAllUsers)) return GetAll();
            
            if (SecurityContext.HasPermissions(Permission.ViewOwnUsers)) return GetOwnUsers();

            throw new MissingPermissionException(Permission.ViewAllUsers, Permission.ViewOwnUsers);
        }

        private List<User> GetAll()
        {
            using (var db = new DataBaseContext())
                return db.Users
                    .Include(u => u.Department)
                    .OrderByDescending(u => u.Department.Id)
                    .ToList();
        }

        private List<User> GetOwnUsers()
        {
            using (var db = new DataBaseContext())
                return db.Users
                    .Include(u => u.Department)
                    .Where(u => u.DepartmentId == SecurityContext.CurrentUser.DepartmentId 
                            || u.DepartmentId == null )
                    .OrderByDescending(u => u.Department.Id)
                    .ToList();
        }

        public void UpdateEmployee(User user, string firstName, string lastName, string email, Role role, Department department, string hourlyWage, DateTime birthDate, string address, string phoneNumber) {
            this.VerifyInputDataUser(firstName, lastName, email, role, department, hourlyWage, birthDate, address, phoneNumber);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Role = role;
            user.Department = department;
            user.DepartmentId = department.Id;
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

        public string GenerateEmployeeEmail(string first, string last, string number = "") {



            string firstLetter = first[0].ToString().ToLower();
            string lastLower = last.ToLower();

            string employeeEmail = firstLetter + number + "." + lastLower + "@mediabazaar.nl";
            
            return employeeEmail;
        }


        public IList<User> GetAllByRole(Role role)
        {
            using (var db = new DataBaseContext())
                return db.Users.Where(u => u.RoleStr == Role.Employee.ToString()).ToList();

        }


        public List<User> FilterEmployees(string p)
        {
            using (var db = new DataBaseContext())
            {
                if (!string.IsNullOrWhiteSpace(p))
                {
                    return GetUsers()
                        .Where(u => u.FullName.ToLower().Contains(p.ToLower()))
                        .ToList();
                }
                return GetUsers();
            }
        }

    }

}
