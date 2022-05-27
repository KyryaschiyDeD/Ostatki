using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses.InfoPrices
{
    public class FilterRequest
    {
        public List<string> offer_id { get; set; }
        public List<string> product_id { get; set; }
        public string visibility { get; set; }
    }
}
