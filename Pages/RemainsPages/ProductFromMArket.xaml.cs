using LiteDB;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;
using Остатки.Classes.Petrovich;
using Остатки.Classes.ProductsClasses;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.RemainsPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class ProductFromMArket : Page
	{
		static DataGrid products = new DataGrid();
		public ProductFromMArket()
		{
			this.InitializeComponent();
			ObservableCollection<ProductFromMarletplace> productFromMarletplaces = new ObservableCollection<ProductFromMarletplace>();
			products.Name = "DataGridProduct";
			products.IsReadOnly = true;
			products.ItemsSource = productFromMarletplaces;
			products.SelectionMode = DataGridSelectionMode.Single;
			
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
            {
                var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> productFromMarletplacesTMP = col.Query().ToList();
				productFromMarletplaces = new ObservableCollection<ProductFromMarletplace>(productFromMarletplacesTMP);
			}
			products.ItemsSource = productFromMarletplaces;
			MainGrid.Children.Clear();
			MainGrid.Children.Add(products);
		}
		public static string getLink(string lnk)
		{
			string code = ProductJobs.GetResponseUpdates(lnk);
			//Thread.Sleep(3000);
			if (code.Contains("ничего не найдено"))
			{
				return "null";
			}
			Regex regexLinks = new Regex(@"<a href="".*?""");
			MatchCollection matcheslinks = regexLinks.Matches(code);
			string tmpLinks = "";
			foreach (var item in matcheslinks)
			{
				tmpLinks += item.ToString();
			}
			regexLinks = new Regex(@""".*?""");
			matcheslinks = regexLinks.Matches(tmpLinks);

			foreach (var item in matcheslinks)
			{
					if (!item.ToString().Contains("http"))
						if (!item.ToString().Replace(@"""", "").StartsWith("/shop/") && !item.ToString().Replace(@"""", "").StartsWith("/catalogue/") && !item.ToString().Replace(@"""", "").StartsWith("/advice/") && !item.ToString().Replace(@"""", "").StartsWith("/offer/"))
							return "https://leroymerlin.ru" + item.ToString().Replace(@"""", "");
			}
			return null;
		}

		private static bool ArticleNumberProductId(List<ArticleNumber> oldList, ArticleNumber newArticle)
        {
            foreach (var item in oldList)
            {
				if (item.ArticleOzon == newArticle.ArticleOzon && item.OurArticle.Equals(newArticle.OurArticle))
					return true;
            }
			return false;
        }

		private void AddProduct_Click(object sender, RoutedEventArgs e)
		{
			List<ProductFromMarletplace> allProductsToAdd = new List<ProductFromMarletplace>();
			List<ProductFromMarletplace> ProductToUpdate = new List<ProductFromMarletplace>();

			List<Product> ProductFromBackground = new List<Product>();

			List<Product> ProductToAddRemains = new List<Product>();
			List<Product> ProductToAddArchive = new List<Product>();

			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				allProductsToAdd = col.Query().ToList();
			}

			using (var db = new LiteDatabase($@"{Global.folder.Path}/Background.db"))
			{
				var productsToSpizd = db.GetCollection<Product>("Products");
				ProductFromBackground = productsToSpizd.Query().ToList();
			}

			int offer = -1;
			allProductsToAdd.Reverse();
			List<ProductFromMarletplace> tovar = new List<ProductFromMarletplace>(allProductsToAdd.Where(x => x.offer_id.Contains("pv-")));
			List<Product> productList = new List<Product>();
			int countError = 0;

			List<Product> allProducts = new List<Product>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				allProducts = col.Query().ToList();
			}
			List<Product> Updates = new List<Product>();

			List<ApiKeys> Keyses = ApiKeysesJob.GetAllApiList();

			while (tovar.Count > 0)
			{
				ProductFromMarletplace item = tovar.First();
				tovar.Remove(item);
				using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
				{
					var col = db.GetCollection<Product>("Products");
					productList = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				}
				if (item == null)
					while (item == null)
					{
						item = tovar.First();
					}

				Product OneProductBR = ProductFromBackground.Find(x => x.ArticleNumberUnicList.Equals(item.offer_id));
				if (OneProductBR == null)
				{
					if (item.offer_id.Contains("ld-") || item.offer_id.Contains("lnrd_"))
					{
						string article = "";

						if (item.offer_id.Contains("x10"))
						{
							article = item.offer_id.Substring(0, item.offer_id.Length - 4).Replace("ld-", "").Replace("lnrd_", "");
						}
						else
						{
							article = item.offer_id.Substring(0, item.offer_id.Length - 3).Replace("ld-", "").Replace("lnrd_", "");
						}

						bool isFindDB = false;
						Product findProductInDB = new Product();

						foreach (var ourProduct in allProducts)
						{
							if (isFindDB)
								break;

							if (isFindDB)
								break;
							List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(item.Key.ClientId);
							if (newLst != null && newLst.Count > 0)
								foreach (var oneArt in newLst)
								{
									if (isFindDB)
										break;
									if (oneArt.OurArticle.Contains(article))
									{
										isFindDB = true;
										findProductInDB = ourProduct;
										break;
									}
								}

						}


						if (isFindDB)
						{


							if (!findProductInDB.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
								findProductInDB.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());

							if (!findProductInDB.ArticleNumberUnicList.Contains(item.productID_OfferID.OurArticle))
								findProductInDB.ArticleNumberUnicList.Add(item.productID_OfferID.OurArticle);

							if (!findProductInDB.ArticleNumberProductId[item.Key.ClientId].Contains(item.productID_OfferID))
								findProductInDB.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

							using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
							{
								var col = db.GetCollection<Product>("Products");
								col.Update(findProductInDB);
							}
						}
						else
						{
							int countHosts = Global.WebHosting.Count;
							int hostsUseNow = 0;

							Product oneProduct = LeonardoJobs.AddOneProductNoCombo(Global.WebHosting[0].Link + "?tov=" + article);
							oneProduct.ProductLink = "https://leonardo.ru/ishop/good_" + article;
							hostsUseNow++;
							if (hostsUseNow >= countHosts)
								hostsUseNow = 0;
							if (oneProduct == null)
							{
								//tovar.Enqueue(item);
								Thread.Sleep(3000);

								if (countError == 3)
								{
									Thread.Sleep(70000);
									countError = 0;
								}
								else
								{
									countError++;
								}
							}
							else
							if (oneProduct.ArticleNumberInShop.Length > 0)
							{
								oneProduct.ArticleNumberUnicList = new List<string>();
								oneProduct.ArticleNumberUnicList.Add(item.offer_id);
								oneProduct.ArticleNumberProductId = new Dictionary<string, List<ArticleNumber>>();

								if (!oneProduct.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
									oneProduct.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());
								if (!oneProduct.ArticleNumberProductId[item.Key.ClientId].Contains(item.productID_OfferID))
									oneProduct.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

								using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
								{
									var col = db.GetCollection<Product>("Products");
									col.Insert(oneProduct);
								}
							}
						}

					}
					else
					if (item.offer_id.Contains("pv-"))
					{
						string article = "";

						if (item.offer_id.Contains("x10"))
						{
							article = item.offer_id.Substring(0, item.offer_id.Length - 4).Replace("pv-", "");
						}
						else
						if (item.offer_id.Contains("x"))
						{
							article = item.offer_id.Substring(0, item.offer_id.Length - 3).Replace("pv-", "");
						}
						else
                        {
							article = item.offer_id.Replace("pv-", "");
						}
						bool isFindDB = false;
						Product findProductInDB = null;


						var finding = allProducts.Where(x => string.Compare(x.ArticleNumberInShop, article) == 0);
						List<ProductFromMarletplace> analog = tovar.Where(x => x.offer_id.Contains(article)).ToList();
						if (finding.Count() != 0)
						{
							isFindDB = true;
							findProductInDB = finding.First();
						}
						
						/*foreach (var ourProduct in allProducts)
						{
							List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(item.Key.ClientId);
							foreach (var oneKeyOffFrom in Keyses)
							{
								newLst.AddRange(ourProduct.ArticleNumberProductId.GetValueOrDefault(oneKeyOffFrom.ClientId));
							}

							if (string.Compare(ourProduct.ArticleNumberInShop, article) == 0)
                            {
								isFindDB = true;
								findProductInDB = ourProduct;
								break;
							}

							if (newLst != null && newLst.Count > 0)
								foreach (var oneArt in newLst)
								{
									//if (isFindDB)
									//	break;
									if (oneArt.OurArticle.Contains(article))
									{
										isFindDB = true;
										findProductInDB = ourProduct;
										break;
									}
								} 
						}*/

						if (isFindDB)
						{
							analog.Add(item);

                            foreach (var OneAnalogitem in analog)
                            {
								if (!findProductInDB.ArticleNumberProductId.ContainsKey(OneAnalogitem.Key.ClientId))
									findProductInDB.ArticleNumberProductId.Add(OneAnalogitem.Key.ClientId, new List<ArticleNumber>());

								if (!findProductInDB.ArticleNumberUnicList.Contains(OneAnalogitem.productID_OfferID.OurArticle))
									findProductInDB.ArticleNumberUnicList.Add(OneAnalogitem.productID_OfferID.OurArticle);

								if (!ArticleNumberProductId(findProductInDB.ArticleNumberProductId[OneAnalogitem.Key.ClientId], OneAnalogitem.productID_OfferID))
									findProductInDB.ArticleNumberProductId[OneAnalogitem.Key.ClientId].Add(OneAnalogitem.productID_OfferID);

								tovar.Remove(OneAnalogitem);
							}

							using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
							{
								var col = db.GetCollection<Product>("Products");
								col.Update(findProductInDB);
							}
						}
						else
						{
							Classes.JobWhithApi.PetrovichJobs.Root productRoot = PetrovichJobsWithCatalog.GetProduct(article);
							if (productRoot == null)
							{
								Thread.Sleep(5000);
								productRoot = PetrovichJobsWithCatalog.GetProduct(article);
							}
							if (productRoot != null)
							{
								Classes.JobWhithApi.PetrovichJobs.Product ProductPetrovich = productRoot.data.product;


								Classes.Product newProduct = new Classes.Product();
								newProduct.Name = ProductPetrovich.title;
								newProduct.NowPrice = ProductPetrovich.price.retail;
								newProduct.RemainsWhite = ProductPetrovich.remains.total;
								newProduct.RemainsBlack = PetrovichJobsWithCatalog.GetRemainsBlack(ProductPetrovich.remains);

								newProduct.ProductLink = @"https://moscow.petrovich.ru/catalog/" + ProductPetrovich.breadcrumbs[0].code + "/" + ProductPetrovich.code;
								newProduct.ArticleNumberInShop = ProductPetrovich.code.ToString();
								newProduct.ArticleNumberUnicList = new List<string>();
								newProduct.TypeOfShop = "petrovich";
								newProduct.Weight = ProductPetrovich.weight;
								newProduct.DateHistoryRemains.Add(DateTime.Now);

								if (newProduct != null)
								{
									if (!newProduct.Name.Equals("error"))
									{
										newProduct.ArticleNumberUnicList = new List<string>();
										newProduct.ArticleNumberUnicList.Add(item.offer_id);
										newProduct.ArticleNumberProductId = new Dictionary<string, List<ArticleNumber>>();

										if (!newProduct.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
											newProduct.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());
										if (!newProduct.ArticleNumberProductId[item.Key.ClientId].Contains(item.productID_OfferID))
											newProduct.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

										foreach (var OneAnalogitem in analog)
										{
											if (!newProduct.ArticleNumberProductId.ContainsKey(OneAnalogitem.Key.ClientId))
												newProduct.ArticleNumberProductId.Add(OneAnalogitem.Key.ClientId, new List<ArticleNumber>());

											if (!newProduct.ArticleNumberUnicList.Contains(OneAnalogitem.productID_OfferID.OurArticle))
												newProduct.ArticleNumberUnicList.Add(OneAnalogitem.productID_OfferID.OurArticle);

											if (!ArticleNumberProductId(newProduct.ArticleNumberProductId[OneAnalogitem.Key.ClientId], OneAnalogitem.productID_OfferID))
												newProduct.ArticleNumberProductId[OneAnalogitem.Key.ClientId].Add(OneAnalogitem.productID_OfferID);

											tovar.Remove(OneAnalogitem);
										}

										using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
										{
											var col = db.GetCollection<Product>("Products");
											col.Insert(newProduct);
										}
									}
									else
									{
										using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
										{
											var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
											if (col.FindById(item.Id) == null)
												col.Insert(item);
										}
									}
								}
								else
								{
									//tovar.Enqueue(item);
									Thread.Sleep(3000);

									if (countError == 3)
									{
										Thread.Sleep(5000);
										countError = 0;
									}
									else
									{
										countError++;
									}
								}
							}
							else
							{
								//tovar.Enqueue(item);
							}
						}
					}
					/*if (item.offer_id.Contains("lm-"))
					{
						string article = "";

						if (item.offer_id.Contains("x10"))
						{
							article = item.offer_id.Substring(0, item.offer_id.Length - 4).Replace("lm-", "");
						}
						else
						{
							article = item.offer_id.Substring(0, item.offer_id.Length - 3).Replace("lm-", "");
						}


						bool isFindDB = false;
						Product findProductInDB = new Product();

						foreach (var ourProduct in allProducts)
						{
							if (isFindDB)
								break;
							List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(item.Key.ClientId);
							if (newLst != null && newLst.Count > 0)
								foreach (var oneArt in newLst)
								{
									if (isFindDB)
										break;
									if (oneArt.OurArticle.Contains(article))
									{
										isFindDB = true;
										findProductInDB = ourProduct;
										break;
									}
								}

						}

						if (isFindDB)
						{
							if (!findProductInDB.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
								findProductInDB.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());

							if (!findProductInDB.ArticleNumberUnicList.Contains(item.productID_OfferID.OurArticle))
								findProductInDB.ArticleNumberUnicList.Add(item.productID_OfferID.OurArticle);

							if (!ArticleNumberProductId(findProductInDB.ArticleNumberProductId[item.Key.ClientId], item.productID_OfferID))
								findProductInDB.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

							using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
							{
								var col = db.GetCollection<Product>("Products");
								col.Update(findProductInDB);
							}
						}
						else
						{
							string linkProduct = getLink("https://leroymerlin.ru/search/?q=" + article);

							if (linkProduct == null)
							{
								//tovar.Enqueue(item);
								Thread.Sleep(3000);

								if (countError == 3)
								{
									Thread.Sleep(5000);
									countError = 0;
								}
								else
								{
									countError++;
								}
							}
							else
							{
								if (linkProduct.Equals("null"))
								{
									using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
									{
										var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
										if (col.FindById(item.Id) == null)
											col.Insert(item);
									}
								}
								else
								{
									if (linkProduct != null)
									{
										Product newProduct = ProductJobs.GetProductLeroyByLink(linkProduct);

										if (newProduct != null)
										{
											if (!newProduct.Name.Equals("error"))
											{
												newProduct.ArticleNumberUnicList = new List<string>();
												newProduct.ArticleNumberUnicList.Add(item.offer_id);
												newProduct.ArticleNumberProductId = new Dictionary<string, List<ArticleNumber>>();

												if (!newProduct.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
													newProduct.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());
												if (!newProduct.ArticleNumberProductId[item.Key.ClientId].Contains(item.productID_OfferID))
													newProduct.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

												using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
												{
													var col = db.GetCollection<Product>("Products");
													col.Insert(newProduct);
												}
											}
											else
											{
												using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
												{
													var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
													col.Insert(item);
												}
											}
										}
										else
										{
											//tovar.Enqueue(item);
											Thread.Sleep(3000);

											if (countError == 3)
											{
												Thread.Sleep(5000);
												countError = 0;
											}
											else
											{
												countError++;
											}
										}
									}
									else
									{
										if (countError == 3)
										{
											Thread.Sleep(5000);
											countError = 0;
										}
										else
										{
											countError++;
										}
									}
								}
							}
						}
					}
					else
					if (int.TryParse(item.offer_id, out offer))
					{
						string article = offer.ToString();

						bool isFindDB = false;
						Product findProductInDB = new Product();

						foreach (var ourProduct in allProducts)
						{
							if (isFindDB)
								break;
							List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(item.Key.ClientId);
							if (newLst != null && newLst.Count > 0)
								foreach (var oneArt in newLst)
								{
									if (isFindDB)
										break;
									if (oneArt.OurArticle.Contains(article))
									{
										isFindDB = true;
										findProductInDB = ourProduct;
										break;
									}
								}

						}
						if (isFindDB)
						{


							if (!findProductInDB.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
								findProductInDB.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());

							if (!findProductInDB.ArticleNumberUnicList.Contains(item.productID_OfferID.OurArticle))
								findProductInDB.ArticleNumberUnicList.Add(item.productID_OfferID.OurArticle);

							if (!ArticleNumberProductId(findProductInDB.ArticleNumberProductId[item.Key.ClientId], item.productID_OfferID))
								findProductInDB.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

							using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
							{
								var col = db.GetCollection<Product>("Products");
								col.Update(findProductInDB);
							}
						}
						else
						{
							string linkProduct = getLink("https://leroymerlin.ru/search/?q=" + article);

							if (linkProduct == null)
							{
								tovar.Enqueue(item);
								Thread.Sleep(3000);

								if (countError == 3)
								{
									Thread.Sleep(5000);
									countError = 0;
								}
								else
								{
									countError++;
								}
							}
							else
							{

								if (linkProduct.Equals("null"))
								{
									using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
									{
										var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
										if (col.FindById(item.Id) == null)
											col.Insert(item);
									}
								}
								else
								{
									if (linkProduct != null && !linkProduct.Equals("https://leroymerlin.ru/product/lampa-svetodiodnaya-lexman-clear-g53-175-250-v-6-vt-prozrachnaya-500-lm-teplyy-belyy-svet-82991617/"))
									{
										Product newProduct = ProductJobs.GetProductLeroyByLink(linkProduct);
										if (!newProduct.Name.Equals("error"))
										{
											if (newProduct != null)
											{
												newProduct.ArticleNumberUnicList = new List<string>();
												newProduct.ArticleNumberUnicList.Add(item.offer_id);
												newProduct.ArticleNumberProductId = new Dictionary<string, List<ArticleNumber>>();

												if (!newProduct.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
													newProduct.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());
												newProduct.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

												using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
												{
													var col = db.GetCollection<Product>("Products");
													col.Insert(newProduct);
												}
											}
											else
											{
												tovar.Enqueue(item);
												Thread.Sleep(3000);

												if (countError == 3)
												{
													Thread.Sleep(5000);
													countError = 0;
												}
												else
												{
													countError++;
												}
											}
										}
										else
										{
											using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
											{
												var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
												col.Insert(item);
											}
										}

									}
									else
									{
										if (countError == 3)
										{
											Thread.Sleep(5000);
											countError = 0;
										}
										else
										{
											countError++;
										}
									}
								}
							}


						}
					}
					else
					{
						using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
						{
							var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
							if (col.FindById(item.Id) == null)
								col.Insert(item);
						}
					}*/
				}
				else
				{
					Classes.Product newProduct = OneProductBR;
					newProduct.DateHistoryRemains.Add(DateTime.Now);

					if (newProduct != null)
					{
						newProduct.ArticleNumberUnicList = new List<string>();
						newProduct.ArticleNumberUnicList.Add(item.offer_id);
						newProduct.ArticleNumberProductId = new Dictionary<string, List<ArticleNumber>>();

						if (!newProduct.ArticleNumberProductId.ContainsKey(item.Key.ClientId))
							newProduct.ArticleNumberProductId.Add(item.Key.ClientId, new List<ArticleNumber>());
						newProduct.ArticleNumberProductId[item.Key.ClientId].Add(item.productID_OfferID);

						using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
						{
							var col = db.GetCollection<Product>("Products");
							col.Insert(newProduct);
						}
						using (var db = new LiteDatabase($@"{Global.folder.Path}/Background.db"))
						{
							var productsToSpizd = db.GetCollection<Product>("Products");
							productsToSpizd.Delete(OneProductBR.Id);
							ProductFromBackground = productsToSpizd.Query().ToList();
						}
					}
				}

			}


			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				foreach (var item in ProductToUpdate)
				{
					col.Update(item);
				}
			}

			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
                foreach (var item in Updates)
                {
					col.Update(item);
				}
			}


			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var products = db.GetCollection<Product>("Products");
				products.InsertBulk(ProductToAddRemains);
				var productsArchive = db.GetCollection<Product>("ProductsArchive");
				productsArchive.InsertBulk(ProductToAddArchive);
			}
		}

		private void VisibleProduct_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> allProducts = col.Query().Where(x => x.status == "VISIBLE").ToList();
				products.ItemsSource = new ObservableCollection<ProductFromMarletplace>(allProducts);
			}
		}
		
		private void ModeratedProduct_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> allProducts = col.Query().Where(x => x.status == "MODERATED").ToList();
				products.ItemsSource = new ObservableCollection<ProductFromMarletplace>(allProducts);
			}
		}
		private void DisabledProduct_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> allProducts = col.Query().Where(x => x.status == "DISABLED").ToList();
				products.ItemsSource = new ObservableCollection<ProductFromMarletplace>(allProducts);
			}
		}
		private void In_SaleProduct_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> allProducts = col.Query().Where(x => x.status == "IN_SALE").ToList();
				products.ItemsSource = new ObservableCollection<ProductFromMarletplace>(allProducts);
			}
		}
		private void Removed_From_SaleProduct_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> allProducts = col.Query().Where(x => x.status == "REMOVED_FROM_SALE").ToList();
				products.ItemsSource = new ObservableCollection<ProductFromMarletplace>(allProducts);
			}
		}
		private void ArchivedProduct_Click(object sender, RoutedEventArgs e)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> allProducts = col.Query().Where(x => x.status == "ARCHIVED").ToList();
				products.ItemsSource = new ObservableCollection<ProductFromMarletplace>(allProducts);
			}
		}
	}
}
