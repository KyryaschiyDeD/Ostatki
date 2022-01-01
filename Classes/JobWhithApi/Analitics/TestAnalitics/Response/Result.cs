using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class Result
    {
        public List<Posting> postings { get; set; }
        public bool has_next { get; set; }
    }
}
