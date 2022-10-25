using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.Taxation.TaxClasses
{
    public class Metadata
    {
        public long id { get; set; }
        public string ofdId { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string address { get; set; }

        public string subtype { get; set; }
        public DateTime receiveDate { get; set; }
    }
}
