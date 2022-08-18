using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class Posting
    {
        [JsonProperty("posting_number")]
        public string posting_number { get; set; }
        [JsonProperty("order_id")]
        public int order_id { get; set; }
        [JsonProperty("order_number")]
        public string order_number { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("delivery_method")]
        public DeliveryMethod delivery_method { get; set; }
        [JsonProperty("tracking_number")]
        public string tracking_number { get; set; }
        [JsonProperty("tpl_integration_type")]
        public string tpl_integration_type { get; set; }
        [JsonProperty("in_process_at")]
        public DateTime in_process_at { get; set; }
        [JsonProperty("shipment_date")]
        public DateTime shipment_date { get; set; }
        [JsonProperty("delivering_date")]
        public object delivering_date { get; set; }
        [JsonProperty("cancellation")]
        public Cancellation cancellation { get; set; }
        [JsonProperty("customer")]
        public object customer { get; set; }
        [JsonProperty("products")]
        public List<Product> products { get; set; }
        [JsonProperty("addressee")]
        public object addressee { get; set; }
        [JsonProperty("barcodes")]
        public object barcodes { get; set; }
        [JsonProperty("analytics_data")]
        public object analytics_data { get; set; }
        [JsonProperty("financial_data")]
        public object financial_data { get; set; }
        [JsonProperty("is_express")]
        public bool is_express { get; set; }
        [JsonProperty("requirements")]
        public Requirements requirements { get; set; }

    }
}
