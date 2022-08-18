using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class Requirements
    {
        [JsonProperty("products_requiring_gtd")]
        public List<object> products_requiring_gtd { get; set; }
        [JsonProperty("products_requiring_country")]
        public List<object> products_requiring_country { get; set; }
        [JsonProperty("products_requiring_mandatory_mark")]
        public List<object> products_requiring_mandatory_mark { get; set; }

    }
}
