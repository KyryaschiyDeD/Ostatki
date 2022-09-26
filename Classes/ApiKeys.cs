using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class ApiKeys
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ClientId { get; set; }
		public string ApiKey { get; set; }
		public bool ItIsTop { get; set; }	// Дейтсвует ли топ продаж?
		public bool InDB { get; set; }  // Проверка, в продаже этот товар или нет, перед парсингом
		public bool IsOstatkiUpdate { get; set; } // Обновляем ли остатки?
		public bool IsPriceUpdate { get; set; } // Обнолвяем ли цены?
		public bool IsTheMaximumPrice { get; set; } // Максимальная цена на этом аккаунте? 
		public bool IsFullfilment { get; set; } // Считаем ли ФФ на этом аккаунте? 
		public int MaxCountTopProduct { get; set; }	// Максимальное кол-во товара, если действует топ. 
		public DateTime DateCreate {get; set;}
	}
}
