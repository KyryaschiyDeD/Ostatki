using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.PetrovichJobs
{
    public class Product
    {
        public string product_guid { get; set; }
        public int code { get; set; }
        public string title { get; set; }
        public Section section { get; set; }
        public string cover_image { get; set; }
        public List<Breadcrumb> breadcrumbs { get; set; }
        public List<AssocBundle> assoc_bundles { get; set; }
        public string checkout_type { get; set; }
        public bool has_alt_unit { get; set; }
        public bool is_unit_float { get; set; }
        public bool is_service { get; set; }
        public bool is_active { get; set; }
        public bool is_city_defined { get; set; }
        public bool is_in_wizart { get; set; }
        public bool is_marked { get; set; }
        public object services { get; set; }
        public List<string> special_terms { get; set; }
        public string unit_title { get; set; }
        public int unit_ratio { get; set; }
        public int unit_ratio_alt { get; set; }
        public string unit_title_alt { get; set; }
        public UnitTitleFull unit_title_full { get; set; }
        public double weight { get; set; }
        public List<Property> properties { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int increment_step { get; set; }
        public Price price { get; set; }
        public PriceAlt price_alt { get; set; }
        public int bonus { get; set; }
        public object attributes { get; set; }
        public bool is_not_in_stock { get; set; }
        public bool is_infinite_shelf { get; set; }
        public bool is_solitary_shipping { get; set; }
        public bool is_in_showroom { get; set; }
        public object gift { get; set; }
        public List<Promo> promos { get; set; }
        public int supply_days { get; set; }
        public Remains remains { get; set; }
        public bool is_gift_certificate { get; set; }
        public string description { get; set; }
        public string extended_description { get; set; }
        public List<string> certificates { get; set; }
        public List<string> instructions { get; set; }
        public List<string> images { get; set; }
        public object videos { get; set; }
        public int volume_ratio { get; set; }
        public object calculator_components { get; set; }
        public string lifting_tariff_guid { get; set; }
        public int external_id { get; set; }
        public int compression_ratio { get; set; }
        public object loan_monthly_payment { get; set; }
    }
}
