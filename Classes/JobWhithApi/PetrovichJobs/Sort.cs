using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{ 
    public class Sort
    {
        public string key { get; set; }
        public string title { get; set; }
        public List<string> direction { get; set; }
        public string active { get; set; }
    }
}
