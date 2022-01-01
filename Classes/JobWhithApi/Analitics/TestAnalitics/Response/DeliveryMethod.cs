using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class DeliveryMethod
    {
        public object id { get; set; }
        public string name { get; set; }
        public object warehouse_id { get; set; }
        public string warehouse { get; set; }
        public int tpl_provider_id { get; set; }
        public string tpl_provider { get; set; }
    }
}
