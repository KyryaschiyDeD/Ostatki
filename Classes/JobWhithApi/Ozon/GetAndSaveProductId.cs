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
        [JsonProperty("page")]
        public int page { get; set; }
        [JsonProperty("page_size")]
        public int page_size { get; set; }
    }


    public class GetAndSaveProductId
	{
        public static void GetProductsId2()
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
            Dictionary<ApiKeys, int> counApiProductId = new Dictionary<ApiKeys, int>();
            List<Product> productsTOAdd = new List<Product>(); 
            List<Product> productsTODell = new List<Product>(); 
            List<string> problemProduct = new List<string>(); 
            foreach (var key in apiKeys)
            {
                Root responseWhithProduct_id = PostRequestAsync2(key.ClientId, key.ApiKey, 1, 1);
                int total = responseWhithProduct_id.result.total;
                int allCount = total;
                int AllOstatok = allCount;

                remains2.UpdateProgress(0, 0, $"Стартуем "+ key.Name);
                remains2.UpdateProgress(0, 0, $"Всего "+ allCount);

                int pageCount = total / 1000 + 1;
                int truePage = 1;
                while (AllOstatok > 0)
				{
                    remains2.UpdateProgress(allCount, allCount - AllOstatok, $"Остаток " + AllOstatok);
                    responseWhithProduct_id = PostRequestAsync2(key.ClientId, key.ApiKey, truePage, 1000);
                    truePage++;
                    AllOstatok -= responseWhithProduct_id.result.items.Count;
                    int countItems = 0;
					foreach (var item in responseWhithProduct_id.result.items)
					{
                        countItems++;
                        remains2.UpdateProgress(responseWhithProduct_id.result.items.Count, responseWhithProduct_id.result.items.Count - countItems, $"Остаток " + AllOstatok);

                        string offerId = item.offer_id;
                        string offerIdWhithLNRD = "";
                        if (offerId.Contains("lnrd"))
						{
                            offerIdWhithLNRD = offerId;
                            offerId = offerId.Replace("lnrd", "ld");
                            offerId = offerId.Replace("_", "-");
                        }

                        if (offerId.Contains("ld"))
						{
                            offerId = offerId.Substring(3);

                            if (offerId.Contains("x10"))
                                offerId = offerId.Substring(0, offerId.Length - 4);
                            else
                                if (offerId.Contains("x"))
                                offerId = offerId.Substring(0, offerId.Length - 3);
                            
                            Product pr = productsTOAdd.Find(x => x.ProductLink == "https://leonardo.ru/ishop/good_" + offerId + "/");
                            if (pr != null)
                            {
                                productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberUnicList.Add(item.offer_id);
                                if (offerIdWhithLNRD.Length != 0)
                                    productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberUnicList.Add(offerIdWhithLNRD);

                                if (!productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberProductId.ContainsKey(key.ClientId))
                                    productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberProductId.Add(key.ClientId, new List<ArticleNumber>());

                                productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberProductId[key.ClientId].Add(new ArticleNumber() { ArticleOzon = item.product_id, OurArticle = item.offer_id });
                            }
                            else
							{
                                //Thread.Sleep(1500);
                                Product newPos = LeonardoJobs.AddOneProductRT("https://leonardo.ru/ishop/good_" + offerId);
                                if (newPos != null)
								{
                                    newPos.ArticleNumberUnicList.Clear();
                                    newPos.ArticleNumberProductId.Clear();
                                    newPos.ArticleNumberUnicList.Add(item.offer_id);
                                    if (!newPos.ArticleNumberProductId.ContainsKey(key.ClientId))
                                        newPos.ArticleNumberProductId.Add(key.ClientId, new List<ArticleNumber>());
                                    newPos.ArticleNumberProductId[key.ClientId].Add(new ArticleNumber() { ArticleOzon = item.product_id, OurArticle = item.offer_id });
                                    productsTOAdd.Add(newPos);
                                    productsTODell.Add(newPos);
                                }
                                else
								{
                                    problemProduct.Add(item.offer_id);
                                }
                            }
                        }
                        else
						{
                            if (offerId.Contains("lm"))
                                offerId = offerId.Substring(3);

                            if (offerId.Contains("x10"))
                                offerId = offerId.Substring(0, offerId.Length - 4);
                            else
                                if (offerId.Contains("x"))
                                offerId = offerId.Substring(0, offerId.Length - 3);

                            Product product = AllProductsInTheBase.Find(x => x.ArticleNumberInShop.Equals(offerId));
                            if (product != null)
							{
                                Product pr = productsTOAdd.Find(x => x.ProductLink == product.ProductLink);
                                if (pr != null)
								{
                                    productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberUnicList.Add(item.offer_id);
                                    if (!productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberProductId.ContainsKey(key.ClientId))
                                        productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberProductId.Add(key.ClientId, new List<ArticleNumber>());
                                    productsTOAdd[productsTOAdd.IndexOf(pr)].ArticleNumberProductId[key.ClientId].Add(new ArticleNumber() { ArticleOzon = item.product_id, OurArticle = item.offer_id });
                                }
                                else
								{
                                    productsTODell.Add(product);
                                    product.ArticleNumberUnicList.Clear();
                                    product.ArticleNumberProductId.Clear();
                                    product.ArticleNumberUnicList.Add(item.offer_id);
                                    if (!product.ArticleNumberProductId.ContainsKey(key.ClientId))
                                        product.ArticleNumberProductId.Add(key.ClientId, new List<ArticleNumber>());
                                    product.ArticleNumberProductId[key.ClientId].Add(new ArticleNumber() { ArticleOzon = item.product_id, OurArticle = item.offer_id });
                                    productsTOAdd.Add(product);
                                }
                                
                                //DataBaseJob.DeleteProduct(product);
                                
                            }
                            else
							{
                                problemProduct.Add(item.offer_id);
                            }
                        }
                        

                    }
                }
                string problem = "";
                foreach (var item in problemProduct)
				{
                    problem += item + "\n";
                }
                if (problem.Length > 0)
				{
                    SaveDataInFile(problem, "Проблемы на аккаунте " + key.Name);

                }
            }

            DataBaseJob.AddListToRemains(productsTOAdd);

			foreach (var item in productsTODell)
			{
                DataBaseJob.DeleteProduct(item);
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
        private static Root PostRequestAsync2(string clientId, string apiKey, int page, int count)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/list");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            RootRequest rootRequest = new RootRequest();
            rootRequest.page = page;
            rootRequest.page_size = count;

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
