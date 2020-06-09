using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreemMachineLibrary.Exceptions;
using KreemMachineLibrary.Extensions.Date;
using KreemMachineLibrary.Models.Statics;
using KreemMachineLibrary.Services;

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

        [Index("email", IsUnique = true), Required]
        public string Email { get; set; }

        [Column("password_hash"), Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// String property for the database
        [Column("role"), Required]
        public string RoleStr { get; private set; }


        /// <summary>
        /// Role exposes an enum for the rest of the application
        /// is calculated entirely based on <code> RoleStr </code>
        /// </summary>
        [NotMapped]
        public Role? Role
        {
            get => string.IsNullOrEmpty(RoleStr) ? null : Enum.Parse(typeof(Role), RoleStr) as Role? ;
            set => RoleStr = value.ToString();
        }

        [Column("hourly_wage"), Required]
        public float HourlyWage { get; set; }

        [Column("birth_date"), Required]
        public DateTime? Birthdate { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        [Column("Department_Id")]
        public long? DepartmentId { get; set; }

        public virtual Department Department { get; set; }

        public virtual IList<UserScheduledShift> ScheduledShifts { get; set; }

        /// <summary>
        /// Auto generated password in plain text, not stored in the database
        /// </summary>
        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string FullName { get => FirstName + " " + LastName; }

        /// <summary>
        /// Temporairly hardcoded, later to be stored in the db
        /// </summary>
        [NotMapped]
        public float MaxMonthlyHoours => 40;

        /// <summary>
        /// Temporairly hardcoded, later to be stored in the db
        /// </summary>
        [NotMapped]
        public float MinMonthlyHours => 20;

        public virtual List<BlockOff> BlockOffs { get; set; }

        public User() { }

        public User(string firstName, string lastName, string email, Role role, Department department, float hourlyWage,
                    DateTime? birthdate, string adress = null, string phoneNumber = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Department = department;
            DepartmentId = department.Id;
            HourlyWage = hourlyWage;
            Birthdate = birthdate;
            Address = adress;
            PhoneNumber = phoneNumber;
        }

        /// <summary>
        /// Calculate the number of hours worked in any given week.
        /// <b> WARNING!! Requires acces to <i>ScheduledShifts</i> 
        /// Eagger load or don't dispose of Context before running the object </b>
        /// </summary>
        /// <param name="date"> any date in the week to calculate, uses 
        /// <i>KreemMachineLibrary.Extensions.Date.StartOfWeek</i> and
        /// <i>KreemMachineLibrary.Extensions.Date.NextWeek</i> to find the week range </param>
        /// <returns> Number of hours worked in the given week </returns>
        public double GetHoursWorkedInWeek(DateTime date)
        {
            return this.ScheduledShifts
                ?.Where(s => s.ScheduledShift.Date >= date.StartOfWeek() && s.ScheduledShift.Date < date.NextWeek())
                ?.Sum(s => s.ScheduledShift.Duration) ?? 0;
        }


    }
}
