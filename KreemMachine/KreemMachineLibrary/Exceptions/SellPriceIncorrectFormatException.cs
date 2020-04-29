using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.Exceptions
{
    public class SellPriceIncorrectFormatException : Exception
    {
        public SellPriceIncorrectFormatException(string message) : base(message) { }
    }
}
