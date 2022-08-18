using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings
{
    public class Filter
    {
        public string EngName { get; set; }
        public string RuName { get; set; }
        public bool IsFbo { get; set; }
        public Filter(string engName, string ruName, bool isFbo)
        {
            EngName = engName;
            RuName = ruName;
            IsFbo = isFbo;
        }
    }
}
