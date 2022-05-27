using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.ProductInfo
{
    public class ResultQInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string offer_id { get; set; }
        public string barcode { get; set; }
        public string buybox_price { get; set; }
        public int category_id { get; set; }
        public DateTime created_at { get; set; }
        public List<object> images { get; set; }
        public string marketing_price { get; set; }
        public string min_ozon_price { get; set; }
        public string old_price { get; set; }
        public string premium_price { get; set; }
        public string price { get; set; }
        public string recommended_price { get; set; }
        public string min_price { get; set; }
        public List<Source> sources { get; set; }
        public Stocks stocks { get; set; }
        public List<object> errors { get; set; }
        public string vat { get; set; }
        public bool visible { get; set; }
        public VisibilityDetails visibility_details { get; set; }
        public string price_index { get; set; }
        public List<Commission> commissions { get; set; }
        public double volume_weight { get; set; }
        public bool is_prepayment { get; set; }
        public bool is_prepayment_allowed { get; set; }
        public List<object> images360 { get; set; }
        public string color_image { get; set; }
        public string primary_image { get; set; }
        public Status status { get; set; }
        public string state { get; set; }
        public string service_type { get; set; }
        public int fbo_sku { get; set; }
        public int fbs_sku { get; set; }
    }
}
