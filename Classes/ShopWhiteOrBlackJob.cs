using LiteDB;
using System;
using System.Collections.Generic;
using Остатки.Classes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class ShopWhiteOrBlackJob
	{
		public static bool Search(string a, string b)
		{
			if (a.ToUpper() == b.ToUpper()) 
				return true;
			else
				return false;
        }
		public static void CreateNewShop(string shopName, string shopCode, bool White, bool Only, string whatIsShop)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				var proverk = col.FindOne(x => x.Name == shopName);
				int ch = 0;
				if (int.TryParse(shopCode, out ch))
				{
					proverk = col.FindOne(x => x.Code == Convert.ToInt32(shopCode) && x.Name.ToUpper().Replace(" ", "").Equals(shopName.ToUpper().Replace(" ", "")));
					if (proverk == null)
						col.Insert(new ShopWhiteOrBlack() { Name = shopName, Code = Convert.ToInt32(shopCode), StringCode = shopCode, ShopType = White, ShopIsOnly = Only, WhatIsShop = whatIsShop });
				}
				else
				{
					proverk = col.FindOne(x => x.StringCode == shopCode && x.Name.ToUpper().Replace(" ", "").Equals(shopName.ToUpper().Replace(" ", "")));
					if (proverk == null)
						col.Insert(new ShopWhiteOrBlack() { Name = shopName, Code = 0, StringCode = shopCode, ShopType = White, ShopIsOnly = Only, WhatIsShop = whatIsShop });
				}

			}
		}
		public static void RedactOldShop(string shopName, string shopCode, bool White, bool Only, string whatIsShop)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				int ch = 0;
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				if (int.TryParse(shopCode, out ch))
				{
					var proverk = col.FindOne(x => x.Code == Convert.ToInt32(shopCode) && x.WhatIsShop == whatIsShop);
					if (proverk != null)
					{
						if (shopName.Length > 0)
							proverk.Name = shopName;
						proverk.ShopType = White;
						proverk.ShopIsOnly = Only;
						col.Update(proverk);
					}
				}
				else
				{
					var proverk = col.FindOne(x => x.StringCode == shopCode && x.WhatIsShop == whatIsShop);
					if (proverk != null)
					{
						if (shopName.Length > 0)
							proverk.Name = shopName;
						proverk.ShopType = White;
						proverk.ShopIsOnly = Only;
						col.Update(proverk);
					}
				}

			}
		}

		public static List<ShopWhiteOrBlack> GetShopListSpecifically(string nameShop)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				return col.Query().Where(x => x.WhatIsShop == nameShop).ToList();
			}
		}
		public static List<int> GetShopIdListSpecifically(string nameShop)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				return col.Query().Where(x => x.WhatIsShop == nameShop).Select(x => x.Code).ToList();
			}
		}

		public static List<ShopWhiteOrBlack> GetAllShopList()
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				return col.Query().ToList();
			}
		}
	}
}
