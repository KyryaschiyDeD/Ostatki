using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Остатки.Classes.ProductsClasses.InfoPrices;

namespace Остатки.Classes.JobWhithApi.Ozon
{
    public class GetProductPriceInfo
    {
        public static void GetAndSaveProductPrice()
        {
            List<ApiKeys> apiKeys = ApiKeysesJob.GetAllApiList();
            List<ItemInfoPrices> items = new List<ItemInfoPrices>();

            foreach (var item in apiKeys)
            {
                RootInfoPrices NewCom = PostRequestAsync(item.ClientId, item.ApiKey, "", 1);
                items.AddRange(NewCom.result.items);
                int allCount = NewCom.result.total;
                allCount -= NewCom.result.items.Count;

                while (allCount != 0)
                {
                    int cc = 1000;
                    if (allCount < 1000)
                        cc = allCount;
                    
                    NewCom = PostRequestAsync(item.ClientId, item.ApiKey, NewCom.result.last_id, cc);

                    allCount -= NewCom.result.items.Count;
                    
                    items.AddRange(NewCom.result.items);
                }
            }

            int ctest = items.Count();

            Queue<Product> products = new Queue<Product>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                products = new Queue<Product>(col.Query().ToList());
                while (products.Count != 0)
                {
                    Product oneProduct = products.Dequeue();
                    foreach (var keyValuePair in oneProduct.ArticleNumberProductId)
                    {
                        foreach (var articleNumberOneProduct in keyValuePair.Value)
                        {
                            foreach (var item in items)
                            {
                                if (item.product_id == articleNumberOneProduct.ArticleOzon)
                                {
                                    oneProduct.ArticleNumberProductId[keyValuePair.Key][oneProduct.ArticleNumberProductId[keyValuePair.Key]
                                .IndexOf(articleNumberOneProduct)].productInfoPriceFromOzon = item.commissions;
                                    oneProduct.ArticleNumberProductId[keyValuePair.Key][oneProduct.ArticleNumberProductId[keyValuePair.Key]
                                .IndexOf(articleNumberOneProduct)].PriceOnOzon = item.price.price;
                                }
                            }
                        }
                    }
                    col.Update(oneProduct);
                }
            }
        }



        public static RootInfoPrices PostRequestAsync(string clientId, string apiKey, string last_id, int limit)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v4/product/info/prices");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            RequestInfoPrices rootRequest = new RequestInfoPrices();
            rootRequest.filter = new FilterRequest() {visibility = "ALL"};
            rootRequest.last_id = last_id;
            rootRequest.limit = limit;
            Thread.Sleep(500);
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
                return JsonConvert.DeserializeObject<RootInfoPrices>(result);
            }
        }

        public static RootInfoPrices PostRequestAsyncWhithList(string clientId, string apiKey, string last_id, int limit, List<string> list)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v4/product/info/prices");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            RequestInfoPrices rootRequest = new RequestInfoPrices();
            rootRequest.filter = new FilterRequest() { offer_id = list, visibility = "ALL" };
            rootRequest.last_id = last_id;
            rootRequest.limit = limit;
            Thread.Sleep(500);
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
                return JsonConvert.DeserializeObject<RootInfoPrices>(result);
            }
        }
    }

}

