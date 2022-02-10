using LiteDB;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;
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
			MainGrid.Children.Add(products);
		}
		public static string getLink(string lnk)
		{
			string code = StealProducts.getResponse(lnk);
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

			Queue<ProductFromMarletplace> tovar = new Queue<ProductFromMarletplace>(allProductsToAdd);
			List<Product> productList = new List<Product>();	
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				productList = col.Query().OrderBy(x => x.RemainsWhite).ToList();
			}
			int countError = 0;

			while (tovar.Count > 0)
			{
				ProductFromMarletplace item = tovar.Dequeue();

				List<Product> allProducts = new List<Product>();
				using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
				{
					var col = db.GetCollection<Product>("Products");
					allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				}

				ArticleNumber art = new ArticleNumber() { ArticleOzon = item.product_id, OurArticle = item.offer_id };
				if (item.status.Equals("VISIBLE"))
				{
					Product OneProductBR = ProductFromBackground.Find(x => x.ArticleNumberUnicList.Equals(item.offer_id));
					if (OneProductBR == null)
					{
						if (item.offer_id.Contains("ld-") || item.offer_id.Contains("lnrd_"))
						{
							/*string article = "";

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
								foreach (var accauntKey in item.AccauntKey)
								{
									if (isFindDB)
										break;
									List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(accauntKey.ClientId);
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
							}


							if (isFindDB)
							{
								foreach (var accauntKey in item.AccauntKey)
								{

									if (!findProductInDB.ArticleNumberProductId.ContainsKey(accauntKey.ClientId))
										findProductInDB.ArticleNumberProductId.Add(accauntKey.ClientId, new List<ArticleNumber>());

									if (!findProductInDB.ArticleNumberUnicList.Contains(art.OurArticle))
										findProductInDB.ArticleNumberUnicList.Add(art.OurArticle);

									if (!findProductInDB.ArticleNumberProductId[accauntKey.ClientId].Contains(art))
										findProductInDB.ArticleNumberProductId[accauntKey.ClientId].Add(art);
								}
								using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
								{
									var col = db.GetCollection<Product>("Products");
									col.Update(findProductInDB);
								}
							}
							else
							{
								Product oneProduct = LeonardoJobs.AddOneProductNoCombo("https://leonardo.ru/ishop/good_" + article);
								if (oneProduct == null)
								{
									tovar.Enqueue(item);
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
									foreach (var accauntKey in item.AccauntKey)
									{
										if (!oneProduct.ArticleNumberProductId.ContainsKey(accauntKey.ClientId))
											oneProduct.ArticleNumberProductId.Add(accauntKey.ClientId, new List<ArticleNumber>());
										oneProduct.ArticleNumberProductId[accauntKey.ClientId].Add(art);
									}
									using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
									{
										var col = db.GetCollection<Product>("Products");
										col.Insert(oneProduct);
									}
								}
							}
							*/
						}
						else
					if (item.offer_id.Contains("pv-"))
						{

						}
						else
					if (item.offer_id.Contains("lm-"))
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
								foreach (var accauntKey in item.AccauntKey)
								{
									if (isFindDB)
										break;
									List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(accauntKey.ClientId);
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
							}


							if (isFindDB)
							{
								foreach (var accauntKey in item.AccauntKey)
								{

									if (!findProductInDB.ArticleNumberProductId.ContainsKey(accauntKey.ClientId))
										findProductInDB.ArticleNumberProductId.Add(accauntKey.ClientId, new List<ArticleNumber>());

									if (!findProductInDB.ArticleNumberUnicList.Contains(art.OurArticle))
										findProductInDB.ArticleNumberUnicList.Add(art.OurArticle);

									if (!findProductInDB.ArticleNumberProductId[accauntKey.ClientId].Contains(art))
										findProductInDB.ArticleNumberProductId[accauntKey.ClientId].Add(art);
								}
								using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
								{
									var col = db.GetCollection<Product>("Products");
									col.Update(findProductInDB);
								}
							}
							else
							{
								string linkProduct = getLink("https://leroymerlin.ru/search/?q=" + article);

								if (linkProduct.Equals("null"))
								{
									using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
									{
										var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
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
												foreach (var accauntKey in item.AccauntKey)
												{
													if (!newProduct.ArticleNumberProductId.ContainsKey(accauntKey.ClientId))
														newProduct.ArticleNumberProductId.Add(accauntKey.ClientId, new List<ArticleNumber>());
													newProduct.ArticleNumberProductId[accauntKey.ClientId].Add(art);
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
													col.Insert(item);
												}
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
								foreach (var accauntKey in item.AccauntKey)
								{
									if (isFindDB)
										break;
									List<ArticleNumber> newLst = ourProduct.ArticleNumberProductId.GetValueOrDefault(accauntKey.ClientId);
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
							}
							if (isFindDB)
							{
								foreach (var accauntKey in item.AccauntKey)
								{

									if (!findProductInDB.ArticleNumberProductId.ContainsKey(accauntKey.ClientId))
										findProductInDB.ArticleNumberProductId.Add(accauntKey.ClientId, new List<ArticleNumber>());

									if (!findProductInDB.ArticleNumberUnicList.Contains(art.OurArticle))
										findProductInDB.ArticleNumberUnicList.Add(art.OurArticle);

									if (!findProductInDB.ArticleNumberProductId[accauntKey.ClientId].Contains(art))
										findProductInDB.ArticleNumberProductId[accauntKey.ClientId].Add(art);
								}
								using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
								{
									var col = db.GetCollection<Product>("Products");
									col.Update(findProductInDB);
								}
							}
							else
							{
								string linkProduct = getLink("https://leroymerlin.ru/search/?q=" + article);

								if (linkProduct.Equals("null"))
								{
									using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
									{
										var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
										col.Insert(item);
									}
								}
								else
								{
									if (linkProduct != null)
									{
										Product newProduct = ProductJobs.GetProductLeroyByLink(linkProduct);
										if (!newProduct.Name.Equals("error"))
										{
											if (newProduct != null)
											{
												newProduct.ArticleNumberUnicList = new List<string>();
												newProduct.ArticleNumberUnicList.Add(item.offer_id);
												newProduct.ArticleNumberProductId = new Dictionary<string, List<ArticleNumber>>();
												foreach (var accauntKey in item.AccauntKey)
												{
													if (!newProduct.ArticleNumberProductId.ContainsKey(accauntKey.ClientId))
														newProduct.ArticleNumberProductId.Add(accauntKey.ClientId, new List<ArticleNumber>());
													newProduct.ArticleNumberProductId[accauntKey.ClientId].Add(art);
												}
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
