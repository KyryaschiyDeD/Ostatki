using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Request
{
    public class Root
    {
        [JsonProperty("dir")]
        public string dir { get; set; }
        [JsonProperty("filter")]
        public Filter filter { get; set; }
        [JsonProperty("limit")]
        public int limit { get; set; }
        [JsonProperty("offset")]
        public int offset { get; set; }
        [JsonProperty("translit")]
        public bool translit { get; set; }
        [JsonProperty("with")]
        public With with { get; set; }
    }
}
