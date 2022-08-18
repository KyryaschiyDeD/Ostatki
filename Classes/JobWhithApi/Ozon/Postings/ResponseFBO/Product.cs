using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.ResponseFBO
{
    public class Product
    {
        [JsonProperty("sku")]
        public int sku { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("quantity")]
        public int quantity { get; set; }
        [JsonProperty("offer_id")]
        public string offer_id { get; set; }
        [JsonProperty("price")]
        public string price { get; set; }
        [JsonProperty("digital_codes")]
        public List<object> digital_codes { get; set; }
        [JsonProperty("commission_amount")]
        public double commission_amount { get; set; }
        [JsonProperty("commission_percent")]
        public int commission_percent { get; set; }
        [JsonProperty("payout")]
        public double payout { get; set; }
        [JsonProperty("product_id")]
        public int product_id { get; set; }
        [JsonProperty("old_price")]
        public int old_price { get; set; }
        [JsonProperty("total_discount_value")]
        public int total_discount_value { get; set; }
        [JsonProperty("total_discount_percent")]
        public double total_discount_percent { get; set; }
        [JsonProperty("actions")]
        public List<string> actions { get; set; }
        [JsonProperty("picking")]
        public object picking { get; set; }
        [JsonProperty("client_price")]
        public string client_price { get; set; }
        [JsonProperty("item_services")]
        public ItemServices item_services { get; set; }
    }
}
