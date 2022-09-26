using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings
{
    public class PostingSave
    {
        public Guid Id { get; set; }
        public object delivering_date { get; set; } // Дата передачи отправления в доставку.
        public string clientId { get; set; } // Client id аккаунта
        public object in_process_at { get; set; } // Дата и время начала обработки отправления.
        public string order_number { get; set; } // Номер заказа, к которому относится отправление.
        public string parent_posting_number { get; set; }// Номер родительского отправления, в результате разделения которого появилось текущее.
        public string posting_number { get; set; } // Номер отправления
        public OzonProductAndOurProduct products { get; set; }// Товары
        public string status { get; set; } // Статус
        public DateTime createDate { get; set; }  // Дата добавления в приложение
        public DateTime updateDate { get; set; }  // Дата обновления в приложении

    }
}
