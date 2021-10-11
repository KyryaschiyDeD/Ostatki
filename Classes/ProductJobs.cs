using LiteDB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Остатки.Classes;
using System.Threading;

namespace Остатки.Classes
{
	public class ItemProsuctOfferIDs
	{
		public int product_id { get; set; }
		public string offer_id { get; set; }
	}
	public class ErrorsArticle
	{
		public Guid Id { get; set; }
		public string OfferId { get; set; }
		public long ProductId { get; set; }
	}
	class ProductJobs
	{
		static object locker = new object();
		public static ConcurrentQueue<string> ocher = new ConcurrentQueue<string>();
		public static ConcurrentQueue<Product> NewRemaintProductLerya = new ConcurrentQueue<Product>();

		public static string getResponse(string uri)
		{
			string htmlCode = "";
			using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
			{
				client.Headers["User-Agent"] = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 YaBrowser/16.3.0.7146 Yowser/2.5 Safari/537.36";
				try
				{
					htmlCode = client.DownloadString(uri);
				}
				catch (Exception)
				{
					Message.errorsList.Add("Ошибка в распозновании ссылки");
				}
			}
			return htmlCode;
		}

		public static Dictionary<string, int> kolvoUpdatePopitka = new Dictionary<string, int>();
		public static List<ShopWhiteOrBlack> shopsWhiteOrBlack = ShopWhiteOrBlackJob.GetAllShopList();
		static void AddProxy(HttpWebRequest request)
		{
			var proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			request.Proxy = proxy;
		}

		private static string GetResponseUpdates(string uri)
		{
			Thread.Sleep(3000);
			string htmlCode = "";
			//driver.Navigate().GoToUrl(uri);
			//driver.Url = uri;
			HttpWebRequest proxy_request = (HttpWebRequest)WebRequest.Create(uri);
			//proxy_request.Method = "GET";
			proxy_request.ContentType = "application/x-www-form-urlencoded";
			//proxy_request.Headers.Add("Accept-Language: ru-ru");
			proxy_request.UserAgent = HTMLJob.userAgent[HTMLJob.CountOfUserAgent];
			proxy_request.KeepAlive = false;
			proxy_request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			proxy_request.Timeout = 20000;
			proxy_request.Referer = @"https://leroymerlin.ru/";
			proxy_request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
			proxy_request.Host = "leroymerlin.ru";
			proxy_request.Headers.Add("Bx-ajax", "true");
			var cookieContainer = new CookieContainer();
			proxy_request.CookieContainer = cookieContainer;
			HttpWebResponse resp = null;
			string html = "";
			try
			{
				resp = proxy_request.GetResponse() as HttpWebResponse;
				if (resp != null)
					using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
						html = sr.ReadToEnd();
				//htmlCode = driver.PageSource;
				htmlCode = html.Trim();
				//driver.Quit();
				if (String.IsNullOrEmpty(htmlCode) || htmlCode.Contains("blocked"))
				{
					if (kolvoUpdatePopitka.ContainsKey(uri))
					{
						kolvoUpdatePopitka[uri]++;
						if (!(kolvoUpdatePopitka[uri] > 3))
							ocher.Enqueue(uri);
					}
					else
					{
						kolvoUpdatePopitka.Add(uri,1);
						ocher.Enqueue(uri);
					}
						
				}
			}
			catch (Exception)
			{
				if (!ocher.Contains(uri))
				{
					if (kolvoUpdatePopitka.ContainsKey(uri))
					{
						kolvoUpdatePopitka[uri]++;
						if (!(kolvoUpdatePopitka[uri] > 3))
							ocher.Enqueue(uri);
					}
					else
					{
						kolvoUpdatePopitka.Add(uri, 1);
						ocher.Enqueue(uri);
					}
				}
			}
			if (htmlCode.Length == 0 && !ocher.Contains(uri))
			{
				if (kolvoUpdatePopitka.ContainsKey(uri))
				{
					kolvoUpdatePopitka[uri]++;
					if (!(kolvoUpdatePopitka[uri] > 3))
						ocher.Enqueue(uri);
				}
				else
				{
					kolvoUpdatePopitka.Add(uri, 1);
					ocher.Enqueue(uri);
				}
			}
			HTMLJob.CountOfUserAgent++;
			HTMLJob.CountproxyIp++;
			HTMLJob.CountproxyPort++;
			return htmlCode;
		}

