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
				proverk = col.FindOne(x => x.Code == Convert.ToInt32(shopCode) && x.Name.ToUpper().Replace(" ", "").Equals(shopName.ToUpper().Replace(" ","")));
				if (proverk == null)
					col.Insert(new ShopWhiteOrBlack() { Name = shopName, Code = Convert.ToInt32(shopCode), ShopType = White, ShopIsOnly = Only, WhatIsShop = whatIsShop });
			}
		}
		public static void RedactOldShop(string shopName, string shopCode, bool White, bool Only)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				var proverk = col.FindOne(x => x.Name == shopName);
				proverk = col.FindOne(x => x.Code == Convert.ToInt32(shopCode));
				if (proverk != null)
				{
					proverk.Name = shopName;
					proverk.ShopType = White;
					proverk.ShopIsOnly = Only;
					col.Update(proverk);
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
