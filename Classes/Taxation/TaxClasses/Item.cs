using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.Taxation.TaxClasses
{
    public class Item
    {
        public int Id { get; set; }


        public Guid ReceiptOnDBID { get; set; }

        /// <summary>
        /// Найден ли элемент из чека в проданных товарах
        /// </summary>
        public bool IsFound { get; set; }

        /// <summary>
        /// Id отправления в БД, если найден в проданных
        /// </summary>
        public List<Guid> IdPosting { get; set; }

        /// <summary>
        /// Остаток не найденного кол-ва в чеках.
        /// </summary>
        public double remainsQuantity { get; set; }

        public List<string> PostingNumber { get; set; }

        public int nds { get; set; }
        public int sum { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double quantity { get; set; } // Не забыть в будущем сделать double, например бенз
        public int paymentType { get; set; }
        public int productType { get; set; }

        public int ColNum { get; set; } // Временное, для теста

        public int tmpIDProductFromPosting { get; set; }

        public DateTime dateBuy {get; set; }    
        public string comment { get; set; }
    }
}
