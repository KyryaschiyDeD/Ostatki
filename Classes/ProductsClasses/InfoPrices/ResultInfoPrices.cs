using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses.InfoPrices
{
    public class ResultInfoPrices
    {
        public List<ItemInfoPrices> items { get; set; }
        public int total { get; set; }
        public string last_id { get; set; }
    }
}
