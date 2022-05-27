using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses.InfoPrices
{
    public class RequestInfoPrices
    {
        public FilterRequest filter { get; set; }
        public string last_id { get; set; }
        public int limit { get; set; }
    }
}
