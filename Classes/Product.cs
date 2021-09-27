using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Остатки.Classes
{
	public class Product
	{
		public Guid Id { get; set; }
		public string ProductLink { get; set; }
		public string Name { get; set; } // Наименование
		public long ArticleNumberLerya { get; set; } // Леруа артикл
		public Dictionary<string, long> ArticleNumberOzonDict { get; set; } = new Dictionary<string, long>();
		public string ArticleNumberOzonDictCount
		{
			get
			{
				return ArticleNumberOzonDict.Count.ToString();
			}
		}
		//public long ArticleNumberOzon { get; set; } // Озон артикл
		public string ArticleNumberUnic { get; set; } // Озон Кривой (который мы сочиняли сами)
		public int RemainsWhite { get; set; } // Остатки из белого списка
		public Dictionary<int, int> remainsDictionary { get; set; }
		public List<DateTime> DateHistoryRemains { get; set; } = new List<DateTime>();// Даты и время проверки остатков
		public List<int> HistoryRemainsWhite { get; set; } = new List<int>();// История Остатки из белого списка
		public int RemainsBlack { get; set; } // Остальные остатки
		public List<int> HistoryRemainsBlack { get; set; } = new List<int>();// История Остальные остатки
		public double NowPrice { get; set; } // Цена Леруа сейчас
		public double OldPriceCh
		{
			get
			{
				if (OldPrice.Count > 0)
					return OldPrice[OldPrice.Count() - 1];
				else
					return Convert.ToDouble(0);
			}
		}
		public double Weight { get; set; } // Вес
		public bool NewPriceIsSave { get; set; } // Изменён ли на Озон новый ценник
		public bool ArticleError { get; set; } // Существует ли артикул на Озон у нас в БД
		public bool ArticleErrorIgnore { get; set; } // Игнорируем ли мы конфликт
		public List<double> OldPrice { get; set; } = new List<double>(); // Цена Леруа цена была
		public List<DateTime> DateOldPrice { get; set; } = new List<DateTime>();
		public Dictionary<string, bool> AccauntOzonID { get; set; } = new Dictionary<string, bool>();
		public string ClientIDAPi
		{
			get
			{
				string APIs = "";
				if (AccauntOzonID.Count > 0)
				{
					foreach (var item in AccauntOzonID.Keys)
					{
						APIs += item + " ";
					}
				}
				else
					return "Пуста";
				return APIs;
			}
		}
		public int CountOfTrueAccaunt
		{
			get
			{
				int kolvo = 0;
				foreach (var item in AccauntOzonID)
				{
					if (item.Value)
						kolvo++;
				}
				return kolvo;
			}
		}
		public override string ToString()
		{
			return $"{ArticleNumberLerya}";
		}
	}
}
