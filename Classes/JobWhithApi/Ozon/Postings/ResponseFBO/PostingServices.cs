using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.ResponseFBO
{
    public class PostingServices
    {
        [JsonProperty("marketplace_service_item_fulfillment")]
        public int marketplace_service_item_fulfillment { get; set; }
        [JsonProperty("marketplace_service_item_pickup")]
        public int marketplace_service_item_pickup { get; set; }
        [JsonProperty("marketplace_service_item_dropoff_pvz")]
        public int marketplace_service_item_dropoff_pvz { get; set; }
        [JsonProperty("marketplace_service_item_dropoff_sc")]
        public int marketplace_service_item_dropoff_sc { get; set; }
        [JsonProperty("marketplace_service_item_dropoff_ff")]
        public int marketplace_service_item_dropoff_ff { get; set; }
        [JsonProperty("marketplace_service_item_direct_flow_trans")]
        public int marketplace_service_item_direct_flow_trans { get; set; }
        [JsonProperty("marketplace_service_item_return_flow_trans")]
        public int marketplace_service_item_return_flow_trans { get; set; }
        [JsonProperty("marketplace_service_item_deliv_to_customer")]
        public int marketplace_service_item_deliv_to_customer { get; set; }
        [JsonProperty("marketplace_service_item_return_not_deliv_to_customer")]
        public int marketplace_service_item_return_not_deliv_to_customer { get; set; }
        [JsonProperty("marketplace_service_item_return_part_goods_customer")]
        public int marketplace_service_item_return_part_goods_customer { get; set; }
        [JsonProperty("marketplace_service_item_return_after_deliv_to_customer")]
        public int marketplace_service_item_return_after_deliv_to_customer { get; set; }
    }
}
