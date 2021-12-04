using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class Filter
    {
        public string key { get; set; }
        public bool is_show { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string unit { get; set; }
        public List<Value> values { get; set; }
    }
}
