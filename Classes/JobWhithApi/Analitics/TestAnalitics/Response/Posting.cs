using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class Posting
    {
        public string posting_number { get; set; }
        public int order_id { get; set; }
        public string order_number { get; set; }
        public string status { get; set; }
        public DeliveryMethod delivery_method { get; set; }
        public string tracking_number { get; set; }
        public string tpl_integration_type { get; set; }
        public DateTime in_process_at { get; set; }
        public DateTime shipment_date { get; set; }
        public DateTime? delivering_date { get; set; }
        public Cancellation cancellation { get; set; }
        public object customer { get; set; }
        public List<Product> products { get; set; }
        public object addressee { get; set; }
        public object barcodes { get; set; }
        public object analytics_data { get; set; }
        public FinancialData financial_data { get; set; }
        public bool is_express { get; set; }
        public Requirements requirements { get; set; }
    }
}
