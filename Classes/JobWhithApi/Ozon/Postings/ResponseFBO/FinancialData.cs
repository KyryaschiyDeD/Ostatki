using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.ResponseFBO
{
    public class FinancialData
    {
        [JsonProperty("products")]
        public List<Product> products { get; set; }
        [JsonProperty("posting_services")]
        public PostingServices posting_services { get; set; }
    }
}
