using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// String property for the database
        [Column("role"), Required]
        public string RoleStr { get; private set; }

        /// <summary>
        /// Role exposes an enum for the rest of the application
        /// is calculated entirely based on <code> role </code>
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

        public string Address { get; set; }

        [Column("phone_number")]
        public string PhoneNumber { get; set; }

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

        public User() { }

        public User(string firstName, string lastName, string email, Role role, float hourlyWage,
                    DateTime? birthdate, string adress = null, string phoneNumber = null)
        {
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
