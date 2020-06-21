using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreemMachineLibrary.DTO
{
    public class StockStatisticsDTO
    {
        public string Item { get; set; }
        public int Amount { get; set; }

        public double BuyPrice { get; set; }
    
        public double Income { get; set; }

        public int AmountSold { get; set; }

        private double _profit;
        public double ProfitDouble { set => _profit = value; }
        public string Profit { get => _profit.ToString("n2"); }
    }
}
