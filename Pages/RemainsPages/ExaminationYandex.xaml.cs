using LiteDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.RemainsPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class ExaminationYandex : Page
	{

		int count = 0;
		static ConcurrentQueue<string> UnRedactProductsQueue = new ConcurrentQueue<string>();

		public ExaminationYandex()
		{
			this.InitializeComponent();
		}

		private async void ExaminationYandexFile_Click(object sender, RoutedEventArgs e)
		{
			var picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".txt");
			StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				IList<string> linksProductTXT = await Windows.Storage.FileIO.ReadLinesAsync(file);
				foreach (var item in linksProductTXT)
				{
					UnRedactProductsQueue.Enqueue(item);
					count++;
				}
			}
			List<Product> allProducts = new List<Product>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				//var productsArchive = db.GetCollection<Product>("ProductsArchive");
				//var productsWait = db.GetCollection<Product>("ProductsWait");
				allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				//allProducts.AddRange(productsArchive.Query().ToList());
				//allProducts.AddRange(productsWait.Query().ToList());
			}
			List<string> productNuYandexNull = new List<string>();
			List<string> productToAdd = new List<string>();
			foreach (var item in UnRedactProductsQueue)
			{
				Product product = allProducts.Find(x => x.ArticleNumberUnicList.Contains(item));
				if (product != null)
				{
					if (product.RemainsWhite == 0 || (product.TypeOfShop == "Леонардо" && product.RemainsWhite <= 1))
						productNuYandexNull.Add(item);
					else
						productToAdd.Add(item);
				}
				else
					productNuYandexNull.Add(item);
			}
			
			
			string allToDell = "";
			foreach (var item in productNuYandexNull)
			{
				allToDell += item + "\n";
			}
			string allToAdd = "";
			foreach (var item in productToAdd)
			{
				allToAdd += item + "\n";
			}
			DateTime timeDateNow = DateTime.Now;
			string dateTimeStr = timeDateNow.ToString().Replace(":", ".");

			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();
			
			await fileWithLinks.CreateFileAsync("Удаление из яндекса от " + dateTimeStr + ".txt", CreationCollisionOption.ReplaceExisting);
			StorageFile myFile = await fileWithLinks.GetFileAsync("Удаление из яндекса от " + dateTimeStr + ".txt");
			string data = allToDell;
			await FileIO.WriteTextAsync(myFile, data);

			await fileWithLinks.CreateFileAsync("Правильные остатки яндекса от " + dateTimeStr + ".txt", CreationCollisionOption.ReplaceExisting);
			myFile = await fileWithLinks.GetFileAsync("Правильные остатки яндекса от " + dateTimeStr + ".txt");
			data = allToAdd;
			await FileIO.WriteTextAsync(myFile, data);


			UnRedactProductsQueue = new ConcurrentQueue<string>();
			allToDell = "";
			allToAdd = "";
		}


		private async void GoToUpdateYzndexPriceFromFile_Click(object sender, RoutedEventArgs e)
		{
			var picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".txt");
			StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				IList<string> linksProductTXT = await Windows.Storage.FileIO.ReadLinesAsync(file);
				foreach (var item in linksProductTXT)
				{
					UnRedactProductsQueue.Enqueue(item);
					count++;
				}
			}
			List<Product> allProducts = new List<Product>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				//var productsArchive = db.GetCollection<Product>("ProductsArchive");
				//var productsWait = db.GetCollection<Product>("ProductsWait");
				allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				//allProducts.AddRange(productsArchive.Query().ToList());
				//allProducts.AddRange(productsWait.Query().ToList());
			}
			List<string> productNuYandexNull = new List<string>();
			List<string> productToAdd = new List<string>();
			List<string> PriceToUpdate = new List<string>();
			foreach (var item in UnRedactProductsQueue)
			{
				Product product = allProducts.Find(x => x.ArticleNumberUnicList.Contains(item));
				if (product != null)
				{
					if (product.RemainsWhite == 0 || (product.TypeOfShop == "Леонардо" && product.RemainsWhite <= 1))
                    {
						productNuYandexNull.Add(item);
					}
					else
                    {
						
						List<ArticleNumber> articleNumbers = product.ArticleNumberProductId.First().Value;

                        foreach (var articleNumber in articleNumbers)
                        {
							if (string.Compare(articleNumber.OurArticle, item) == 0)
                            {
								double newPrice = 0;
								if (double.TryParse(articleNumber.productInfoFromOzon.result.price.Replace(".",","), out newPrice))
                                {
									productToAdd.Add(item);
									PriceToUpdate.Add((newPrice).ToString());
								}
								else
                                {
									PriceToUpdate.Add("0");
									productNuYandexNull.Add(item);
								}
							}
						}
					}
				}
				else
					productNuYandexNull.Add(item);
			}


			string allToDell = "";
			foreach (var item in productNuYandexNull)
			{
				allToDell += item + "\n";
			}
			string allToAdd = "";
			foreach (var item in productToAdd)
			{
				allToAdd += item + "\n";
			}
			string allToUpdatePrice = "";
			foreach (var item in PriceToUpdate)
			{
				allToUpdatePrice += item + "\n";
			}
			DateTime timeDateNow = DateTime.Now;
			string dateTimeStr = timeDateNow.ToString().Replace(":", ".");

			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();

			//if (fileWithLinks != null)
            //{
				await fileWithLinks.CreateFileAsync("Удаление из яндекса от " + dateTimeStr + ".txt", CreationCollisionOption.ReplaceExisting);
				StorageFile myFile = await fileWithLinks.GetFileAsync("Удаление из яндекса от " + dateTimeStr + ".txt");
				await FileIO.WriteTextAsync(myFile, allToDell);

				await fileWithLinks.CreateFileAsync("Правильные остатки яндекса от " + dateTimeStr + ".txt", CreationCollisionOption.ReplaceExisting);
				myFile = await fileWithLinks.GetFileAsync("Правильные остатки яндекса от " + dateTimeStr + ".txt");
				await FileIO.WriteTextAsync(myFile, allToAdd);

				await fileWithLinks.CreateFileAsync("Правильные цены яндекса от " + dateTimeStr + ".txt", CreationCollisionOption.ReplaceExisting);
				myFile = await fileWithLinks.GetFileAsync("Правильные цены яндекса от " + dateTimeStr + ".txt");
				await FileIO.WriteTextAsync(myFile, allToUpdatePrice);
			//}

			UnRedactProductsQueue = new ConcurrentQueue<string>();
			allToDell = "";
			allToAdd = "";
			allToUpdatePrice = "";
		}
	}
}
