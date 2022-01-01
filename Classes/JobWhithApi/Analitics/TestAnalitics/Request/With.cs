using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Request
{
    public class With
    {
        [JsonProperty("financial_data")]
        public bool financial_data { get; set; }
    }
}
