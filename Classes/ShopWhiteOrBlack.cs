using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class ShopWhiteOrBlack
	{
		public Guid Id { get; set; }
		public string Name { get; set; } // Наименование магазина
		public int Code { get; set; } // Его код
		public bool ShopType { get; set; } // Белый или чёрный
		public bool ShopIsOnly { get; set; } // Устраивают ли нас остатки только там? 
	}
}
