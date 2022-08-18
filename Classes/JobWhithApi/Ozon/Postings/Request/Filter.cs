using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Request
{
    public class Filter
    {
        [JsonProperty("since")]
        public DateTime since { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("to")]
        public DateTime to { get; set; }
    }
}
