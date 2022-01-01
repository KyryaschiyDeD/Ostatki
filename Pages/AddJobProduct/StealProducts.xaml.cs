using LiteDB;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{


	public class Result
	{
		public List<ItemProsuctOfferIDs> items { get; set; }
		public int total { get; set; }
	}

	public class Root
	{
		public Result result { get; set; }
	}
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class StealProducts : Page
	{
		Dictionary<string, string> specificationsDict = new Dictionary<string, string>();

		static ConcurrentQueue<string> UnRedactLinksQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> LinksQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> HtmlQueue = new ConcurrentQueue<string>();

		public static List<ShopWhiteOrBlack> shopsWhiteOrBlackLeroy = ShopWhiteOrBlackJob.GetShopListSpecifically("Леруа Мерлен");

		static List<Product> AllProduct = new List<Product>();

		static bool CheckedProductInDataBase = false;
		public static bool productCountComplect1 = false;
		public static bool productCountComplect2 = false;
		public static bool productCountComplect3 = false;
		public static bool productCountComplect5 = false;
		public static bool productCountComplect10 = false;


		List<string> fullLinks = new List<string>();
		int count;
		static int trueLinksCount = 0;

		static object locker = new object();

		StorageFolder fileWithLinks;
		public void UpdateProgress(double kolvo, double apply)
		{
			// Construct a NotificationData object;
			string tag = "Product";
			string group = "Lerya";

			// Create NotificationData and make sure the sequence number is incremented
			/* since last update, or assign 0 for updating regardless of order*/
			var data = new NotificationData
			{
				SequenceNumber = 0
			};

			// Assign new values
			// Note that you only need to assign values that changed. In this example
			// we don't assign progressStatus since we don't need to change it
			data.Values["myProgressValue"] = (apply / kolvo).ToString().Replace(",", ".");
			data.Values["progressValueString"] = $"{apply}/{kolvo} товаров";

			// Update the existing notification's data by using tag/group
			ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
		}
		public void UpdateProgresslnk(double kolvo, double apply)
		{
			// Construct a NotificationData object;
			string tag = "ProductLinksCode";
			string group = "Lerya";

			// Create NotificationData and make sure the sequence number is incremented
			/* since last update, or assign 0 for updating regardless of order*/
			var data = new NotificationData
			{
				SequenceNumber = 0
			};

			// Assign new values
			// Note that you only need to assign values that changed. In this example
			// we don't assign progressStatus since we don't need to change it
			data.Values["myProgressValue"] = (apply / kolvo).ToString().Replace(",", ".");
			data.Values["progressValueString"] = $"{apply}/{kolvo} товаров";

			// Update the existing notification's data by using tag/group
			ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
		}
		public void UpdateProgressLinks(string kolvo, string apply, string fullssilm, string progr)
		{
			// Construct a NotificationData object;
			string tag = "Tovar";
			string group = "Lerya";

			// Create NotificationData and make sure the sequence number is incremented
			/* since last update, or assign 0 for updating regardless of order*/
			var data = new NotificationData
			{
				SequenceNumber = 0
			};

			// Assign new values
			// Note that you only need to assign values that changed. In this example
			// we don't assign progressStatus since we don't need to change it
			data.Values["myProgressValue"] = progr;
			data.Values["progressValueString"] = $"{apply}/{fullssilm} ссылок";
			data.Values["progressStatus"] = $"{kolvo} товаров";

			// Update the existing notification's data by using tag/group
			ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
		}

		public static string RemoveInvalidChars(string file_name)
		{
			foreach (Char invalid_char in Path.GetInvalidFileNameChars())
			{
				file_name = file_name.Replace(oldValue: invalid_char.ToString(), newValue: "");
			}
			return file_name;
		}

		public static string getResponse(string uri)
		{
			string htmlCode = "";
			HttpWebRequest proxy_request = (HttpWebRequest)WebRequest.Create(uri);
			proxy_request.Method = "GET";
			proxy_request.ContentType = "application/x-www-form-urlencoded";
			proxy_request.UserAgent = HTMLJob.userAgent[HTMLJob.CountOfUserAgent];
			proxy_request.KeepAlive = false;
			proxy_request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			
			HttpWebResponse resp = null;
			string html = "";
			bool isByll = false;
			try
			{
				resp = proxy_request.GetResponse() as HttpWebResponse;
				trueLinksCount++;
				if (resp != null)
					using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
						html = sr.ReadToEnd();
				htmlCode = html.Trim();
				if (String.IsNullOrEmpty(htmlCode) || htmlCode.Contains("blocked"))
				{
					isByll = true;
					UnRedactLinksQueue.Enqueue(uri);
				}
			}
			catch (Exception)
			{
				if (isByll || resp == null)
					UnRedactLinksQueue.Enqueue(uri);
			}
			if (isByll && !UnRedactLinksQueue.Contains(uri))
				UnRedactLinksQueue.Enqueue(uri);
			if (htmlCode.Length == 0 && !UnRedactLinksQueue.Contains(uri))
				UnRedactLinksQueue.Enqueue(uri);
			HTMLJob.CountOfUserAgent++;
			HTMLJob.CountproxyIp++;
			HTMLJob.CountproxyPort++;

			return htmlCode;
		}

		public void createProductThread()
		{
			string tag1 = "ProductLinksCode";
			string group1 = "Lerya";

			// Construct the toast content with data bound fields
			var content1 = new ToastContentBuilder()
				.AddText("ВОРУЕМ!!!")
				.AddVisualChild(new AdaptiveProgressBar()
				{
					Title = "Товар",
					Value = new BindableProgressBarValue("myProgressValue"),
					ValueStringOverride = new BindableString("progressValueString"),
					Status = new BindableString("progressStatus")
				})
				.GetToastContent();

			// Generate the toast notification
			var toast1 = new ToastNotification(content1.GetXml());

			// Assign the tag and group
			toast1.Tag = tag1;
			toast1.Group = group1;

			// Assign initial NotificationData values
			// Values must be of type string
			toast1.Data = new NotificationData();
			toast1.Data.Values["progressValue"] = "0";
			toast1.Data.Values["progressValueString"] = "0/0 товаров";
			toast1.Data.Values["progressStatus"] = "Получаем данные...";

			// Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
			toast1.Data.SequenceNumber = 0;

			// Show the toast notification to the user
			ToastNotificationManager.CreateToastNotifier().Show(toast1);

			int addCountProduct = 0;
			int kolInBlackList = 0;
			int countToApply = 0;
			int allCount = UnRedactLinksQueue.Count;

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
			if (!specificationsDict.ContainsKey("Начальная цена"))
				specificationsDict.Add("Начальная цена", null);
			if (!specificationsDict.ContainsKey("Цена в 0"))
				specificationsDict.Add("Цена в 0", null);
			Random rnd = new Random();
			Action action = () =>
			{
				//Thread.Sleep(rnd.Next(2500, 5500));
				while (!UnRedactLinksQueue.IsEmpty && HtmlQueue.Count != allCount)
				{
					string lnk = "";
					UnRedactLinksQueue.TryDequeue(out lnk);
					if (lnk != null)
					{
						string Code = getResponse(lnk);
						if (Code.Length != 0)
							if (!LinksQueue.Contains(lnk))
							{
								LinksQueue.Enqueue(lnk);
								HtmlQueue.Enqueue(Code);
							}
					}
					UpdateProgresslnk(Convert.ToDouble(allCount), Convert.ToDouble(HtmlQueue.Count));
				}
			};

			Parallel.Invoke(action, action, action, action, action);

			string tag = "Product";
			string group = "Lerya";

			// Construct the toast content with data bound fields
			var content = new ToastContentBuilder()
				.AddText("ВОРУЕМ!!!")
				.AddVisualChild(new AdaptiveProgressBar()
				{
					Title = "Товар",
					Value = new BindableProgressBarValue("myProgressValue"),
					ValueStringOverride = new BindableString("progressValueString"),
					Status = new BindableString("progressStatus")
				})
				.GetToastContent();

			// Generate the toast notification
			var toast = new ToastNotification(content.GetXml());

			// Assign the tag and group
			toast.Tag = tag;
			toast.Group = group;

			// Assign initial NotificationData values
			// Values must be of type string
			toast.Data = new NotificationData();
			toast.Data.Values["progressValue"] = "0";
			toast.Data.Values["progressValueString"] = "HtmlQueue = " + HtmlQueue.Count.ToString();
			toast.Data.Values["progressStatus"] = "Крадём...";

			// Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
			toast.Data.SequenceNumber = 0;

			// Show the toast notification to the user
			ToastNotificationManager.CreateToastNotifier().Show(toast);


			ConcurrentBag<Product> productListBackground = new ConcurrentBag<Product>();

			while (HtmlQueue.Count != 0)
			{
				List<int> productCount = new List<int>(); // Кол-во
				List<int> productLocation = new List<int>(); // Место
				Product onePos = new Product();
				string prdctLink = "";
				LinksQueue.TryDequeue(out prdctLink);

				onePos.ProductLink = prdctLink;
				string code = "";
				HtmlQueue.TryDequeue(out code);
				//onePos.ProductLink = UnRedactLinksQueue.Dequeue();
				//string code = getResponse(onePos.ProductLink);
				bool kolvoTru = true;
				Regex regexCount = new Regex(@"stock=""[\w\.]+""");
				Regex regexLocation = new Regex(@"store-code=""(\w+)""");
				string countLocaionCode = "";
				if (code.IndexOf("<uc-elbrus-pdp-stocks-list") != -1)
					countLocaionCode = code.Substring(code.IndexOf("<uc-elbrus-pdp-stocks-list"), code.IndexOf("</uc-elbrus-pdp-stocks-list") - code.IndexOf("<uc-elbrus-pdp-stocks-list"));

				bool IsProductInDatabase = false;
				if (CheckedProductInDataBase)
				{
					int select = AllProduct.FindIndex(x => x.ProductLink == prdctLink);
					if (select == -1)
						IsProductInDatabase = true;
				}
				if (!IsProductInDatabase && CheckedProductInDataBase || !CheckedProductInDataBase)
				{
					MatchCollection matchesCount = regexCount.Matches(countLocaionCode);
					MatchCollection matchesLocation = regexLocation.Matches(countLocaionCode);

					// Кол-во
					if (matchesCount.Count > 0)
					{
						foreach (Match match in matchesCount)
						{
							string[] digits = Regex.Split(match.Value, @"\D+");

							
							if (digits[1].Contains("."))
							{
								double number;
								if (double.TryParse(digits[1], out number))
								{
									productCount.Add((int)number);
								}
							}
							else
							{
								int number;
								if (int.TryParse(digits[1], out number))
								{
									productCount.Add(number);
								}
							}
						}
					}
					else
					{
						kolvoTru = false;
						specificationsDict["Неудачные ссылки или онлайн"] += onePos.ProductLink + "\n";
					}

					bool locationTrue = true;
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
							//string[] s2 = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
							//productLocation.Add(match.Value);
						}

					}
					else
					{
						locationTrue = false;
					}
					int countOfWhoiteList = 0;
					Dictionary<int, int> remainsDictionary = new Dictionary<int, int>();
					if (locationTrue && kolvoTru)
					{
						foreach (var item in productLocation)
						{
							if (Global.whiteListLeroy.Contains(item) && productCount.ElementAt(productLocation.IndexOf(item)) >= 10)
							{
								onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));
								if (shopsWhiteOrBlackLeroy.Find(x => x.Code == item).ShopIsOnly && productCount.ElementAt(productLocation.IndexOf(item)) >= 15)
									countOfWhoiteList += 3;
								countOfWhoiteList++;
							}
							else
							if (Global.blackListLeroy.Contains(item))
								onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
							else
								onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
							remainsDictionary.Add(item, productCount.ElementAt(productLocation.IndexOf(item)));
						}
						onePos.remainsDictionary = remainsDictionary;
						onePos.DateHistoryRemains.Add(DateTime.Now);
						if (countOfWhoiteList < 3)
						{
							onePos.RemainsBlack += onePos.RemainsWhite;
							onePos.RemainsWhite = 0;
						}
					}
					else
					{
						onePos.RemainsBlack = 0;
						onePos.RemainsWhite = 0;
					}
					if (onePos.RemainsWhite >= 15)
					{

						List<int> countOfComplect = new List<int>();
						if (StealProducts.productCountComplect1)
							countOfComplect.Add(1);

						if (onePos.NowPrice <= 500)
						{
							if (StealProducts.productCountComplect2)
								countOfComplect.Add(2);
							if (StealProducts.productCountComplect3)
								countOfComplect.Add(3);
							if (StealProducts.productCountComplect5)
								countOfComplect.Add(5);
							if (StealProducts.productCountComplect10)
								countOfComplect.Add(10);
						}
						else
						if (onePos.NowPrice <= 1000)
						{
							if (StealProducts.productCountComplect2)
								countOfComplect.Add(2);
							if (StealProducts.productCountComplect3)
								countOfComplect.Add(3);
							if (StealProducts.productCountComplect5)
								countOfComplect.Add(5);
						}
						else
						if (onePos.NowPrice <= 2000)
						{
							if (StealProducts.productCountComplect2)
								countOfComplect.Add(2);
							if (StealProducts.productCountComplect3)
								countOfComplect.Add(3);
						}
						else
						if (onePos.NowPrice <= 3000)
						{
							if (StealProducts.productCountComplect2)
								countOfComplect.Add(2);
						}



						double weight = 5000;

						// Получаем строку с наименованием, артикулом и ценой
						Regex regexArticleNumber = new Regex(@"<div data-rel="".*?"" class="".*?"" data-ga-root data-path="".*?"" data-product-is-available="".*?"" data-product-id="".*?"" data-product-name="".*?"" data-product-price="".*?""");


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

						// Получаем чисто артикул, наименование и цену
						regexArticleNumber = new Regex(@""".*?""");
						matchesArticleNumberName = regexArticleNumber.Matches(finishNameArticleId);


						// Артикл и имя
						string resultNameArticleId = "";
						if (matchesArticleNumberName.Count > 0)
						{
							foreach (Match match in matchesArticleNumberName)
							{
								resultNameArticleId += match;
							}
						}

						string[] namesAndArticleId = resultNameArticleId.Split('"');


						if (namesAndArticleId[3].Contains("»"))
							namesAndArticleId[3].Replace("»", "");
						if (namesAndArticleId[3].Contains("«"))
							namesAndArticleId[3].Replace("«", "");


						onePos.NowPrice = Convert.ToDouble(namesAndArticleId[5].Replace('.', ','));

						/*double newPrice = 0;
						if (onePos.NowPrice < 400)
							newPrice = (onePos.NowPrice + 45 + weight / 1000 * 20 + 50 + 50) * 1.075 * 1.1 * 1.35;
						else
							newPrice = (onePos.NowPrice + 45 + 20 * weight / 1000) * 1.075 * 1.044 * 1.1 * 1.35;
						int nowPrice = Convert.ToInt32(newPrice) / 10 * 10; */



						int indexOfStartOpis = code.IndexOf("<h2>Описание<");
						int indexOfEndOpis = code.IndexOf("</uc-pdp-section-vlimited>");

						bool noOpis = false;

						if (indexOfStartOpis < 0)
							noOpis = true;
						string OpisCode = "";
						if (!noOpis)
						{
							OpisCode = code.Substring(indexOfStartOpis, indexOfEndOpis - indexOfStartOpis);
						}
						else
							OpisCode = "";

						int indexOfStartImg = code.IndexOf(@"id=""picture-box-id-generated-0""");
						int indexOfEndImg = code.IndexOf(@".jpg"">");

						string XaractCode = code.Substring(code.IndexOf(@"<dl class=""def-list"">"), code.IndexOf("</dl>") - code.IndexOf(@"<dl class=""def-list"">"));

						Regex regexXaracterName = new Regex(@"<dt class=""def-list__term"">(.)*>");
						MatchCollection xaracterName = regexXaracterName.Matches(XaractCode);

						Regex regexXaracterText = new Regex(@"<dd class=""def-list__definition"">([^""]+)<\/dd>");
						MatchCollection xaracterText = regexXaracterText.Matches(XaractCode);

						List<string> xaracterNameList = new List<string>();

						List<string> xaracterTextList = new List<string>();

						for (int i = 0; i < xaracterName.Count(); i++)
						{
							int start1 = xaracterName[i].ToString().IndexOf(@"<dt class=""def-list__term"">");
							int start2 = xaracterText[i].ToString().IndexOf(@"<dd class=""def-list__definition"">");
							int end1 = xaracterName[i].ToString().IndexOf("</dt>");
							int end2 = xaracterText[i].ToString().IndexOf("</dd>");
							xaracterNameList.Add(xaracterName[i].ToString().Replace(@"<dt class=""def-list__term"">", "").Replace("</dt>", ""));
							xaracterTextList.Add(xaracterText[i].ToString().Replace(@"<dd class=""def-list__definition"">", "").Replace("</dd>", "").Replace("\n", ""));
						}
						bool massa = false;
						bool shirina = false;
						bool glybina = false;
						bool visota = false;
						bool dlina = false;
						for (int i = 0; i < xaracterNameList.Count; i++)
						{
							xaracterNameList[i] = xaracterNameList[i].Replace("  ", "");
							xaracterNameList[i].Trim();
							xaracterTextList[i] = xaracterTextList[i].Replace("  ", "");
							xaracterTextList[i].Trim();

							if (!StandardNamesOfXaract.Contains(xaracterNameList[i]))
							{
								StandardNamesOfXaract.Add(xaracterNameList[i]);
								specificationsDict.Add(xaracterNameList[i], null);
								for (int k = 0; k < addCountProduct; k++)
								{
									for (int j = 0; j < countOfComplect.Count; j++)
									{
										specificationsDict[xaracterNameList[i]] += "\n";
									}
								}
								//specificationsDict[xaracterNameList[i]] += xaracterTextList[i] + "\n";
							}
							for (int j = 0; j < countOfComplect.Count; j++)
							{
								specificationsDict[xaracterNameList[i]] += xaracterTextList[i] + "\n";
							}
							if (xaracterNameList[i].StartsWith("Вес") || xaracterTextList[i].StartsWith("вес"))
							{
								massa = true;
								if (xaracterNameList[i].Contains("кг"))
								{
									weight = Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 1000;
									for (int j = 0; j < countOfComplect.Count; j++)
									{
										specificationsDict["Вес расчётный (в граммах)"] += weight.ToString() + "\n";
									}
								}
								else
								if (xaracterNameList[i].Contains("г"))
								{
									weight = Convert.ToDouble(xaracterTextList[i].Replace(".", ","));
									for (int j = 0; j < countOfComplect.Count; j++)
									{
										specificationsDict["Вес расчётный (в граммах)"] += weight.ToString() + "\n";
									}
								}
							}
							onePos.Weight = weight;
							double tmpDouble;
							if ((xaracterNameList[i].StartsWith("Ширина") || xaracterTextList[i].StartsWith("ширина")) && Double.TryParse(xaracterTextList[i].Replace(".", ","), out tmpDouble))
							{
								shirina = true;
								for (int j = 0; j < countOfComplect.Count; j++)
								{
									specificationsDict["Ширина в мм"] += (tmpDouble * 10).ToString() + "\n";
								}
							}
							if ((xaracterNameList[i].StartsWith("Длина") || xaracterTextList[i].StartsWith("длина")) && Double.TryParse(xaracterTextList[i].Replace(".", ","), out tmpDouble))
							{
								dlina = true;
								for (int j = 0; j < countOfComplect.Count; j++)
								{
									specificationsDict["Длина в мм"] += (tmpDouble * 10).ToString() + "\n";
								}
							}
							if ((xaracterNameList[i].StartsWith("Высота") || xaracterTextList[i].StartsWith("высота")) && Double.TryParse(xaracterTextList[i].Replace(".", ","), out tmpDouble))
							{
								visota = true;
								for (int j = 0; j < countOfComplect.Count; j++)
								{
									specificationsDict["Высота в мм"] += (tmpDouble * 10).ToString() + "\n";
								}
							}
							if ((xaracterNameList[i].StartsWith("Глубина") || xaracterTextList[i].StartsWith("глубина")) && Double.TryParse(xaracterTextList[i].Replace(".", ","), out tmpDouble))
							{
								glybina = true;
								for (int j = 0; j < countOfComplect.Count; j++)
								{
									specificationsDict["Глубина в мм"] += (tmpDouble * 10).ToString() + "\n";
								}
							}
						}

						if (!massa)
						{
							for (int j = 0; j < countOfComplect.Count; j++)
							{
								specificationsDict["Вес расчётный (в граммах)"] += "\n";
							}
						}
						if (!shirina)
						{
							for (int j = 0; j < countOfComplect.Count; j++)
							{
								specificationsDict["Ширина в мм"] += "\n";
							}
						}
						if (!glybina)
						{
							for (int j = 0; j < countOfComplect.Count; j++)
							{
								specificationsDict["Глубина в мм"] += "\n";
							}
						}
						if (!visota)
						{
							for (int j = 0; j < countOfComplect.Count; j++)
							{
								specificationsDict["Высота в мм"] += "\n";
							}
						}
						if (!dlina)
						{
							for (int j = 0; j < countOfComplect.Count; j++)
							{
								specificationsDict["Длина в мм"] += "\n";
							}
						}
						foreach (string item in StandardNamesOfXaract)
						{
							if (!xaracterNameList.Contains(item))
							{
								for (int j = 0; j < countOfComplect.Count; j++)
								{
									specificationsDict[item] += "\n";
								}
							}
						}
						string Opisanie = "";

						if (!noOpis)
						{
							Opisanie = HTMLJob.UnHtml(OpisCode);
							if (Opisanie.Contains("Описание"))
								Opisanie = Opisanie.Replace("Описание", "");
							if (Opisanie.Contains("СКАЧАТЬ ИНСТРУКЦИЮ"))
								Opisanie = Opisanie.Replace("СКАЧАТЬ ИНСТРУКЦИЮ", "");
							if (Opisanie.Contains("Скачать документы сертификации"))
								Opisanie = Opisanie.Replace("Скачать документы сертификации", "");
							if (Opisanie.Contains("СКАЧАТЬ ПРАВИЛА ЭКСПЛУАТАЦИИ"))
								Opisanie = Opisanie.Replace("СКАЧАТЬ ПРАВИЛА ЭКСПЛУАТАЦИИ", "");
							Opisanie = Opisanie.Trim();
						}
						else
							Opisanie = "";

						Regex regexImg = new Regex(@"srcset=""([^""]+)""");
						MatchCollection matchesImg = regexImg.Matches(code);
						int kol = 0;
						List<string> Allimg = new List<string>();
						foreach (var item in matchesImg)
						{
							string srlItem = item.ToString().Replace("srcset=", "").Replace(@"""", "");
							if (srlItem.Contains("2000"))
							{
								kol++;
								Allimg.Add(srlItem);
							}
						}
						if (kol == 0)
							Allimg.Add(matchesImg[0].ToString());

						string[] words = code.Split(new string[] { "<uc-store-stock", "</uc-store-stock>" }, StringSplitOptions.RemoveEmptyEntries);

						DateTime date = DateTime.Now;
						string dateStr = date.ToString("dd.MM.yyyy") + date.ToString("hh:mm:ss:ff");
						dateStr = dateStr.Replace(".", "").Replace(":", "").Replace(" ", "");

						onePos.ArticleNumberInShop = namesAndArticleId[1];
						onePos.TypeOfShop = "LeroyMerlen";
						onePos.Name = namesAndArticleId[3];
						foreach (var item in countOfComplect)
						{
							onePos.ArticleNumberUnicList.Add("lm-" + namesAndArticleId[1] + "-" + "x" + item.ToString());
							specificationsDict["Артикул"] += "lm-" + namesAndArticleId[1] + "-" + "x" + item.ToString() + "\n";
							specificationsDict["Объединить на одной карточке"] += namesAndArticleId[1] + "_one_cart\n";
						}
						specificationsDict["Ссылка One"] += onePos.ProductLink + "\n";
						foreach (var item in countOfComplect)
						{
							specificationsDict["Наименование"] += namesAndArticleId[3] + "\n";
							specificationsDict["КолВо завод в отправл"] += item.ToString() + "\n";
							int newPrice = (int)(Convert.ToInt32(Convert.ToDouble(onePos.NowPrice) * item + 45 + onePos.Weight / 1000 * 20 + 50) * 1.075 * 1.1 * 1.25 * 1.1 + 5) / 10 * 10;
							int priceToNull = (int)(Convert.ToInt32(Convert.ToDouble(onePos.NowPrice) * item + 45 + onePos.Weight / 1000 * 20 + 50) * 1.075 * 1.1 * 1.1 + 5) / 10 * 10;
							specificationsDict["Цена"] += newPrice.ToString() + "\n";
							specificationsDict["Начальная цена"] += onePos.NowPrice + "\n";
							specificationsDict["Цена в 0"] += priceToNull + "\n";
							specificationsDict["Цена + 40%"] += (newPrice * 1.4).ToString() + "\n";
							specificationsDict["Ссылка"] += onePos.ProductLink + "\n";
							specificationsDict["Главные фото"] += Allimg[0] + "\n";
							specificationsDict["Г + Д через ,"] += Allimg[0] + ",";
							specificationsDict["Описание"] += Opisanie + "\n";
							specificationsDict["ШтрихКод"] += dateStr + item.ToString() + "\n";
							if (Allimg.Count > 1)
								for (int i = 1; i < Allimg.Count; i++)
								{
									specificationsDict["Доп фото"] += Allimg[i] + " ";
									specificationsDict["Г + Д через ,"] += Allimg[i] + ",";
								}
							specificationsDict["Доп фото"] += "\n";
							specificationsDict["Г + Д через ,"] += "\n";
						}
						addCountProduct++;
						productListBackground.Add(onePos);
					}
					else
						kolInBlackList++;
					countToApply++;
					UpdateProgress(Convert.ToDouble(allCount), Convert.ToDouble(allCount - HtmlQueue.Count));
				}
			}
			DataBaseJob.AddListToBackground(productListBackground.ToList());
		}

		private async Task FormatLinks()
		{
			for (int i = 0; i < fullLinks.Count; i++)
			{
				fullLinks[i] = "https://leroymerlin.ru" + fullLinks[i];
			}
			if (fileWithLinks != null)
			{
				string allLinksFormat = "";
				foreach (var item in fullLinks)
				{
					allLinksFormat += item + "\n";
				}
				await fileWithLinks.CreateFileAsync("Форматированные ссылки.txt", CreationCollisionOption.ReplaceExisting);
				StorageFile myFile = await fileWithLinks.GetFileAsync("Форматированные ссылки.txt");
				string data = allLinksFormat;
				await FileIO.WriteTextAsync(myFile, data);
			}
			UnRedactLinksQueue = new ConcurrentQueue<string>(fullLinks);
			fullLinks.Clear();
		}

		public void getLinksThread()
		{
			string tag = "Tovar";
			string group = "Lerya";

			var content = new ToastContentBuilder()
				.AddText("Воруем ссылки на товар")
				.AddVisualChild(new AdaptiveProgressBar()
				{
					Title = "Товар",
					Value = new BindableProgressBarValue("myProgressValue"),
					ValueStringOverride = new BindableString("progressValueString"),
					Status = new BindableString("progressStatus")
				})
				.GetToastContent();
			var toast = new ToastNotification(content.GetXml());

			toast.Tag = tag;
			toast.Group = group;

			toast.Data = new NotificationData();
			toast.Data.Values["progressValue"] = "0";
			toast.Data.Values["progressValueString"] = "0 ссылок";
			toast.Data.Values["progressStatus"] = "0 товаров";
			toast.Data.SequenceNumber = 0;

			ToastNotificationManager.CreateToastNotifier().Show(toast);
			trueLinksCount = 0;
			int allLinksCount = UnRedactLinksQueue.Count;
			while (UnRedactLinksQueue.Count != 0 && trueLinksCount != allLinksCount)
			{
				//Thread.Sleep(3000);
				string lnk = "";
				UnRedactLinksQueue.TryDequeue(out lnk);
				string code = getResponse(lnk);
				//Thread.Sleep(3000);
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
					if (!fullLinks.Contains(item.ToString().Replace(@"""", "")))
						if (!item.ToString().Contains("http"))
							if (!item.ToString().Replace(@"""", "").StartsWith("/shop/") && !item.ToString().Replace(@"""", "").StartsWith("/catalogue/") && !item.ToString().Replace(@"""", "").StartsWith("/advice/") && !item.ToString().Replace(@"""", "").StartsWith("/offer/"))
								fullLinks.Add(item.ToString().Replace(@"""", ""));
				}
				UpdateProgressLinks(fullLinks.Count.ToString(), (count - UnRedactLinksQueue.Count).ToString(), count.ToString(), ((double)fullLinks.Count / ((double)count)).ToString().Replace(",", "."));
			}

		}

		public async void getLinksTreadPerexod()
		{
			Thread getLinks = new Thread(getLinksThread);
			getLinks.Start();
			getLinks.Join();
			if (fileWithLinks != null)
			{
				string allLinks = "";
				foreach (var item in fullLinks)
				{
					allLinks += item + "\n";
				}
				await fileWithLinks.CreateFileAsync("Все ссылки категории.txt", CreationCollisionOption.ReplaceExisting);
				StorageFile myFile = await fileWithLinks.GetFileAsync("Все ссылки категории.txt");
				string data = allLinks;
				await FileIO.WriteTextAsync(myFile, data);
			}
			await FormatLinks();
			createProductThread();
			if (fileWithLinks != null)
			{
				await fileWithLinks.CreateFolderAsync("X", CreationCollisionOption.ReplaceExisting);
				StorageFolder SaveFolder = await fileWithLinks.GetFolderAsync("X");
				foreach (var item in specificationsDict)
				{
					await SaveFolder.CreateFileAsync(RemoveInvalidChars(item.Key) + ".txt", CreationCollisionOption.ReplaceExisting);
					StorageFile myFile = await SaveFolder.GetFileAsync(RemoveInvalidChars(item.Key) + ".txt");
					string data = item.Value;
					if (String.IsNullOrEmpty(data))
						data = "--------------";
					await FileIO.WriteTextAsync(myFile, data);
				}
			}
			specificationsDict.Clear();
		}

		private async void GetLinks()
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
					UnRedactLinksQueue.Enqueue(item);
					count++;
				}
			}

			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			fileWithLinks = await folderPicker.PickSingleFolderAsync();
			CheckedProductInDataBase = TrueProductsDatabase.IsChecked.Value;
			Thread getLinks = new Thread(getLinksTreadPerexod);
			getLinks.Start();
		}

		public StealProducts()
		{
			this.InitializeComponent();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				AllProduct = new List<Product>(allProducts);
			}
		}

		private static Root PostRequestAsync(int pageOzon, string clientId, string apiKey)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/list");
			httpWebRequest.Headers.Add("Client-Id", clientId);
			httpWebRequest.Headers.Add("Api-Key", apiKey);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			string json = @"{
  ""filter"": {

	""visibility"": ""ALL""
  },
  ""page"": " + $"{pageOzon}" + @",
  ""page_size"": 1000
}";
			using (var requestStream = httpWebRequest.GetRequestStream())
			using (var writer = new StreamWriter(requestStream))
			{
				writer.Write(json);
			}
			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				//ответ от сервера
				var result = streamReader.ReadToEnd();

				//Сериализация
				return JsonConvert.DeserializeObject<Root>(result);
			}
		}

		static List<ItemProsuctOfferIDs> AllErrorsProduct = new List<ItemProsuctOfferIDs>();
		static List<Product> allProducts = new List<Product>();
		static List<Product> allProductsUpdate = new List<Product>();

		public void getComplect()
		{
			productCountComplect1 = ProductCountComplect1.IsChecked.Value;
			productCountComplect2 = ProductCountComplect2.IsChecked.Value;
			productCountComplect3 = ProductCountComplect3.IsChecked.Value;
			productCountComplect5 = ProductCountComplect5.IsChecked.Value;
			productCountComplect10 = ProductCountComplect10.IsChecked.Value;
		}
		private void CheckingAndReconciliationOfArticles_Click(object sender, RoutedEventArgs e)
		{
			List<ItemProsuctOfferIDs> AllErrorsProduct = new List<ItemProsuctOfferIDs>();
			List<Product> allProducts = new List<Product>();
			List<Product> allProductsUpdate = new List<Product>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
			}
			foreach (var item in ApiKeysesJob.GetAllApiList())
			{
				for (int i = 1; i <= 19; i++)
				{
					List<ItemProsuctOfferIDs> items = PostRequestAsync(i, item.ClientId, item.ApiKey).result.items;
					foreach (var item1 in items)
					{
						Product OneProduct = null;
						try
						{
							OneProduct = allProducts.Single(itemDB => itemDB.ArticleNumberUnicList.Contains(item1.offer_id));
						}
						catch (Exception)
						{
							OneProduct = null;
						}
						if (OneProduct != null && !OneProduct.ArticleNumberOzonDictList.ContainsKey(item.ClientId))
						{
							OneProduct.ArticleNumberOzonDictList.Add(item.ClientId, new List<long>() { Convert.ToInt64(item1.product_id) });
							OneProduct.ArticleError = false;
							allProductsUpdate.Add(OneProduct);
						}
						else
						if (OneProduct != null && OneProduct.ArticleNumberOzonDictList.ContainsKey(item.ClientId))
						{
							if (!OneProduct.ArticleNumberOzonDictList[item.ClientId].Contains(item1.product_id))
								OneProduct.ArticleNumberOzonDictList[item.ClientId].Add(item1.product_id);
						}
						else
						{
							AllErrorsProduct.Add(item1);
						}
						//writeToTxtExperiense += $"Наш артикул: {item.offer_id}  Озон артикул: {item.product_id}\n";
					}
				}
			}
			DataBaseJob.UpdateList(allProductsUpdate);
			DataBaseJob.ErrorArticle(AllErrorsProduct);
			//StorageFile helloFile = await Global.folder.CreateFileAsync("hello.txt",
			//									CreationCollisionOption.ReplaceExisting);

			//await FileIO.WriteTextAsync(helloFile, writeToTxtExperiense);
		}
		private void StealProductLeroys_Click(object sender, RoutedEventArgs e)
		{
			getComplect();
			if (productCountComplect1 || productCountComplect2 || productCountComplect3 || productCountComplect5 || productCountComplect10)
				GetLinks();
		}
		private void StealProductLeonardos_Click(object sender, RoutedEventArgs e)
		{
			getComplect();
			if (productCountComplect1 || productCountComplect2 || productCountComplect3 || productCountComplect5 || productCountComplect10)
				LeonardoJobs.GetLinks(TrueProductsDatabase.IsChecked.Value);
		}
		public List<bool> GetComplectsList()
		{
			getComplect();
			List<bool> listComplect = new List<bool>();
			listComplect.Add(productCountComplect1);
			listComplect.Add(productCountComplect2);
			listComplect.Add(productCountComplect3);
			listComplect.Add(productCountComplect5);
			listComplect.Add(productCountComplect10);
			return listComplect;
		}
		private void StealProductPetrovich_Click(object sender, RoutedEventArgs e)
		{
			Global.complects = GetComplectsList();
			Frame.Navigate(typeof(PetrovichPage));
		}
	}
}
