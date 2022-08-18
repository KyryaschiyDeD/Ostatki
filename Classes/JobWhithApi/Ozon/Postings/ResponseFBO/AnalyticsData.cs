using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.ResponseFBO
{
    public class AnalyticsData
    {
        [JsonProperty("region")]
        public string region { get; set; }
        [JsonProperty("city")]
        public string city { get; set; }
        [JsonProperty("delivery_type")]
        public string delivery_type { get; set; }
        [JsonProperty("is_premium")]
        public bool is_premium { get; set; }
        [JsonProperty("payment_type_group_name")]
        public string payment_type_group_name { get; set; }
        [JsonProperty("warehouse_id")]
        public long warehouse_id { get; set; }
        [JsonProperty("warehouse_name")]
        public string warehouse_name { get; set; }
        [JsonProperty("is_legal")]
        public bool is_legal { get; set; }
    }
}
