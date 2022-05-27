using System;
using System.Collections.Generic;
using System.Linq;
using Остатки.Classes.JobWhithApi.Ozon;
using Остатки.Classes.JobWhithApi.Ozon.ProductInfo;
using Остатки.Classes.ProductsClasses.InfoPrices;

namespace Остатки.Classes
{

	public class ArticleNumber
	{
		public string OurArticle { get; set; }
		public long ArticleOzon { get; set; }
		public ProductInfoFromOzon productInfoFromOzon { get; set; }
		public CommissionsInfoPrice productInfoPriceFromOzon { get; set; }
	}

	public class Product
	{
		public Guid Id { get; set; }
		public string ProductLink { get; set; } // Ссылка 
		public string Name { get; set; } // Наименование
		public bool NameIsRedact { get; set; } // Менялось ли наименование?
		public string ArticleNumberInShop { get; set; } // Артикул в магазине
		public Dictionary<string, List<long>> ArticleNumberOzonDictList { get; set; } = new Dictionary<string, List<long>>(); // Словарь ClientID => список артикулов
		public Dictionary<string, List<ArticleNumber>> ArticleNumberProductId { get; set; } = new Dictionary<string, List<ArticleNumber>>(); // Словарь 2.0 ClientID => список артикулов (и на)
		public long ArticleNumberOzonDictGetByClientID(string clientID)
		{
			try
			{
				return ArticleNumberProductId[clientID].Count();
			}
			catch (Exception)
			{
				return -1;
			}
		} // Первый озоновский артикул по ClientID
		public string ArticleNumberOzonDictCount
		{
			get
			{
				return ArticleNumberOzonDictList.Count.ToString();
			}
		}  // Кол-во аккаунтов у товара
		public List<string> ArticleNumberUnicList { get; set; } = new List<string>(); // Озон артикул, списком
		public int RemainsWhite { get; set; } // Остатки из белого списка
		public Dictionary<int, int> remainsDictionary { get; set; } // Остатки по магазинам
		public List<DateTime> DateHistoryRemains { get; set; } = new List<DateTime>();// Даты и время проверки остатков
		public List<int> HistoryRemainsWhite { get; set; } = new List<int>();// История Остатки из белого списка
		public int RemainsBlack { get; set; } // Остальные остатки
		public List<int> HistoryRemainsBlack { get; set; } = new List<int>();// История Остальные остатки
		public double NowPrice { get; set; } // Цена магазина сейчас
		public bool PriceIsChanged { get; set; } // Изменилась ли цена?
		public double OldPriceCh
		{
			get
			{
				if (OldPrice.Count > 0)
					return OldPrice[OldPrice.Count() - 1];
				else
					return 0;
			}
		}
		public double Weight { get; set; } // Вес
		public bool NewPriceIsSave { get; set; } // Изменён ли на Озон новый ценник
		public bool ArticleError { get; set; } // Существует ли артикул на Озон у нас в БД
		public List<double> OldPrice { get; set; } = new List<double>(); // Цена Леруа цена была
		public List<DateTime> DateOldPrice { get; set; } = new List<DateTime>();
		public Dictionary<string, bool> AccauntOzonID { get; set; } = new Dictionary<string, bool>();
		public string TypeOfShop { get; set; }
		public override string ToString()
		{
			//return ProductLink;
			return ArticleNumberInShop;
		}
		
