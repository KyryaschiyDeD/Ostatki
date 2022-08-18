using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class Result
    {
        [JsonProperty("postings")]
        public List<Posting> postings { get; set; }
        [JsonProperty("has_next")]
        public bool has_next { get; set; }
    }
}
