using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class AssocBundle
    {
        public string section_guid { get; set; }
        public int code { get; set; }
        public string title { get; set; }
        public string cover_image { get; set; }
        public object seo_link { get; set; }
        public int product_qty { get; set; }
    }
}
