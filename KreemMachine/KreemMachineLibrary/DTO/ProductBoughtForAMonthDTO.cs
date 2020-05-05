using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class ProductBoughtForAMonthDTO
    {
        public DateTime? Month { get; set; }
        public int QuantityBought { get; set; }
        public float Cost { get; set; }
    }
}
