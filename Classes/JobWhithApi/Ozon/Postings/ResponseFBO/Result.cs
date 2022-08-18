using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.ResponseFBO
{
    public class Result
    {
        [JsonProperty("order_id")]
        public int order_id { get; set; }
        [JsonProperty("order_number")]
        public string order_number { get; set; }
        [JsonProperty("posting_number")]
        public string posting_number { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("cancel_reason_id")]
        public int cancel_reason_id { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("in_process_at")]
        public DateTime in_process_at { get; set; }
        [JsonProperty("products")]
        public List<Product> products { get; set; }
        [JsonProperty("analytics_data")]
        public AnalyticsData analytics_data { get; set; }
        [JsonProperty("financial_data")]
        public FinancialData financial_data { get; set; }
        [JsonProperty("additional_data")]
        public List<object> additional_data { get; set; }
    }
}
