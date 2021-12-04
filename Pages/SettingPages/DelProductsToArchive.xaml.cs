using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

namespace Остатки.Pages.SettingPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	/// 

	public class ProductsIdsss
	{
		[JsonProperty("product_id")]
		public List<long> product_id { get; set; }
	}

	public sealed partial class DelProductsToArchive : Page
	{

		public static bool accaunt104333 = false;
		public static bool accaunt200744 = false;

		int count = 0;
		static ConcurrentQueue<string> UnRedactProductsQueue = new ConcurrentQueue<string>();

		public DelProductsToArchive()
		{
			this.InitializeComponent();
		}
		private void getAccauntIDs()
		{
			accaunt104333 = btn104333.IsChecked.Value;
			accaunt200744 = btn200744.IsChecked.Value;
		}

		private async void GetIDsAndGoToArchive()
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
			GoArchiveOOO();
		}

        private void GoArchiveOOO()
        {
            List<Product> allProducts = new List<Product>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
            }
			List<Dictionary<ApiKeys, ProductsIdsss>> productsIdsssLIST = new List<Dictionary<ApiKeys, ProductsIdsss>>();
			List<ApiKeys> keys = ApiKeysesJob.GetAllApiList();
			int countOfZapros104333 = 0;
			int countOfZapros200744 = 0;
			if (accaunt104333 && accaunt200744)
			{
				productsIdsssLIST.Add(new Dictionary<ApiKeys, ProductsIdsss>());
				productsIdsssLIST[countOfZapros104333].Add(keys.Find(x => x.ClientId == "104333"), new ProductsIdsss());
				productsIdsssLIST[countOfZapros200744].Add(keys.Find(x => x.ClientId == "200744"), new ProductsIdsss());
				foreach (var item in productsIdsssLIST[countOfZapros200744].Keys)
				{
					productsIdsssLIST[countOfZapros200744][item].product_id = new List<long>();
					foreach (var ID in UnRedactProductsQueue)
					{
						long ch = 0;
						long.TryParse(ID, out ch);
						productsIdsssLIST[countOfZapros200744][item].product_id.Add(ch);
						if (productsIdsssLIST[countOfZapros200744][item].product_id.Count >= 499)
						{
							productsIdsssLIST.Add(new Dictionary<ApiKeys, ProductsIdsss>());
							countOfZapros104333++;
							countOfZapros200744++;
							productsIdsssLIST[countOfZapros104333].Add(keys.Find(x => x.ClientId == "104333"), new ProductsIdsss());
							productsIdsssLIST[countOfZapros200744].Add(keys.Find(x => x.ClientId == "200744"), new ProductsIdsss());
							productsIdsssLIST[countOfZapros200744][item].product_id = new List<long>();
						}
					}
				}
			}
			else
			if (accaunt104333)
			{
				productsIdsssLIST.Add(new Dictionary<ApiKeys, ProductsIdsss>());
				productsIdsssLIST[countOfZapros104333].Add(keys.Find(x => x.ClientId == "104333"), new ProductsIdsss());
				foreach (var item in productsIdsssLIST[countOfZapros104333].Keys)
				{
					productsIdsssLIST[countOfZapros200744][item].product_id = new List<long>();
					foreach (var ID in UnRedactProductsQueue)
					{
						long ch = 0;
						long.TryParse(ID, out ch);
						productsIdsssLIST[countOfZapros104333][item].product_id.Add(ch);
						if (productsIdsssLIST[countOfZapros104333][item].product_id.Count >= 499)
						{
							productsIdsssLIST.Add(new Dictionary<ApiKeys, ProductsIdsss>());
							countOfZapros104333++;
							productsIdsssLIST[countOfZapros104333].Add(keys.Find(x => x.ClientId == "104333"), new ProductsIdsss());
							productsIdsssLIST[countOfZapros104333][item].product_id = new List<long>();
						}
					}
				}
			}
			else
			if (accaunt200744)
			{
				productsIdsssLIST.Add(new Dictionary<ApiKeys, ProductsIdsss>());
				productsIdsssLIST[countOfZapros200744].Add(keys.Find(x => x.ClientId == "200744"), new ProductsIdsss());
				foreach (var item in productsIdsssLIST[countOfZapros200744].Keys)
				{
					productsIdsssLIST[countOfZapros200744][item].product_id = new List<long>();
					foreach (var ID in UnRedactProductsQueue)
					{
						long ch = 0;
						long.TryParse(ID, out ch);
						productsIdsssLIST[countOfZapros200744][item].product_id.Add(ch);
						if (productsIdsssLIST[countOfZapros200744][item].product_id.Count >= 499)
						{
							productsIdsssLIST.Add(new Dictionary<ApiKeys, ProductsIdsss>());
							countOfZapros200744++;
							productsIdsssLIST[countOfZapros200744].Add(keys.Find(x => x.ClientId == "200744"), new ProductsIdsss());
							productsIdsssLIST[countOfZapros200744][item].product_id = new List<long>();
						}
					}
				}
			}
			foreach (var item in productsIdsssLIST)
            {
				Thread.Sleep(2500);
				foreach (var oneZapros in item)
				{
					Susseess susseess = PostRequestAsync(oneZapros.Key, oneZapros.Value);
				}
            }
        }
		private static Susseess PostRequestAsync(ApiKeys key, ProductsIdsss pageOzon)
		{
			var jsort = JsonConvert.SerializeObject(pageOzon);
			var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/archive");
			httpWebRequest.Headers.Add("Client-Id", key.ClientId);
			httpWebRequest.Headers.Add("Api-Key", key.ApiKey);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";

			using (var requestStream = httpWebRequest.GetRequestStream())
			using (var writer = new StreamWriter(requestStream))
			{
				writer.Write(jsort);
			}
			HttpWebResponse httpResponse = new HttpWebResponse();
			try
			{
				httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					//ответ от сервера
					var result = streamReader.ReadToEnd();
					//Сериализация
					return JsonConvert.DeserializeObject<Susseess>(result);
				}
			}
			catch (WebException ex)
			{
				HttpWebResponse exeps = (HttpWebResponse)ex.Response;
				if (exeps.StatusCode == HttpStatusCode.BadRequest)
					return new Susseess() { message = exeps.StatusDescription, result = true };
				else
					return new Susseess() { message = exeps.StatusDescription, result = false };
			}


		}
		private void GoToArchiveFromFile_Click(object sender, RoutedEventArgs e)
		{
			getAccauntIDs();
			if (accaunt104333 || accaunt200744)
				GetIDsAndGoToArchive();
		}
	}
}
