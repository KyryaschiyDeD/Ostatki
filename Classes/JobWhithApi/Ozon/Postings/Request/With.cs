using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Request
{
    public class With
    {
        [JsonProperty("analytics_data")]
        public bool analytics_data { get; set; }
        [JsonProperty("financial_data")]
        public bool financial_data { get; set; }
    }
}
