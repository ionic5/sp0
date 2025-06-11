using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SP0.Client.Core.Entities
{
    public class StockItemInfo
    {
        public string CompanyName;
        public string StockCode;
        public long MarketCap;

        public StockItemInfo()
        {
            CompanyName = string.Empty;
            StockCode = string.Empty;
        }
    }
}
