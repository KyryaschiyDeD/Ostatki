using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon
{
    public class Item
    {
        public int product_id { get; set; }
        public string offer_id { get; set; }
    }

    public class Result
    {
        public List<Item> items { get; set; }
        public int total { get; set; }
    }

    public class Root
    {
        public Result result { get; set; }
    }

    public class Filter
    {
        [JsonProperty("offer_id")]
        public List<string> offer_id { get; set; }
        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class RootRequest
    {
        [JsonProperty("filter")]
        public Filter filter { get; set; }
        //[JsonProperty("page")]
        //public int page { get; set; }
        //[JsonProperty("page_size")]
        //public int page_size { get; set; }
    }


    public class GetAndSaveProductId
	{
        public static void GetProductsId()
		{
            List<Product> Remains = new List<Product>();
            List<Product> Wait = new List<Product>();
            List<Product> Archive = new List<Product>();
            List<Product> Del = new List<Product>();

            DataBaseJob.GetAllProductFromTheBase(out Remains, out Wait, out Archive, out Del);
            List<Product> AllProductsInTheBase = new List<Product>();

            AllProductsInTheBase.AddRange(Remains);
            AllProductsInTheBase.AddRange(Wait);
            AllProductsInTheBase.AddRange(Archive);
            AllProductsInTheBase.AddRange(Del);

            List<ApiKeys> apiKeys = ApiKeysesJob.GetAllApiList();

    //        foreach (var item in AllProductsInTheBase)
    //        {
				//for (int i = 0; i < item.ArticleNumberUnicList.Count; i++)
				//{
    //                if (item.ArticleNumberUnicList[i].Contains("lnrd"))
    //                    item.ArticleNumberUnicList[i] = item.ArticleNumberUnicList[i].Replace("lnrd", "ld");
    //                if (item.ArticleNumberUnicList[i].Contains("_"))
    //                    item.ArticleNumberUnicList[i] = item.ArticleNumberUnicList[i].Replace("_", "-");
    //            }
    //        }

            ConcurrentBag<Product> productToUpdate = new ConcurrentBag<Product>();
            Dictionary<ApiKeys,int> counApiProductId = new Dictionary<ApiKeys, int>();
            foreach (var item in apiKeys)
            {
                counApiProductId.Add(item, 0);
                IEnumerable<List<Product>> groups = AllProductsInTheBase.Select((x, y) => new { Index = y, Value = x })
    .GroupBy(x => x.Index / 500)
    .Select(x => x.Select(y => y.Value).ToList())
    .ToList();
                foreach (var product500 in groups)
				{
                    Root responseWhithProduct_id = PostRequestAsync(item.ClientId, item.ApiKey, new List<string>());
                    int pageCount = responseWhithProduct_id.result.total / 1000 + 1;
                    int polnoeKolvo = responseWhithProduct_id.result.total;
                    int polnoeKolMinus = responseWhithProduct_id.result.total;
                    remains2.UpdateProgress(0, 0, $"всего: {responseWhithProduct_id.result.total} страниц: {pageCount}");
                    //for (int i = 0; i < pageCount; i++)
                    //{
                    List<string> lst = new List<string>();
                    foreach (var onePr in product500)
                    {
                        foreach (var onePrOffer in onePr.ArticleNumberUnicList)
                        {
                            lst.Add(onePrOffer);
                        }
                    }
                    IEnumerable<List<string>> groupslst = lst.Select((x, y) => new { Index = y, Value = x })
    .GroupBy(x => x.Index / 500)
    .Select(x => x.Select(y => y.Value).ToList())
    .ToList();
					foreach (var product500lst in groupslst)
					{
                        responseWhithProduct_id = PostRequestAsync(item.ClientId, item.ApiKey, product500lst);
                        remains2.UpdateProgress(0, 0, $"Получено: {responseWhithProduct_id.result.items.Count}");
                        ConcurrentQueue<Item> fdshjgkl = new ConcurrentQueue<Item>(responseWhithProduct_id.result.items);
                        int kol = fdshjgkl.Count();
                        Action action = () =>
                        {
                            while (fdshjgkl.Count != 0)
                            {
                                Item oneProductID = new Item();
                                fdshjgkl.TryDequeue(out oneProductID);
                                polnoeKolMinus--;
                                remains2.UpdateProgress(polnoeKolvo, polnoeKolvo - polnoeKolMinus, counApiProductId[item].ToString());
                                if (oneProductID != null)
                                {
                                    Product productsPos = AllProductsInTheBase.Find(x => x.ArticleNumberUnicList.Contains(oneProductID.offer_id));
                                    if (!productsPos.ArticleNumberProductId.ContainsKey(item.ClientId))
                                        productsPos.ArticleNumberProductId.Add(item.ClientId, new List<ArticleNumber>());
                                    if (productsPos.ArticleNumberProductId[item.ClientId].Count() == 0)
                                        productsPos.ArticleNumberProductId[item.ClientId] = new List<ArticleNumber>();

                                    if (productsPos != null)
                                    {
                                        if (!productsPos.ArticleNumberOzonDictList.ContainsKey(item.ClientId))
										{
                                            productsPos.ArticleNumberOzonDictList.Add(item.ClientId, new List<long>());
                                            counApiProductId[item]++;
                                        }
                                        if (!productsPos.ArticleNumberOzonDictList[item.ClientId].Contains(oneProductID.product_id))
										{
                                            productsPos.ArticleNumberOzonDictList[item.ClientId].Add(oneProductID.product_id);
                                            counApiProductId[item]++;
                                        }
                                        if (productsPos.ArticleNumberProductId[item.ClientId].Find(x => x.ArticleOzon == oneProductID.product_id && x.OurArticle == oneProductID.offer_id) == null)
										{
                                            productsPos.ArticleNumberProductId[item.ClientId].Add(new ArticleNumber() { OurArticle = oneProductID.offer_id, ArticleOzon = oneProductID.product_id });
                                            counApiProductId[item]++;
                                        }
                                        productToUpdate.Add(productsPos);
                                    }
                                }


                                // foreach (var onePos in productsPos)
                                // {
                                //if (!onePos.ArticleNumberUnicList.Contains(oneProductID.offer_id))
                                //    onePos.ArticleNumberUnicList.Add(oneProductID.offer_id);


                                //}
                            }
                        };

                        Parallel.Invoke(action);
                    }
                    
                    //}
                

     //               foreach (var oneProductID in responseWhithProduct_id.result.items)
					//{
     //                   List<Product> productsPos = AllProductsInTheBase.Where(x => x.ArticleNumberUnicList.Contains(oneProductID.offer_id) || x.ArticleNumberInShop == oneProductID.offer_id).ToList();
					//	foreach (var onePos in productsPos)
					//	{
     //                       if (!onePos.ArticleNumberUnicList.Contains(oneProductID.offer_id))
     //                           onePos.ArticleNumberUnicList.Add(oneProductID.offer_id);

     //                       if (!onePos.ArticleNumberOzonDictList.ContainsKey(item.ClientId))
     //                           onePos.ArticleNumberOzonDictList.Add(item.ClientId, new List<long>());

     //                       if (!onePos.ArticleNumberOzonDictList[item.ClientId].Contains(oneProductID.product_id))
     //                           onePos.ArticleNumberOzonDictList[item.ClientId].Add(oneProductID.product_id);
     //                       productToUpdate.Add(onePos);
     //                   }
     //               }
                }
            }
            remains2.UpdateProgress(0, 0, $"Сохраняем!!!!!!!!!!!!");
            DataBaseJob.UpdateList(productToUpdate.ToList());
            string req = "";
			foreach (var item in counApiProductId)
			{
                req += item.Key.Name + ": " + item.Value + " "; 
            }
            remains2.UpdateProgress(0, 0, req);
        }

        private static Root PostRequestAsync(string clientId, string apiKey, List<string> offerId)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/list");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            RootRequest rootRequest = new RootRequest();
            rootRequest.filter = new Filter();
            rootRequest.filter.offer_id = offerId;
            rootRequest.filter.visibility = "ALL";


            var jsort = JsonConvert.SerializeObject(rootRequest);
            using (var requestStream = httpWebRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(jsort);
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
    }
}
