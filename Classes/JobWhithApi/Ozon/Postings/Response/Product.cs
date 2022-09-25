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
        public int quantity { get; set; } // Количество товара в отправлении.
        [JsonProperty("mandatory_mark")]
        public List<object> mandatory_mark { get; set; } // Обязательная маркировка товара.
        [JsonProperty("currency_code")]
        public string currency_code { get; set; } // Валюта цен.

    }
}
