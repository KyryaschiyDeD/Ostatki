using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response
{
    public class ItemServices
    {
        public double marketplace_service_item_fulfillment { get; set; } // Сборка заказа.
        public double marketplace_service_item_pickup { get; set; } // Выезд транспортного средства по адресу продавца для забора отправлений (Pick-up).
        public double marketplace_service_item_dropoff_pvz { get; set; } // отправления на ПВЗ.
        public double marketplace_service_item_dropoff_sc { get; set; } // отправления на СЦ.
        public double marketplace_service_item_dropoff_ff { get; set; } // Обработка отправления на фулфилмент складе (ФФ).
        public double marketplace_service_item_direct_flow_trans { get; set; } // Магистраль.
        public double marketplace_service_item_return_flow_trans { get; set; } // Обратная магистраль.
        public double marketplace_service_item_deliv_to_customer { get; set; } // Последняя миля.
        public double marketplace_service_item_return_not_deliv_to_customer { get; set; } // Обработка отмен.
        public double marketplace_service_item_return_part_goods_customer { get; set; } // Обработка невыкупа.
        public double marketplace_service_item_return_after_deliv_to_customer { get; set; } // Обработка возврата.
    }
}
