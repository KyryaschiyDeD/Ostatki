using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class Remains
    {
        public bool city_selected { get; set; }
        public string availability { get; set; }
        public int total { get; set; }
        public List<SupplyWay> supply_ways { get; set; }
    }
}
