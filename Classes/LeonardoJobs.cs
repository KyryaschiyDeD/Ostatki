using HtmlAgilityPack;
using LiteDB;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Остатки.Pages;

namespace Остатки.Classes
{
	public class LeonardoJobs
	{
		static bool CheckedProductInDataBase = false;
		static bool AddXaract = false;

		static int count = 0;
		static int trueLinksCount = 0;

		static List<Product> AllProduct = new List<Product>();

		static Dictionary<string, string> specificationsDict = new Dictionary<string, string>();

		static ConcurrentQueue<string> UnRedactLinksQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> HtmlQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> HtmlRemainsQueue = new ConcurrentQueue<string>();
		static ConcurrentQueue<string> LinksQueue = new ConcurrentQueue<string>();

		static List<string> fullLinks = new List<string>();

		static StorageFolder fileWithLinks;
		public static string GetRemainsPostLeo(string url, string id_goods)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://leonardo.ru/ajax/itemshops.ajax.php");
			request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			HTMLJob.CountOfUserAgent++;
			HTMLJob.CountproxyIp++;
			HTMLJob.CountproxyPort++;
			request.Method = "POST";
			request.Headers["authority"] = "leonardo.ru";
			request.Headers["scheme"] = "https";
			request.Headers["sec-ch-ua"] = @"\""Chromium\"";v=\""94\"", \"" Not A;Brand\"";v=\""99\"", \""Opera\"";v=\""80\""";
			request.Headers["dnt"] = "1";
			request.Headers["sec-ch-ua-mobile"] = "?0";
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.31 Safari/537.36 OPR/80.0.4170.4 (Edition beta)";
			request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			request.Accept = "*/*";
			request.Headers["x-requested-with"] = "XMLHttpRequest";
			request.Headers["sec-ch-ua-platform"] = @"\""Windows\""";
			request.Headers["origin"] = "https://leonardo.ru";
			request.Headers["sec-fetch-site"] = "same-origin";
			request.Headers["sec-fetch-mode"] = "cors";
			request.Headers["sec-fetch-dest"] = "empty";
			request.Headers["referer"] = url;
			//request.Headers["accept-encoding"] = "gzip, deflate, br";
			request.Headers["accept-language"] = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";
			request.Headers["cookie"] = "geocity=moskow; JivoSiteLoaded=1; switcher_cookie=0; client=PrivetOtParsera; PHPSESSID=TotSamiyParser; cityconfirmed=true; storytime=00000000; city=moskow";
			var postData = "id_good=" + id_goods;
			int a = 0;
			postData += "&id_detail=" + url.Where(x => int.TryParse(x.ToString(), out a));
			postData += "&tab_flag=first";
			var data = Encoding.ASCII.GetBytes(postData);
			request.ContentLength = data.Length;
			//Console.WriteLine("Отправляем");
			using (var stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			var response = (HttpWebResponse)request.GetResponse();

			return new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)).ReadToEnd();
			//Console.WriteLine(responseString);
		}
		static LeonardoJobs()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				AllProduct = new List<Product>(allProducts);
			}
		}
		public static async void getLinksTreadPerexod()
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
			/*int colll = 0;
			await fileWithLinks.CreateFolderAsync("X", CreationCollisionOption.ReplaceExisting);
			StorageFolder SaveFolder = await fileWithLinks.GetFolderAsync("X");
			bool one = false;
			foreach (var item in UnRedactLinksQueue)
			{
				string Code = "";
				//string Code = GetResponseNewLeoAsync(item);
				await SaveFolder.CreateFileAsync(colll.ToString() + ".txt", CreationCollisionOption.ReplaceExisting);
				StorageFile myFile = await SaveFolder.GetFileAsync(colll.ToString() + ".txt");
				string data = Code;
				if (!one)
				{
					//Message.infoList.Add(ConvertWin1251ToUTF8(data));
					Message.infoList.Add(data);
					one = true;
				}
				await FileIO.WriteTextAsync(myFile, data);
				colll++;
			}*/
			createProductThread();
			if (fileWithLinks != null)
			{
				await fileWithLinks.CreateFolderAsync("X", CreationCollisionOption.ReplaceExisting);
				StorageFolder SaveFolder = await fileWithLinks.GetFolderAsync("X");
				foreach (var item in specificationsDict)
				{
					await SaveFolder.CreateFileAsync(RemoveInvalidChars(item.Key) + ".txt", CreationCollisionOption.ReplaceExisting);
					StorageFile myFile = await SaveFolder.GetFileAsync(RemoveInvalidChars(item.Key) + ".txt");
					if (!String.IsNullOrEmpty(item.Value))
					await FileIO.WriteTextAsync(myFile, item.Value);
				}
			}
			specificationsDict.Clear(); 
		}

		public static async void GetLinks(bool TrueProductsDatabase)
		{
			CheckedProductInDataBase = TrueProductsDatabase;
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

		private static string GetIdGoodsOnHTMLLeo(string code)
		{
			Regex regexCountHtml = new Regex(@"data-good-id=""(\w+)""");
			MatchCollection matchesCount = regexCountHtml.Matches(code);
			string str = "";
			foreach (var item in matchesCount)
			{
				str += item;
			}
			int a = 0;
			return str.Replace("data-good-id=","").Replace(@"""","");
		}

		static string getResponseBuf(string uri)
		{
			StringBuilder sb = new StringBuilder();
			byte[] buf = new byte[16384];
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream resStream = response.GetResponseStream();
			int count = 0;
			do
			{
				count = resStream.Read(buf, 0, buf.Length);
				if (count != 0)
				{
					sb.Append(Encoding.GetEncoding(1251).GetString(buf, 0, count));
				}
			}
			while (count > 0);
			return sb.ToString();
		}
		public static string getResponse(string uri)
		{
			string htmlCode = "";
			HttpWebRequest proxy_request = (HttpWebRequest)WebRequest.Create(uri);
			proxy_request.Method = "GET";
			proxy_request.ContentType = "application/x-www-form-urlencoded";
			proxy_request.UserAgent = HTMLJob.userAgent[HTMLJob.CountOfUserAgent];
			proxy_request.KeepAlive = true;
			proxy_request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
			proxy_request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			HttpWebResponse resp = null;
			string html = "";
			bool isByll = false;
			try
			{
				resp = proxy_request.GetResponse() as HttpWebResponse;
				trueLinksCount++;
				if (resp != null)
					using (StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding(1251)))
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

		public static void getLinksThread()
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
				string lnk = "";
				UnRedactLinksQueue.TryDequeue(out lnk);
				string code = getResponse(lnk);
				Regex regexLinks = new Regex(@"<a class=""link-on-product"" href="".*?""");
				MatchCollection matcheslinks = regexLinks.Matches(code);
				string tmpLinks = "";
				foreach (var item in matcheslinks)
				{
					tmpLinks += item.ToString();
				}
				regexLinks = new Regex(@"href="".*?""");
				matcheslinks = regexLinks.Matches(tmpLinks);

