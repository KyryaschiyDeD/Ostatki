using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class SupplyWay
    {
        public string supply_way_type { get; set; }
        public string delivery_type { get; set; }
        public object request_date { get; set; }
        public object supply_date { get; set; }
        public List<SubdivisionList> subdivision_list { get; set; }
    }
}
