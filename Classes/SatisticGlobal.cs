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

		public long AllProductsLeroy { get; set; } // Всего продуктов Леруа
		public long AllProductsLeroy0 { get; set; } // Всего продуктов Леруа = 0
		public long AllProductsLeroy20 { get; set; } // Всего продуктов Леруа меньше 20
		public long AllProductsLeroy50 { get; set; } // Всего продуктов Леруа меньше 50
		public long AllProductsLeroy100 { get; set; } // Всего продуктов Леруа меньше 100
		public long AllProductsLeroy100More { get; set; } // Всего продуктов Леруа больше 100


		public Dictionary<string, long> AllProductsLeonardoPos { get; set; } = new Dictionary<string, long>(); // Всего позиций Леонардо 
		public Dictionary<string, long> AllProductsLeroyPos { get; set; } = new Dictionary<string, long>(); // Всего позиций Leroy 
		public Dictionary<string, long> AllProductsPetrovichPos { get; set; } = new Dictionary<string, long>(); // Всего позиций Leroy 

		public long AllProductsLeonardo { get; set; } // Всего продуктов Леонардо
		public long AllProductsLeonardo0 { get; set; } // Всего продуктов Леонардо 0 
		public long AllProductsLeonardo20 { get; set; } // Всего продуктов Леонардо меньше 20 
		public long AllProductsLeonardo50 { get; set; } // Всего продуктов Леонардо меньше 50
		public long AllProductsLeonardo100 { get; set; } // Всего продуктов Леонардо меньше 100
		public long AllProductsLeonardo100More { get; set; } // Всего продуктов Леонардо больше 100

		public long AllProductsPetrovich { get; set; } // Всего продуктов Петрович
		public long AllProductsPetrovich0 { get; set; } // Всего продуктов Петрович 0 
		public long AllProductsPetrovich20 { get; set; } // Всего продуктов Петрович меньше 20
		public long AllProductsPetrovich50 { get; set; } // Всего продуктов Петрович меньше 50
		public long AllProductsPetrovich100 { get; set; } // Всего продуктов Петрович меньше 100
		public long AllProductsPetrovich100More { get; set; } // Всего продуктов Петрович больше 100

		public DateTime DateCreate { get; set; } // Дата создания

		public Dictionary<string, long> ProductsAtAccaunt { get; set; } // Словаь (ClientId, count) товаров в продаже
		public long ProductProblemName { get; set; } // Продукты с пустыми наимнованиеями
		public long DoubleProduct { get; set; } // Повторяющиеся продукты 
		public Dictionary<string, List<int>> CountProductsUpdates { get; set; }// Количество обновлённых товаров последний раз

		public double  NowAveragePrice { get; set; } // Текущая средняя цена
		public double OldAveragePrice { get; set; } // Старая средняя цена
	}
}
