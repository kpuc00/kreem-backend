﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreemMachineLibrary.Exceptions;

namespace KreemMachineLibrary.Models
{
    /// <summary>
    /// A user of the system as stored in the DB
    /// </summary>
    [Table("user")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Column("first_name"), Required]
        public string FirstName { get; set; }

        [Column("last_name"), Required]
        public string LastName { get; set; }

        [Index("UQ_Email", IsUnique = true), Required]
        public string Email { get; set; }

        [Column("password_hash"), Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// String property for the database calculated based on the public enum
        /// </summary>
        [Column("role"), Required]
        private string _role
        {
            get => Role.ToString();

            //Parse the string from the database into an enum to expose to the app
            set => Role = (Role)Enum.Parse(typeof(Role), value, ignoreCase: true);
        }

        /// <summary>
        /// Role exposes an enum for the rest of the application
        /// </summary>
        [NotMapped]
        public Role Role { get; set;  }

        [Column("hourly_wage"), Required]
        public float HourlyWage { get; set; }

        [Column("birth_date"), Required]
        public DateTime? Birthdate { get; set; }

        public string Address { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        public virtual IList<UserScheduledShift> ScheduledShifts { get; set; }

        /// <summary>
        /// Auto generated password in plain text, not stored in the database
        /// </summary>
        [NotMapped]
        public string Password { get; set; }

        public User() { }

        public User(string firstName, string lastName, string email, Role role, float hourlyWage, 
                    DateTime? birthdate, string adress = null, string phoneNumber = null)
        {
            if (String.IsNullOrWhiteSpace(firstName) || String.IsNullOrWhiteSpace(lastName) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(role.ToString()) || String.IsNullOrWhiteSpace(hourlyWage.ToString()) || String.IsNullOrWhiteSpace(birthdate.ToString()))
            {
                throw new RequiredFieldsEmpty("The required fields are empty");
            }
            else {
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                Role = role;
                HourlyWage = hourlyWage;
                Birthdate = birthdate;
                Address = adress;
                PhoneNumber = phoneNumber;
            }
        }
    }
}
