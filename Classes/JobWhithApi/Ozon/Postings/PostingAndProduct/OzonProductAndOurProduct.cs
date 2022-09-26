using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings
{
    public class OzonProductAndOurProduct
    {
        public List<Остатки.Classes.JobWhithApi.Ozon.Postings.Response.Product> ozonProducts { get; set; }
        public List<OurProductAndPriceSale> ourProductAndPriceSales { get; set; }
    }
}
