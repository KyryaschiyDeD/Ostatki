using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class Product
    {
        public string price { get; set; }
        public string offer_id { get; set; }
        public string name { get; set; }
        public int sku { get; set; }
        public int quantity { get; set; }
        public List<object> mandatory_mark { get; set; }
        public double commission_amount { get; set; }
        public int commission_percent { get; set; }
        public double payout { get; set; }
        public int product_id { get; set; }
        public int old_price { get; set; }
        public int total_discount_value { get; set; }
        public double total_discount_percent { get; set; }
        public List<string> actions { get; set; }
        public object picking { get; set; }
        public string client_price { get; set; }
        public ItemServices item_services { get; set; }
    }
}
