using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ProductSoldForAMonthDTO
    {
        public DateTime? Month { get; set; }
        public int QuantitySold { get; set; }
        public float Profit { get; set; }
    }
}