		public static void parseLerya(object ink)
		{
			string link = ink.ToString();
			List<int> productCount = new List<int>(); // Кол-во
			List<int> productLocation = new List<int>(); // Место
			Product onePos = new Product();
			onePos.ProductLink = link;
			string code = getResponse(link);
			int indexOfStart = code.IndexOf("<uc-elbrus-pdp-stocks-list");
			int indexOfEnd = code.IndexOf("</uc-elbrus-pdp-stocks-list");
			string countLocaionCode = code.Substring(indexOfStart, indexOfEnd - indexOfStart);
			string[] words = code.Split(new string[] { "<uc-store-stock", "</uc-store-stock>" }, StringSplitOptions.RemoveEmptyEntries);

			// Получаем строку с наименованием, артикулом и ценой
			Regex regexArticleNumber = new Regex(@"<div data-rel="".*?"" class="".*?"" data-ga-root data-path="".*?"" data-product-is-available="".*?"" data-product-id="".*?"" data-product-name="".*?"" data-product-price="".*?""");
			Regex regexCount = new Regex(@"stock=""(\w+)""");
			Regex regexLocation = new Regex(@"store-code=""(\w+)""");

			string nextNameArticleId = "";
			MatchCollection matchesArticleNumberName = regexArticleNumber.Matches(code);
			if (matchesArticleNumberName.Count > 0)
			{
				foreach (Match match in matchesArticleNumberName)
				{
					nextNameArticleId += match;
				}
			}
			else
			{
				Message.errorsList.Add("Совпадений не найдено");
			}
			// Выделяем только наименование, артикул и цену
			regexArticleNumber = new Regex(@"data-product-id=""\w+"" data-product-name="".*?"" data-product-price="".*?""");
			matchesArticleNumberName = regexArticleNumber.Matches(nextNameArticleId);

			string finishNameArticleId = "";
			if (matchesArticleNumberName.Count > 0)
			{
				foreach (Match match in matchesArticleNumberName)
				{
					finishNameArticleId += match;
				}
			}
			else
			{
				Message.errorsList.Add("Наименование, цена и артикул не найдены!!!");
			}
			// Получаем чисто артикул, наименование и цену
			regexArticleNumber = new Regex(@""".*?""");
			matchesArticleNumberName = regexArticleNumber.Matches(finishNameArticleId);

			MatchCollection matchesCount = regexCount.Matches(countLocaionCode);
			MatchCollection matchesLocation = regexLocation.Matches(countLocaionCode);
			// Артикл и имя
			string resultNameArticleId = "";
			if (matchesArticleNumberName.Count > 0)
			{
				foreach (Match match in matchesArticleNumberName)
				{
					resultNameArticleId += match;
				}
			}
			else
			{
				Message.errorsList.Add("Наименование, цена и артикул не найдены!!!");
			}
			string[] namesAndArticleId = resultNameArticleId.Split('"');
			// Вносим в переменные 
			try
			{
				onePos.Name = namesAndArticleId[3];
				onePos.NowPrice = Convert.ToDouble(namesAndArticleId[5].Replace('.', ','));
				onePos.ArticleNumberInShop = namesAndArticleId[1];
			}
			catch (Exception)
			{
				Message.errorsList.Add("Наименование, цена и артикул в неправильном формате!!!");
			}

			// Кол-во
			if (matchesCount.Count > 0)
			{
				foreach (Match match in matchesCount)
				{
					string[] digits = Regex.Split(match.Value, @"\D+");
					foreach (string value in digits)
					{
						int number = 0;
						if (int.TryParse(value, out number))
						{
							productCount.Add(number);
						}
					}
				}

			}
			else
			{
				Message.errorsList.Add("Не удалось найти и загрузить остатки!!!");
			}
			// Место
			if (matchesLocation.Count > 0)
			{
				foreach (Match match in matchesLocation)
				{
					string[] digits = Regex.Split(match.Value, @"\D+");
					foreach (string value in digits)
					{
						int number;
						if (int.TryParse(value, out number))
						{
							productLocation.Add(number);
						}
					}
				}

			}
			onePos.TypeOfShop = "LeroyMerlen";
			foreach (var item in productLocation)
			{
				if (Global.whiteListLeroy.Contains(item))

					onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));


				else
				if (Global.blackListLeroy.Contains(item))

					onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));

			}
			lock (locker)
			{
				DataBaseJob.AddNewProduct(onePos);
			}
		}
		public static void parseLeryaUpdate(object ink)
		{
			if (ink != null)
			{
				string link = ink.ToString();
				List<int> productCount = new List<int>(); // Кол-во
				List<int> productLocation = new List<int>(); // Место
				Product onePos = new Product();
				onePos.ProductLink = link;
				onePos.RemainsWhite = 0;
				onePos.RemainsBlack = 0;
				onePos.DateHistoryRemains.Add(DateTime.Now);
				string code = GetResponseUpdates(link);
				int indexOfStart = code.IndexOf("<uc-elbrus-pdp-stocks-list");
				int indexOfEnd = code.IndexOf("</uc-elbrus-pdp-stocks-list");
				if (indexOfStart >= 0)
				{
					string countLocaionCode = code.Substring(indexOfStart, indexOfEnd - indexOfStart);

					Regex regexArticleNumber = new Regex(@"<div data-rel="".*?"" class="".*?"" data-ga-root data-path="".*?"" data-product-is-available="".*?"" data-product-id="".*?"" data-product-name="".*?"" data-product-price="".*?""");
					Regex regexCount = new Regex(@"stock=""(\w+)""");
					Regex regexLocation = new Regex(@"store-code=""(\w+)""");

					string nextNameArticleId = "";
					MatchCollection matchesArticleNumberName = regexArticleNumber.Matches(code);
					if (matchesArticleNumberName.Count > 0)
					{
						foreach (Match match in matchesArticleNumberName)
						{
							nextNameArticleId += match;
						}
					}
					//else
					//{
					//	Message.errorsList.Add("Совпадений не найдено");
					//}
					// Выделяем только наименование, артикул и цену
					regexArticleNumber = new Regex(@"data-product-id=""\w+"" data-product-name="".*?"" data-product-price="".*?""");
					matchesArticleNumberName = regexArticleNumber.Matches(nextNameArticleId);

					string finishNameArticleId = "";
					if (matchesArticleNumberName.Count > 0)
					{
						foreach (Match match in matchesArticleNumberName)
						{
							finishNameArticleId += match;
						}
					}
					else
					{
						Message.errorsList.Add("Наименование, цена и артикул не найдены!!!");
					}
					// Получаем чисто артикул, наименование и цену
					regexArticleNumber = new Regex(@""".*?""");
					matchesArticleNumberName = regexArticleNumber.Matches(finishNameArticleId);

					MatchCollection matchesCount = regexCount.Matches(countLocaionCode);
					MatchCollection matchesLocation = regexLocation.Matches(countLocaionCode);
					// Артикл и имя
					string resultNameArticleId = "";
					if (matchesArticleNumberName.Count > 0)
					{
						foreach (Match match in matchesArticleNumberName)
						{
							resultNameArticleId += match;
						}
					}
					//else
					//{
					//	Message.errorsList.Add("Наименование, цена и артикул не найдены!!!");
					//}
					string[] namesAndArticleId = resultNameArticleId.Split('"');
					// Вносим в переменные 
					try
					{
						onePos.Name = namesAndArticleId[3];
						onePos.NowPrice = Convert.ToDouble(namesAndArticleId[5].Replace('.', ','));
						onePos.ArticleNumberInShop = namesAndArticleId[1];
					}
					catch (Exception)
					{
						Message.errorsList.Add("Наименование, цена и артикул в неправильном формате!!!");
					}

					// Кол-во
					if (matchesCount.Count > 0)
					{
						foreach (Match match in matchesCount)
						{
							string[] digits = Regex.Split(match.Value, @"\D+");
							foreach (string value in digits)
							{
								int number;
								if (int.TryParse(value, out number))
								{
									productCount.Add(number);
								}
							}
						}

					}
					else
					{
						Message.errorsList.Add("Не удалось найти и загрузить остатки!!!");
					}
					// Место
					if (matchesLocation.Count > 0)
					{
						foreach (Match match in matchesLocation)
						{
							string[] digits = Regex.Split(match.Value, @"\D+");
							foreach (string value in digits)
							{
								int number;
								if (int.TryParse(value, out number))
								{
									productLocation.Add(number);
								}
							}
						}

					}
					else
					{
						Message.errorsList.Add("Не удалось найти и загрузить местоположение!!!");
					}
					int countOfWhoiteList = 0;
					Dictionary<int, int> remainsDictionary = new Dictionary<int, int>();
					foreach (var item in productLocation)
					{
						if (Global.whiteListLeroy.Contains(item) && productCount.ElementAt(productLocation.IndexOf(item)) > 5)
						{
							onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));
							if (shopsWhiteOrBlack.Find(x => x.Code == item).ShopIsOnly && productCount.ElementAt(productLocation.IndexOf(item)) >= 10)
								countOfWhoiteList += 3;
							countOfWhoiteList ++;
						}
						else
						if (Global.blackListLeroy.Contains(item))
							onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
						else
							onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
						remainsDictionary.Add(item, productCount.ElementAt(productLocation.IndexOf(item)));
					}
					if (countOfWhoiteList < 3)
					{
						onePos.RemainsBlack += onePos.RemainsWhite;
						onePos.RemainsWhite = 0;
					}
					string XaractCode = code.Substring(code.IndexOf(@"<dl class=""def-list"">"), code.IndexOf("</dl>") - code.IndexOf(@"<dl class=""def-list"">"));

					Regex regexXaracterName = new Regex(@"<dt class=""def-list__term"">(.)*>");
					MatchCollection xaracterName = regexXaracterName.Matches(XaractCode);

					Regex regexXaracterText = new Regex(@"<dd class=""def-list__definition"">([^""]+)<\/dd>");
					MatchCollection xaracterText = regexXaracterText.Matches(XaractCode);

					List<string> xaracterNameList = new List<string>();

					List<string> xaracterTextList = new List<string>();
					bool massa = false;
					for (int i = 0; i < xaracterName.Count(); i++)
					{
						int start1 = xaracterName[i].ToString().IndexOf(@"<dt class=""def-list__term"">");
						int start2 = xaracterText[i].ToString().IndexOf(@"<dd class=""def-list__definition"">");
						int end1 = xaracterName[i].ToString().IndexOf("</dt>");
						int end2 = xaracterText[i].ToString().IndexOf("</dd>");
						xaracterNameList.Add(xaracterName[i].ToString().Replace(@"<dt class=""def-list__term"">", "").Replace("</dt>", ""));
						xaracterTextList.Add(xaracterText[i].ToString().Replace(@"<dd class=""def-list__definition"">", "").Replace("</dd>", "").Replace("\n", ""));
					}

					for (int i = 0; i < xaracterNameList.Count; i++)
					{
						if (xaracterNameList[i].StartsWith("Вес") || xaracterTextList[i].StartsWith("вес"))
						{
							massa = true;
							if (xaracterNameList[i].Contains("кг"))
							{
								onePos.Weight = Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 1000;
							}
							else
							if (xaracterNameList[i].Contains("г"))
							{
								onePos.Weight = Convert.ToDouble(xaracterTextList[i].Replace(".", ","));
							}
							break;
						}
					}
					if (!massa)
						onePos.Weight = 5000;
					onePos.remainsDictionary = remainsDictionary;
					NewRemaintProductLerya.Enqueue(onePos);
				}
				else
				{
					if (kolvoUpdatePopitka.ContainsKey(ink.ToString()))
					{
						if (kolvoUpdatePopitka[ink.ToString()] < 10)
						{
							ProductJobs.ocher.Enqueue(ink.ToString());
							kolvoUpdatePopitka[ink.ToString()]++;
						}
					}
					else
					{
						kolvoUpdatePopitka.Add(ink.ToString(), 1);
						ProductJobs.ocher.Enqueue(ink.ToString());
					}
				}
			}
		}

		public static void UpdateOneProduct()
		{
			if (NewRemaintProductLerya.Count != 0)
			{
				DataBaseJob.SaveNewRemains(NewRemaintProductLerya);
			}
		}
		public static List<Product> productList = new List<Product>();
		public static void AddNewApiClientID(string clientId, string link)
		{

			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var products = db.GetCollection<Product>("Products");
				var proverk = products.FindOne(x => x.ProductLink == link);
				if (proverk == null)
				{
					var productsArchive = db.GetCollection<Product>("ProductsArchive");
					proverk = productsArchive.FindOne(x => x.ProductLink == link);
					if (proverk == null)
					{
						var productsWait = db.GetCollection<Product>("ProductsWait");
						proverk = productsWait.FindOne(x => x.ProductLink == link);
						if (proverk != null)
						{
							if (!proverk.AccauntOzonID.ContainsKey(clientId))
								proverk.AccauntOzonID.Add(clientId, true);
							productsWait.Update(proverk);
						}
					}
					else
					{
						if (!proverk.AccauntOzonID.ContainsKey(clientId))
							proverk.AccauntOzonID.Add(clientId, true);
						productsArchive.Update(proverk);
					}

				}
				else
				{
					if (!proverk.AccauntOzonID.ContainsKey(clientId))
						proverk.AccauntOzonID.Add(clientId, true);
					products.Update(proverk);
				}
			}

		}

		public static string GetProductLink(Product pf)
		{
			return pf.ProductLink;
		}

	}
}
