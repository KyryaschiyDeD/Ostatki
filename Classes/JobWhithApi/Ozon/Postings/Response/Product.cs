using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings.Response
{
    public class Product
    {
        [JsonProperty("price")]
        public string price { get; set; } // Цена товара
        [JsonProperty("offer_id")]
        public string offer_id { get; set; } // Идентификатор товара в системе продавца — артикул.
        [JsonProperty("name")]
        public string name { get; set; } // Название товара.
        [JsonProperty("sku")]
        public int sku { get; set; } // Идентификатор товара в системе Ozon — SKU.
        [JsonProperty("quantity")]
        public double quantity { get; set; } // Количество товара в отправлении.
        [JsonProperty("mandatory_mark")]
        public List<object> mandatory_mark { get; set; } // Обязательная маркировка товара.
        [JsonProperty("currency_code")]
        public string currency_code { get; set; } // Валюта цен.

        public string posting_number { get; set; }

        public DateTime shipment_date { get; set; }
        public DateTime in_process_at { get; set; } // Дата заказа
        public Guid PostingId { get; set; }
        public int ColNum { get; set; } // Временное, для теста

        public bool Finding { get; set; }
        public int ID { get; set; }

        public override string ToString()
        {
            return ID.ToString();
        }
    }
}