		public string ToStringInfo()
		{
			string data = "";
			data += $"Артикул товара в магазе: {ArticleNumberInShop} \n";

			data += $"Аккаунты: \n";
			int countAccaunt = 0;
			Dictionary<ApiKeys, int> countProductonOnAccaunt = new Dictionary<ApiKeys, int>();
			foreach (var item in ApiKeysesJob.GetAllApiList())
			{
				long chID = ArticleNumberOzonDictGetByClientID(item.ClientId);
				if (!countProductonOnAccaunt.ContainsKey(item))
				{
					countProductonOnAccaunt.Add(item, 0);
				}
				if (chID != -1)
				{
					data += $"\t {ApiKeysesJob.GetApiName(item)}: ";
					if (ArticleNumberOzonDictList.ContainsKey(item.ClientId))
					foreach (var item1 in ArticleNumberOzonDictList[item.ClientId])
					{
						data += $"{item1} ";
						countProductonOnAccaunt[item]++;
					}
					data += $"\n";
					countAccaunt++;
				}
			}
			if (countAccaunt == 0)
				data += $"\t Товар не найден в БД на озон!\n";

			//data += $"Кол-во аккаунтов: {ArticleNumberOzonDictCount} \n";
			//data += $"Остатки на белизне: {RemainsWhite} \n";
			//data += $"Остатки ин жопенн: {RemainsBlack} \n";
			if (DateHistoryRemains.Count > 0)
				data += $"Послед. дата проверки: {DateHistoryRemains.Last()} \n";
			else
				data += $"Послед. дата проверки: отсутвует \n";
			data += $"\tЦена: {NowPrice}\n";
			if (OldPriceCh != 0)
			{
				data += $"\t   Ранее: {OldPriceCh} \n";
				data += $"\t   Изменение замечено: {DateOldPrice.Last()} \n";
			}
			else
				data += $"\t Цена не менялась \n";
			data += $"Вес: {Weight}\n";
			data += $"Магазин: {TypeOfShop}\n";
			data += $"-------\t-------\t-------\n";
			data += $"Ожидаемое кол-во на одном акке: {ArticleNumberUnicList.Count}\n";
			data += $"Фактическое: \n";
			foreach (var item in countProductonOnAccaunt)
			{
				if (ArticleNumberProductId.ContainsKey(item.Key.ClientId))
					data += $"\t{ApiKeysesJob.GetApiName(item.Key)}: {ArticleNumberProductId[item.Key.ClientId].Count()}\n";
			}
			data += $"-------\t-------\t-------\n";
			data += $"Offer Id: \n";
			foreach (var item in ArticleNumberUnicList)
			{
				data += $"\t{item}\n";
			}

			data += $"Product Id: \n";
			foreach (var item in ArticleNumberProductId)
			{
				data += $"\t{item.Key}:\n";
				foreach (var article in item.Value)
				{
					data += $"\t{article.ArticleOzon}\n";
				}
				data += $"-------\t-------\t-------\n";
			}

			data += $"-------\t-------\t-------\n";
			data += $"Инфа с озона: \n";
			if (ArticleNumberProductId.First().Value.First().productInfoFromOzon != null)
			{
				ResultQInfo articleNumberContent = ArticleNumberProductId.First().Value.First().productInfoFromOzon.result;

				data += $"{articleNumberContent.name} \n" +
					$"Цены и комиссии: \n";

				foreach (var key in ApiKeysesJob.GetAllApiList())
				{
					if (ArticleNumberProductId.ContainsKey(key.ClientId))
					{
						data += $"{key.ClientId}: \n" +
							$"<------\t------>\n";
						foreach (var item in ArticleNumberProductId[key.ClientId])
						{
							ResultQInfo infoFromOzon = item.productInfoFromOzon.result;

							data += $"Артикул: {articleNumberContent.offer_id}\n";
							data += $"Цена с учётом акций: {articleNumberContent.marketing_price}\n";
							data += $"Цена: {articleNumberContent.price}\n";
							data += $"Цена до скидки: {articleNumberContent.old_price}\n";
							data += $"Комиссии: \n";

							CommissionsInfoPrice commissionsInfoPriceOne = item.productInfoPriceFromOzon;

							data += $"%: {commissionsInfoPriceOne.sales_percent}\n" +
								$"FBO: \n" +
								$"\tСборка: {commissionsInfoPriceOne.fbo_fulfillment_amount}\n" +
								$"\tМагистраль: {commissionsInfoPriceOne.fbo_direct_flow_trans_min_amount}-{commissionsInfoPriceOne.fbo_direct_flow_trans_max_amount} \n" +
								$"\tПоследняя миля: {commissionsInfoPriceOne.fbo_deliv_to_customer_amount}\n" +
								$"\tКомиссия за возврат и отмену: {commissionsInfoPriceOne.fbo_return_flow_amount}\n" +
								$"\tКомиссия за обратную логистику: {commissionsInfoPriceOne.fbo_return_flow_trans_min_amount}-{commissionsInfoPriceOne.fbo_return_flow_trans_max_amount} \n" +
								$"FBS: \n" +
								$"\tКомиссия за обработку отправления: {commissionsInfoPriceOne.fbs_first_mile_min_amount}-{commissionsInfoPriceOne.fbs_first_mile_max_amount} \n" +
								$"\tМагистраль: {commissionsInfoPriceOne.fbs_direct_flow_trans_min_amount}-{commissionsInfoPriceOne.fbs_direct_flow_trans_max_amount} \n" +
								$"\tПоследняя миля: {commissionsInfoPriceOne.fbs_deliv_to_customer_amount}\n" +
								$"\tКомиссия за возврат, обработка: {commissionsInfoPriceOne.fbs_return_flow_amount}\n" +
								$"\tКомиссия за возврат, магистраль: {commissionsInfoPriceOne.fbs_return_flow_trans_min_amount}-{commissionsInfoPriceOne.fbs_return_flow_trans_max_amount} \n";

							data += $"-------\t-------\t-------\n";

							data += $"Сейчас на складе: {articleNumberContent.stocks.present}\n";
							data += $"Зарезервировано: {articleNumberContent.stocks.reserved}\n";

							data += $"-------\t-------\t-------\n";

							data += $"В продаже?: {articleNumberContent.visible}\n";
							data += $"Создан: {articleNumberContent.created_at}\n";
							data += $"<------>\n";
						}
						data += $"<-------\t-------\t------->\n";
					}
				}
			}
			data += $"-------\t-------\t-------\n";

			return data;
		}
	}
}
