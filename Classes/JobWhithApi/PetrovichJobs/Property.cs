using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{ 
    public class Property
    {
        public string slug { get; set; }
        public string title { get; set; }
        public List<Value> value { get; set; }
        public string unit { get; set; }
        public bool is_compare { get; set; }
        public bool is_description { get; set; }
    }
}
