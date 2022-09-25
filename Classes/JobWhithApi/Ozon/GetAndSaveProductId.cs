using LiteDB;
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
using Windows.Storage;
using Windows.Storage.Pickers;
using Остатки.Classes.ProductsClasses;

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
        public string last_id { get; set; }
    }

    public class Root
    {
        public Result result { get; set; }
    }

    public class Filter
    {
        [JsonProperty("offer_id")]
        public List<string> offer_id { get; set; }
        [JsonProperty("product_id")]
        public List<string> product_id { get; set; }
        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class RootRequest
    {
        [JsonProperty("filter")]
        public Filter filter { get; set; }
        [JsonProperty("last_id")]
        public string last_id { get; set; }
        [JsonProperty("limit")]
        public int limit { get; set; }
    }


    public class GetAndSaveProductId
	{
        private static async void SaveDataInFile(string data, string nameFile)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();
            if (fileWithLinks != null)
            {
                await fileWithLinks.CreateFileAsync(nameFile + ".txt", CreationCollisionOption.ReplaceExisting);
                StorageFile myFile = await fileWithLinks.GetFileAsync(nameFile + ".txt");
                await FileIO.WriteTextAsync(myFile, data);
            }
        }

        public static void GetProductsIdAndArticle()
        {
            List<ApiKeys> apiKeys = ApiKeysesJob.GetAllApiList();
            List<ProductFromMarletplace> productFromMarletplaces = new List<ProductFromMarletplace>();

            foreach (var key in apiKeys)
            {
                if (key.IsOstatkiUpdate)
                {
                    foreach (var oneFilter in FulterStatuses.FilterList)
                    {
                        Root responseWhithProduct_id = PostRequestAsync2(key.ClientId, key.ApiKey, null, 1, oneFilter);
                        int total = responseWhithProduct_id.result.total;
                        int allCount = total;
                        int AllOstatok = allCount;

                        remains2.UpdateProgress(0, 0, $"Стартуем " + key.Name);
                        remains2.UpdateProgress(0, 0, $"Всего " + allCount);

                        int pageCount = (int)Math.Ceiling((double)total / (double)1000);
                        int truePage = 1;
                        while (AllOstatok > 0)
                        {
                            remains2.UpdateProgress(allCount, allCount - AllOstatok, $"{key.Name}, {oneFilter} Остаток: " + AllOstatok);
                            responseWhithProduct_id = PostRequestAsync2(key.ClientId, key.ApiKey, responseWhithProduct_id.result.last_id, 1000, oneFilter);
                            truePage++;
                            AllOstatok -= responseWhithProduct_id.result.items.Count;
                            int countItems = 0;
                            foreach (var item in responseWhithProduct_id.result.items)
                            {
                                countItems++;
                                remains2.UpdateProgress(responseWhithProduct_id.result.items.Count, responseWhithProduct_id.result.items.Count - countItems, $"{key.Name}, Остаток " + AllOstatok);

                                ArticleNumber art = new ArticleNumber() { ArticleOzon = item.product_id, OurArticle = item.offer_id };

                                ProductFromMarletplace oneProduct = new ProductFromMarletplace();
                                oneProduct.productID_OfferID = art;
                                oneProduct.status = oneFilter;
                                oneProduct.Key = key;
                                oneProduct.offer_id = item.offer_id;
                                oneProduct.dateTimeCreate = DateTime.Now;
                                productFromMarletplaces.Add(oneProduct);

                            }
                            if (truePage * 1000 > total + 2000)
                                break;
                        }
                    }
                }
            }

           

            remains2.UpdateProgress(0, 0, $"Сохраняем!!!!!!!!!!!!");
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
            {
                var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
                col.DeleteAll();
                foreach (var item in productFromMarletplaces)
                {
                    col.Insert(item);
                }
                //col.InsertBulk(productFromMarletplaces);
            }
            remains2.UpdateProgress(0, 0, $"Сохранили!!!!!!!!!!!!");
        }

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
                            }
                        };

                        Parallel.Invoke(action);
                    }
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
        private static Root PostRequestAsync2(string clientId, string apiKey, string last_id, int count, string status)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v2/product/list");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            RootRequest rootRequest = new RootRequest();
            rootRequest.last_id = last_id;
            rootRequest.limit = count;
            rootRequest.filter = new Filter() { visibility = status };
            var jsort = JsonConvert.SerializeObject(rootRequest);
            using (var requestStream = httpWebRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(jsort);
            }
            Thread.Sleep(500);
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
