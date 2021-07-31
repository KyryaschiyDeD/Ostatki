using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using System.IO;
using System.Web;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>

	public sealed partial class spizdiliRemains : Page
	{
		static string[] whiteList = { "Алтуфьево", "Мытищи", "Зеленоград", "ЗИЛ", "Люберцы", "Каширское",
			"Варшавское", "Лефортово", "Рязанский", "Косино", "Сокольники", "Красногорск", "Химки" };
		static string[] blackList = { "Пушкино", "Троицк", "Интернет-магазин МСК", "Истра", "Климовск", "Киевское",
			"Домодедово", "Шолохово", "Жуковский", "Новая", "Юдино" };

		static string[] userAgent = {
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 11_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36",
		"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 11.4; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (X11; Linux i686; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (Linux x86_64; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (X11; Ubuntu; Linux i686; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:89.0) Gecko/20100101 Firefox/89.0",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 11_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1 Safari/605.1.15",
		"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0)",
		"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; WOW64; Trident/4.0;)",
		"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)",
		"Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.0)",
		"Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1)",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)",
		"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2)",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 YaBrowser/16.3.0.7146 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 YaBrowser/16.3.0.7843 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 YaBrowser/16.4.1.8950 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 YaBrowser/16.6.1.30165 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 YaBrowser/16.7.0.3342 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 YaBrowser/16.7.1.20936 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 YaBrowser/16.7.1.20937 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 YaBrowser/16.9.1.1192 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 YaBrowser/16.9.1.863 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 YaBrowser/16.10.0.2564 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 YaBrowser/16.10.1.1052 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 YaBrowser/16.10.1.1114 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 YaBrowser/16.10.1.1116 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 YaBrowser/16.11.0.2680 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 YaBrowser/16.11.1.673 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 YaBrowser/16.11.1.676 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.0.1974 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.0.2033 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.0.2034 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.0.2036 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.1.1003 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.1.1004 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 YaBrowser/17.1.1.1005 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.68 YaBrowser/17.3.0.1196 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 YaBrowser/16.3.0.7146 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 YaBrowser/16.3.0.7843 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 YaBrowser/16.4.1.8950 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 YaBrowser/16.6.1.30165 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 YaBrowser/16.7.0.3342 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 YaBrowser/16.7.1.20936 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 YaBrowser/16.7.1.20937 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 YaBrowser/16.9.1.1192 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 YaBrowser/16.9.1.863 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 YaBrowser/16.10.0.2564 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 YaBrowser/16.10.1.1052 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 YaBrowser/16.10.1.1114 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 YaBrowser/16.11.0.2680 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 YaBrowser/16.10.1.1116 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 YaBrowser/16.11.1.673 Yowser/2.5 Safari/537.36",
		"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.100 YaBrowser/16.11.1.676 Yowser/2.5 Safari/537.36"
		};

		static string[] proxyIp = {
		"95.165.4.178",
		"77.236.230.177",
		"89.178.199.118",
		"37.1.25.69",
		"82.114.119.81",
		"178.159.40.19",
		"109.232.106.236",
		};
		static int[] proxyPort = {
		8080,
		1256,
		8080,
		53281,
		8080,
		8080,
		49565,
		};

		static int countOfUserAgent = 0;
		static int countproxyIp = 0;
		static int countproxyPort = 0;

		StorageFile file;
		StorageFolder folderSave;
		StorageFolder fileWithLinks;

		Dictionary<string, string> specificationsDict = new Dictionary<string, string>();
		List<string> LinksProductStart = new List<string>();
		List<string> fullLinks = new List<string>();
		static Queue<string> ocheredOutLinks = new Queue<string>();
		static Queue<string> ocheredOutProduct = new Queue<string>();

		public static string getResponse(string uri)
		{

			string htmlCode = "";
			HttpWebRequest proxy_request = (HttpWebRequest)WebRequest.Create(uri);
			proxy_request.Method = "GET";
			proxy_request.ContentType = "application/x-www-form-urlencoded";
			proxy_request.UserAgent = userAgent[countOfUserAgent];
			proxy_request.KeepAlive = false;
			proxy_request.Proxy = new WebProxy(proxyIp[countproxyIp], proxyPort[countproxyPort]);
			HttpWebResponse resp = null;
			//Thread.Sleep(200);
			try
			{
				resp = proxy_request.GetResponse() as HttpWebResponse;
			}
			catch (Exception)
			{
				//Thread.Sleep(200);
				ocheredOutLinks.Enqueue(uri);
			}

			string html = "";
			if (resp != null)
				using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
					html = sr.ReadToEnd();
			htmlCode = html.Trim();
			countOfUserAgent++;
			if (countOfUserAgent > userAgent.Length - 1)
				countOfUserAgent = 0;
			countproxyIp++;
			countproxyPort++;
			if (countproxyIp > proxyIp.Length - 1)
			{
				countproxyIp = 0;
				countproxyPort = 0;
			}
			/*using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
			 {
				 client.Headers["User-Agent"] = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 YaBrowser/16.3.0.7146 Yowser/2.5 Safari/537.36";
				 int value = 403;
				 while (value == 403 || value == 404)
				 {
					 try
					 {
						 htmlCode = client.DownloadString(uri);
					 }
					 catch (Exception)
					 {
						 value = 403;
						 Thread.Sleep(50);
					 }

					 int.TryParse(string.Join("", htmlCode.Where(c => char.IsDigit(c))), out value);
				 }
			 } */
			return htmlCode;
		}
		public spizdiliRemains()
		  {
			  this.InitializeComponent();
		  }

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

		private static readonly Regex _tags_ = new Regex(@"<[^>]+?>", RegexOptions.Multiline | RegexOptions.Compiled);

		//add characters that are should not be removed to this regex
		private static readonly Regex _notOkCharacter_ = new Regex(@"[^\w;&#@.:/\?=|%!() -]", RegexOptions.Compiled);

		public static String UnHtml(String html)
		{
			html = HttpUtility.UrlDecode(html);
			html = HttpUtility.HtmlDecode(html);

			html = RemoveTag(html, "<!--", "-->");
			html = RemoveTag(html, "<script", "</script>");
			html = RemoveTag(html, "<style", "</style>");

			//replace matches of these regexes with space
			html = _tags_.Replace(html, " ");
			html = _notOkCharacter_.Replace(html, " ");
			html = SingleSpacedTrim(html);

			return html;
		}

		private static String RemoveTag(String html, String startTag, String endTag)
		{
			Boolean bAgain;
			do
			{
				bAgain = false;
				Int32 startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
				if (startTagPos < 0)
					continue;
				Int32 endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
				if (endTagPos <= startTagPos)
					continue;
				html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
				bAgain = true;
			} while (bAgain);
			return html;
		}

		private static String SingleSpacedTrim(String inString)
		{
			StringBuilder sb = new StringBuilder();
			Boolean inBlanks = false;
			foreach (Char c in inString)
			{
				switch (c)
				{
					case '\r':
					case '\n':
					case '\t':
					case ' ':
						if (!inBlanks)
						{
							inBlanks = true;
							sb.Append(' ');
						}
						continue;
					default:
						inBlanks = false;
						sb.Append(c);
						break;
				}
			}
			return sb.ToString().Trim();
		}

		public void createProductThread(object LinksFromFile)
		{
			string tag = "Product";
			string group = "Lerya";

			// Construct the toast content with data bound fields
			var content = new ToastContentBuilder()
				.AddText("Крадём... Или крадёмся...")
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

			List<string> Links = (List<string>)LinksFromFile;
			int onlyOnline = 0;

			int addCountProduct = 0;
			int kolInBlackList = 0;
			int countToApply = 0;

			List<string> StandardNamesOfXaract = new List<string>();

			specificationsDict.Add("Наименование", null);
			specificationsDict.Add("Артикул", null);
			specificationsDict.Add("Цена", null);
			specificationsDict.Add("Ссылка", null);
			specificationsDict.Add("Главные фото", null);
			specificationsDict.Add("Доп фото", null);
			specificationsDict.Add("Описание", null);
			specificationsDict.Add("Вес расчётный (в граммах)", null);
			specificationsDict.Add("Ширина в мм", null);
			specificationsDict.Add("Длина в мм", null);
			specificationsDict.Add("Высота в мм", null);
			specificationsDict.Add("Глубина в мм", null);
			specificationsDict.Add("Неудачные ссылки или онлайн", null);
			specificationsDict.Add("Кол-во потерь", null);
			foreach (var ProductLink in Links)
			{
				List<int> productCount = new List<int>(); // Кол-во
				List<string> productLocation = new List<string>(); // Место
				Product onePos = new Product();
				onePos.ProductLink = ProductLink;
				string code = getResponse(onePos.ProductLink);
				while (ocheredOutLinks.Count != 0)
				{
					Thread.Sleep(200);
					code = getResponse(ocheredOutLinks.Dequeue());
				} 
				bool kolvoTru = true;
				Regex regexCount = new Regex(@"stock=""(\w+)""");
				Regex regexLocation = new Regex(@"<span>Леруа Мерлен \w+");
				string countLocaionCode = "";
				try
				{
					countLocaionCode =  code.Substring(code.IndexOf("<uc-elbrus-pdp-stocks-list"), code.IndexOf("</uc-elbrus-pdp-stocks-list") - code.IndexOf("<uc-elbrus-pdp-stocks-list"));
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
					Message.errorsList.Add("Не удалось найти и загрузить остатки!!!" + " ---- " + ProductLink);
				}

				bool locationTrue = true;
				// Место
				if (matchesLocation.Count > 0)
				{
					foreach (Match match in matchesLocation)
					{
						string[] s2 = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
						productLocation.Add(s2[2]);
					}

				}
				else
				{
					locationTrue = false;
					onlyOnline++;
					specificationsDict["Неудачные ссылки или онлайн"] += ProductLink + "\n";
				}

				if (locationTrue && kolvoTru)
					foreach (var item in productLocation)
					{
						if (whiteList.Contains(item))
							onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));
						else
						if (blackList.Contains(item))
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
						OpisCode = "11111111";

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
						Opisanie = UnHtml(OpisCode);
						if (Opisanie.Contains("Описание"))
							Opisanie = Opisanie.Replace("Описание","");
						if (Opisanie.Contains("СКАЧАТЬ ИНСТРУКЦИЮ"))
							Opisanie = Opisanie.Replace("СКАЧАТЬ ИНСТРУКЦИЮ", "");
						if (Opisanie.Contains("Скачать документы сертификации"))
							Opisanie = Opisanie.Replace("Скачать документы сертификации", "");
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
					else
					{
						Message.errorsList.Add("Наименование, цена и артикул не найдены!!!" + " ---- " + ProductLink);
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
					else
					{
						Message.errorsList.Add("Наименование, цена и артикул не найдены!!!" + " ---- " + ProductLink);
					}
					string[] namesAndArticleId = resultNameArticleId.Split('"');
					// Вносим в переменные 
					try
					{
						specificationsDict["Наименование"] += namesAndArticleId[3] + "\n";
						specificationsDict["Артикул"] += namesAndArticleId[1] + "\n";

						onePos.NowPrice = Convert.ToDouble(namesAndArticleId[5].Replace('.', ','));
					}
					catch (Exception)
					{
						Message.errorsList.Add("Наименование, цена и артикул в неправильном формате!!!" + " ---- " + ProductLink);
					}
					double newPrice = 0;
					if (onePos.NowPrice < 400)
						newPrice = (onePos.NowPrice + 45 + weight / 1000 * 20 + 50 + 50) * 1.075 * 1.1 * 1.35;
					else
						newPrice = (onePos.NowPrice + 45 + 20 * weight / 1000) * 1.075 * 1.044 * 1.1 * 1.35;
					int nowPrice = Convert.ToInt32(newPrice) / 10 * 10;
					specificationsDict["Цена"] += nowPrice.ToString() + "\n";
					specificationsDict["Ссылка"] += ProductLink + "\n";
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
				UpdateProgress(Convert.ToDouble(Links.Count), Convert.ToDouble(countToApply));
			}
		}

		public static string RemoveInvalidChars(string file_name)
		{
			foreach (Char invalid_char in Path.GetInvalidFileNameChars())
			{
				file_name = file_name.Replace(oldValue: invalid_char.ToString(), newValue: "");
			}
			return file_name;
		}

		public async void doTread()
		{
			List<string> Links = new List<string>();
			if (file != null)
			{
				IList<string> linksProductTXT = await Windows.Storage.FileIO.ReadLinesAsync(file);
				foreach (var item in linksProductTXT)
				{
					Links.Add(item);
				}
			}
			else
			{
				Message.errorsList.Add("Вы не выбали файл.");
			}
			//Message.AllErrors();

			if (Links.Count > 0)
			{
				Thread gogogo = new Thread(new ParameterizedThreadStart(createProductThread));
				gogogo.Start(Links);
				gogogo.Join();
				if (folderSave != null)
				{
					foreach (var item in specificationsDict)
					{
						await folderSave.CreateFileAsync(RemoveInvalidChars(item.Key) + ".txt", CreationCollisionOption.ReplaceExisting);
						StorageFile myFile = await folderSave.GetFileAsync(RemoveInvalidChars(item.Key) + ".txt");
						string data = item.Value;
						if (String.IsNullOrEmpty(data))
							data = "--------";
						await FileIO.WriteTextAsync(myFile, data);
					}
				}
			}
		}

		public async void createProduct_Click(object sender, RoutedEventArgs e)
		{
			var picker = new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".txt");
			file = await picker.PickSingleFileAsync();

			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			folderSave = await folderPicker.PickSingleFolderAsync();

			Thread goFile = new Thread(doTread);
			goFile.Start();
		}

		public void getLinksThread()
		{
			string tag = "Tovar";
			string group = "Lerya";

			// Construct the toast content with data bound fields
			var content = new ToastContentBuilder()
				.AddText("Крадём... Или крадёмся...")
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
			toast.Data.Values["progressValueString"] = "0 ссылок";
			toast.Data.Values["progressStatus"] = "0 товаров";
			// Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
			toast.Data.SequenceNumber = 0;

			// Show the toast notification to the user
			ToastNotificationManager.CreateToastNotifier().Show(toast);
			while (ocheredOutLinks.Count != 0)
			{
				string code = getResponse(ocheredOutLinks.Dequeue());
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
							if (!item.ToString().Replace(@"""", "").StartsWith("/shop/") && !item.ToString().Replace(@"""", "").StartsWith("/catalogue/"))
								fullLinks.Add(item.ToString().Replace(@"""", ""));
				}
				UpdateProgressLinks((fullLinks.Count).ToString(), (LinksProductStart.Count - ocheredOutLinks.Count).ToString(), LinksProductStart.Count.ToString(), ((double)(LinksProductStart.Count - ocheredOutLinks.Count) / ((double)fullLinks.Count)).ToString().Replace(",", "."));
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
		}

		private async void getLinks_Click(object sender, RoutedEventArgs e)
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
					LinksProductStart.Add(item);
					ocheredOutLinks.Enqueue(item);
				}
			}
			else
			{
				Message.errorsList.Add("Вы не выбали файл.");
			}
			Message.AllErrors();

			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			fileWithLinks = await folderPicker.PickSingleFolderAsync();

			Thread getLinks = new Thread(getLinksTreadPerexod);
			getLinks.Start();
		}

		private async void formatLinks_Click(object sender, RoutedEventArgs e)
		{
			var picker = new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".txt");
			Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				IList<string> linksProductTXT = await Windows.Storage.FileIO.ReadLinesAsync(file);
				Result.Text += $"Всего прочитано: {linksProductTXT.Count} \n";
				List<string> ochered = new List<string>();
				int provKoVo = 0;
				foreach (var item in linksProductTXT)
				{
					ochered.Add("https://leroymerlin.ru" + item);
					provKoVo++;
				}
				Result.Text += $"Всего проверенно: {provKoVo} \n";
				var res = ochered.Distinct();
				Result.Text += $"Всего in res: {res.ToList().Count} \n";
				string allLinks = "";
				int kolvoLinks = 0;
				foreach (var item in res.ToList())
				{
					allLinks += item + "\n";
					kolvoLinks++;
				}
				Result.Text += $"Всего добавлено в строку: {kolvoLinks} \n";
				SaveFile("Форматированные ссылки", allLinks);
			}
			else
			{
				Message.errorsList.Add("Не удалось открыть файл");
			}
			Message.AllErrors();
		}

		async void SaveFile(string name, string allData)
		{
			var savePicker = new FileSavePicker();
			// место для сохранения по умолчанию
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			// устанавливаем типы файлов для сохранения
			savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
			// устанавливаем имя нового файла по умолчанию
			savePicker.SuggestedFileName = name;
			savePicker.CommitButtonText = "Сохранить";

			var new_file = await savePicker.PickSaveFileAsync();
			if (new_file != null)
			{
				await FileIO.WriteTextAsync(new_file, allData);
			}
		}
	}
}
