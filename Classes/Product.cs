using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

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
	public class Product
	{
		static object locker = new object();

		public Guid Id { get; set; }
		public string ProductLink { get; set; }
		public string Name { get; set; } // Наименование
		public long ArticleNumberLerya { get; set; } // Леруа артикл
		public long ArticleNumberOzon { get; set; } // Озон артикл
		public int RemainsWhite { get; set; } // Остатки из белого списка
		public List<DateTime> DateHistoryRemains { get; set; } = new List<DateTime>();// Даты и время проверки остатков
		public List<int> HistoryRemainsWhite { get; set; } = new List<int>();// История Остатки из белого списка
		public int RemainsBlack { get; set; } // Остальные остатки
		public List<int> HistoryRemainsBlack { get; set; } = new List<int>();// История Остальные остатки
		public double NowPrice { get; set; } // Цена Леруа сейчас
		public double OldPriceCh
		{
			get
			{
				if (OldPrice.Count > 0)
					return OldPrice[OldPrice.Count() - 1];
				else 
					return Convert.ToDouble(0);
			}
		}
		public bool NewPriceIsSave { get; set; } // Изменён ли на Озон новый ценник
		public bool ArticleError { get; set; } // Существует ли артикул на Озон у нас в БД
		public bool ArticleErrorIgnore { get; set; } // Игнорируем ли мы конфликт
		public List<double> OldPrice { get; set; } = new List<double>(); // Цена Леруа цена была
		public List<DateTime> DateOldPrice { get; set; } = new List<DateTime>();

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
			Regex regexLocation = new Regex(@"<span>Леруа Мерлен \w+");

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
				onePos.ArticleNumberLerya = Convert.ToInt64(namesAndArticleId[1]);
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
					//string[] s2 = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
					//productLocation.Add(match.Value);
				}

			}

			foreach (var item in productLocation)
			{
				if (Global.whiteList.Contains(item))

					onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));


				else
				if (Global.blackList.Contains(item))

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
				string code = getResponse(link);
				int indexOfStart = code.IndexOf("<uc-elbrus-pdp-stocks-list");
				int indexOfEnd = code.IndexOf("</uc-elbrus-pdp-stocks-list");
				if (indexOfStart >= 0)
				{
					string countLocaionCode = code.Substring(indexOfStart, indexOfEnd - indexOfStart);
					//string[] words = code.Split(new string[] { "<uc-store-stock", "</uc-store-stock>" }, StringSplitOptions.RemoveEmptyEntries);




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
						onePos.ArticleNumberLerya = Convert.ToInt64(namesAndArticleId[1]);
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
							//string[] s2 = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
							//productLocation.Add(match.Value);
						}

					}
					else
					{
						Message.errorsList.Add("Не удалось найти и загрузить местоположение!!!");
					}

					foreach (var item in productLocation)
					{
						if (Global.whiteList.Contains(item))
							onePos.RemainsWhite += productCount.ElementAt(productLocation.IndexOf(item));
						else
						if (Global.blackList.Contains(item))
							onePos.RemainsBlack += productCount.ElementAt(productLocation.IndexOf(item));
					}

					lock (locker)
					{
						DataBaseJob.UpdateRemainsOldProduct(onePos);
					}
				}
			}
		}
		public override string ToString()
		{
			return $"{Name} {ArticleNumberLerya} {RemainsWhite} {NowPrice}";
		}
	}
}
