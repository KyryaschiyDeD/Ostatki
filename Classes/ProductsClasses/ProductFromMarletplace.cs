using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses
{
	internal class ProductFromMarletplace
	{
		public Guid Id { get; set; }
		public long product_id { get; set; } // Артикул маркетплейса
		public string offer_id { get; set; } // Наш артикул
		public string status { get; set; } // Статус товара
		public List<ApiKeys> AccauntKey { get; set; } // Клиент ID аккаунта 
		public DateTime dateTimeCreate { get; set; } // Дата и время добавления в приложение
		public DateTime dateTimeRedact { get; set; } // Дата и время изменения в приложении
		public bool IsFindFromBackground { get; set; } // Найден есть ли товар в приложении.
	}
}
