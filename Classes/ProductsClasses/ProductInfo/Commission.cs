using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.ProductInfo
{
    public class Commission
    {
        public int percent { get; set; }
        public int min_value { get; set; }
        public double value { get; set; }
        public string sale_schema { get; set; }
        public int delivery_amount { get; set; }
        public int return_amount { get; set; }
    }
}
