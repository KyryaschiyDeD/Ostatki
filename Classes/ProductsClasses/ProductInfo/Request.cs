using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.ProductInfo
{
    public class Request
    {
        public List<string> offer_id { get; set; }
        public List<object> product_id { get; set; }
        public List<object> sku { get; set; }
    }
}
