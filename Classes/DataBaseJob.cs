using LiteDB;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace Остатки.Classes
{
	public class DataBaseJob
	{
		public static void ErrorArticle(List<ItemProsuctOfferIDs> articleOzon)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
			{
				var col = db.GetCollection<ErrorsArticle>("ErrorArticle");
				foreach (var item in articleOzon)
				{
					var proverk = col.FindOne(x => x.OfferId == item.offer_id);
					if (proverk == null)
						col.Insert(new ErrorsArticle { OfferId = item.offer_id, ProductId = Convert.ToInt64(item.product_id) });
				}
				
			}
		}
		public static void RemainsToWait(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var wait = db.GetCollection<Product>("ProductsWait");
				var online = db.GetCollection<Product>("Products");
				var proverk = wait.FindOne(x => x.ArticleNumberLerya == product.ArticleNumberLerya);
				if (proverk == null)
				{
					wait.Insert(product);
					online.Delete(product.Id);
					Message.infoList.Add("Товар успешно перемещён в ожидание!!!");
				}
				else
				{
					Message.errorsList.Add("Данный товар уже в ожидании!!!");
				}

			}
		}
		public static void AddNewProduct(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var proverk = col.FindOne(x => x.ArticleNumberLerya == product.ArticleNumberLerya);
				if (proverk == null)
					col.Insert(product);
			}
		}

		public static void UpdateList(List<Product> product)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				foreach (var item in product)
				{
					var proverk = col.FindOne(x => x.ArticleNumberLerya == item.ArticleNumberLerya);
					if (proverk != null)
					{
						proverk = item;
						col.Update(proverk);
					}
				}
			}
		}
		public static void UpdateRemainsOldProduct(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var proverk = col.FindOne(x => x.ArticleNumberLerya == product.ArticleNumberLerya);

				proverk.DateHistoryRemains.Add(System.DateTime.Now);

				proverk.HistoryRemainsWhite.Add(proverk.RemainsWhite);
				proverk.RemainsWhite = product.RemainsWhite;

				proverk.HistoryRemainsBlack.Add(proverk.RemainsBlack);
				proverk.RemainsBlack = product.RemainsBlack;

				if (product.NowPrice != proverk.NowPrice)
				{
					proverk.OldPrice.Add(proverk.NowPrice);
					proverk.NowPrice = product.NowPrice;
					proverk.DateOldPrice.Add(System.DateTime.Now);
				}
				col.Update(proverk);
			}
		}
	}
}
