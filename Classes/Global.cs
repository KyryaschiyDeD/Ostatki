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

		public static List<int> whiteList = new List<int>();
		public static List<int> blackList = new List<int>();
		
		public static void GetWhiteBlackShops()
		{
			List<ShopWhiteOrBlack> list = ShopWhiteOrBlackJob.GetAllShopList();
			foreach (var item in list)
			{
				if (item.ShopType)
					whiteList.Add(item.Code);
				else
					blackList.Add(item.Code);
			}
		}

	}
}
