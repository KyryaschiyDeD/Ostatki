using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses.InfoPrices
{
    public class CommissionsInfoPrice
    {
        public double sales_percent { get; set; } // Процент комиссии за продажу (FBO и FBS).
        public double fbo_fulfillment_amount { get; set; } // Комиссия за сборку заказа (FBO).
        public double fbo_direct_flow_trans_min_amount { get; set; } // Магистраль от (FBO).
        public double fbo_direct_flow_trans_max_amount { get; set; } // Магистраль до (FBO).
        public double fbo_deliv_to_customer_amount { get; set; } // Последняя миля (FBO).
        public double fbo_return_flow_amount { get; set; } // Комиссия за возврат и отмену (FBO).
        public double fbo_return_flow_trans_min_amount { get; set; } // Комиссия за обратную логистику от (FBO).
        public double fbo_return_flow_trans_max_amount { get; set; } // Комиссия за обратную логистику до (FBO).
        public double fbs_first_mile_min_amount { get; set; } // Комиссия за обработку отправления от (FBS).
        public double fbs_first_mile_max_amount { get; set; } // Комиссия за обработку отправления до (FBS).
        public double fbs_direct_flow_trans_min_amount { get; set; } // Магистраль от (FBS).
        public double fbs_direct_flow_trans_max_amount { get; set; } // Магистраль до (FBS).
        public double fbs_deliv_to_customer_amount { get; set; } // Последняя миля (FBS).
        public double fbs_return_flow_amount { get; set; } // Комиссия за возврат и отмену, обработка отправления (FBS).
        public double fbs_return_flow_trans_min_amount { get; set; } // Комиссия за возврат и отмену, магистраль от (FBS).
        public double fbs_return_flow_trans_max_amount { get; set; } // Комиссия за возврат и отмену, магистраль до (FBS).
    }
}
