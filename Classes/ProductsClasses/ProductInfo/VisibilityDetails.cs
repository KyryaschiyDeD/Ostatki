using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Остатки.Classes.ProductsClasses.ProductInfo;

namespace Остатки.Classes.JobWhithApi.Ozon.ProductInfo
{
    public class VisibilityDetails
    {
        public bool has_price { get; set; }
        public bool has_stock { get; set; }
        public bool active_product { get; set; }
        public Reasons reasons { get; set; }
    }
}
