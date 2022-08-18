using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class DeliveryMethod
    {
        [JsonProperty("id")]
        public long id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("warehouse_id")]
        public long warehouse_id { get; set; }
        [JsonProperty("warehouse")]
        public string warehouse { get; set; }
        [JsonProperty("tpl_provider_id")]
        public int tpl_provider_id { get; set; }
        [JsonProperty("tpl_provider")]
        public string tpl_provider { get; set; }
    }
}
