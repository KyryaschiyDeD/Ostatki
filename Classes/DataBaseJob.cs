﻿using LiteDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var wait = db.GetCollection<Product>("ProductsWait");
				var online = db.GetCollection<Product>("Products");
				var proverk = wait.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				if (proverk == null)
				{
					wait.Insert(product);
					online.Delete(product.Id);
				}
				else
				{
					online.Delete(product.Id);
				}
			}
		}
		public static void WaitToRemains(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var wait = db.GetCollection<Product>("ProductsWait");
				var online = db.GetCollection<Product>("Products");
				var proverk = online.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				if (proverk == null)
				{
					online.Insert(product);
					wait.Delete(product.Id);
				}
				else
				{
					wait.Delete(product.Id);
				}
			}
		}
		public static void ArchiveToRemains(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var archive = db.GetCollection<Product>("ProductsArchive");
				var online = db.GetCollection<Product>("Products");
				var proverk = online.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				if (proverk == null)
				{
					online.Insert(product);
					archive.Delete(product.Id);
				}
				else
				{
					archive.Delete(product.Id);
				}
			}
		}
		public static void RemainsToArchive(Product product)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var wait = db.GetCollection<Product>("ProductsArchive");
				var online = db.GetCollection<Product>("Products");
				var proverk = wait.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				if (proverk == null)
				{
					wait.Insert(product);
					online.Delete(product.Id);
				}
				else
				{
					online.Delete(product.Id);
				}
			}
		}
		public static void WaitToArchive(Product product)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var archive = db.GetCollection<Product>("ProductsArchive");
				var wait = db.GetCollection<Product>("ProductsWait");
				var proverk = archive.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				if (proverk == null)
				{
					archive.Insert(product);
					wait.Delete(product.Id);
				}
				else
				{
					wait.Delete(product.Id);
				}
			}
		}
		public static void AddNewProduct(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var wait = db.GetCollection<Product>("ProductsWait");
				var proverk = col.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				var proverkWait = wait.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);
				if (proverk == null && proverkWait == null)
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
					//var proverk = col.FindOne(x => x.ProductLink == item.ProductLink);
					//if (proverk != null)
					//{
						//proverk = item;
						col.Update(item);
					//}
				}
			}
		}
		public static void AddListToBackground(List<Product> product)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Background.db"))
			{
				var col = db.GetCollection<Product>("Products");
				col.InsertBulk(product);
				/*foreach (var item in product)
				{
					col.Insert(item);
				}*/
			}
		}
		public static void AddListToRemains(List<Product> product)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Background.db"))
			{
				var col = db.GetCollection<Product>("Products");
				col.InsertBulk(product);
				/*foreach (var item in product)
				{
					col.Insert(item);
				}*/
			}
		}
		public static void UpdateRemainsOldProduct(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var proverk = col.FindOne(x => x.ArticleNumberInShop == product.ArticleNumberInShop);

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
		public static void UpdateOneProduct(Product product)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var proverk = col.FindOne(x => x.Id == product.Id);
				if (proverk == null)
				{
					col = db.GetCollection<Product>("ProductsWait");
					proverk = col.FindOne(x => x.Id == product.Id);
					if (proverk == null)
					{
						col = db.GetCollection<Product>("ProductsArchive");
						proverk = col.FindOne(x => x.Id == product.Id);
					}
				}
				proverk.ArticleNumberUnicList = product.ArticleNumberUnicList;
				col.Update(proverk);
			}
		}
		public static void SaveNewRemains(ConcurrentQueue<Product> NewRemaintProductLerya)
		{
			List<Product> lstPr = new List<Product>();
			List<Product> UpdateWait = new List<Product>();
			List<Product> UpdateOnline = new List<Product>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var online = db.GetCollection<Product>("Products");
				var wait = db.GetCollection<Product>("ProductsWait");
				int kollvo = NewRemaintProductLerya.Count();
				Action action = () =>
				{
					while (!NewRemaintProductLerya.IsEmpty)
					{
						Product OneProduct;
						NewRemaintProductLerya.TryDequeue(out OneProduct);
						bool error = false;
						Product proverk = new Product();
						try
						{
							proverk = online.FindOne(x => x.ProductLink == OneProduct.ProductLink);
						}
						catch (Exception)
						{
							error = true;
							if (OneProduct.ProductLink.Contains("leonardo"))
								ProductJobs.ocherLeonardo.Enqueue(OneProduct);
							else
							if (OneProduct.ProductLink.Contains("leroy"))
								ProductJobs.ocherLeroy.Enqueue(OneProduct.ProductLink);
						}


						bool ItsWait = false;
						if (proverk == null)
						{
							proverk = wait.FindOne(x => x.ArticleNumberInShop == OneProduct.ArticleNumberInShop);
							ItsWait = true;
						}
						proverk.DateHistoryRemains.Add(DateTime.Now);

						proverk.HistoryRemainsWhite.Add(proverk.RemainsWhite);
						proverk.RemainsWhite = OneProduct.RemainsWhite;

						proverk.HistoryRemainsBlack.Add(proverk.RemainsBlack);
						proverk.RemainsBlack = OneProduct.RemainsBlack;

						proverk.DateHistoryRemains = OneProduct.DateHistoryRemains;

						proverk.Weight = OneProduct.Weight;

						if (OneProduct.NowPrice != proverk.NowPrice)
						{
							proverk.OldPrice.Add(proverk.NowPrice);
							proverk.NowPrice = OneProduct.NowPrice;
							proverk.DateOldPrice.Add(DateTime.Now);
						}
						if (OneProduct.Name != proverk.Name || String.IsNullOrEmpty(proverk.Name))
						{
							proverk.Name = OneProduct.Name;
							proverk.NameIsRedact = true;
						}
						if (!error)
						{
							if (ItsWait)
							{
								wait.Update(proverk);
							}
							else
								online.Update(proverk);
						}

						remains2.UpdateProgress(kollvo, kollvo - NewRemaintProductLerya.Count(), "Сохраняем...");
					}
				};
				Parallel.Invoke(action);
			}
			if (ProductJobs.ocherLeroy.Count != 0)
				remains2.GoUpdateAllDataBaseLerya();
		}

		public static void GetAllProductFromTheBase
			(
			out List<Product> Remains,
			out List<Product> Wait,
			out List<Product> Archive,
			out List<Product> Del
			)
		{
			Remains = new List<Product>();
			Wait = new List<Product>();
			Archive = new List<Product>();
			Del = new List<Product>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				Wait = db.GetCollection<Product>("ProductsWait").Query().ToList();
				Archive = db.GetCollection<Product>("ProductsArchive").Query().ToList();
				Remains = db.GetCollection<Product>("Products").Query().ToList();
			}
		}
	}
}
