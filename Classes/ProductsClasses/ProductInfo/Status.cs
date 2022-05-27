using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.ProductInfo
{
    public class Status
    {
        public string state { get; set; }
        public string state_failed { get; set; }
        public string moderate_status { get; set; }
        public List<object> decline_reasons { get; set; }
        public string validation_state { get; set; }
        public string state_name { get; set; }
        public string state_description { get; set; }
        public bool is_failed { get; set; }
        public bool is_created { get; set; }
        public string state_tooltip { get; set; }
        public List<object> item_errors { get; set; }
        public DateTime state_updated_at { get; set; }
    }
}
