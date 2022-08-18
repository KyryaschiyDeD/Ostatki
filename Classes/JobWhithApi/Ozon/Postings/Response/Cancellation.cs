using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class Cancellation
    {
        [JsonProperty("cancel_reason_id")]
        public int cancel_reason_id { get; set; }
        [JsonProperty("cancel_reason")]
        public string cancel_reason { get; set; }
        [JsonProperty("cancellation_type")]
        public string cancellation_type { get; set; }
        [JsonProperty("cancelled_after_ship")]
        public bool cancelled_after_ship { get; set; }
        [JsonProperty("affect_cancellation_rating")]
        public bool affect_cancellation_rating { get; set; }
        [JsonProperty("cancellation_initiator")]
        public string cancellation_initiator { get; set; }

    }
}
