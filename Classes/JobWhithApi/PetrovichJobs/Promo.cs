using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class Promo
    {
        public int promo_id { get; set; }
        public string promo_guid { get; set; }
        public int context_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string description_full { get; set; }
        public string desc_link { get; set; }
        public string begin_date { get; set; }
        public string end_date { get; set; }
        public bool is_unlimited { get; set; }
        public bool is_set { get; set; }
        public bool is_gift { get; set; }
        public bool is_show_with_sections { get; set; }
        public string icon { get; set; }
        public List<string> banners { get; set; }
        public string banner_preview { get; set; }
        public string promo_rules { get; set; }
        public object promo_steps { get; set; }
        public bool is_only_fiz_user { get; set; }
        public string description_for_only_fiz_user { get; set; }
        public Mobile mobile { get; set; }
        public string type { get; set; }
        public string color { get; set; }
    }
}
