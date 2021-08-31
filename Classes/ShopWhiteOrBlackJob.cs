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
		public static void CreateNewShop(string shopName, string shopCode, bool White)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ShopWhiteOrBlack>("ShopWhiteOrBlack");
				var proverk = col.FindOne(x => x.Name == shopName);
				proverk = col.FindOne(x => x.Code == Convert.ToInt32(shopCode));
				if (proverk == null)
					col.Insert(new ShopWhiteOrBlack() { Name = shopName, Code = Convert.ToInt32(shopCode), ShopType = White });
			}
		}
		public static void RedactOldShop(string shopName, string shopCode, bool White)
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
					col.Update(proverk);
				}
					
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
