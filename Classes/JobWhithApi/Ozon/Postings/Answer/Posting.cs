using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Answer
{
    public class Posting
    {
        public string offer_id { get; set; }  
        public string name { get; set; }  
        public double quantity { get; set; } 
        public int sku { get; set; } 
        public bool itIsFBO { get; set; }
        public int FBOQuantity { get; set; }
    }
}
