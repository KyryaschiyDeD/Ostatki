using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class PostingServices
    {
        public int marketplace_service_item_fulfillment { get; set; } // сборка заказа.
        public int marketplace_service_item_pickup { get; set; } //  выезд транспортного средства по адресу продавца для забора отправлений (Pick-up).
        public int marketplace_service_item_dropoff_pvz { get; set; } // Обработка отправления в ПВЗ.
        public int marketplace_service_item_dropoff_sc { get; set; } // Обработка отправления в сортировочном центре.
        public int marketplace_service_item_dropoff_ff { get; set; } // Обработка отправления на фулфилмент складе (ФФ).
        public int marketplace_service_item_direct_flow_trans { get; set; } // Магистраль.
        public int marketplace_service_item_return_flow_trans { get; set; } // Обратная магистраль.
        public double marketplace_service_item_deliv_to_customer { get; set; } // Последняя миля.
        public int marketplace_service_item_return_not_deliv_to_customer { get; set; } // Обработка отмен.
        public int marketplace_service_item_return_part_goods_customer { get; set; } // Обработка невыкупа.
        public int marketplace_service_item_return_after_deliv_to_customer { get; set; } // Обработка возврата.
    }
}
