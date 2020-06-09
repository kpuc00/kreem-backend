using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Models
{
    [Table("department")]
    public class Department
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column("deleted")]
        public int Deleted { get; set; }

        public virtual List<User> Users { get; set; }

        public Department() { }

        public Department(string name) {
            this.Name = name;
        }

    }
}
