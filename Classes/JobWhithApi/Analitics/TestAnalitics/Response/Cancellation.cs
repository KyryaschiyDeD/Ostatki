using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class Cancellation
    {
        public int cancel_reason_id { get; set; }
        public string cancel_reason { get; set; }
        public string cancellation_type { get; set; }
        public bool cancelled_after_ship { get; set; }
        public bool affect_cancellation_rating { get; set; }
        public string cancellation_initiator { get; set; }
    }
}
