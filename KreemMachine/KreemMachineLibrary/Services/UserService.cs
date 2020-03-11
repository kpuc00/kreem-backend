﻿using KreemMachineLibrary.Helpers;
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
            // TODO: Use randomly generated passwords latter
            user.PasswordHash = PasswordHashHelper.CreateHash("qweqwe");
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

        public string GenerateEmployeeEmail(string first, string last) {
            string firstLetter = first[0].ToString().ToLower();
            string lastLower = last.ToLower();

            string employeeEmail = firstLetter + "." + lastLower + "@mediabazaar.nl";
            
            return employeeEmail;
        }



    }

}
