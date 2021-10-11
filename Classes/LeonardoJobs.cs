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
		static List<int> shopWhiteOrBlacks = new List<int>();

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
			shopWhiteOrBlacks = ShopWhiteOrBlackJob.GetAllShopList().Where(x => x.WhatIsShop == "Леонардо" && x.ShopType == true).Select(u => u.Code).ToList();
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

		private static void CreateToastProductJob()
		{
			string tag = "Update";
			string group = "Lerya";

			// Construct the toast content with data bound fields
			var content = new ToastContentBuilder()
				.AddText("Обновляем!!!")
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
		public static void UpdateProgress(double kolvo, double apply, string status)
		{
			// Construct a NotificationData object;
			string tag = "Update";
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

		public static void createProductThread()
		{
			CreateToastProductJob();
			UpdateProgress(0,0,"Создаём");
			int addStroka = 0;
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
				if (!specificationsDict.ContainsKey("Ссылка ONE"))
					specificationsDict.Add("Ссылка ONE", null);
				if (!specificationsDict.ContainsKey("Главные фото"))
					specificationsDict.Add("Главные фото", null);
				if (!specificationsDict.ContainsKey("Доп фото"))
					specificationsDict.Add("Доп фото", null);
				if (!specificationsDict.ContainsKey("Описание"))
					specificationsDict.Add("Описание", null);
				if (!specificationsDict.ContainsKey("КолВо завод в отправл"))
					specificationsDict.Add("КолВо завод в отправл", null);
				if (!specificationsDict.ContainsKey("Объединить на одной карточке"))
					specificationsDict.Add("Объединить на одной карточке", null);
				if (!specificationsDict.ContainsKey("Название модели для Ozon"))
					specificationsDict.Add("Название модели для Ozon", null);
				if (!specificationsDict.ContainsKey("Неудачка"))
					specificationsDict.Add("Неудачка", null);
				if (!specificationsDict.ContainsKey("Проблемная ссылка"))
					specificationsDict.Add("Проблемная ссылка", null);
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
					UpdateProgress(allCount, allCount - HtmlQueue.Count, "Воруем");
				}
			};
			Parallel.Invoke(action, action, action, action, action, action, action, action, action, action, action, action, action, action, action, action);
			List<Product> productListBackground = new List<Product>();
			List<string> NamesOpisesXaract = new List<string>();
			while (HtmlQueue.Count != 0)
			{
				string code = "";
				HtmlQueue.TryDequeue(out code);
				string prdctLink = "";
				LinksQueue.TryDequeue(out prdctLink);
				List<int> productCount = new List<int>(); // Кол-во
				List<int> productLocation = new List<int>(); // Место

				string id_goods = GetIdGoodsOnHTMLLeo(code);
				string remainsCode = GetRemainsPostLeo(prdctLink, id_goods);

				Regex regexLocation = new Regex(@"<label for=""(\w+)"">");
				MatchCollection matchColLocation = regexLocation.Matches(remainsCode);
				foreach (var item in matchColLocation)
				{
					int shopCode;
					int.TryParse(string.Join("", item.ToString().Where(c => char.IsDigit(c))), out shopCode);
					productLocation.Add(shopCode);
				}

				if (remainsCode.Contains("no_exist") || remainsCode.Contains("exist"))
				{
					string colCount = "";
					try
					{
						if (remainsCode.Contains(@"</td></tr><tr style=""height: 45px; "">"))
							colCount = remainsCode.Substring(remainsCode.IndexOf(@"<tr style=""height: 45px; ""><td class=""imgbgr2  bgr_tdnotfirst""><svg class"), remainsCode.IndexOf(@"</td></tr><tr style=""height: 45px; "">") - remainsCode.IndexOf(@"<tr style=""height: 45px; ""><td class=""imgbgr2  bgr_tdnotfirst""><svg class"));
						else
							colCount = remainsCode;
						Regex regexCount = new Regex(@"<svg class=""(\w+)"">");
						MatchCollection matchColCount = regexCount.Matches(colCount);
						foreach (var item in matchColCount)
						{
							if (item.ToString().Contains("no_exist"))
								productCount.Add(0);
							else
							if (item.ToString().Contains("exist"))
								productCount.Add(1);
							else
								productCount.Add(0);
						}
					}
					catch (Exception)
					{
						specificationsDict["Проблемная ссылка"] += colCount + " ";
						specificationsDict["Проблемная ссылка"] += "\n";
						specificationsDict["Проблемная ссылка"] += "-------------------------------------------\n";
					}
				}
				else
				if (remainsCode.Contains("нет в наличии") || remainsCode.Contains("много") || remainsCode.Contains("заканчивается") || remainsCode.Contains("мало"))
				{
					Regex regexCount = new Regex(@"<img src="".*?"" title="".*?"">");
					MatchCollection matchColCount = regexCount.Matches(remainsCode);
					string[] keys = new string[] { "нет в наличии", "много", "заканчивается", "мало" };
					foreach (var item in matchColCount)
					{
						string sKeyResult = keys.FirstOrDefault(s => item.ToString().Contains(s));

						switch (sKeyResult)
						{
							case "нет в наличии":
								productCount.Add(0);
								break;
							case "мало":
								productCount.Add(1);
								break;
							case "заканчивается":
								productCount.Add(2);
								break;
							case "много":
								productCount.Add(3);
								break;
						} 
						/*if (item.ToString().Contains("нет в наличии"))
							productCount.Add(0);
						else
						if (item.ToString().Contains("мало"))
							productCount.Add(1);
						else
						if (item.ToString().Contains("заканчивается"))
							productCount.Add(2);
						else
						if (item.ToString().Contains("много"))
							productCount.Add(3); */
					}
				}

				int remaintWhiteTMP = 0;
				int remaintBlackTMP = 0;

				//if (productCount.Count != productLocation.Count)
				//{
				//	if (productCount.Count < productLocation.Count)
				//		for (int i = 0; i < productLocation.Count - productCount.Count; i++)
				//		{
				//			productCount.Add(0);
				//		}
				//}
				Dictionary<int, int> remainsDictionaryTMP = new Dictionary<int, int>();
				for (int i = 0; i < productCount.Count - 1; i++)
				{
					remainsDictionaryTMP.Add(productLocation[i], productCount[i]);
					if (shopWhiteOrBlacks.Contains(productLocation[i]))
						remaintWhiteTMP += productCount[i];
					else
						remaintBlackTMP += productCount[i];
				}

				//foreach (var item in productLocation)
				//{
				//	specificationsDict["Проблемная ссылка"] += item + " ";
				//}

				//specificationsDict["Проблемная ссылка"] += "\n";

				//foreach (var item in productCount)
				//{
				//	specificationsDict["Проблемная ссылка"] += item + " ";
				//}

				//specificationsDict["Проблемная ссылка"] += "\n";
				//specificationsDict["Проблемная ссылка"] += "-------------------------------------------";
				//specificationsDict["Проблемная ссылка"] += "\n";

				if (remaintWhiteTMP > 1)
				{
					Product onePos = new Product();
					specificationsDict["Ссылка ONE"] += prdctLink + "\n";
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
					

					List<int> countOfComplect = new List<int>();
					countOfComplect.Add(1);

					if (onePos.NowPrice <= 500)
					{
						countOfComplect.Add(3);
						countOfComplect.Add(5);
						countOfComplect.Add(10);
					}
					else
					if (onePos.NowPrice <= 1000)
					{
						countOfComplect.Add(3);
						countOfComplect.Add(5);
					}
					else
					if (onePos.NowPrice <= 2000)
					{
						countOfComplect.Add(3);
					}
					onePos.ProductLink = prdctLink;

					Regex regexImg = new Regex(@"<a href=""(.*?)"" class=""fbimage"" rel=""fbgallery"">");
					MatchCollection matchColImg = regexImg.Matches(code);
					string mainPhoto = "";
					string dopPhoto = "";
					foreach (var item in matchColImg)
					{
						string photoTmp = "https:" + item.ToString().Substring(item.ToString().IndexOf(@"""") + 1, item.ToString().IndexOf(@"""", item.ToString().IndexOf(@"""") + 1) - (item.ToString().IndexOf(@"""") + 1));
						if (mainPhoto.Length == 0)
							mainPhoto += photoTmp;
						else
							dopPhoto += photoTmp + " ";
					}



					long good;
					long.TryParse(string.Join("", prdctLink.Where(c => char.IsDigit(c))), out good);


					onePos.ArticleNumberInShop = id_goods.ToString();
					foreach (var item in countOfComplect)
					{
						onePos.ArticleNumberUnicList.Add("lnrd_" + good.ToString() + "_" + "x" + item.ToString());
					}



					int startIndexName = code.IndexOf(@"<h1 class=""product-title-text"">") + @"<h1 class=""product-title-text"">".Length;
					int lenIndexName = code.IndexOf("</h1>") - startIndexName - "</h1>".Length;
					string name = code.Substring(startIndexName, lenIndexName).Replace("\n", "").Replace("  ", "").Trim();

					onePos.Name = name;



					int startIndexOpisCode = code.IndexOf(@" <div id=""collapsedesc""") + @"<div id=""collapsedesc""".Length;
					int lenIndexOpisCode = code.IndexOf(@"<div id=""collapsereviews""") - startIndexOpisCode - @"<div id=""collapsereviews""".Length;
					string opisCode = code.Substring(startIndexOpisCode, lenIndexOpisCode);

					if (code.Contains("Описание"))
					{
						int startIndexOpis = opisCode.IndexOf(@"aria-labelledby=""itemdesc-tab"">") + @"aria-labelledby=""itemdesc-tab"">".Length;
						int lenIndexOpis = opisCode.IndexOf(@"</div>") - startIndexOpis - @"</div>".Length;
						if (startIndexOpis > 0 && lenIndexOpis > 0)
						{
							string opis = opisCode.Substring(startIndexOpis, lenIndexOpis);
							foreach (var item in countOfComplect)
							{
								specificationsDict["Описание"] += HTMLJob.UnHtml(opis) + "\n";
							}
						}
						else
							foreach (var item in countOfComplect)
							{
								specificationsDict["Описание"] += "\n";
							}
					}
					else
						foreach (var item in countOfComplect)
						{
							specificationsDict["Описание"] += "\n";
						}

					List<string> XaractListOneProduct = new List<string>();

					string xaractCodeAll = code.Substring(code.IndexOf(@"<div class=""featureline""><div class=""featuretitle col-lg-5""><span>"), code.IndexOf(@"</div></div></div></div><a class=""tab-collapse-btn""") - code.IndexOf(@"<div class=""featureline""><div class=""featuretitle col-lg-5""><span>") + @"</div></div></div></div><a class=""tab-collapse-btn""".Length);

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
						{
							specificationsDict.Add(item, "");
							for (int i = 0; i < addStroka; i++)
							{
								specificationsDict[item] += "\n";
							}
							
						}
							
						for (int i = 0; i < countOfComplect.Count; i++)
						{
							specificationsDict[item] += contens[names.IndexOf(item)] + "\n";
						}

					}

					foreach (var oneName in NamesOpisesXaract)
					{
						if (!XaractListOneProduct.Contains(oneName))
							for (int i = 0; i < countOfComplect.Count; i++)
							{
								specificationsDict[oneName] += "\n";
							}
					}

					foreach (var item in countOfComplect)
					{
						specificationsDict["Главные фото"] += mainPhoto + "\n";
						specificationsDict["Доп фото"] += dopPhoto + "\n";
						specificationsDict["Ссылка"] += prdctLink + "\n";
						specificationsDict["АртикулDataBase"] += "lnrd_" + good.ToString() + "_" + "x" + item.ToString() + "\n";
						specificationsDict["good_"] += good.ToString() + "\n";
						specificationsDict["id_goods"] += id_goods + "\n";
						specificationsDict["Наименование"] += name + "\n";
						double newPrice = 0;
						if (onePos.NowPrice < 400)
							newPrice = (Convert.ToDouble(price) + 45 + 2000 / 1000 * 20 + 50 + 50) * 1.075 * 1.1 * 1.35;
						else
							newPrice = (Convert.ToDouble(price) + 45 + 20 * 2000 / 1000) * 1.075 * 1.044 * 1.1 * 1.35;

						newPrice = newPrice*1.15;

						int nowPrice = 0;

						switch (item)
						{
							case 1:
								nowPrice = Convert.ToInt32(newPrice) / 10 * 10;
								break;
							case 3:
								nowPrice = Convert.ToInt32(newPrice * item * 0.85) / 10 * 10;
								break;
							case 5:
								nowPrice = Convert.ToInt32(newPrice * item * 0.825) / 10 * 10;
								break;
							case 10:
								nowPrice = Convert.ToInt32(newPrice * item * 0.8) / 10 * 10;
								break;
							default:
								break;
						}

						specificationsDict["Цена"] += nowPrice + "\n";
						specificationsDict["КолВо завод в отправл"] += item + "\n";
						specificationsDict["Название модели для Ozon"] += name + "\n";
						specificationsDict["Объединить на одной карточке"] += id_goods + "_one_cart\n";
						addStroka++;
					}

					onePos.RemainsWhite = remaintWhiteTMP;
					onePos.RemainsBlack = remaintBlackTMP;
					onePos.remainsDictionary = remainsDictionaryTMP;
					onePos.DateHistoryRemains.Add(DateTime.Now);
					onePos.Weight = 2000;
					onePos.TypeOfShop = "Леонардо";
					productListBackground.Add(onePos);
				}

				UpdateProgress(allCount, allCount - HtmlQueue.Count, "Создаём");
			}
			DataBaseJob.AddListToBackground(productListBackground);
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
