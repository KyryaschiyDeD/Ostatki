using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class Section
    {
        public string section_guid { get; set; }
        public int code { get; set; }
        public string title { get; set; }
        public string cover_image { get; set; }
        public object seo_link { get; set; }
        public int product_qty { get; set; }
        public List<Breadcrumb> breadcrumbs { get; set; }
        public List<Child> children { get; set; }
        public bool is_roof { get; set; }
        public bool has_alt_unit { get; set; }
        public string view_template { get; set; }
        public bool? is_current { get; set; }
        public object sections { get; set; }
    }
}
