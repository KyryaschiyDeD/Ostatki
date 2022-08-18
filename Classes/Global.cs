using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Остатки.Classes
{
	public class Global
	{
		public static StorageFolder folder = ApplicationData.Current.LocalFolder;

		public static List<int> whiteListLeroy = new List<int>();
		public static List<int> blackListLeroy = new List<int>();

		public static List<Hosts> WebHosting = new List<Hosts>();

		public static List<int> whiteListLeonardo = new List<int>();
		public static List<int> blackListLeonardo = new List<int>();

		public static List<bool> complects = new List<bool>();
		public static bool trueProductsDatabase = false;

		public static void GetWhiteBlackShopsLeroy()
		{
			List<ShopWhiteOrBlack> list = ShopWhiteOrBlackJob.GetShopListSpecifically("Леруа Мерлен");
			foreach (var item in list)
			{
				if (item.ShopType)
					whiteListLeroy.Add(item.Code);
				else
					blackListLeroy.Add(item.Code);
			}
		}
		public static void GetWhiteBlackShopsLeonardo()
		{
			List<ShopWhiteOrBlack> list = ShopWhiteOrBlackJob.GetShopListSpecifically("Леонардо");
			foreach (var item in list)
			{
				if (item.ShopType)
					whiteListLeonardo.Add(item.Code);
				else
					blackListLeonardo.Add(item.Code);
			}
		}

	}
}
