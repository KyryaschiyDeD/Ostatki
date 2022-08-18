using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class Product
    {
        [JsonProperty("price")]
        public string price { get; set; }
        [JsonProperty("offer_id")]
        public string offer_id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("sku")]
        public int sku { get; set; }
        [JsonProperty("quantity")]
        public int quantity { get; set; }
        [JsonProperty("mandatory_mark")]
        public List<object> mandatory_mark { get; set; }
        [JsonProperty("currency_code")]
        public string currency_code { get; set; }

    }
}
