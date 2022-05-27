using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses.InfoPrices
{
    public class ItemInfoPrices
    {
        public int product_id { get; set; }
        public string offer_id { get; set; }
        public Price price { get; set; }
        public string price_index { get; set; }
        public CommissionsInfoPrice commissions { get; set; }
        public object marketing_actions { get; set; }
        public double volume_weight { get; set; }
    }
}
