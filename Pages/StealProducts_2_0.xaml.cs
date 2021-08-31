using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>


	public sealed partial class StealProducts_2_0 : Page
	{
		static ConcurrentDictionary<string, string> specificationsDict = new ConcurrentDictionary<string, string>();

		static ConcurrentQueue<string> UnRedactLinksQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> LinksQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> HtmlQueue = new ConcurrentQueue<string>();

		static int trueLinksCount = 0;

		public StealProducts_2_0()
		{
			this.InitializeComponent();
		}

		private void addSpecificationsDict()
		{
			if (!specificationsDict.ContainsKey("Наименование"))
				specificationsDict.TryAdd("Наименование", null);
			if (!specificationsDict.ContainsKey("Артикул"))
				specificationsDict.TryAdd("Артикул", null);
			if (!specificationsDict.ContainsKey("Цена"))
				specificationsDict.TryAdd("Цена", null);
			if (!specificationsDict.ContainsKey("Ссылка"))
				specificationsDict.TryAdd("Ссылка", null);
			if (!specificationsDict.ContainsKey("Главные фото"))
				specificationsDict.TryAdd("Главные фото", null);
			if (!specificationsDict.ContainsKey("Доп фото"))
				specificationsDict.TryAdd("Доп фото", null);
			if (!specificationsDict.ContainsKey("Описание"))
				specificationsDict.TryAdd("Описание", null);
			if (!specificationsDict.ContainsKey("Вес расчётный (в граммах)"))
				specificationsDict.TryAdd("Вес расчётный (в граммах)", null);
			if (!specificationsDict.ContainsKey("Ширина в мм"))
				specificationsDict.TryAdd("Ширина в мм", null);
			if (!specificationsDict.ContainsKey("Длина в мм"))
				specificationsDict.TryAdd("Длина в мм", null);
			if (!specificationsDict.ContainsKey("Высота в мм"))
				specificationsDict.TryAdd("Высота в мм", null);
			if (!specificationsDict.ContainsKey("Глубина в мм"))
				specificationsDict.TryAdd("Глубина в мм", null);
			if (!specificationsDict.ContainsKey("Неудачные ссылки или онлайн"))
				specificationsDict.TryAdd("Неудачные ссылки или онлайн", null);
			if (!specificationsDict.ContainsKey("ШтрихКод"))
				specificationsDict.TryAdd("ШтрихКод", null);
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


		private static void CreateToastProductJob()
		{
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
			toast.Data.Values["progressValueString"] = "0/0 товаров";
			toast.Data.Values["progressStatus"] = "Работаем...";

			// Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
			toast.Data.SequenceNumber = 0;

			// Show the toast notification to the user
			ToastNotificationManager.CreateToastNotifier().Show(toast);
		}
		public void UpdateProgress(double kolvo, double apply, string status)
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
			data.Values["progressStatus"] = status;

			// Update the existing notification's data by using tag/group
			ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
		}

		public List<int> GetProductCount(string countLocaionCode, out bool isKolvo)
		{
			List<int> count = new List<int>();
			isKolvo = true;

			Regex regexCount = new Regex(@"stock=""(\w+)""");
			MatchCollection matchesCount = regexCount.Matches(countLocaionCode);
			// Кол-во
			if (matchesCount.Count > 0)
			{
				foreach (Match match in matchesCount)
				{
					string[] digits = Regex.Split(match.Value, @"\D+");

					int number;
					if (int.TryParse(digits[1], out number))
					{
						count.Add(number);
					}
				}
			}
			else
				isKolvo = false;

			return count;
		}

		public List<int> GetProductLocation(string countLocaionCode, out bool isLocation)
		{
			List<int> location = new List<int>();

			Regex regexLocation = new Regex(@"store-code=""(\w+)""");
			MatchCollection matchesLocation = regexLocation.Matches(countLocaionCode);
			isLocation = true;
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
							location.Add(number);
						}
					}

				}

			}
			else
			{
				isLocation = false;
			}
			return location;
		}

		public void createProductThread()
		{
			List<string> StandardNamesOfXaract = new List<string>();

			CreateToastProductJob(); // Показываем уведомление о работе.
			addSpecificationsDict(); // Вносим в словарь стандартные позиции.

			int addCountProduct = 0; // Сколько продуктов добавлено
			int kolInBlackList = 0; // Чёрный список
			int countToApply = 0; // Успешно
			int allCount = UnRedactLinksQueue.Count; // Всего продуктов

			Action action = () =>
			{
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
					UpdateProgress(Convert.ToDouble(allCount), Convert.ToDouble(HtmlQueue.Count), "Получаем код товаров...");
				}
			};

			Parallel.Invoke(action, action, action, action, action, action,
				action, action, action, action, action, action,
				action, action, action, action, action, action,
				action, action, action, action, action, action,
				action, action, action, action, action, action);

			UpdateProgress(Convert.ToDouble(0), Convert.ToDouble(0), "Обрабатываем полученную информацию...");
			while (HtmlQueue.Count != 0)
			{
				List<int> productCount = new List<int>(); // Кол-во
				List<int> productLocation = new List<int>(); // Место
				Product oneProduct = new Product(); // Текущий продукт
				string prdctLink = "";
				LinksQueue.TryDequeue(out prdctLink);

				oneProduct.ProductLink = prdctLink;

				string code = "";
				HtmlQueue.TryDequeue(out code);

				bool kolvoTru;
				bool locationTrue;

				string countLocaionCode = "";
				if (code.IndexOf("<uc-elbrus-pdp-stocks-list") != -1)
					countLocaionCode = code.Substring(code.IndexOf("<uc-elbrus-pdp-stocks-list"), code.IndexOf("</uc-elbrus-pdp-stocks-list") - code.IndexOf("<uc-elbrus-pdp-stocks-list"));

				productCount = new List<int>(GetProductCount(countLocaionCode, out kolvoTru));
				productLocation = new List<int>(GetProductLocation(countLocaionCode, out locationTrue));

				if (locationTrue && kolvoTru)
					foreach (var item in productLocation)
					{
						if (Global.whiteList.Contains(item) && productCount.ElementAt(productLocation.IndexOf(item)) > 3)
						{
							oneProduct.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));
						}
						else
						if (Global.blackList.Contains(item))
							oneProduct.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
						else
							oneProduct.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
					}
				else
				{
					oneProduct.RemainsBlack = 0;
					oneProduct.RemainsWhite = 0;
				}

				if (oneProduct.RemainsWhite >= 10)
				{
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

					double weight = 5000;
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
							specificationsDict.TryAdd(xaracterNameList[i], null);
							for (int k = 0; k < addCountProduct; k++)
							{
								specificationsDict[xaracterNameList[i]] += "\n";
							}
							specificationsDict[xaracterNameList[i]] += xaracterTextList[i] + "\n";
						}
						else
						{
							specificationsDict[xaracterNameList[i]] += xaracterTextList[i] + "\n";
						}
						if (xaracterNameList[i].StartsWith("Вес") || xaracterTextList[i].StartsWith("вес"))
						{
							massa = true;
							if (xaracterNameList[i].Contains("кг"))
							{
								weight = Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 1000;
								specificationsDict["Вес расчётный (в граммах)"] += weight.ToString() + "\n";
							}
							else
							if (xaracterNameList[i].Contains("г"))
							{
								weight = Convert.ToDouble(xaracterTextList[i].Replace(".", ","));
								specificationsDict["Вес расчётный (в граммах)"] += weight.ToString() + "\n";
							}
						}

						if (xaracterNameList[i].StartsWith("Ширина") || xaracterTextList[i].StartsWith("ширина"))
						{
							shirina = true;
							specificationsDict["Ширина в мм"] += (Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 10).ToString() + "\n";
						}

						if (xaracterNameList[i].StartsWith("Длина") || xaracterTextList[i].StartsWith("длина"))
						{
							dlina = true;
							specificationsDict["Длина в мм"] += (Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 10).ToString() + "\n";
						}

						if (xaracterNameList[i].StartsWith("Высота") || xaracterTextList[i].StartsWith("высота"))
						{
							visota = true;
							specificationsDict["Высота в мм"] += (Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 10).ToString() + "\n";
						}
						if (xaracterNameList[i].StartsWith("Глубина") || xaracterTextList[i].StartsWith("глубина"))
						{
							glybina = true;
							specificationsDict["Глубина в мм"] += (Convert.ToDouble(xaracterTextList[i].Replace(".", ",")) * 10).ToString() + "\n";
						}
					}

					if (!massa)
					{
						specificationsDict["Вес расчётный (в граммах)"] += "\n";
					}
					if (!shirina)
					{
						specificationsDict["Ширина в мм"] += "\n";
					}
					if (!glybina)
					{
						specificationsDict["Глубина в мм"] += "\n";
					}
					if (!visota)
					{
						specificationsDict["Высота в мм"] += "\n";
					}
					if (!dlina)
					{
						specificationsDict["Длина в мм"] += "\n";
					}
					foreach (string item in StandardNamesOfXaract)
					{
						if (!xaracterNameList.Contains(item))
						{
							specificationsDict[item] += "\n";
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

					Regex regexImg = new Regex(@"srcset=""([^""]+)""");
					MatchCollection matchesImg = regexImg.Matches(code);

					int kolImg = 0;

					List<string> Allimg = new List<string>();
					foreach (var item in matchesImg)
					{
						string srlItem = item.ToString().Replace("srcset=", "").Replace(@"""", "");
						if (srlItem.Contains("2000"))
						{
							kolImg++;
							Allimg.Add(srlItem);
						}
					}
					if (kolImg == 0)
						Allimg.Add(matchesImg[0].ToString());

					string[] words = code.Split(new string[] { "<uc-store-stock", "</uc-store-stock>" }, StringSplitOptions.RemoveEmptyEntries);

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
					// Вносим в переменные 
					if (namesAndArticleId[3].Contains("»"))
						namesAndArticleId[3].Replace("»", "");
					if (namesAndArticleId[3].Contains("«"))
						namesAndArticleId[3].Replace("«", "");
					specificationsDict["Наименование"] += namesAndArticleId[3] + "\n";
					specificationsDict["Артикул"] += namesAndArticleId[1] + "\n";

					oneProduct.NowPrice = Convert.ToDouble(namesAndArticleId[5].Replace('.', ','));

					double newPrice = 0;
					if (oneProduct.NowPrice < 400)
						newPrice = (oneProduct.NowPrice + 45 + weight / 1000 * 20 + 50 + 50) * 1.075 * 1.1 * 1.35;
					else
						newPrice = (oneProduct.NowPrice + 45 + 20 * weight / 1000) * 1.075 * 1.044 * 1.1 * 1.35;
					int nowPrice = Convert.ToInt32(newPrice) / 10 * 10;
					specificationsDict["Цена"] += nowPrice.ToString() + "\n";
					specificationsDict["Ссылка"] += oneProduct.ProductLink + "\n";
					specificationsDict["Главные фото"] += Allimg[0] + "\n";
					specificationsDict["Описание"] += Opisanie + "\n";
					DateTime date = DateTime.Now;
					string dateStr = date.ToString("dd.MM.yyyy") + date.ToString("hh:mm:ss:ffff");
					dateStr = dateStr.Replace(".", "").Replace(":", "").Replace(" ", "");
					specificationsDict["ШтрихКод"] += dateStr + "\n";
					if (Allimg.Count > 1)
						for (int i = 1; i < Allimg.Count; i++)
						{
							specificationsDict["Доп фото"] += Allimg[i] + " ";
						}
					specificationsDict["Доп фото"] += "\n";
					addCountProduct++;
				}
				else
					kolInBlackList++;
				countToApply++;
				UpdateProgress(Convert.ToDouble(allCount), Convert.ToDouble(allCount - HtmlQueue.Count), "Обрабатываем данные");

			}
		}


		

	}
}