				foreach (var item in matcheslinks)
				{
					fullLinks.Add(item.ToString().Replace("href=","").Replace(@"""", ""));
				}
				UpdateProgressLinks(fullLinks.Count.ToString(), (count - UnRedactLinksQueue.Count).ToString(), count.ToString(), ((double)fullLinks.Count / ((double)count)).ToString().Replace(",", "."));
			}

		}

		private static async Task FormatLinks()
		{
			for (int i = 0; i < fullLinks.Count; i++)
			{
				fullLinks[i] = "https://leonardo.ru" + fullLinks[i];
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

		public static void UpdateProgressLinks(string kolvo, string apply, string fullssilm, string progr)
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
		public static void UpdateProgresslnk(double kolvo, double apply)
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
		public static void createProductThread()
		{
			
			int addCountProduct = 0;
			int kolInBlackList = 0;
			int countToApply = 0;
			int allCount = UnRedactLinksQueue.Count;

			List<string> StandardNamesOfXaract = new List<string>();
			if (!AddXaract)
			{
				AddXaract = true;
				if (!specificationsDict.ContainsKey("Наименование"))
					specificationsDict.Add("Наименование", null);
				if (!specificationsDict.ContainsKey("good_"))
					specificationsDict.Add("good_", null);
				if (!specificationsDict.ContainsKey("id_goods"))
					specificationsDict.Add("id_goods", null);
				if (!specificationsDict.ContainsKey("АртикулDataBase"))
					specificationsDict.Add("АртикулDataBase", null);
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
			}
			

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
					UpdateProgresslnk(Convert.ToDouble(allCount), Convert.ToDouble(HtmlQueue.Count));
				}
			};
			Parallel.Invoke(action, action, action, action, action, action, action, action, action, action, action, action, action, action, action, action);

			List<string> NamesOpisesXaract = new List<string>();
			while (HtmlQueue.Count != 0)
			{
				string code = "";
				HtmlQueue.TryDequeue(out code);
				string prdctLink = "";
				LinksQueue.TryDequeue(out prdctLink);

				List<int> productCount = new List<int>(); // Кол-во
				List<int> productLocation = new List<int>(); // Место
				Product onePos = new Product();

				string id_goods = GetIdGoodsOnHTMLLeo(code);
				string remainsCode = GetRemainsPostLeo(prdctLink, id_goods);
				onePos.ProductLink = prdctLink;
				long good;
				long.TryParse(string.Join("", prdctLink.Where(c => char.IsDigit(c))), out good);
				specificationsDict["АртикулDataBase"] += "lnrd_" + good.ToString() + "\n";
				onePos.ArticleNumberInShop = id_goods.ToString();
				onePos.ArticleNumberUnic = "lnrd_" + good.ToString();
				specificationsDict["good_"] += good.ToString() + "\n";
				specificationsDict["id_goods"] += id_goods + "\n";

				int startIndexName = code.IndexOf(@"<h1 class=""product-title-text"">") + @"<h1 class=""product-title-text"">".Length;
				int lenIndexName = code.IndexOf("</h1>") - startIndexName - "</h1>".Length;
				string name = code.Substring(startIndexName, lenIndexName).Replace("\n", "").Replace("  ", "").Trim();
				specificationsDict["Наименование"] += name + "\n";
				onePos.Name = name;

				int startIndexPrice = code.IndexOf(@"<div class=""actual-price"">") + @"<div class=""actual-price"">".Length;
				int lenIndexPrice = code.IndexOf(@"<span", startIndexPrice) - startIndexPrice - @"<span".Length;
				string priceTMP = code.Substring(startIndexPrice, 5).Trim();
				string price = "";
				foreach (var item in priceTMP)
				{
					if (Char.IsDigit(item) || item == ',')
						price += item;
				}
				onePos.NowPrice = Convert.ToDouble(price);
				specificationsDict["Цена"] += price + "\n";

				int startIndexOpisCode = code.IndexOf(@" < div id=""collapsedesc""") + @"<div id=""collapsedesc""".Length;
				int lenIndexOpisCode = code.IndexOf(@"<div id=""collapsereviews""") - startIndexOpisCode - @"<div id=""collapsereviews""".Length;
				string opisCode = code.Substring(startIndexOpisCode, lenIndexOpisCode);

				if (code.Contains("Описание"))
				{
					int startIndexOpis = opisCode.IndexOf(@"aria-labelledby=""itemdesc-tab"">") + @"aria-labelledby=""itemdesc-tab"">".Length;
					int lenIndexOpis = opisCode.IndexOf(@"</div>") - startIndexOpis - @"</div>".Length;
					if (startIndexOpis > 0 && lenIndexOpis > 0)
					{
						string opis = opisCode.Substring(startIndexOpis, lenIndexOpis);
						specificationsDict["Описание"] += HTMLJob.UnHtml(opis) + "\n";
					}
					else
						specificationsDict["Описание"] += "\n";
				}
				else
					specificationsDict["Описание"] += "\n";

				List<string> XaractListOneProduct = new List<string>();

				string xaractCodeAll = code.Substring(code.IndexOf(@"<div class=""featureline""><div class=""featuretitle col-lg-5""><span>"), code.IndexOf(@"</div></div></div></div><a class=""tab-collapse-btn""")- code.IndexOf(@"<div class=""featureline""><div class=""featuretitle col-lg-5""><span>") + @"</div></div></div></div><a class=""tab-collapse-btn""".Length);

				Regex regexXaractName = new Regex(@"<span>([^<]+)<\/span>");
				MatchCollection xaracterName = regexXaractName.Matches(xaractCodeAll);

				Regex regexXaractContent = new Regex(@"<div class=""featurefield col-lg-7"">([^<]+)<\/div>");
				MatchCollection xaractContent = regexXaractContent.Matches(xaractCodeAll);

				List<string> names = new List<string>();
				List<string> contens = new List<string>();

				foreach (var item in xaracterName)
				{
					names.Add(HTMLJob.UnHtml(item.ToString()));
				}
				foreach (var item in xaractContent)
				{
					contens.Add(HTMLJob.UnHtml(item.ToString()));
				}

				foreach (var item in names)
				{
					XaractListOneProduct.Add(item);
					if (!NamesOpisesXaract.Contains(item))
						NamesOpisesXaract.Add(item);

					if (!specificationsDict.ContainsKey(item))
						specificationsDict.Add(item, "");
					specificationsDict[item] += contens[names.IndexOf(item)] + "\n";
				} 

				foreach (var oneName in NamesOpisesXaract)
				{
					if (!XaractListOneProduct.Contains(oneName))
						specificationsDict[oneName] += "\n";
				}


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
	}
}
