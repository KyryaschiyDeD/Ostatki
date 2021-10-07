using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class SatisticGlobal
	{
		public long AllProducts { get; set; } // Всего продуктов в базе
		public long AllProductsRemains { get; set; } // Всего продуктов в остатках
		public long AllProductsWait { get; set; } // Всего продуктов в ожидаем появления
		public long AllProductsArchive { get; set; } // Всего продуктов в архиве
		public long AllProductsDel { get; set; } // Всего продуктов удалено

		public DateTime DateCreate { get; set; } // Дата создания

		public Dictionary<string, long> ProductsAtAccaunt { get; set; } // Словаь (ClientId, count) товаров в продаже

	}
}
