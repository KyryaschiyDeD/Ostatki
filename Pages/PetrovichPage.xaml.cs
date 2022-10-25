using LiteDB;
using OfficeOpenXml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Остатки.Classes;
using Остатки.Classes.JobWhithApi.PetrovichJobs;
using Остатки.Classes.Petrovich;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	/// 
	
	public sealed partial class PetrovichPage : Page
	{
		private static LinkedList<int> HistoryListBack = new LinkedList<int>();
		private static LinkedList<int> HistoryListForward = new LinkedList<int>();

		private static List<string> BarcodeList = new List<string>();

		public static bool CheckedProductInDataBase { get; set; }

		public static bool productCountComplect1 = true;
		public static bool productCountComplect2 = true;
		public static bool productCountComplect3 = true;
		public static bool productCountComplect5 = true;
		public static bool productCountComplect10 = true;
		public static Dictionary<string, string> specificationsDict = new Dictionary<string, string>();
		
		ExcelPackage package;

		private static int HistoryNow { get; set; }

		private void PetrovichProduct_Click(object sender, RoutedEventArgs e)
		{
			GridPetrovich.Children.Clear();
			Classes.JobWhithApi.PetrovichJobs.Root oneProduct = PetrovichJobsWithCatalog.GetProduct((sender as Button).Tag.ToString());
			string info = oneProduct.data.product.title + "\n";
			TextBlock textBlock = new TextBlock();
			textBlock.Text = info;
			GridPetrovich.Children.Add(textBlock);
		}
		public static string RemoveInvalidChars(string file_name)
		{
			foreach (Char invalid_char in Path.GetInvalidFileNameChars())
			{
				file_name = file_name.Replace(oldValue: invalid_char.ToString(), newValue: "");
			}
			return file_name;
		}
		private void GetProducts_ClickAsync(object sender, RoutedEventArgs e)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			package = new ExcelPackage();

			List<bool> listComl = Global.complects;
			bool isCheckBD = Global.trueProductsDatabase;

			List<string> ourArtikulInDb = new List<string>();

			if (isCheckBD)
            {
				List<Classes.Product> Remains = new List<Classes.Product>();
				List<ApiKeys> apiKeys = ApiKeysesJob.GetAllApiList().Where(x => x.InDB).ToList();
				using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
				{
					Remains = db.GetCollection<Classes.Product>("Products").Query().ToList();
				}
                foreach (var item in apiKeys)
                {
					ourArtikulInDb.AddRange(Remains.Where(x => x.ArticleNumberProductId.Keys.ToList().Contains(item.ClientId)).Select(x => x.ArticleNumberInShop).ToList());
				}
			}

			productCountComplect1 = listComl[0];
			productCountComplect2 = listComl[1];
			productCountComplect3 = listComl[2];
			productCountComplect5 = listComl[3];
			productCountComplect10 = listComl[4];

			Classes.JobWhithApi.PetrovichJobs.Root nextCat = PetrovichJobsWithCatalog.GetCatalogDop((sender as Button).Tag.ToString(), 0);
			int countProduct = nextCat.data.pagination.products_count;
			countProduct -= 20;
			int skippedCountProduct = 20;
			List<Classes.JobWhithApi.PetrovichJobs.Product> newProduct = new List<Classes.JobWhithApi.PetrovichJobs.Product>();
			newProduct.AddRange(nextCat.data.products);
			while (countProduct > 0)
			{
				nextCat = PetrovichJobsWithCatalog.GetCatalogDop((sender as Button).Tag.ToString(), skippedCountProduct);
				newProduct.AddRange(nextCat.data.products);
				skippedCountProduct += 20;
				countProduct -= 20;
			}

			int addCountProduct = 0;

			List<string> StandardNamesOfXaract = new List<string>();

			if (!specificationsDict.ContainsKey("Наименование"))
				specificationsDict.Add("Наименование", null);
			if (!specificationsDict.ContainsKey("Артикул"))
				specificationsDict.Add("Артикул", null);
			if (!specificationsDict.ContainsKey("Цена"))
				specificationsDict.Add("Цена", null);
			if (!specificationsDict.ContainsKey("Ссылка"))
				specificationsDict.Add("Ссылка", null);
			if (!specificationsDict.ContainsKey("Ссылка One"))
				specificationsDict.Add("Ссылка One", null);
			if (!specificationsDict.ContainsKey("КолВо завод в отправл"))
				specificationsDict.TryAdd("КолВо завод в отправл", null);
			if (!specificationsDict.ContainsKey("Объединить на одной карточке"))
				specificationsDict.TryAdd("Объединить на одной карточке", null);
			if (!specificationsDict.ContainsKey("Главные фото"))
				specificationsDict.Add("Главные фото", null);
			if (!specificationsDict.ContainsKey("Доп фото"))
				specificationsDict.Add("Доп фото", null);
			if (!specificationsDict.ContainsKey("Описание"))
				specificationsDict.Add("Описание", null);
			if (!specificationsDict.ContainsKey("Вес расчётный (в граммах)"))
				specificationsDict.Add("Вес расчётный (в граммах)", null);
			if (!specificationsDict.ContainsKey("Ширина в мм"))
				specificationsDict.Add("Ширина в мм", null);
			if (!specificationsDict.ContainsKey("Длина в мм"))
				specificationsDict.Add("Длина в мм", null);
			if (!specificationsDict.ContainsKey("Высота в мм"))
				specificationsDict.Add("Высота в мм", null);
			if (!specificationsDict.ContainsKey("Глубина в мм"))
				specificationsDict.Add("Глубина в мм", null);
			if (!specificationsDict.ContainsKey("Неудачные ссылки или онлайн"))
				specificationsDict.Add("Неудачные ссылки или онлайн", null);
			if (!specificationsDict.ContainsKey("Кол-во потерь"))
				specificationsDict.Add("Кол-во потерь", null);
			if (!specificationsDict.ContainsKey("ШтрихКод"))
				specificationsDict.Add("ШтрихКод", null);
			if (!specificationsDict.ContainsKey("Цена + 40%"))
				specificationsDict.Add("Цена + 40%", null);
			if (!specificationsDict.ContainsKey("Г + Д через ,"))
				specificationsDict.Add("Г + Д через ,", null);

			ConcurrentBag<Classes.Product> productListBackground = new ConcurrentBag<Classes.Product>();
			

			var sheet = package.Workbook.Worksheets
							.Add("Магия");
			int koefSTR = 1;

			foreach (Classes.JobWhithApi.PetrovichJobs.Product productFromList in newProduct)
			{
				Classes.JobWhithApi.PetrovichJobs.Root productRoot = PetrovichJobsWithCatalog.GetProduct(productFromList.code.ToString());
				if (productRoot != null)
					if (productRoot.data.product != null)
					{
						Classes.JobWhithApi.PetrovichJobs.Product item = productRoot.data.product;
						if (isCheckBD && !ourArtikulInDb.Contains(item.code.ToString()) || !isCheckBD)
							if (item.remains.total > 5 && item.images != null)
							{
								Classes.Product onePos = new Classes.Product();
								onePos.Name = item.title;
								onePos.NowPrice = item.price.retail;
								onePos.RemainsWhite = item.remains.total;
								onePos.RemainsBlack = PetrovichJobsWithCatalog.GetRemainsBlack(item.remains);

								List<int> countOfComplect = new List<int>();
								if (productCountComplect1)
									countOfComplect.Add(1);

								if (onePos.NowPrice <= 500)
								{
									if (productCountComplect2)
										countOfComplect.Add(2);
									if (productCountComplect3)
										countOfComplect.Add(3);
									if (productCountComplect5)
										countOfComplect.Add(5);
									if (productCountComplect10)
										countOfComplect.Add(10);
								}
								else
								if (onePos.NowPrice <= 1000)
								{
									if (productCountComplect2)
										countOfComplect.Add(2);
									if (productCountComplect3)
										countOfComplect.Add(3);
									if (productCountComplect5)
										countOfComplect.Add(5);
								}
								else
								if (onePos.NowPrice <= 2000)
								{
									if (productCountComplect2)
										countOfComplect.Add(2);
									if (productCountComplect3)
										countOfComplect.Add(3);
								}
								else
								if (onePos.NowPrice <= 3000)
								{
									if (productCountComplect2)
										countOfComplect.Add(2);
								}

								onePos.ProductLink = @"https://moscow.petrovich.ru/catalog/" + item.breadcrumbs[0].code + "/" + item.code;
								onePos.ArticleNumberInShop = item.code.ToString();
								onePos.ArticleNumberUnicList = new List<string>();
								onePos.TypeOfShop = "petrovich";
								onePos.Weight = item.weight;
								onePos.DateHistoryRemains.Add(DateTime.Now);
								specificationsDict["Ссылка One"] += onePos.ProductLink + "\n";
								List<string> oneXaract = new List<string>();
								foreach (var xaract in item.properties)
								{
									if (!StandardNamesOfXaract.Contains(xaract.title))
									{
										StandardNamesOfXaract.Add(xaract.title);
										if (!specificationsDict.ContainsKey(xaract.title))
											specificationsDict.Add(xaract.title, "");

										for (int k = 0; k < addCountProduct; k++)
										{
											for (int j = 0; j < countOfComplect.Count; j++)
											{
												specificationsDict[xaract.title] += "\n";
											}
										}
									}
									for (int j = 0; j < countOfComplect.Count; j++)
									{
										specificationsDict[xaract.title] += xaract.value[0].title + "\n";
										if (xaract.title.Contains("Страна"))
											sheet.Cells[koefSTR, 24].Value = xaract.value[0].title;
									}
									oneXaract.Add(xaract.title);
								}

								foreach (var prOneXaract in StandardNamesOfXaract)
								{
									if (!oneXaract.Contains(prOneXaract))
									{
										for (int j = 0; j < countOfComplect.Count; j++)
										{
											specificationsDict[prOneXaract] += "\n";
										}
									}
								}



								foreach (var compl in countOfComplect)
								{
									sheet.Cells[koefSTR, 1].Value = koefSTR;

									onePos.ArticleNumberUnicList.Add("pv-" + onePos.ArticleNumberInShop + "-" + "x" + compl.ToString());
									specificationsDict["Артикул"] += "pv-" + onePos.ArticleNumberInShop + "-" + "x" + compl.ToString() + "\n";
									sheet.Cells[koefSTR, 2].Value = "pv-" + onePos.ArticleNumberInShop + "-" + "x" + compl.ToString();

									specificationsDict["Объединить на одной карточке"] += onePos.ArticleNumberInShop + "_one_cart\n";

									specificationsDict["Наименование"] += item.title + "\n";
									sheet.Cells[koefSTR, 3].Value = item.title;

									specificationsDict["КолВо завод в отправл"] += compl.ToString() + "\n";

									specificationsDict["Вес расчётный (в граммах)"] += onePos.Weight * 1000 + "\n";
									sheet.Cells[koefSTR, 10].Value = onePos.Weight * 1000;

									int nowPrice = (int)(Convert.ToInt32(Convert.ToDouble(onePos.NowPrice) * compl + 45 + onePos.Weight * 20 + 50) * 1.075 * 1.1 * 1.25 * 1.1 + 5) / 10 * 10;


									specificationsDict["Цена"] += nowPrice.ToString() + "\n";
									sheet.Cells[koefSTR, 4].Value = nowPrice.ToString();

									specificationsDict["Цена + 40%"] += (nowPrice * 1.4).ToString() + "\n";
									sheet.Cells[koefSTR, 5].Value = (nowPrice * 1.4).ToString();

									sheet.Cells[koefSTR, 6].Value = "Не облагается";
									sheet.Cells[koefSTR, 18].Value = "Нет бренда";

									specificationsDict["Ссылка"] += onePos.ProductLink + "\n";

									if (item.images.Count > 0)
									{
										specificationsDict["Главные фото"] += "https:" + item.images[0] + "\n";
										sheet.Cells[koefSTR, 14].Value = "https:" + item.images[0];

										specificationsDict["Г + Д через ,"] += item.images[0] + ",";
										int countImg = item.images.Count;
										if (countImg > 1)
											for (int i = 1; i < countImg; i++)
											{
												specificationsDict["Доп фото"] += "https:" + item.images[i] + " ";
												sheet.Cells[koefSTR, 15].Value += "https:" + item.images[i] + "\n";

												specificationsDict["Г + Д через ,"] += "https:" + item.images[i] + ",";
											}
										specificationsDict["Доп фото"] += "\n";
										specificationsDict["Г + Д через ,"] += "\n";
									}

									//specificationsDict["Описание"] += item.description + "\n";
									specificationsDict["Описание"] += Regex.Replace(item.description.Replace("</li>"," "), "<[^>]+>", string.Empty) + "\n";
									//specificationsDict["Описание"] += HTMLJob.UnHtml(item.description) + "\n";
									//sheet.Cells[koefSTR, 26].Value = item.description;
									sheet.Cells[koefSTR, 26].Value = Regex.Replace(item.description.Replace("</li>", " "), "<[^>]+>", string.Empty);
									//sheet.Cells[koefSTR, 26].Value = HTMLJob.UnHtml(item.description);

									bool IsUnic = false;
									DateTime date = DateTime.Now;
									while (!IsUnic)
									{
										string dateStr = date.ToString("dd.MM.yyyy") + date.ToString("hh:mm:ss:ff");
										dateStr = dateStr.Replace(".", "").Replace(":", "").Replace(" ", "");
										if (!BarcodeList.Contains(dateStr))
										{
											IsUnic = true;
											specificationsDict["ШтрихКод"] += dateStr + "\n";
											BarcodeList.Add(dateStr);
										}
										date.AddMinutes(1);
										IsUnic = true;
									}
									specificationsDict["Ширина в мм"] += item.width + "\n";
									sheet.Cells[koefSTR, 11].Value = item.width;

									specificationsDict["Длина в мм"] += item.length + "\n";
									sheet.Cells[koefSTR, 13].Value = item.length;

									specificationsDict["Высота в мм"] += item.height + "\n";
									sheet.Cells[koefSTR, 12].Value = item.height;
									koefSTR++;
								}

								addCountProduct++;
								productListBackground.Add(onePos);
							}
					}
			}

			GoToCreateFils();

			DataBaseJob.AddListToBackground(productListBackground.ToList());
		}
		private async void GoToCreateFils()
		{
			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();
			if (fileWithLinks != null)
			{
				await fileWithLinks.CreateFileAsync(RemoveInvalidChars("шабл") + ".xlsx", CreationCollisionOption.ReplaceExisting);
				StorageFile myFile = await fileWithLinks.GetFileAsync(RemoveInvalidChars("шабл") + ".xlsx");
				await FileIO.WriteBytesAsync(myFile, package.GetAsByteArray());

				await fileWithLinks.CreateFolderAsync("X", CreationCollisionOption.ReplaceExisting);
				StorageFolder SaveFolder = await fileWithLinks.GetFolderAsync("X");


				foreach (var item in specificationsDict)
				{
					await SaveFolder.CreateFileAsync(RemoveInvalidChars(item.Key) + ".txt", CreationCollisionOption.ReplaceExisting);
					myFile = await SaveFolder.GetFileAsync(RemoveInvalidChars(item.Key) + ".txt");
					string data = item.Value;
					if (String.IsNullOrEmpty(data))
						data = "";
					await FileIO.WriteTextAsync(myFile, data);
				}
			}
			specificationsDict.Clear();
		}
		private void PetrovichCategory_Click(object sender, RoutedEventArgs e)
		{
			GridPetrovich.Children.Clear();
			Classes.JobWhithApi.PetrovichJobs.Root nextCat = PetrovichJobsWithCatalog.GetCatalogDop((sender as Button).Tag.ToString(), 0);
			HistoryListBack.AddFirst((int)(sender as Button).Tag);
			if (nextCat.data.section.children == null && nextCat.data.products != null)
			{
				Button buttonGetProducts = new Button();
				buttonGetProducts.Content = "Украсть";
				buttonGetProducts.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15 };
				buttonGetProducts.Margin = new Thickness() { Bottom = 0, Left = 5, Right = 0, Top = 10 };
				buttonGetProducts.Click += GetProducts_ClickAsync;
				buttonGetProducts.Tag = nextCat.data.section.code;
				GridPetrovich.Children.Add(buttonGetProducts);
			}

			if (nextCat.data.section.children != null)
			{
				foreach (var item in nextCat.data.section.children)
				{
					StackPanel stackPanel = new StackPanel();
					stackPanel.Name = "PetrovichrRow";
					stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
					stackPanel.VerticalAlignment = VerticalAlignment.Center;
					stackPanel.Orientation = Orientation.Horizontal;

					Button button = new Button();
					button.Content = item.title + ": " + item.product_qty;
					button.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15 };
					button.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 10 };
					button.Click += PetrovichCategory_Click;
					button.Tag = item.code;

					Button buttonGetProducts = new Button();
					buttonGetProducts.Content = "Украсть";
					buttonGetProducts.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15 };
					buttonGetProducts.Margin = new Thickness() { Bottom = 0, Left = 5, Right = 0, Top = 10 };
					buttonGetProducts.Click += GetProducts_ClickAsync;
					buttonGetProducts.Tag = item.code;

					stackPanel.Children.Add(button);
					
					//if (nextCat.data.products != null)
						stackPanel.Children.Add(buttonGetProducts);
					GridPetrovich.Children.Add(stackPanel);
				}
			}
			else
			{
				int countProduct = nextCat.data.pagination.products_count;
				countProduct -= 20;
				int skippedCountProduct = 20;
				while (countProduct > 0)
				{
					foreach (var OneProduct in nextCat.data.products)
					{
						Button button = new Button();
						button.Content = OneProduct.title;
						button.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15 };
						button.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 10 };
						button.Click += PetrovichProduct_Click;
						button.Tag = OneProduct.code;
						GridPetrovich.Children.Add(button);
					}
					nextCat = PetrovichJobsWithCatalog.GetCatalogDop((sender as Button).Tag.ToString(), skippedCountProduct);
					skippedCountProduct += 20;
					countProduct -= 20;
				}
			}
		}

		public PetrovichPage()
		{
			this.InitializeComponent();
			Classes.JobWhithApi.PetrovichJobs.Root list = PetrovichJobsWithCatalog.GetCatalog();
			HistoryListBack.AddFirst(-1);
			foreach (var item in list.data.sections)
			{
				Button button = new Button();
				button.Content = item.title;
				button.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15};
				button.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 10};
				button.Click += PetrovichCategory_Click; 
				button.Tag = item.code;
				GridPetrovich.Children.Add(button);
			}
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = new Button();
			button.Tag = HistoryListBack.First;
			HistoryListBack.RemoveFirst();
			PetrovichCategory_Click(button, new RoutedEventArgs());
		}

		private void ForwardButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
