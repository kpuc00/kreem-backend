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
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Index("UQ_Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]
        public string Hash { get; set; }

        [ForeignKey(nameof(Role))]
        public long role { get; set; }

        [Required]
        public Role Role { get; set; }

        /// <summary>
        /// This field is the plain-text password used when creating a user
        /// It is NOT stored in the database
        /// </summary>
        [NotMapped]
        public string Password { get; set; }


        public User(){}

        public User(string name, string email, Role role, string hash)
        {
            Name = name;
            Email = email;
            Role = role;
            Hash = hash;
        }

    }
}
