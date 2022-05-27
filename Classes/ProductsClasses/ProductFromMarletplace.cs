using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Остатки.Classes.JobWhithApi.Ozon.ProductInfo;

namespace Остатки.Classes.ProductsClasses
{
	internal class ProductFromMarletplace
	{
		public Guid Id { get; set; }

		public string status { get; set; } // Статус товара
		public ArticleNumber productID_OfferID { get;set;}
		public ApiKeys Key { get;set;}
		public string offer_id { get; set; }
		public ProductInfoFromOzon productInfoFromOzon { get; set; } // Информация с озона при выполнении /v2/product/list
		public DateTime dateTimeCreate { get; set; } // Дата и время добавления в приложение
		public DateTime dateTimeRedact { get; set; } // Дата и время изменения в приложении
		public bool IsFindFromBackground { get; set; } // Найден есть ли товар в приложении.
	}
}
