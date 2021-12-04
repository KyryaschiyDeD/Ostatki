 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class Data
    {
        public List<Product> products { get; set; }
        public Section section { get; set; }
        public List<Section> sections { get; set; }
        public List<Filter> filters { get; set; }
        public List<TopFilter> top_filters { get; set; }
        public List<Sort> sort { get; set; }
        public Pagination pagination { get; set; }
        public Product product { get; set; }
    }
}
