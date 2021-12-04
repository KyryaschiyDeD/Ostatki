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
		public string WhatIsShop { get; set; } // Что за магазин?
		public int Code { get; set; } // Его код
		public string StringCode { get; set; } // Его код, если он не цифровой (например петрович)
		public bool ShopType { get; set; } // Белый или чёрный
		public bool ShopIsOnly { get; set; } // Устраивают ли нас остатки только там? 
		public string GetCode
		{
			get
			{
				if (Code == 0)
					return StringCode;
				return Code.ToString();
			}
		}
	}
}
