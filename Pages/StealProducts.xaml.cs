using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class StealProducts : Page
	{
		Dictionary<string, string> specificationsDict = new Dictionary<string, string>();

		static Queue<string> UnRedactLinksQueue = new Queue<string>();
		static Queue<string> LinksQueue = new Queue<string>();
		static Queue<string> HtmlQueue = new Queue<string>();

		List<string> fullLinks = new List<string>();
		int count;
		static int trueLinksCount = 0;
		static List<string> LinksCodelList = new List<string>();
		static List<string> CodeHtmlList = new List<string>();
		static object locker = new object();

		public void GetCodeByLink(object lnk)
		{
			if (!String.IsNullOrEmpty(lnk.ToString()))
			{
				string Code = getResponse(lnk.ToString());
				lock (locker)
				{
					if (!LinksQueue.Contains(lnk.ToString()))
					{
						LinksQueue.Enqueue(lnk.ToString());
						HtmlQueue.Enqueue(Code);
					}

				}
			}
		}

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
			proxy_request.UserAgent = Global.userAgent[Global.countOfUserAgent];
			proxy_request.KeepAlive = false;
			proxy_request.Proxy = new WebProxy(Global.proxyIp[Global.countproxyIp], Global.proxyPort[Global.countproxyPort]);
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
				if (!isByll)
					UnRedactLinksQueue.Enqueue(uri);
			}
			Global.countOfUserAgent++;
			if (Global.countOfUserAgent > Global.userAgent.Length - 1)
				Global.countOfUserAgent = 0;
			Global.countproxyIp++;
			Global.countproxyPort++;
			if (Global.countproxyIp > Global.proxyIp.Length - 1)
			{
				Global.countproxyIp = 0;
				Global.countproxyPort = 0;
			}
			return htmlCode;
		}

		public void createProductThread()
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
			toast.Data.Values["progressStatus"] = "Крадём...";

			// Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
			toast.Data.SequenceNumber = 0;

			// Show the toast notification to the user
			ToastNotificationManager.CreateToastNotifier().Show(toast);

			int onlyOnline = 0;

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

			int testCount = UnRedactLinksQueue.Count;
			while (testCount != HtmlQueue.Count && UnRedactLinksQueue.Count != 0)
			{
				Thread thread1 = new Thread(new ParameterizedThreadStart(GetCodeByLink));
				if (UnRedactLinksQueue.Count > 0)
				{
					string str = UnRedactLinksQueue.Dequeue();
					if (!String.IsNullOrEmpty(str))
						thread1.Start(str);
				}
			}
			while (HtmlQueue.Count != 0)
			{
				List<int> productCount = new List<int>(); // Кол-во
				List<int> productLocation = new List<int>(); // Место
				Product onePos = new Product();
				onePos.ProductLink = LinksQueue.Dequeue();
				string code = HtmlQueue.Dequeue();
				//onePos.ProductLink = UnRedactLinksQueue.Dequeue();
				//string code = getResponse(onePos.ProductLink);
				bool kolvoTru = true;
				Regex regexCount = new Regex(@"stock=""(\w+)""");
				Regex regexLocation = new Regex(@"<span>Леруа Мерлен \w+");
				string countLocaionCode = "";
				try
				{
					countLocaionCode = code.Substring(code.IndexOf("<uc-elbrus-pdp-stocks-list"), code.IndexOf("</uc-elbrus-pdp-stocks-list") - code.IndexOf("<uc-elbrus-pdp-stocks-list"));
				}
				catch (Exception)
				{
					//Result.Text = code;
				}

				MatchCollection matchesCount = regexCount.Matches(countLocaionCode);
				MatchCollection matchesLocation = regexLocation.Matches(countLocaionCode);

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
					kolvoTru = false;
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
					onlyOnline++;
					specificationsDict["Неудачные ссылки или онлайн"] += onePos.ProductLink + "\n";
				}

				if (locationTrue && kolvoTru)
					foreach (var item in productLocation)
					{
						if (Global.whiteList.Contains(item))
							onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));
						else
						if (Global.blackList.Contains(item))
							onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
					}
				else
				{
					onePos.RemainsBlack = 0;
					onePos.RemainsWhite = 0;
				}
				if (onePos.RemainsWhite > 10)
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
					else
						OpisCode = "-----";

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
							specificationsDict.Add(xaracterNameList[i], null);
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
					else
						Opisanie = "---------";

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
					// Вносим в переменные 
					if (namesAndArticleId[3].Contains("»"))
						namesAndArticleId[3].Replace("»","");
					if (namesAndArticleId[3].Contains("«"))
						namesAndArticleId[3].Replace("«", "");
					specificationsDict["Наименование"] += namesAndArticleId[3] + "\n";
					specificationsDict["Артикул"] += namesAndArticleId[1] + "\n";

					onePos.NowPrice = Convert.ToDouble(namesAndArticleId[5].Replace('.', ','));

					double newPrice = 0;
					if (onePos.NowPrice < 400)
						newPrice = (onePos.NowPrice + 45 + weight / 1000 * 20 + 50 + 50) * 1.075 * 1.1 * 1.35;
					else
						newPrice = (onePos.NowPrice + 45 + 20 * weight / 1000) * 1.075 * 1.044 * 1.1 * 1.35;
					int nowPrice = Convert.ToInt32(newPrice) / 10 * 10;
					specificationsDict["Цена"] += nowPrice.ToString() + "\n";
					specificationsDict["Ссылка"] += onePos.ProductLink + "\n";
					specificationsDict["Главные фото"] += Allimg[0] + "\n";
					specificationsDict["Описание"] += Opisanie + "\n";

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
				UpdateProgress(Convert.ToDouble(allCount), Convert.ToDouble(allCount - HtmlQueue.Count));
			}
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
			UnRedactLinksQueue = new Queue<string>(fullLinks);
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
				string code = getResponse(UnRedactLinksQueue.Dequeue());
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
							if (!item.ToString().Replace(@"""", "").StartsWith("/shop/") && !item.ToString().Replace(@"""", "").StartsWith("/catalogue/") && !item.ToString().Replace(@"""", "").StartsWith("/advice/"))
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
						data = "--------";
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

			Thread getLinks = new Thread(getLinksTreadPerexod);
			getLinks.Start();
		}

		public StealProducts()
		{
			this.InitializeComponent();
		}

		private void StealProducts_Click(object sender, RoutedEventArgs e)
		{
			GetLinks();
		}
	}
}
