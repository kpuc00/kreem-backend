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
    /// A user's Role as stored in the DB
    /// </summary>
    public class Role
    {
        /// <summary>
        /// These constatnts hold the names of all the roles
        /// </summary>

        [Key]
        public long Id { get; set; }

        [Required]
        [Index("UQ_Name", IsUnique = true)]
        public string Name { get; set; }

        public Role(){}

        public Role(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public Role(string name)
        {
            Name = name;
        }
    }
}
